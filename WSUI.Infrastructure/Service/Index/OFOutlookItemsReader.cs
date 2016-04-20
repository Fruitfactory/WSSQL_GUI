﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Documents;
using Microsoft.Practices.Unity;
using Nest;
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
    public class OFOutlookItemsReader : OFViewModelBase, IOutlookItemsReader
    { 
        private string AttachSchema = "http://schemas.microsoft.com/mapi/proptag/0x37010102";
        private Thread _thread;
        private CancellationTokenSource _cancellationSource;
        private CancellationToken _cancellationToken;
        private IElasticSearchIndexOutlookItemsClient _indexOutlookItemsClient;
        private DateTime? _lastUpdated;
        private readonly object _lock = new object();
        
        private readonly AutoResetEvent _eventPause = new AutoResetEvent(false);
        private static readonly string DateFormat = "MM/dd/yyyy HH:mm:ss";
        private static int PageMaxSize = 65536;

        
        public OFOutlookItemsReader()
        {
            Status = PstReaderStatus.None;
            _indexOutlookItemsClient = new OFElasticSeachIndexOutlookItemsClient();
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
                    _thread = new Thread(ProcessOutlookItems);
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

        private void ProcessOutlookItems(object arg)
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

                _indexOutlookItemsClient.SendOutlookItemsCount(folderList.Sum(f => f.Items.Count));

                foreach (var mapiFolder in folderList)
                {
                    CheckCancellation();
                    TryToWait();
                    OFLogger.Instance.LogDebug("Attachment Reader => Folder name: {0}", mapiFolder.Name);
                    if (mapiFolder.Items.Count == 0)
                    {
                        Marshal.ReleaseComObject(mapiFolder);
                        continue;
                    }
                    foreach (var result in mapiFolder.Items)
                    {
                        CheckCancellation();
                        TryToWait();
                        try
                        {
                            if (result is Outlook.MailItem)
                            {
                                ProcessEmailItem(result as Outlook.MailItem, mapiFolder);
                            }
                            else if (result is Outlook.ContactItem)
                            {
                                ProcessContactItem(result as Outlook.ContactItem);
                            }
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(result);
                        }
                    }
                    Marshal.ReleaseComObject(mapiFolder);
                    // begin - guys recomend o_O - https://social.msdn.microsoft.com/Forums/vstudio/en-US/e8fd2d43-7c2d-46f4-85a8-37d30b4774d9/closing-mailitems-some-help-needed?forum=vsto
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    // end
                    CheckCancellation();
                }
            }
            catch (AggregateException ex)
            {
                OFLogger.Instance.LogError("Canceled: {0}", ex.ToString());
            }
            catch (Exception common)
            {
                OFLogger.Instance.LogError(common.ToString());
            }
            finally
            {
                SendOutlookItems(null,null,null, OFOutlookItemsIndexProcess.End);
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


        private void ProcessContactItem(Outlook.ContactItem contactItem)
        {
            if (contactItem == null)
            {
                return;
            }
            OFContact contact = new OFContact();
            contact.Firstname = contactItem.FirstName;
            contact.Lastname = contactItem.LastName;
            contact.Emailaddress1 = contactItem.Email1Address;
            contact.Emailaddress2 = contactItem.Email2Address;
            contact.Emailaddress3 = contactItem.Email3Address;
            contact.Businesstelephone = contactItem.BusinessTelephoneNumber;
            contact.Hometelephone = contactItem.HomeTelephoneNumber;
            contact.Mobiletelephone = contactItem.MobileTelephoneNumber;
            
            contact.HomeAddressCity = contactItem.HomeAddressCity;
            contact.HomeAddressCountry = contactItem.HomeAddressCountry;
            contact.HomeAddressPostalCode = contactItem.HomeAddressPostalCode;
            contact.HomeAddressState = contactItem.HomeAddressState;
            contact.HomeAddressStreet = contactItem.HomeAddressStreet;
            contact.HomeAddressPostOfficeBox = contactItem.HomeAddressPostOfficeBox;


            contact.BusinessAddressCity = contactItem.BusinessAddressCity;
            contact.BusinessAddressCountry = contactItem.BusinessAddressCountry;
            contact.BusinessAddressState = contactItem.BusinessAddressState;
            contact.BusinessAddressStreet = contactItem.BusinessAddressStreet;
            contact.BusinessAddressPostOfficeBox = contactItem.BusinessAddressPostOfficeBox;

            contact.Keyword = "";
            contact.Location = contactItem.OfficeLocation;
            contact.CompanyName = contactItem.CompanyName;
            contact.Title = contactItem.Title;
            contact.DepartmentName = contactItem.Department;
            contact.MiddleName = contactItem.MiddleName;
            contact.DisplyNamePrefix = "";
            contact.Profession = contactItem.Profession;
            contact.Note = "";
            contact.HomeAddress = contactItem.HomeAddress;
            contact.WorkAddress = contactItem.BusinessAddress;
            contact.OtherAddress = contactItem.OtherAddress;
            contact.Birthday = null; //contactItem.Birthday;

            contact.Entryid = contactItem.EntryID;

            contact.Addresstype = "";


            CheckCancellation();
            TryToWait();

            SendOutlookItems(null,null,contact,OFOutlookItemsIndexProcess.Chunk);
        }


        private void ProcessEmailItem(Outlook.MailItem emailItem, Outlook.MAPIFolder mapiFolder)
        {
            try
            {
                if (emailItem == null)
                {
                    return;
                }
                if (_lastUpdated.HasValue && emailItem.ReceivedTime < _lastUpdated.Value)
                {
                    return;
                }
                //OFLogger.Instance.LogWarning("Subject: {0}", emailItem.Subject.ToUpperInvariant());
                //if (!string.IsNullOrEmpty(emailItem.Subject) && emailItem.Subject.ToUpperInvariant().Equals("RE: Indexing stuck".ToUpperInvariant()))
                {
                    OFEmail email = new OFEmail();
                    ProcessEmail(email, emailItem, mapiFolder);

                    List<OFAttachmentContent> attachmentContents = new List<OFAttachmentContent>();
                    ProcessAttachmentsFull(emailItem, attachmentContents);

                    CheckCancellation();
                    TryToWait();

                    SendOutlookItems(email, attachmentContents, null, OFOutlookItemsIndexProcess.Chunk);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogDebug(ex.ToString());
            }
        }


        private void ProcessAttachmentsFull(Outlook.MailItem result, List<OFAttachmentContent> attachmentContents)
        {
            foreach (var attachment in result.Attachments.OfType<Outlook.Attachment>())
            {
                try
                {
                    byte[] contentBytes = attachment.FileName.IsFileAllowed()
                        ? GetContentByProperty(attachment)
                        : null;
                    if (attachment.FileName.IsFileAllowed() && contentBytes == null)
                    {
                        contentBytes = GetContentByTempFile(attachment);
                    }
                    AddAttachment(attachmentContents, result, attachment, contentBytes);
                }
                catch (COMException comEx)
                {
                    OFLogger.Instance.LogError("----COM Attachment Failed => {0}", attachment.FileName);
                    OFLogger.Instance.LogError(comEx.Message);

                    byte[] conBytes = attachment.FileName.IsFileAllowed()
                        ? GetContentByTempFile(attachment)
                        : null;
                    AddAttachment(attachmentContents, result, attachment, conBytes);
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError("---- Attachment Failed => {0}", attachment.FileName);
                    OFLogger.Instance.LogError("---- Type Failed => {0}", ex.GetType().Name);
                    OFLogger.Instance.LogError(ex.ToString());
                }
                finally
                {
                    Marshal.ReleaseComObject(attachment);
                }
                CheckCancellation();
                TryToWait();
            }
        }

        private void ProcessEmail(OFEmail email, Outlook.MailItem result, Outlook.MAPIFolder folder)
        {
            if (result == null)
            {
                return;
            }
            email.ItemName = result.Subject;
            email.ItemUrl = result.Subject;
            email.Folder = folder.Name;
            email.Foldermessagestoreidpart = folder.EntryID;
            email.Storagename = folder.Name;
            email.Datecreated = result.CreationTime;
            var recev = result.ReceivedTime;    
            email.Datereceived = new DateTime(recev.Year,recev.Month,recev.Day,recev.Hour,recev.Minute,recev.Second,111);
            email.Size = result.Size;
            email.Entryid = result.EntryID;
            email.Conversationid = result.EntryID;
            email.Conversationindex = result.ConversationIndex;
            email.Outlookconversationid = result.ConversationIndex;
            email.Subject = result.Subject;
            email.Content = !string.IsNullOrEmpty(result.Body) ?  Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Body)) : "";
            email.Htmlcontent = !string.IsNullOrEmpty(result.HTMLBody) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(result.HTMLBody)) : "";
            email.Fromname = result.SenderName;
            email.Fromaddress = result.GetSenderSMTPAddress();
            ProcessRecipients(email, result);
            ProcessEmailAttachments(email,result);
            email.Hasattachments = (email.Attachments != null && email.Attachments.Length > 0).ToString();
        }

        private void ProcessEmailAttachments(OFEmail email, Outlook.MailItem result)
        {
            if (result == null || result.Attachments.Count == 0)
            {
                return;
            }

            var listAttachments = new List<OFAttachment>();
            foreach (var att in result.Attachments.OfType<Outlook.Attachment>())
            {
                try
                {
                    var attachment = new OFAttachment();
                    attachment.FileName = att.FileName;
                    attachment.MimeTag = att.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001E");
                    attachment.Path = att.PathName;
                    attachment.Size = att.Size;
                    attachment.Entryid = result.EntryID;
                    listAttachments.Add(attachment);
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
            }
            email.Attachments = listAttachments.ToArray();
        }

        private void ProcessRecipients(OFEmail email, Outlook.MailItem result)
        {
            var listTo = new List<OFRecipient>();
            var listCC = new List<OFRecipient>();
            var listBCC = new List<OFRecipient>();

            foreach (var recipient in result.Recipients.OfType<Outlook.Recipient>())
            {
                var r = new OFRecipient(){Address = recipient.GetSMTPAddress(),Name = recipient.Name,Emailaddresstype = recipient.Type.ToString(),Entryid =  recipient.EntryID};
                switch (recipient.Type)
                {
                    case 0:
                        if (email.Fromaddress.IsStringEmptyOrNull() || !email.Fromaddress.IsEmail())
                        {
                            email.Fromaddress = recipient.GetSMTPAddress();
                        }
                        break;
                    case 1: // To
                        listTo.Add(r);
                        break;
                    case 2: // CC
                        listCC.Add(r);
                        break;
                    case 3: //BCC
                        listBCC.Add(r);
                        break;
                }
            }

            email.To = listTo.ToArray();
            email.Cc = listCC.ToArray();
            email.Bcc = listBCC.ToArray();
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
            indexAttach.Emailid = email.EntryID;
            indexAttach.Outlookemailid = email.EntryID;
            var recev = email.ReceivedTime;
            indexAttach.Datecreated = new DateTime(recev.Year, recev.Month, recev.Day, recev.Hour, recev.Minute, recev.Second, 111);
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



        private void SendOutlookItems(OFEmail email,IEnumerable<OFAttachmentContent> attachments, OFContact contact, OFOutlookItemsIndexProcess process)
        {
            if (_indexOutlookItemsClient.IsNotNull())
            {
                OFOutlookItemsIndexingContainer container = new OFOutlookItemsIndexingContainer() { Email =  email,Attachments = attachments, Contact = contact, Process = process };
                _indexOutlookItemsClient.SendOutlookItemsToIndex(container);
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
            if (application == null || IsMainWiwdowOFOutlookOpened() || isExistingProcess)
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


        private bool IsMainWiwdowOFOutlookOpened()
        {
            var process = Process.GetProcessesByName("outlook".ToUpperInvariant()).FirstOrDefault();
            return process != null && process.MainWindowHandle != IntPtr.Zero;
        }
    }
}