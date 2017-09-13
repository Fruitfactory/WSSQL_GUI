using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
using RestSharp;
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
        private IOFElasticsearchShortContactClient _contactClient;
        private IOFElasticsearchStoreClient _storeClient;
        private readonly Dictionary<string, object> _processedItems = new Dictionary<string, object>();
        private DateTime? _lastUpdated;
        private readonly object _lock = new object();
        private static readonly int COUNT_ITEMS_FOR_COLLECT = 20;

        private readonly List<IOFIOutlookItemsReaderObserver> _observers = new List<IOFIOutlookItemsReaderObserver>();


        private readonly Dictionary<string,OFShortContact> _contacts = new Dictionary<string, OFShortContact>();
        private List<string> _existingContacts = null;

        private readonly AutoResetEvent _eventPause = new AutoResetEvent(false);

        private static readonly long ContentMaxSize = 5 * 1024 * 1024;

        private static readonly int RPCUnavaibleErrorCode = unchecked((int)0x000006BA);
        private static readonly int SyncErrorCode = unchecked((int)0x00000009);
        private static readonly int NetworkProblemErrorCode = unchecked((int)0x00000115);

        Outlook.Application _application = null;


        public OFOutlookItemsReader()
        {
            Status = PstReaderStatus.None;
            _indexOutlookItemsClient = new OFElasticSeachIndexOutlookItemsClient();
            _storeClient = new OFElasticsearchStoreClient();
            _contactClient = new OFElasticsearchShortContactClient();
        }

        public PstReaderStatus Status
        {
            get { return Get(() => Status); }
            private set
            {
                Set(() => Status, value);
                InternalNofity(value);
            }
        }

        public void Close()
        {

        }

        public string Folder { get; private set; }

        public bool IsSuspended
        {
            get { return Get(() => IsSuspended); }
            private set { Set(() => IsSuspended, value); }
        }

        public bool IsStarted
        {
            get { return Get(() => IsStarted); }
            private set { Set(() => IsStarted, value); }
        }

        public int Count { get; private set; }


        public void Start(DateTime? lastUpdated)
        {
            lock (_lock)
            {
                LoadExistingCntacts();

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

        

        public void Join()
        {
            if (_thread != null)
            {
                _thread.Join();
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
            Status = PstReaderStatus.Suspended;
            OFLogger.Instance.LogDebug("Reader Attachment has been Suspended");
        }

        public void Resume(DateTime? lastUpdated)
        {
            _lastUpdated = lastUpdated;
            _eventPause.Set();
            IsSuspended = false;
            Status = PstReaderStatus.Busy;
            OFLogger.Instance.LogDebug("Reader Attachment has been Resumed");
            OFLogger.Instance.LogDebug("Last Updated Date: {0}", lastUpdated.HasValue ? lastUpdated.Value.ToString() : "N/a");
        }

        private void LoadExistingCntacts()
        {
            if (_contactClient.IsNull())
            {
                return;
            }
            OFLogger.Instance.LogDebug("Loading existing contacts...");
            var contacts = _contactClient.GetAllSuggestionContacts();
            if (contacts.IsNull() || !contacts.Any())
            {
                _existingContacts = new List<string>();
                OFLogger.Instance.LogDebug("Contact list is empty...");
            }
            else
            {
                _existingContacts = new List<string>(contacts.Select(c => c.Email));
                OFLogger.Instance.LogDebug($"Contact list has {_existingContacts.Count} items...");
            }
        }

        private bool CheckIfContactExist(string email)
        {
            if (_existingContacts.IsNull() || !_existingContacts.Any())
            {
                return false;
            }
            return _existingContacts.Contains(email);
        }


        private void ProcessOutlookItems(object arg)
        {
            OFMessageFilter.Register();


            var isExistingProcess = false;
            Outlook.NameSpace ns = null;
            try
            {
                TryToWait();
                Status = PstReaderStatus.Busy;
                var resultApplication = OFOutlookHelper.Instance.GetApplication();
                _application = resultApplication.Item1 as Outlook.Application;
                ns = _application.GetNamespace("MAPI");
                isExistingProcess = resultApplication.Item2;

                if (ns.Folders.Count == 0)
                {
                    CloseApplication(_application, isExistingProcess);
                    return;
                }
                CollectCOMItems();
                try
                {
                    Count = GetItemCount(_application);
                }
                catch (System.UnauthorizedAccessException exception)
                {
                    OFLogger.Instance.LogError(exception.ToString());
                    OFLogger.Instance.LogWarning("Try to set counts one more time...");
                    CollectCOMItems();
                    Count = GetItemCount(_application);
                }

                CollectCOMItems();


                TryToWait();

                try
                {

                    var listOutlookStores =
                        ns.Stores.OfType<Outlook.Store>().Select(s => new OFStore() { Name = s.DisplayName, Storeid = s.StoreID }).ToList();
                    var listEsStores = _storeClient.GetStores();
                    var listMustIndexStore =
                        listOutlookStores.Select(s => s.Storeid).Except(listEsStores.Select(s => s.Storeid)).ToList();

                    var listMustDeletedStore =
                        listEsStores.Select(s => s.Storeid).Except(listOutlookStores.Select(s => s.Storeid)).ToList();

                    var listMstUpdateStore =
                        listOutlookStores.Select(s => s.Storeid).Intersect(listEsStores.Select(s => s.Storeid)).ToList();


                    foreach (var indexStoreId in listMustIndexStore)
                    {
                        CheckCancellation();
                        TryToWait();
                        var store = ns.Stores.OfType<Outlook.Store>().FirstOrDefault(s => s.StoreID == indexStoreId);
                        if (store.IsNotNull())
                        {
                            ProcessFolders(store.GetRootFolder(), true);
                            var s = new OFStore() {Name = store.DisplayName, Storeid = store.StoreID};
                            _storeClient.SaveStore(s);
                            OFLogger.Instance.LogDebug($"Index Store = {s}");
                        }
                        else
                        {
                            OFLogger.Instance.LogDebug($"Store is null for id {indexStoreId}");
                        }
                        Marshal.ReleaseComObject(store);
                    }
                    foreach (var deletedStoreId in listMustDeletedStore)
                    {
                        CheckCancellation();
                        TryToWait();
                        var s = new OFStore() { Storeid = deletedStoreId };
                        _storeClient.DeleteStore(s);
                        OFLogger.Instance.LogDebug($"Delete store = {s}");
                    }
                    foreach (var updateStoreId in listMstUpdateStore)
                    {
                        CheckCancellation();
                        TryToWait();
                        var store = ns.Stores.OfType<Outlook.Store>().FirstOrDefault(s => s.StoreID == updateStoreId);
                        var st = new OFStore() { Name = store.DisplayName, Storeid = store.StoreID };
                        OFLogger.Instance.LogDebug($"Update store = {st}");
                        if (store.IsNotNull())
                        {
                            ProcessFolders(store.GetRootFolder());
                        }
                        Marshal.ReleaseComObject(store);
                    }

                    SaveShortContacts();
                }
                catch (COMException com)
                {
                    OFLogger.Instance.LogError(com.ToString());
                    OFLogger.Instance.LogInfo("Reading Outlook items: ErrorCode={0}, {1}", com.ErrorCode.GetErrorCode(), com.ToString());
                    if (com.ErrorCode.GetErrorCode() == RPCUnavaibleErrorCode)
                    {
                        OFLogger.Instance.LogInfo("Restart the indexing process...");
                        ProcessOutlookItems(null);
                    }
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
                if (ns != null)
                {
                    Marshal.ReleaseComObject(ns);
                    ns = null;
                }
                SendOutlookItems(null, null, null, OFOutlookItemsIndexProcess.End);
                _processedItems.Clear();
                Status = PstReaderStatus.Finished;
                CloseApplication(_application, isExistingProcess);
                _application = null;
                OFLogger.Instance.LogInfo("Exit From Attachment Reader...");
                IsStarted = false;
                _thread = null;
                if (_cancellationSource != null)
                {
                    _cancellationSource.Dispose();
                    _cancellationSource = null;
                }
                OFMessageFilter.Revoke();
            }
        }

        private void SaveShortContacts()
        {
            if (!_contacts.Any())
            {
                OFLogger.Instance.LogDebug($"There is nothing to save. Contact list is empty...");
                return;
            }
            try
            {
                OFLogger.Instance.LogDebug($"Save contacts: {_contacts.Count} items...");
                _contactClient.SaveShortContacts(new List<OFShortContact>(_contacts.Values));
                if (_existingContacts.IsNotNull())
                {
                    _existingContacts.AddRange(_contacts.Keys);
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        private void ProcessFolders(Outlook.MAPIFolder mapiFolder, bool ignoreUpdating = false)
        {

            if (mapiFolder.IsNull())
            {
                OFLogger.Instance.LogDebug($"Folder is null");
                return;
            }
            try
            {
                OFLogger.Instance.LogDebug($"Folder '{mapiFolder.FullFolderPath}' Items Count: {mapiFolder.Items.Count}");
                if (mapiFolder.Items.Count > 0 )
                {
                    ProcessItems(mapiFolder, ignoreUpdating);
                }
                
                foreach (Outlook.MAPIFolder folder in mapiFolder.Folders)
                {
                    try
                    {
                        ProcessFolders(folder, ignoreUpdating);
                    }
                    catch (COMException com)
                    {
                        if (com.ErrorCode.GetErrorCode() == NetworkProblemErrorCode)
                        {
                            OFLogger.Instance.LogError("Network problem: {0}", com.ToString());
                        }
                    }
                    catch (Exception ex)
                    {
                        OFLogger.Instance.LogError(ex.ToString());
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(folder);
                    }
                }
            }
            catch (COMException com)
            {
                if (com.ErrorCode.GetErrorCode() == NetworkProblemErrorCode)
                {
                    OFLogger.Instance.LogError("Network problem: {0}", com.ToString());
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void ProcessItems(Outlook.MAPIFolder mapiFolder, bool ignoreUpdating)
        {
            try
            {
                OFLogger.Instance.LogDebug("OutlookItems Reader => Folder name: {0}", mapiFolder.Name);
                Folder = $"{mapiFolder.Name} ({mapiFolder.Store.DisplayName})";
                int count = 0;
                foreach (var result in mapiFolder.Items)
                {
                    CheckCancellation();
                    TryToWait();
                    try
                    {
                        var email = result as Outlook.MailItem;
                        var contact = result as Outlook.ContactItem;

                        if (email != null)
                        {
                            if (_processedItems.ContainsKey(email.EntryID))
                                continue;
                            ProcessEmailItem(email, mapiFolder, ignoreUpdating);
                            _processedItems.Add(email.EntryID, null);
                        }
                        else if (contact != null)
                        {
                            if (_processedItems.ContainsKey(contact.EntryID))
                                continue;
                            ProcessContactItem(contact, mapiFolder.StoreID);
                            _processedItems.Add(contact.EntryID, null);
                        }
                    }
                    catch (COMException com)
                    {
                        OFLogger.Instance.LogInfo("Reading Outlook items: ErrorCode={0}, {1}", com.ErrorCode.GetErrorCode(),
                            com.ToString());
                        if (com.ErrorCode.GetErrorCode() != SyncErrorCode)
                        {
                            throw;
                        }
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(result);
                        count++;
                        if (count == COUNT_ITEMS_FOR_COLLECT)
                        {
                            CollectCOMItems();
                            OFLogger.Instance.LogDebug("Collect COM items...");
                            count = 0;
                        }
                    }
                }
            }
            catch (COMException com)
            {
                if (com.ErrorCode.GetErrorCode() == NetworkProblemErrorCode)
                {
                    OFLogger.Instance.LogError("Network Problem: {0}", com.ToString());
                }
            }
            CollectCOMItems();
            CheckCancellation();
        }

        private static void CollectCOMItems()
        {
            // begin - guys recomend o_O - https://social.msdn.microsoft.com/Forums/vstudio/en-US/e8fd2d43-7c2d-46f4-85a8-37d30b4774d9/closing-mailitems-some-help-needed?forum=vsto
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            // end
        }

        private void OnActivate()
        {
            OFOutlookHelper.Instance.ShowOutlook();
        }

        private void ProcessContactItem(Outlook.ContactItem contactItem, string storeId)
        {
            if (contactItem == null)
            {
                return;
            }
            OFContact contact = new OFContact();
            contact.Storeid = storeId;
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

            ProcessShortContact(contact);

            CheckCancellation();
            TryToWait();

            SendOutlookItems(null, null, contact, OFOutlookItemsIndexProcess.Chunk);
        }

        private void ProcessShortContact(OFContact ofRecipient)
        {
            if (ofRecipient.Emailaddress1.IsNull())
            {
                return;
            }

            var email = ofRecipient.Emailaddress1.ToLowerInvariant();
            if (!email.IsEmail() || _contacts.ContainsKey(email) || CheckIfContactExist(email))
            {
                OFLogger.Instance.LogDebug($"Skip contact with email: {email}");
                return;
            }

            _contacts.Add(email,new OFShortContact() {Email = email, Name = $"{ofRecipient.Firstname} {ofRecipient.Lastname}"});
        }


        private void ProcessEmailItem(Outlook.MailItem emailItem, Outlook.MAPIFolder mapiFolder, bool ignoreUpdating)
        {
            try
            {
                if (emailItem == null)
                {
                    return;
                }
                if (!ignoreUpdating && _lastUpdated.HasValue && emailItem.ReceivedTime < _lastUpdated.Value)
                {
                    return;
                }
                OFEmail email = new OFEmail();
                ProcessEmail(email, emailItem, mapiFolder);

                List<OFAttachmentContent> attachmentContents = new List<OFAttachmentContent>();
                ProcessAttachmentsFull(emailItem, attachmentContents, mapiFolder.StoreID);

                CheckCancellation();
                TryToWait();

                SendOutlookItems(email, attachmentContents, null, OFOutlookItemsIndexProcess.Chunk);
            }
            catch (COMException com)
            {
                if (com.ErrorCode.GetErrorCode() == RPCUnavaibleErrorCode)
                {
                    throw;
                }
                OFLogger.Instance.LogError(com.ToString());
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogDebug(ex.ToString());
            }
        }


        private void ProcessAttachmentsFull(Outlook.MailItem result, List<OFAttachmentContent> attachmentContents, string storeId)
        {
            foreach (var attachment in result.Attachments.OfType<Outlook.Attachment>())
            {
                try
                {
                    byte[] contentBytes = null;
                    if (attachment.Size < ContentMaxSize)
                    {
                        contentBytes = attachment.FileName.IsFileAllowed()
                        ? GetContentByProperty(attachment)
                        : null;
                        if (attachment.FileName.IsFileAllowed() && contentBytes == null)
                        {
                            contentBytes = GetContentByTempFile(attachment);
                        }
                    }
                    AddAttachment(attachmentContents, result, attachment, contentBytes, storeId);
                }
                catch (COMException comEx)
                {
                    if (comEx.ErrorCode.GetErrorCode() == RPCUnavaibleErrorCode)
                    {
                        throw;
                    }

                    OFLogger.Instance.LogError("----COM Attachment Failed => {0}", attachment.FileName);
                    OFLogger.Instance.LogError(comEx.Message);

                    byte[] conBytes = attachment.FileName.IsFileAllowed() && attachment.Size < ContentMaxSize
                        ? GetContentByTempFile(attachment)
                        : null;
                    AddAttachment(attachmentContents, result, attachment, conBytes, storeId);
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
            try { email.ItemName = result.Subject; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.ItemUrl = result.Subject; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Folder = folder.Name; } catch(Exception ex){ OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Foldermessagestoreidpart = folder.EntryID; } catch(Exception ex){ OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Storagename = folder.Name; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Datecreated = result.CreationTime; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try {
                var recev = result.ReceivedTime;
                email.Datereceived = new DateTime(recev.Year, recev.Month, recev.Day, recev.Hour, recev.Minute, recev.Second, 111);
            } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Size = result.Size; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Entryid = result.EntryID; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Conversationid = result.EntryID; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Conversationindex = result.ConversationIndex; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Outlookconversationid = result.ConversationIndex; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Subject = result.Subject; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Content = !string.IsNullOrEmpty(result.Body) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(result.Body)) : ""; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Htmlcontent = !string.IsNullOrEmpty(result.HTMLBody) ? Convert.ToBase64String(Encoding.UTF8.GetBytes(result.HTMLBody)) : ""; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Fromname = result.SenderName; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Fromaddress = result.GetSenderSMTPAddress(); } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { email.Storeid = folder.Store.StoreID; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            
            ProcessRecipients(email, result);
            ProcessEmailAttachments(email, result);
            email.Hasattachments = (email.Attachments != null && email.Attachments.Length > 0).ToString();

            OFLogger.Instance.LogDebug($"Folder: {folder?.FullFolderPath} Email Subject: {result?.Subject}");

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
                var attachment = new OFAttachment();

                OFLogger.Instance.LogInfo($"Attachment type: {att.Type.ToString()}");

                try { attachment.Filename = att.FileName; }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                try { attachment.Mimetag = att.PropertyAccessor.GetProperty("http://schemas.microsoft.com/mapi/proptag/0x370E001E"); }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                try { attachment.Path = string.Empty; }// sometimes it craches on this string, so I had commented it for now. We don't use path.
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                try { attachment.Size = att.Size; }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                try { attachment.Entryid = result.EntryID; }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                listAttachments.Add(attachment);
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
                var r = new OFRecipient() { Address = recipient.GetSMTPAddress(), Name = recipient.Name, Emailaddresstype = recipient.Type.ToString(), Entryid = recipient.EntryID };
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
            ProcessShortContact(email.Fromaddress,email.Fromname);
            listTo.ForEach(ProcessShortContact);
            listCC.ForEach(ProcessShortContact);
            listBCC.ForEach(ProcessShortContact);
            
            email.To = listTo.ToArray();
            email.Cc = listCC.ToArray();
            email.Bcc = listBCC.ToArray();
        }


        private void ProcessShortContact(string email, string name)
        {
            if (email.IsNull())
            {
                return;
            }
            email = email.ToLowerInvariant();
            if (!email.IsEmail() || _contacts.ContainsKey(email) || CheckIfContactExist(email))
            {
                OFLogger.Instance.LogDebug($"Skip contact with email: {email}");
                return;
            }

            _contacts.Add(email, new OFShortContact() { Email = email, Name = name.IsNotNull() ? name : "" });
        }


        private void ProcessShortContact(OFRecipient ofRecipient)
        {
            if (ofRecipient.Address.IsNull())
            {
                return;
            }

            var email = ofRecipient.Address.ToLowerInvariant();
            if (!email.IsEmail() || _contacts.ContainsKey(email) || CheckIfContactExist(email))
            {
                OFLogger.Instance.LogDebug($"Skip contact with email: {email}");
                return;
            }

            _contacts.Add(email,new OFShortContact() {Email = email,Name = ofRecipient.Name.IsNotNull() ? ofRecipient.Name : ""});
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
            return filebyte;
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
            return buffer;
        }

        private void AddAttachment(List<OFAttachmentContent> attachments, Outlook.MailItem email, Outlook.Attachment attachment, byte[] content, string storeId)
        {
            OFAttachmentContent indexAttach = new OFAttachmentContent();
            indexAttach.Storeid = storeId;
            try { indexAttach.Size = attachment.Size; } catch(Exception ex){ OFLogger.Instance.LogError(ex.ToString()); }
            try { indexAttach.Emailid = email.EntryID; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { indexAttach.Outlookemailid = email.EntryID; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try {
                var recev = email.ReceivedTime;
                indexAttach.Datecreated = new DateTime(recev.Year, recev.Month, recev.Day, recev.Hour, recev.Minute, recev.Second, 111);
            } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try { indexAttach.Filename = attachment.FileName; } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }
            try {
                if (content != null)
                {
                    indexAttach.Content = Convert.ToBase64String(content);
                    OFLogger.Instance.LogDebug("---- Attachment => {0}", attachment.FileName);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(string.Format("--- Skip Attachment (by size): {0}", email.Subject));
                }
            } catch (Exception ex) { OFLogger.Instance.LogError(ex.ToString()); }

            attachments.Add(indexAttach);
        }



        private void SendOutlookItems(OFEmail email, IEnumerable<OFAttachmentContent> attachments, OFContact contact, OFOutlookItemsIndexProcess process)
        {
            if (_indexOutlookItemsClient.IsNotNull())
            {
                OFOutlookItemsIndexingContainer container = new OFOutlookItemsIndexingContainer() { Email = email, Attachments = attachments, Contact = contact, Process = process };
                _indexOutlookItemsClient.SendOutlookItemsToIndex(container);
            }
        }

        private void CheckCancellation()
        {
            if (_cancellationToken.IsCancellationRequested)
            {
                _cancellationToken.ThrowIfCancellationRequested();
            }
        }


        public static int GetItemCount(Outlook._Application application)
        {
            if (application == null)
                return 0;
            int count = 0;
            try
            {
                Outlook.NameSpace ns = application.GetNamespace("MAPI");
                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                {
                    GetOutlookFolders(folder, ref count);
                    Marshal.ReleaseComObject(folder);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return count;
        }

        private static void GetOutlookFolders(Outlook.MAPIFolder folder, ref int c)
        {
            try
            {
                if (folder.Folders.Count == 0)
                    return;
                foreach (var subfolder in folder.Folders.OfType<Outlook.MAPIFolder>())
                {
                    try
                    {
                        c += subfolder.Items != null ? subfolder.Items.Count : 0;
                        GetOutlookFolders(subfolder, ref c);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.ToString());
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(subfolder);
                    }
                }
                CollectCOMItems();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
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

        public void Attach(IOFIOutlookItemsReaderObserver observer)
        {
            lock (_lock)
            {
                _observers.Add(observer);
            }
        }

        private void InternalNofity(PstReaderStatus newStatus)
        {
            lock (_lock)
            {
                _observers.ForEach(o => o.UpdateStatus(newStatus));
            }
        }
    }
}