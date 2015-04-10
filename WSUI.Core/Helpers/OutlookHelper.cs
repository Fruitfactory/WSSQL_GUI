﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Practices.ObjectBuilder2;
using WSUI.Core.Core.ElasticSearch;
using WSUI.Core.Data;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers.DetectEncoding.Multilang;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WSUI.Core.Helpers
{
    public class OutlookHelper : IOutlookHelper, IDisposable
    {
        #region static

        private static string OutlookProcessName = "OUTLOOK";
        private static string OutlookApplication = "Outlook.Application";

        //private static OutlookHelper _instance= null;
        private static readonly object _lockObject = new object();

        #endregion static

        private const string ATSUFFIX = "/at=";
        private const int IDLENGHT = 24;

        public const string AllFolders = "All folders";

        public const string MAIL_EXT = "eml";
        public const string MAIL_FILTER = "*." + MAIL_EXT;

        #region fields

        private bool _disposed;
        private Outlook._Application _app;

        #endregion fields

        private static readonly Lazy<OutlookHelper> _instance = new Lazy<OutlookHelper>(() =>
                                                                                   {
                                                                                       var obj = new OutlookHelper();
                                                                                       return obj;
                                                                                   });

        private OutlookHelper()
        {
            _disposed = false;
            InternalHostType = HostType.Unknown;
        }

        public static OutlookHelper Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #region [Outllok application]

        internal HostType InternalHostType
        {
            get;
            set;
        }

        public Outlook._Application OutlookApp
        {
            get
            {
                if (InternalHostType == HostType.Unknown && _app == null)
                {
                    _app = CreateOutlookApplication();
                }
                return _app;
            }
            set
            {
                _app = value;
                InternalHostType = HostType.Plugin;
            }
        }

        #endregion [Outllok application]

        #region public

        public Tuple<string,string> GetCurrentyUserInfo()
        {
            if (OutlookApp == null)
                return null;
            var activeExp = OutlookApp.ActiveExplorer();
            if (activeExp == null)
                return null;
            return new Tuple<string, string>(activeExp.Session.CurrentUser.Name, activeExp.Session.CurrentUser.AddressEntry.Address);
        }

        public string GetEMailTempFileName(BaseSearchObject itemsearch)
        {
            if (itemsearch == null)
                return null;
            string mapiUrl = itemsearch.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic mailItem = GetMailItem(entryID);
            if (mailItem == null)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Mail not found", itemsearch.ItemUrl));
                return null;
            }
            
            string tempFilename = TempFileManager.Instance.GenerateTempFileName(itemsearch);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            if (!File.Exists(tempFilename))
            {
                //ProgressManager.Instance.StartOperation(new ProgressOperation() { Caption = "Saving an Email...", Canceled = false, DelayTime = 2500 });
                mailItem.SaveAs(tempFilename, Type.Missing);
                //ProgressManager.Instance.StopOperation();
            }

            return tempFilename;
        }

        public string GetEmailEmlFilename(EmailSearchObject emailObject)
        {
            if (emailObject.IsNull())
            {
                return string.Empty;
            }

            if (TempFileManager.Instance.IsEmlFileExistForEmailObject(emailObject))
            {
                return TempFileManager.Instance.GetExistEmlFileForEmailObject(emailObject);
            }
            
            string result = string.Empty;
            string tempFolder = TempFileManager.Instance.GenerateTempFolderForObject(emailObject);
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(emailObject.FromAddress,emailObject.FromName);
            if (emailObject.To != null)
            {
                emailObject.To.ForEach(r =>
                {
                    if (r.Address.IsEmail())
                        mail.To.Add(new MailAddress(r.Address, r.Name));
                });
            }
            if (emailObject.Cc != null)
            {
                emailObject.Cc.ForEach(r =>
                {
                    if (r.Address.IsEmail())
                        mail.CC.Add(new MailAddress(r.Address, r.Name));
                });
            }
            if (emailObject.Bcc != null)
            {
                emailObject.Bcc.ForEach(r =>
                {
                    if (r.Address.IsEmail())
                        mail.Bcc.Add(new MailAddress(r.Address, r.Name));
                });
            }

            if (!string.IsNullOrEmpty(emailObject.Content))
            {
                mail.Body = emailObject.Content;
            }
            else
            {
                mail.Body = emailObject.HtmlContent;
                mail.IsBodyHtml = true;
            }
            mail.Subject = emailObject.Subject;
            if (emailObject.Attachments != null)
            {
                var fileList = GetAttachments(emailObject, tempFolder);
                foreach (var filename in fileList)
                {
                    mail.Attachments.Add(new Attachment(filename));
                }
            }
            try
            {
                result = SaveToEML(mail,tempFolder);
                TempFileManager.Instance.SetEmlFileForEmailObject(emailObject,result);
            }
            catch (Exception ex )
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return result;
        }

        public string GetAttachmentTempFileName(BaseSearchObject item)
        {
            if (item == null)
                return null;
            string mapi = item.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapi.Substring(mapi.IndexOf(ATSUFFIX) - IDLENGHT, IDLENGHT));
            string fileNameAttach = mapi.Substring(mapi.LastIndexOf(':') + 1);
            dynamic mi = GetMailItem(entryID);
            Outlook.Attachment att = GetAttacment(mi, fileNameAttach);
            if (att == null)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1} - {2}", "Attachment not found", item.ItemName, item.ItemUrl));
                return null;
            }
            string tempFileName = TempFileManager.Instance.GenerateTempFileName(item);
            if (string.IsNullOrEmpty(tempFileName))
                return null;
            try
            {
                //ProgressManager.Instance.StartOperation(new ProgressOperation() { Caption = "Saving an Attachment...", Canceled = false, DelayTime = 2500 });
                att.SaveAsFile(tempFileName);
                //ProgressManager.Instance.StopOperation();
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1} - {2}", "Save error: ", tempFileName, ex.Message));
                return null;
            }
            return tempFileName;
        }

        public List<string> GetAttachments(BaseSearchObject itemsearch)
        {
            List<string> list = new List<string>();
            if (itemsearch == null)
                return null;
            string mapiUrl = itemsearch.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic mi = GetMailItem(entryID);
            if (mi == null)
                return list;
            foreach (Outlook.Attachment att in mi.Attachments)
            {
                list.Add(att.DisplayName);
            }

            return list;
        }

        public string GetAttachmentTempFile(AttachmentContentSearchObject attachment)
        {
            if (attachment.IsNull() || string.IsNullOrEmpty(attachment.Content) || string.IsNullOrEmpty(attachment.Filename))
                return string.Empty;

            var folder = TempFileManager.Instance.GenerateTempFolderForObject(attachment);
            if (string.IsNullOrEmpty(folder))
                return string.Empty;
            string filename = string.Format("{0}\\{1}", folder, attachment.Filename);
            try
            {
                byte[] content = Convert.FromBase64String(attachment.Content);
                File.WriteAllBytes(filename, content);
                return filename;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return string.Empty;
        }

        public bool HasAttachment(BaseSearchObject item)
        {
            if (item == null)
                return false;
            string mapiUrl = item.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            var mi = GetMailItem(entryID) as Outlook.MailItem;
            if (mi == null)
                return false;
            return mi.Attachments.Count > 0;
        }

        public string GetContactFotoTempFileName(ContactSearchObject data)
        {
            if (data == null)
                return null;
            Outlook.ContactItem ci = GetContact(data);
            if (ci == null)
                return null;
            Outlook.Attachment att = GetFotoAttachment(ci);
            if (att == null)
                return null;
            //data.ItemUrl = att.DisplayName;  TODO: should be changed according our new approach

            string tempFilename = TempFileManager.Instance.GenerateTempFileName(data);

            if (string.IsNullOrEmpty(tempFilename))
                return null;
            att.SaveAsFile(tempFilename);

            return tempFilename;
        }

        public Outlook.MailItem CreateNewEmail()
        {
            if (!IsOutlookAlive() && IsHostIsApplication())
                ReopenOutlook(ref _app);
            var newMail = (Microsoft.Office.Interop.Outlook.MailItem)this.OutlookApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            return newMail;
        }

        public Outlook.MailItem CreateEmailFromTemplate(string filename)
        {
            if (!IsOutlookAlive() && IsHostIsApplication())
                ReopenOutlook(ref _app);
            var newMail = (Microsoft.Office.Interop.Outlook.MailItem) this.OutlookApp.CreateItemFromTemplate(filename);
            return newMail;
        }

        public dynamic GetEmailItem(BaseSearchObject data)
        {
            if (data == null)
                return null;
            string mapiUrl = data.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic mailItem = GetMailItem(entryID);
            return mailItem;
        }

        public Outlook.MailItem PrepareReplyEmailItem(EmailSearchObject data)
        {
            if (data.IsNull())
            {
                return null;
            }
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);

                Outlook.MailItem item = (Outlook.MailItem) this.OutlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                item.Subject = "RE : " + data.Subject;
                item.SendUsingAccount = FindSenderAccount(data);
                item.To = data.FromAddress;
                return item;
            }
            catch (Exception exception)
            {
                WSSqlLogger.Instance.LogError(exception.Message);
            }
            return null;
        }

        public Outlook.MailItem PrepareReplyAllEmailItem(EmailSearchObject data)
        {
            var email = PrepareReplyEmailItem(data);
            if (email.IsNull())
            {
                throw new NullReferenceException("reply");
            }
             if (data.To.IsNotNull())
            {
                email.To = string.Join(";", data.To.Select(r => r.Address).ToArray());    
            }
            if (data.Cc.IsNotNull())
            {
                email.CC = string.Join(";", data.Cc.Select(r => r.Address).ToArray());    
            }
            if (data.Bcc.IsNotNull())
            {
                email.BCC = string.Join(";", data.Bcc.Select(r => r.Address).ToArray());    
            }
            return email;
        }

        public Outlook.MailItem PrepareForwardEmailItem(EmailSearchObject data)
        {
            if (data.IsNull())
            {
                return null;
            }
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);

                Outlook.MailItem item = (Outlook.MailItem)this.OutlookApp.CreateItem(Outlook.OlItemType.olMailItem);
                item.Subject = "FW : " + data.Subject;
                item.SendUsingAccount = FindSenderAccount(data);
                return item;
            }
            catch (Exception exception)
            {
                WSSqlLogger.Instance.LogError(exception.Message);
            }
            return null;
        }

        private Outlook.Account FindSenderAccount(EmailSearchObject data)
        {
            Outlook.Account result = null;
            WSUIRecipient recep = data.To.FirstOrDefault();
            if (recep.IsNull())
            {
                return result;
            }

            foreach (var result1 in this.OutlookApp.Session.Accounts.OfType<Outlook.Account>())
            {
                if (result1.SmtpAddress.ToLowerInvariant().IndexOf(recep.Address.ToLowerInvariant()) > -1)
                {
                    result = result1;
                    break;
                }
            }

            if (result.IsNull())
            {
                result = this.OutlookApp.Session.Accounts.OfType<Outlook.Account>().FirstOrDefault();
            }
            return result;
        }

        public string GetFullFolderPath(BaseSearchObject data)
        {
            BaseEmailSearchObject emailSearch = data as BaseEmailSearchObject;
            if (emailSearch == null)
                return string.Empty;

            Outlook.MAPIFolder folder = null;

            if (emailSearch.IsOst)
            {
                int count = OutlookApp.Session.Stores.Count;
                for (int i = 1; i < count; i++)
                {
                    Outlook.Store store = OutlookApp.Session.Stores[i];
                    if (
                        store.GetRootFolder()
                            .EntryID.ToLowerInvariant()
                            .IndexOf(emailSearch.FolderMessageStoreIdPart.ToLowerInvariant()) > -1)
                    {
                        folder = GetOstFolder(store.GetRootFolder(), emailSearch.FolderMessageStoreIdPart,
                            emailSearch.Folder);
                    }
                }
            }
            else
            {
                int count = OutlookApp.Session.Stores.Count;
                for (int i = 1; i < count; i++)
                {
                    Outlook.Store store = OutlookApp.Session.Stores[i];
                    if (
                        store.GetRootFolder()
                            .Name.ToLowerInvariant()
                            .IndexOf(emailSearch.StorageName.ToLowerInvariant()) > -1)
                    {
                        folder = GetPstFolder(store.GetRootFolder(),emailSearch.Folder);
                    }
                }
            }
            return folder.IsNotNull() ? folder.FullFolderPath : "";
        }

        private Outlook.MAPIFolder GetOstFolder(Outlook.MAPIFolder root, string partId, string nameFolder)
        {
            if (root.EntryID.ToLowerInvariant().IndexOf(partId.ToLowerInvariant()) > -1 &&
                root.Name.ToLowerInvariant().IndexOf(nameFolder.ToLowerInvariant()) > -1)
            {
                return root;
            }
            Outlook.MAPIFolder result = null;
            if (root.Folders.Count > 0)
            {
                int count = root.Folders.Count;
                for (int i = 1; i < count; i++)
                {
                    result = GetOstFolder(root.Folders[i], partId, nameFolder);
                    if (result.IsNotNull())
                        break;
                }
            }
            return result;
        }

        private Outlook.MAPIFolder GetPstFolder(Outlook.MAPIFolder root, string nameFolder)
        {
            if (root.Name.ToLowerInvariant().IndexOf(nameFolder.ToLowerInvariant()) > -1)
            {
                return root;
            }
            Outlook.MAPIFolder result = null;
            if (root.Folders.Count > 0)
            {
                int count = root.Folders.Count;
                for (int i = 1; i < count; i++)
                {
                    result = GetPstFolder(root.Folders[i], nameFolder);
                    if (result.IsNotNull())
                        break;
                }
            }
            return result;
        }


        public List<string> GetFolderNameList()
        {
            List<string> res = new List<string>();
            if (this.OutlookApp == null)
                return res;
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");

                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                    GetOutlookFolders(folder, res);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolderNameList", ex.Message));
                return res;
            }
            res.Insert(0, OutlookHelper.AllFolders);
            return res;
        }

        public List<object> GetFolders()
        {
            List<object> res = new List<object>();
            if (this.OutlookApp == null)
                return res;
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                {
                    res.Add(folder);
                    GetOutlookFolders(folder, res);
                }
                    
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolders", ex.Message));
                return res;
            }
            return res;
        }


        public void Logoff()
        {
            if (this.OutlookApp == null || !IsHostIsApplication())
                return;
            try
            {
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                ns.Logoff();
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(string.Format("LOGOFF: {0}", e.Message));
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public string EIDFromEncodeStringWDS30(string mapi)
        {
            StringBuilder sbEID = new StringBuilder(mapi.Length);
            int Len = mapi.Length;
            for (int index = 0; index < Len; index++)
            {
                ulong offset = 0xAC00; // Hangul char range (AC00-D7AF)
                ulong ulByte = (((ulong)mapi[index]) & 0xffff) - offset;
                sbEID.AppendFormat(
                "{0:X2}", (ulByte & 0x00ff));
            }
            return sbEID.ToString();
        }

        public string GetCalendarTempFileName(BaseSearchObject itemSearch)
        {
            if (itemSearch == null)
                return string.Empty;
            string mapiUrl = itemSearch.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic appointmentItem = GetAppointment(entryID);
            if (appointmentItem == null)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Appointment not found", itemSearch.ItemUrl));
                return null;
            }
            string tempFile = TempFileManager.Instance.GenerateTempFileName(itemSearch);
            if (string.IsNullOrEmpty(tempFile))
                return null;
            if (!File.Exists(tempFile))
            {
                appointmentItem.SaveAs(tempFile, Outlook.OlSaveAsType.olHTML);
            }
            return tempFile;
        }

        public Outlook.ContactItem GetContact(string email)
        {
            if (this.OutlookApp == null)
                return null;
            Outlook.ContactItem ci = null;
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                Outlook.MAPIFolder contacts =
                        ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);

                if (contacts == null)
                    return null;

                foreach (var item in ns.Folders.OfType<Outlook.MAPIFolder>().ToList())
                {
                    try
                    {
                        foreach (var fol in item.Folders.OfType<Outlook.MAPIFolder>().ToList())
                        {
                            if (string.IsNullOrEmpty(fol.AddressBookName))
                                continue;

                            var filter =
                                string.Format(
                                    "[Email1Address] = {0} or [Email2Address] = {0} or [Email3Address] = {0}", email);
                            ci =
                                (Outlook.ContactItem)
                                    fol.Items.Find(filter);

                            if (ci != null)
                                return ci;
                        }
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError("GetContact: {0}",ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.Message));
            }
            return ci;
        }

        public IEnumerable<Outlook.ContactItem> GetContact(string firstname, string lastname)
        {
            if (this.OutlookApp == null)
                return null;
            List<Outlook.ContactItem> ci = new List<Outlook.ContactItem>();
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                
                foreach (var item in ns.Folders.OfType<Outlook.MAPIFolder>().ToList())
                {
                    try
                    {
                        foreach (var fol in item.Folders.OfType<Outlook.MAPIFolder>().ToList())
                        {
                            if (string.IsNullOrEmpty(fol.AddressBookName))
                                continue;

                            var filter =
                                string.Format("([FirstName] = {0} and [LastName] = {1}) or ([FirstName] = {1} and [LastName] = {0})", firstname, lastname);
                            var contact =
                                (Outlook.ContactItem)
                                    fol.Items.Find(filter);

                            if (contact != null)
                                ci.Add(contact);
                        }
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError("GetContact: {0}", ex.Message);
                    }
                }

            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.Message));
            }
            return ci;
        }

        #endregion public

        #region private

        private Outlook._Application GetApplication()
        {
            Outlook._Application ret = GetFromProcess();
            if (ret == null)
            {
                ret = CreateOutlookApplication();
            }

            return ret;
        }

        private Outlook._Application GetFromProcess()
        {
            Outlook._Application ret = null;
            try
            {
                var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutlookProcessName));
                if (outlook.Count() > 0)
                {
                    ret = Marshal.GetActiveObject(OutlookApplication) as Outlook._Application;
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetFromProcess", ex.Message));
                //System.Windows.MessageBox.Show(String.Format("Get Process: {0}", ex.Message));
            }

            return ret;
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
                ns.Logon(Type.Missing, "", Type.Missing, Type.Missing);//ret.DefaultProfileName
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "CreateOutlookApplication", ex.Message));
                //System.Windows.MessageBox.Show(String.Format("Create Process: {0}", ex.Message));
            }

            return ret;
        }

        private dynamic GetMailItem(string entryID)
        {
            if (this.OutlookApp == null)
                return null;
            dynamic mi = null;
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                mi = ns.GetItemFromID(entryID, Type.Missing);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetMailItem", ex.Message));
            }
            return mi;
        }

        private Outlook.Attachment GetAttacment(dynamic mail, string filename)
        {
            Outlook.Attachment att = null;
            if (mail == null)
                return null;
            try
            {
                if (mail.Attachments.Count == 0)
                    return null;
                foreach (Outlook.Attachment attach in mail.Attachments)
                {
                    if (attach.FileName == filename)
                    {
                        att = attach;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetAttachment", ex.Message));
            }
            return att;
        }

        private dynamic GetAppointment(string id)
        {
            if (this.OutlookApp == null)
                return null;
            dynamic appointItem = null;
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");

                appointItem = ns.GetItemFromID(id, Type.Missing);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetAppointment", ex.Message));
            }
            return appointItem;
        }

        /// <summary>
        /// after preview Outlook can close, that why I check Outlook process. If it closed, I create new instance
        /// </summary>
        /// <returns></returns>
        private bool IsOutlookAlive()
        {
            bool res = false;
            try
            {
                var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutlookProcessName));
                if (outlook.Count() > 0)
                    res = true;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "IsOutlookAlive", ex.Message));
            }

            return res;
        }

        private void ReopenOutlook(ref Outlook._Application app)
        {
            Marshal.ReleaseComObject(app);
            app = null;
            app = GetApplication();
            WSSqlLogger.Instance.LogWarning("Outlook was closed. Create new instance.");
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (this.OutlookApp != null)
                    {
                        Marshal.ReleaseComObject(this.OutlookApp);
                        _app = null;
                    }
                }
                _disposed = true;
            }
        }

        private void GetOutlookFolders(Outlook.MAPIFolder folder, List<string> res)
        {
            try
            {
                if (folder.Folders.Count == 0)
                    return;
                foreach (var subfolder in folder.Folders.OfType<Outlook.MAPIFolder>())
                {
                    try
                    {
                        res.Add(subfolder.Name);
                        GetOutlookFolders(subfolder, res);
                    }
                    catch (Exception e)
                    {
                        WSSqlLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders", subfolder.Name, e.Message));
                    }
                }
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetOutlookFolders", e.Message));
            }
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
                        WSSqlLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders", subfolder.Name, e.Message));
                    }
                }
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetOutlookFolders", e.Message));
            }
        }

        private Outlook.ContactItem GetContact(ContactSearchObject data)
        {
            if (this.OutlookApp == null)
                return null;
            Outlook.ContactItem ci = null;
            try
            {
                if (!IsOutlookAlive() && IsHostIsApplication())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                Outlook.MAPIFolder contacts =
                        ns.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderContacts);

                if (contacts == null)
                    return null;

                ci =
                    contacts.Items.OfType<Outlook.ContactItem>().ToList().Find(
                        c => c.FirstName == data.FirstName && c.LastName == data.LastName);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.Message));
            }
            return ci;
        }

        private Outlook.Attachment GetFotoAttachment(Outlook.ContactItem ci)
        {
            var att =
                ci.Attachments.OfType<Outlook.Attachment>().ToList().Where(a => IsPicture(a.DisplayName));
            return att.Count() > 0 ? att.ElementAt(0) : null;
        }

        private bool IsPicture(string filename)
        {
            string ext = Path.GetExtension(filename);
            if (string.IsNullOrEmpty(ext))
                return false;
            switch (ext.ToLower())
            {
                case ".jpg":
                case ".png":
                case ".bmp":
                case ".jpeg":
                    return true;
                default:
                    return false;
            }
        }

        private bool IsHostIsApplication()
        {
            return InternalHostType == HostType.Application;
        }

        private string SaveToEML(MailMessage msg, string tempFolder)
        {
            string result = string.Empty;
            using (var client = new SmtpClient())
            {
                client.UseDefaultCredentials = true;
                client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                client.PickupDirectoryLocation = tempFolder;
                client.Send(msg);
            }
            try
            {
                result = Directory.GetFiles(tempFolder, MAIL_FILTER).Single();
                string filename = Path.Combine(tempFolder, Path.GetFileNameWithoutExtension(Path.GetTempFileName()) + "." + MAIL_EXT);
                File.Copy(result, filename);
                File.Delete(result);
                result = filename;
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(e.Message);
            }
            return result;
        }

        private IEnumerable<string> GetAttachments(EmailSearchObject searchObj, string tempFolder)
        {
            var esClient = new WSUIElasticSearchClient();
            var result = esClient.Search<WSUIAttachmentContent>(s => s.Query(d => d.QueryString(qq => qq.Query(searchObj.EntryID))));
            var fileList = new List<string>();
            if (result.Documents.Any())
            {
                foreach (var attachment in result.Documents)
                {
                    try
                    {
                        byte[] content = Convert.FromBase64String(attachment.Content);
                        var filename = string.Format("{0}/{1}", tempFolder, attachment.Filename);
                        File.WriteAllBytes(filename, content);
                        fileList.Add(filename);

                    }
                    catch (Exception exception)
                    {
                        WSSqlLogger.Instance.LogError(exception.Message);
                    }
                }
            }
            return fileList;
        }

        #endregion private
    }
}