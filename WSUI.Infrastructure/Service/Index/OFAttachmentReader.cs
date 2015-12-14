using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Win32;
using OF.Infrastructure.Implements.ElasticSearch.Clients;
using OF.Infrastructure.Implements.Service;
using Exception = System.Exception;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OF.Infrastructure.Service.Index
{
    public class OFAttachmentReader : OFViewModelBase, IAttachmentReader
    { 
        private string AttachSchema = "http://schemas.microsoft.com/mapi/proptag/0x37010102";
        private Thread _thread;
        private CancellationTokenSource _cancellationSource;
        private CancellationToken _cancellationToken;
        private IElasticSearchIndexAttachmentClient _indexAttachmentClient;
        private DateTime? _lastUpdated;
        private readonly object _lock = new object();
        
        private readonly AutoResetEvent _eventPause = new AutoResetEvent(false);
        private static readonly string DateFormat = "MM/dd/yyyy HH:mm:ss";
        private static int PageMaxSize = 65536;

        
        public OFAttachmentReader()
        {
            Status = PstReaderStatus.None;
            _indexAttachmentClient = new OFElasticSeachIndexAttachmentClient();
        }

        public PstReaderStatus Status
        {
            get { return Get(() => Status); }
            private set { Set(() => Status, value); }
        }

        public void Close()
        {
            
        }

        public bool IsSuspended
        {
            get { return Get(() => IsSuspended); }
            private set { Set(() => IsSuspended,value);}
        }

        public bool IsStarted
        {
            get { return Get(() => IsStarted); }
            private set{Set(() => IsStarted, value);}
        }

        public int Count { get; private set; }


        public void Start(DateTime? lastUpdated)
        {
            lock (_lock)
            {
                if (_thread == null)
                {
                    _thread = new Thread(ProcessAttachment);
                    _thread.SetApartmentState(ApartmentState.STA);
                    _lastUpdated = lastUpdated;
                    _cancellationSource = new CancellationTokenSource();
                    _cancellationToken = _cancellationSource.Token;
                    OFLogger.Instance.LogDebug("Last Updated Date: {0}", lastUpdated.HasValue ? lastUpdated.Value.ToString() : "N/a");
                    IsStarted = true;
                    Status = PstReaderStatus.Busy;
                    _thread.Start();
                }
            }
        }

        public void Stop()
        {
            Resume(_lastUpdated);
            lock (_lock)
            {
                if (_cancellationSource != null)
                {
                    _cancellationSource.Cancel();
                }
                if (_thread != null)
                {
                    _thread.Join();
                    _thread = null;
                }
                Status = PstReaderStatus.Finished;
                IsStarted = false;    
            }
        }

        public void Suspend()
        {
            IsSuspended = true;
            OFLogger.Instance.LogDebug("Reader Attachment has been Suspended");
        }

        public void Resume(DateTime? lastUpdated)
        {
            _lastUpdated = lastUpdated;
            _eventPause.Set();
            IsSuspended = false;
            OFLogger.Instance.LogDebug("Reader Attachment has been Resumed");
            OFLogger.Instance.LogDebug("Last Updated Date: {0}", lastUpdated.HasValue ? lastUpdated.Value.ToString() : "N/a");
        }

        private void ProcessAttachment(object arg)
        {
            OFMessageFilter.Register();

            Outlook._Application application = null;
            var isExistingProcess = false;
            try
            {
                Status = PstReaderStatus.Busy;
                var resultApplication = OFOutlookHelper.Instance.GetApplication();
                application = resultApplication.Item1 as Outlook._Application;
                isExistingProcess = resultApplication.Item2;
                var folderList = GetFolders(application).OfType<Outlook.MAPIFolder>();
                if (!folderList.Any())
                {
                    CloseApplication(application,isExistingProcess);
                    return;
                }
                foreach (var mapiFolder in folderList)
                {
                    CheckCancellation();
                    TryToWait();
                    OFLogger.Instance.LogDebug("Attachment Reader => Folder name: {0}", mapiFolder.Name);
                    if (mapiFolder.Items.Count == 0)
                    {
                        continue;
                    }
                    foreach (var result in mapiFolder.Items.OfType<Outlook.MailItem>())
                    {
                        CheckCancellation();
                        TryToWait();
                        if (result.Attachments.Count == 0 || (_lastUpdated.HasValue && result.ReceivedTime < _lastUpdated.Value))
                        {
                            continue;
                        }
                        List<OFAttachmentContent> attachmentContents = new List<OFAttachmentContent>();
                        foreach (var attachment in result.Attachments.OfType<Outlook.Attachment>())
                        {

                            if (attachment.Size < PageMaxSize)
                            {
                                continue;
                            }
                            CheckCancellation();
                            TryToWait();
                            try
                            {
                                byte[] contentBytes = attachment.FileName.IsFileAllowed() ? GetContentByProperty(attachment) : null;
                                AddAttachment(attachmentContents, result, attachment, contentBytes);
                            }
                            catch (COMException comEx)
                            {
                                OFLogger.Instance.LogError("----COM Attachment Failed => {0}", attachment.FileName);
                                OFLogger.Instance.LogError(comEx.Message);

                                byte[] conBytes = attachment.FileName.IsFileAllowed() ? GetContentByTempFile(attachment) : null;
                                AddAttachment(attachmentContents, result, attachment, conBytes);
                            }
                            catch (Exception ex)
                            {
                                OFLogger.Instance.LogError("---- Attachment Failed => {0}", attachment.FileName);
                                OFLogger.Instance.LogError("---- Type Failed => {0}", ex.GetType().Name);
                                OFLogger.Instance.LogError(ex.ToString());
                            }
                        }
                        CheckCancellation();
                        TryToWait();
                        if (attachmentContents.Count > 0)
                        {
                            SendAttachments(attachmentContents, OFAttachmentIndexProcess.Chunk);
                        }

                    }
                    CheckCancellation();
                }
            }
            catch (AggregateException ex)
            {
                OFLogger.Instance.LogError("Canceled: {0}", ex.ToString());
            }
            catch (Exception common)
            {
                OFLogger.Instance.LogError(common.Message);
            }
            finally
            {
                SendAttachments(null, OFAttachmentIndexProcess.End);
                Status = PstReaderStatus.Finished;
                CloseApplication(application,isExistingProcess);
                application = null;
                OFLogger.Instance.LogInfo("!!!!!!!! Exit From Attachment Reader");
                System.Diagnostics.Debug.WriteLine("!!!!!!!! Exit From Attachment Reader");
                IsStarted = false;
                _thread = null;
                _cancellationSource.Dispose();    
                _cancellationSource = null;
                OFMessageFilter.Revoke();
            }
        }

        private void TryToWait()
        {
            if (IsSuspended)
            {
                _eventPause.WaitOne();
            }
        }


        private byte[] GetContentByProperty(Outlook.Attachment attachment)
        {
            Outlook.PropertyAccessor pacc = attachment.PropertyAccessor;
            byte[] filebyte = (byte[])pacc.GetProperty(AttachSchema);
            return  filebyte;
        }

        private byte[] GetContentByTempFile(Outlook.Attachment attachment)
        {
            byte[] buffer = null;
            try
            {
                attachment.SaveAsFile(attachment.FileName);
                buffer = File.ReadAllBytes(attachment.FileName);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            finally
            {
                if (File.Exists(attachment.FileName))
                {
                    File.Delete(attachment.FileName);
                }
            }
            return  buffer;
        }

        private void AddAttachment(List<OFAttachmentContent> attachments,Outlook.MailItem email, Outlook.Attachment attachment, byte[] content)
        {
            OFAttachmentContent indexAttach = new OFAttachmentContent();
            indexAttach.Size = attachment.Size;
            var messageId = email.Headers("Message-ID");
            indexAttach.Emailid = messageId.Any() ? messageId.FirstOrDefault() : string.Empty;
            indexAttach.Outlookemailid = email.EntryID;
            indexAttach.Datecreated = email.CreationTime;
            
            System.Diagnostics.Debug.WriteLine("Subject => {0} ReceivedTime => {1} TransportMessaageId => {2}",
                email.Subject, email.ReceivedTime.ToString(DateFormat),
                messageId.Any() ? messageId.FirstOrDefault() : "n/a");
            OFLogger.Instance.LogDebug("---- Attachment => {0}", attachment.FileName);
            indexAttach.Filename = attachment.FileName;
            if (content != null)
            {
                indexAttach.Content = Convert.ToBase64String(content);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine(string.Format("--- Skip Attachment (by size): {0}",email.Subject));
            }

            attachments.Add(indexAttach);
        }



        private void SendAttachments(IEnumerable<OFAttachmentContent> attachments, OFAttachmentIndexProcess process)
        {
            if (_indexAttachmentClient.IsNotNull())
            {
                OFAttachmentIndexingContainer container = new OFAttachmentIndexingContainer() { Attachments = attachments, Process = process };
                _indexAttachmentClient.SendAttachmentToIndex(container);
                Count += attachments.IsNotNull() ? attachments.Count() : 0;
            }
        }

        private void CheckCancellation()
        {
            if ( _cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }

        public List<object> GetFolders(Outlook._Application application)
        {
            if (application == null)
                return default(List<object>);

            List<object> res = new List<object>();
            try
            {
                Outlook.NameSpace ns = application.GetNamespace("MAPI");
                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                {
                    res.Add(folder);
                    GetOutlookFolders(folder, res);
                }

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolders", ex.ToString()));
                return res;
            }
            return res;
        }

        private void GetOutlookFolders(Outlook.MAPIFolder folder, List<object> res)
        {
            try
            {
                if (folder.Folders.Count == 0)
                    return;
                foreach (var subfolder in folder.Folders.OfType<Outlook.MAPIFolder>())
                {
                    try
                    {
                        res.Add(subfolder);
                        GetOutlookFolders(subfolder, res);
                    }
                    catch (Exception e)
                    {
                        OFLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders", subfolder.Name, e.Message));
                    }
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetOutlookFolders", e.Message));
            }
        }

        private void CloseApplication(Outlook._Application application, bool isExistingProcess)
        {
            OFLogger.Instance.LogDebug("Outlook is existed: {0}");
            if (application == null || isExistingProcess)
            {
                return;
            }
            try
            {
                application.Quit();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            Marshal.ReleaseComObject(application);
        }
    }
}