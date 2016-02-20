using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Win32;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OF.Core.Helpers
{
    public class OFOutlookHelper : IOutlookHelper, IDisposable
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

        #region fields

        private bool _disposed;
        private Outlook._Application _app;

        #endregion fields

        private static readonly Lazy<OFOutlookHelper> _instance = new Lazy<OFOutlookHelper>(() =>
                                                                                   {
                                                                                       var obj = new OFOutlookHelper();
                                                                                       return obj;
                                                                                   });

        private OFOutlookHelper()
        {
            _disposed = false;
            InternalHostType = OFHostType.Unknown;
        }

        public static OFOutlookHelper Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        #region [Outllok application]

        internal OFHostType InternalHostType
        {
            get;
            set;
        }

        public Outlook._Application OutlookApp
        {
            get
            {
                if (InternalHostType == OFHostType.Unknown && _app == null)
                {
                    _app = CreateOutlookApplication();
                }
                return _app;
            }
            set
            {
                _app = value;
                InternalHostType = OFHostType.Plugin;
            }
        }

        #endregion [Outllok application]

        #region public

        public string GetOfficeVersion(Outlook._Application app)
        {
            var officeVersion = string.Empty;
            try
            {
                if ((app != null))
                {
                    string hostVersion = app.Version;
                    if (hostVersion.StartsWith("9.0"))
                    {
                        officeVersion = "9.0";
                    }
                    if (hostVersion.StartsWith("10.0"))
                    {
                        officeVersion = "10.0";
                    }
                    if (hostVersion.StartsWith("11.0"))
                    {
                        officeVersion = "11.0";
                    }
                    if (hostVersion.StartsWith("12.0"))
                    {
                        officeVersion = "12.0";
                    }
                    if (hostVersion.StartsWith("14.0"))
                    {
                        officeVersion = "14.0";
                    }
                    if (hostVersion.StartsWith("15.0"))
                    {
                        officeVersion = "15.0";
                    }
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            return officeVersion;
        }


        public Tuple<string,string> GetCurrentyUserInfo()
        {
            if (OutlookApp == null)
                return null;
            var activeExp = OutlookApp.ActiveExplorer();
            if (activeExp == null)
                return null;
            return new Tuple<string, string>(activeExp.Session.CurrentUser.Name, activeExp.Session.CurrentUser.AddressEntry.Address);
        }

        public string GetEMailTempFileName(OFBaseSearchObject itemsearch)
        {
            if (itemsearch == null)
                return null;
            string mapiUrl = itemsearch.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic mailItem = GetMailItem(entryID);
            if (mailItem == null)
            {
                OFLogger.Instance.LogWarning(string.Format("{0}: {1}", "Mail not found", itemsearch.ItemUrl));
                return null;
            }
            
            string tempFilename = OFTempFileManager.Instance.GenerateTempFileName(itemsearch);
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

        

        public string GetAttachmentTempFileName(OFBaseSearchObject item)
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
                OFLogger.Instance.LogWarning(string.Format("{0}: {1} - {2}", "Attachment not found", item.ItemName, item.ItemUrl));
                return null;
            }
            string tempFileName = OFTempFileManager.Instance.GenerateTempFileName(item);
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
                OFLogger.Instance.LogWarning(string.Format("{0}: {1} - {2}", "Save error: ", tempFileName, ex.ToString()));
                return null;
            }
            return tempFileName;
        }

        public List<string> GetAttachments(OFBaseSearchObject itemsearch)
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

        public string GetAttachmentTempFile(OFAttachmentContentSearchObject attachment)
        {
            if (attachment.IsNull() || string.IsNullOrEmpty(attachment.Filename))
                return string.Empty;

            var folder = OFTempFileManager.Instance.GenerateTempFolderForObject(attachment);
            if (string.IsNullOrEmpty(folder))
                return string.Empty;
            string filename = string.Format("{0}\\{1}", folder, attachment.Filename);


            if (!string.IsNullOrEmpty(attachment.Content) && string.IsNullOrEmpty(attachment.Outlookemailid))
            {
                try
                {
                    byte[] content = Convert.FromBase64String(attachment.Content);
                    File.WriteAllBytes(filename, content);
                    return filename;
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
            }
            else
            {
                try
                {
                    var email = OFOutlookHelper.Instance.GetEmailItem(attachment.Outlookemailid);
                    var att = OFOutlookHelper.Instance.GetAttacment(email, attachment.Filename) as Outlook.Attachment;
                    if (att.IsNotNull())
                    {
                        att.SaveAsFile(filename);
                        return filename;
                    }
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
            }
            return string.Empty;
        }

        public bool HasAttachment(OFBaseSearchObject item)
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

        public string GetContactFotoTempFileName(OFContactSearchObject data)
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

            string tempFilename = OFTempFileManager.Instance.GenerateTempFileName(data);

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

        public dynamic GetEmailItem(string entryId)
        {
            if (string.IsNullOrEmpty(entryId))
            {
                return null;
            }
            return GetMailItem(entryId);
        }

        public dynamic GetEmailItem(OFBaseSearchObject data)
        {
            if (data == null)
                return null;
            string mapiUrl = data.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic mailItem = GetMailItem(entryID);
            return mailItem;
        }

        public Outlook.MailItem PrepareReplyEmailItem(OFEmailSearchObject data)
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
                string subject = data.Subject;
                if (!subject.ToLowerInvariant().Contains("re:"))
                {
                    subject = string.Format("Re: {0}", subject);
                }
                item.Subject = subject;
                item.SendUsingAccount = FindSenderAccount(data);
                item.To = (new OFRecipient() {Address = data.FromAddress, Name = data.FromName}).ToString();

                return item;
            }
            catch (Exception exception)
            {
                OFLogger.Instance.LogError(exception.Message);
            }
            return null;
        }

        public Outlook.MailItem PrepareReplyAllEmailItem(OFEmailSearchObject data)
        {
            var email = PrepareReplyEmailItem(data);
            if (email.IsNull())
            {
                throw new NullReferenceException("reply");
            }
            var listTo = new List<OFRecipient>(){new OFRecipient(){Address = data.FromAddress, Name = data.FromName}};
             if (data.To.IsNotNull())
            {
                 listTo.AddRange(data.To);
                
            }
            email.To =  string.Join(";", listTo.Select(r => r.ToString()).ToArray());
            if (data.Cc.IsNotNull())
            {
                email.CC = string.Join(";", data.Cc.Select(r => r.ToString()).ToArray());
            }
            if (data.Bcc.IsNotNull())
            {
                email.BCC = string.Join(";", data.Bcc.Select(r => r.ToString()).ToArray());
            }
            return email;
        }

        public Outlook.MailItem PrepareForwardEmailItem(OFEmailSearchObject data)
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
                string subject = data.Subject;
                if (!subject.ToLowerInvariant().Contains("fw:"))
                {
                    subject = string.Format("FW: {0}", subject);
                }
                item.Subject = subject;
                item.SendUsingAccount = FindSenderAccount(data);
                return item;
            }
            catch (Exception exception)
            {
                OFLogger.Instance.LogError(exception.Message);
            }
            return null;
        }

        private Outlook.Account FindSenderAccount(OFEmailSearchObject data)
        {
            Outlook.Account result = null;

            string email = this.OutlookApp.Session.CurrentUser.AddressEntry.GetEmailAddress();

            foreach (var result1 in this.OutlookApp.Session.Accounts.OfType<Outlook.Account>())
            {
                if (result1.SmtpAddress.ToLowerInvariant().IndexOf(email.ToLowerInvariant()) > -1)
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

        public string GetFullFolderPath(OFEmailSearchObject data)
        {
            OFBaseEmailSearchObject emailSearch = data as OFBaseEmailSearchObject;
            if (emailSearch == null)
                return string.Empty;

            Outlook.MAPIFolder folder = OutlookApp.Session.GetFolderFromID(data.FolderMessageStoreIdPart);

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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolderNameList", ex.ToString()));
                return res;
            }
            res.Insert(0, OFOutlookHelper.AllFolders);
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolders", ex.ToString()));
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
                OFLogger.Instance.LogError(string.Format("LOGOFF: {0}", e.Message));
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

        public string GetCalendarTempFileName(OFBaseSearchObject itemSearch)
        {
            if (itemSearch == null)
                return string.Empty;
            string mapiUrl = itemSearch.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic appointmentItem = GetAppointment(entryID);
            if (appointmentItem == null)
            {
                OFLogger.Instance.LogWarning(string.Format("{0}: {1}", "Appointment not found", itemSearch.ItemUrl));
                return null;
            }
            string tempFile = OFTempFileManager.Instance.GenerateTempFileName(itemSearch);
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
                        OFLogger.Instance.LogError("GetContact: {0}",ex.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.ToString()));
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
                        OFLogger.Instance.LogError("GetContact: {0}", ex.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.ToString()));
            }
            return ci;
        }

        public static IEnumerable<string> GetOutlookFiles()
        {
            string path = string.Format("{0}\\Microsoft\\Outlook",
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
            if (Directory.Exists(path))
            {
#if DEBUG
                var files = Directory.GetFiles(path, "*.pst"); ;// new List<string>() { "F:\\Visual\\WORK\\vincent@metajure.com.ost", "F:\\Visual\\WORK\\vpayette@hotmail.com.ost" }; // // new List<string>(){"c:\\Users\\Yariki\\AppData\\Local\\Microsoft\\Outlook\\iyariki.ya@hotmail.com.ost"};    
                var files1 = Directory.GetFiles(path, "*.ost");
                var list = new List<string>(files);
                list.AddRange(files1);

                foreach (var file in list)
                {
                    System.Diagnostics.Debug.WriteLine(file);
                }
                return list;
#else
                var files = Directory.GetFiles(path, "*.pst");
                var files1 = Directory.GetFiles(path, "*.ost");
                var list = new List<string>(files);
                list.AddRange(files1);

                foreach (var file in list)
                {
                    System.Diagnostics.Debug.WriteLine(file);
                }
                return list;
#endif
            }
            return null;
        }

        public Outlook.Attachment GetAttacment(dynamic mail, string filename)
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetAttachment", ex.ToString()));
            }
            return att;
        }

        #endregion public

        #region private

        public Tuple<object,bool> GetApplication()
        {
            bool isProcess = true;
            Outlook._Application ret = GetFromProcess();
            if (ret == null)
            {
                isProcess = false;
                ret = CreateOutlookApplication();
            }

            return new Tuple<object, bool>(ret, isProcess);
        }

        private Outlook._Application GetFromProcess()
        {
            Outlook._Application ret = null;
            try
            {
                var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutlookProcessName));
                if (outlook.Count() > 0 && WindowsFunction.IsOutlookRegisteredInROT())
                {
                    OFLogger.Instance.LogInfo("Outlook is existing and have been registered in ROT...");
                    ret = Marshal.GetActiveObject(OutlookApplication) as Outlook._Application;
                }
                else if (outlook.Count() > 0)
                {
                    WindowsFunction.SetShellWindowActive();
                    Thread.Sleep(1000);
                    OFLogger.Instance.LogInfo("Outlook is existing and trying to be register in ROT...");
                    ret = Marshal.GetActiveObject(OutlookApplication) as Outlook._Application;
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError( ex.ToString());
            }

            return ret;
        }

        private Outlook._Application CreateOutlookApplication()
        {
            Outlook._Application ret = null;
            try
            {
                var ev = new AutoResetEvent(false);
                Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(2500);
                    var hwnd = WindowsFunction.SearchForWindow("#32770", "Choose Profile");
                    if (hwnd != IntPtr.Zero)
                    {
                        var btnHwnd = WindowsFunction.FindWindowEx(hwnd, IntPtr.Zero, "Button", "OK");
                        Console.WriteLine(btnHwnd);
                        WindowsFunction.SendMessage((int)btnHwnd, WindowsFunction.BN_CLICKED, 0, IntPtr.Zero);
                    }
                    ev.Set();
                });

                ret = new Outlook.Application() as Outlook._Application;
                ev.WaitOne();
                Outlook.NameSpace ns = ret.GetNamespace("MAPI");
                ns.Logon(ret.DefaultProfileName, "", false, true);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "CreateOutlookApplication", ex.ToString()));
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetMailItem", ex.ToString()));
            }
            return mi;
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetAppointment", ex.ToString()));
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "IsOutlookAlive", ex.ToString()));
            }

            return res;
        }

        private void ReopenOutlook(ref Outlook._Application app)
        {
            Marshal.ReleaseComObject(app);
            app = null;
            app = GetApplication().Item1 as Outlook._Application;
            OFLogger.Instance.LogWarning("Outlook was closed. Create new instance.");
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
                        OFLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders", subfolder.Name, e.Message));
                    }
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetOutlookFolders", e.Message));
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
                        OFLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders", subfolder.Name, e.Message));
                    }
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetOutlookFolders", e.Message));
            }
        }

        private Outlook.ContactItem GetContact(OFContactSearchObject data)
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.ToString()));
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
            return InternalHostType == OFHostType.Application;
        }

        

        

        #endregion private
    }
}