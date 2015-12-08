﻿using System;
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
        
        private readonly AutoResetEvent _eventPause = new AutoResetEvent(false);
        private Outlook._Application _application;
        private static readonly string DateFormat = "MM/dd/yyyy HH:mm:ss";
        private static int PageMaxSize = 65536;

        
        public OFAttachmentReader()
        {
            Status = PstReaderStatus.None;
            _cancellationSource = new CancellationTokenSource();
            _cancellationToken = _cancellationSource.Token;
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

        public int Count { get; private set; }


        public void Start(DateTime? lastUpdated)
        {
            if (_thread == null)
            {
                _thread = new Thread(ProcessAttachment);
            }
            _lastUpdated = lastUpdated;
            _thread.Start();
            Status = PstReaderStatus.Busy;
        }

        public void Stop()
        {
            _cancellationSource.Cancel();
            _thread.Join();
            Status = PstReaderStatus.Finished;
        }

        public void Suspend()
        {
            IsSuspended = true;
            OFLogger.Instance.LogDebug("Reader Attachment has been Suspended");
        }

        public void Resume()
        {
            _eventPause.Set();
            IsSuspended = false;
            OFLogger.Instance.LogDebug("Reader Attachment has been Resumed");
        }

        private void ProcessAttachment(object arg)
        {
            try
            {
                Status = PstReaderStatus.Busy;
                _application = CreateOutlookApplication();
                var folderList = GetFolders().OfType<Outlook.MAPIFolder>();
                if (!folderList.Any())
                {
                    CloseApplication();
                    return;
                }
                foreach (var mapiFolder in folderList)
                {
                    if (mapiFolder.Items.Count == 0)
                    {
                        continue;
                    }
                    foreach (var result in mapiFolder.Items.OfType<Outlook.MailItem>())
                    {
                        if (result.Attachments.Count == 0 || (_lastUpdated.HasValue && result.ReceivedTime < _lastUpdated.Value))
                        {
                            continue;
                        }
                        TryToWait();
                        List<OFAttachmentContent> attachmentContents = new List<OFAttachmentContent>();
                        foreach (var attachment in result.Attachments.OfType<Outlook.Attachment>())
                        {

                            if (attachment.Size < PageMaxSize)
                            {
                                continue;
                            }
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
                                OFLogger.Instance.LogError(ex.Message);
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
                OFLogger.Instance.LogError("Canceled: {0}", ex.Message);
            }
            catch (Exception common)
            {
                OFLogger.Instance.LogError(common.Message);
            }
            finally
            {
                SendAttachments(null, OFAttachmentIndexProcess.End);
                Status = PstReaderStatus.Finished;
                CloseApplication();
                System.Diagnostics.Debug.WriteLine("!!!!!!!! Exit From Attachment Reader");
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
                OFLogger.Instance.LogError(ex.Message);
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
            if (_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }

        private Outlook._Application CreateOutlookApplication()
        {
            Outlook._Application ret = null;
            try
            {
                ret = new Outlook.Application() as Outlook._Application;
                if (ret == null)
                    return ret;
                Outlook.NameSpace ns = ret.GetNamespace("MAPI");
                ns.Logon(ret.DefaultProfileName, "", Type.Missing, Type.Missing);//ret.DefaultProfileName
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "CreateOutlookApplication", ex.Message));
            }

            return ret;
        }

        public List<object> GetFolders()
        {
            if (_application == null)
                return default(List<object>);

            List<object> res = new List<object>();
            try
            {
                Outlook.NameSpace ns = _application.GetNamespace("MAPI");
                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                {
                    res.Add(folder);
                    GetOutlookFolders(folder, res);
                }

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolders", ex.Message));
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

        private void CloseApplication()
        {
            if (_application == null)
            {
                return;
            }

            _application.Quit();
            _application = null;
        }
    }
}