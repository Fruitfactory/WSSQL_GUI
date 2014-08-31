using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Diagnostics;
using System.Runtime.InteropServices;


namespace WSUI.Core.Helpers
{
    public class OutlookHelper : IOutlookHelper, IDisposable
    {
#region static
        private static string OutlookProcessName = "OUTLOOK";
        private static string OutlookApplication = "Outlook.Application";
        //private static OutlookHelper _instance= null;
        private static readonly object _lockObject = new object();
#endregion

        private const string ATSUFFIX = "/at=";
        private const int IDLENGHT = 24;

        public const string AllFolders = "All folders";

#region fields
        
        private bool _disposed;
        private Outlook._Application _app;
#endregion

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

        public static  OutlookHelper Instance
        {
            get
            {
                return _instance.Value;
            }
        }

        

        #region [Outllok application]

        internal HostType InternalHostType
        {
            get; set;
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


        #endregion


        #region public

        public string GetCurrentyUserEmail()
        {
            if(OutlookApp == null)
                return null;
            var activeExp = OutlookApp.ActiveExplorer();
            if(activeExp == null)
                return null;
            return activeExp.Session.CurrentUser.AddressEntry.Address;
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

        public bool HasAttachment(BaseSearchObject item)
        {
            if(item == null)
                return false;
            string mapiUrl = item.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            var mi = (Outlook.MailItem)GetMailItem(entryID);
            if (mi == null)
                return false;
            return mi.Attachments.Count > 0;
        }

        public string GetContactFotoTempFileName(ContactSearchObject data)
        {
            if (data == null)
                return string.Empty;
            string mapiUrl = data.ItemUrl;
            Outlook.ContactItem ci = GetContact(data);
            if (ci == null)
                return string.Empty;
            Outlook.Attachment att  = GetFotoAttachment(ci);
            if (att == null)
                return string.Empty;
            data.ItemUrl = att.DisplayName;

            string tempFilename = TempFileManager.Instance.GenerateTempFileName(data);
            
            if (string.IsNullOrEmpty(tempFilename))
                return string.Empty;
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

        public dynamic GetEmailItem(BaseSearchObject data)
        {
            if (data == null)
                return null;
            string mapiUrl = data.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic mailItem = GetMailItem(entryID);
            return mailItem;
        }

        public string GetFullFolderPath(BaseSearchObject data)
        {
            if (data == null) 
                return string.Empty;
            Outlook.MailItem item = GetEmailItem(data);
            if (item == null)
                return string.Empty;

            var folder = item.Parent as Outlook.MAPIFolder;
            if (folder == null)
                return string.Empty;
            return folder.FullFolderPath;
        }

        public List<string> GetFolderList()
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
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetFolderList", ex.Message));
                return res;
            }
            res.Insert(0,OutlookHelper.AllFolders);
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
                WSSqlLogger.Instance.LogError(string.Format("LOGOFF: {0}",e.Message));
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
            if(itemSearch == null)
                return string.Empty;
            string mapiUrl = itemSearch.ItemUrl;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            dynamic appointmentItem = GetAppointment(entryID);
            if (appointmentItem == null)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}","Appointment not found",itemSearch.ItemUrl));
                return null;
            }
            string tempFile = TempFileManager.Instance.GenerateTempFileName(itemSearch);
            if (string.IsNullOrEmpty(tempFile))
                return null;
            if (!File.Exists(tempFile))
            {
                appointmentItem.SaveAs(tempFile,Outlook.OlSaveAsType.olHTML);
            }
            return tempFile;
        }

        public Outlook.ContactItem GetContact(string fullname)
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

                string[] split = fullname.Split(' ');
                if (split.Length <= 1)
                    return null;
                foreach (var item in ns.Folders.OfType<Outlook.MAPIFolder>())
                {
                    try
                    {
                        foreach (var fol in item.Folders.OfType<Outlook.MAPIFolder>())
                        {
                            if (string.IsNullOrEmpty(fol.AddressBookName))
                                continue;
                            foreach (var contact in fol.Items.OfType<Outlook.ContactItem>())
                            {
                                if (contact != null && (contact.FirstName == split[0].Trim() && contact.LastName == split[1].Trim()) || (contact.FirstName == split[1].Trim() && contact.LastName == split[0].Trim()))
                                {
                                    ci = contact;
                                    return ci;
                                } 
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "GetContact", ex.Message));
            }
            return ci;            
        }
    

        #endregion

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

        private Outlook.Attachment GetAttacment(dynamic mail,string filename)
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
                if(!IsOutlookAlive() && IsHostIsApplication())
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
                        WSSqlLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders",subfolder.Name, e.Message));
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
            return att.Count() > 0 ? att.ElementAt(0): null ;
        }


        private  bool IsPicture(string filename)
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

        #endregion

    }
}
