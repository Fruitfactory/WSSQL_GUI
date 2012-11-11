using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using WSUI.Infrastructure.Controls.ProgressManager;
using WSUI.Infrastructure.Service.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Diagnostics;
using System.Runtime.InteropServices;
using C4F.DevKit.PreviewHandler.Service.Logger;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Models;


namespace WSUI.Infrastructure.Service.Helpers
{
    public class OutlookHelper : IOutlookHelper, IDisposable
    {
#region static
        private static string OutlookProcessName = "OUTLOOK";
        private static string OutlookApplication = "Outlook.Application";
        private static OutlookHelper _instance= null;
        private static readonly object _lockObject = new object();
#endregion

        private const string ATSUFFIX = "/at=";
        private const int IDLENGHT = 24;

#region fields
        
        private bool _disposed;
        private Outlook.Application _app;
#endregion


        private OutlookHelper()
        {
            _disposed = false;
            _app = GetApplication();
        }

        public static  OutlookHelper Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                        _instance = new OutlookHelper();

                    return _instance;
                }
            }
        }

        #region public
        
        public string GetEMailTempFileName(BaseSearchData itemsearch)
        {
            if (itemsearch == null)
                return null;
            string mapiUrl = itemsearch.Path;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            Outlook.MailItem mailItem = GetMailItem(entryID);
            if (mailItem == null)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Mail not found", itemsearch.Path));
                return null;
            }
            string tempFilename = TempFileManager.Instance.GenerateTempFileName(itemsearch);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            if (!File.Exists(tempFilename))
            {
                ProgressManager.Instance.StartOperation(new ProgressOperation(){Caption = tempFilename,Canceled = false,DelayTime = 1500});
                mailItem.SaveAs(tempFilename, Type.Missing);
                ProgressManager.Instance.StopOperation();
            }

            return tempFilename;
        }

        public string GetAttachmentTempFileName(WSUI.Infrastructure.Core.BaseSearchData item)
        {
            if (item == null)
                return null;
            string mapi = item.Path;
            string entryID = EIDFromEncodeStringWDS30(mapi.Substring(mapi.IndexOf(ATSUFFIX) - IDLENGHT, IDLENGHT));
            string fileNameAttach = mapi.Substring(mapi.LastIndexOf(':') + 1);
            Outlook.MailItem mi = GetMailItem(entryID);
            Outlook.Attachment att = GetAttacment(mi, fileNameAttach);
            if (att == null)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1} - {2}", "Attachment not found", item.Name, item.Path));
                return null;
            }
            string tempFileName = TempFileManager.Instance.GenerateTempFileName(item);
            if (string.IsNullOrEmpty(tempFileName))
                return null;
            try
            {
                att.SaveAsFile(tempFileName);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1} - {2}", "Save error: ", tempFileName, ex.Message));
                return null;
            }
            return tempFileName;
        }

        public List<string> GetAttachments(BaseSearchData itemsearch)
        {
            List<string> list = new List<string>();
            if (itemsearch == null)
                return null;
            string mapiUrl = itemsearch.Path;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            Outlook.MailItem mi = GetMailItem(entryID);
            if (mi == null)
                return list;
            foreach (Outlook.Attachment att in mi.Attachments)
            {
                list.Add(att.DisplayName);
            }
            
            return list;
        }

        public string GetContactFotoTempFileName(ContactSearchData data)
        {
            if (data == null)
                return string.Empty;
            string mapiUrl = data.Path;
            Outlook.ContactItem ci = GetContact(data);
            if (ci == null)
                return string.Empty;
            Outlook.Attachment att  = GetFotoAttachment(ci);
            if (att == null)
                return string.Empty;
            data.Path = att.DisplayName;

            string tempFilename = TempFileManager.Instance.GenerateTempFileName(data);
            
            if (string.IsNullOrEmpty(tempFilename))
                return string.Empty;
            att.SaveAsFile(tempFilename);

            return tempFilename;
        }

        public Outlook.MailItem CreateNewEmail()
        {
            var newMail = (Microsoft.Office.Interop.Outlook.MailItem)_app.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);
            return newMail;
        }

        public List<string> GetFolderList()
        {
            List<string> res = new List<string>();
            if (_app == null)
                return res;
            try
            {
                if (!IsOutlookAlive())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = _app.GetNamespace("MAPI");
                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                    GetOutlookFolders(folder, res);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
                return res;
            }
            return res;
        }

        public void Logoff()
        {
            if (_app == null)
                return;
            try
            {
                Outlook.NameSpace ns = _app.GetNamespace("MAPI");
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


        #endregion

        #region private
        

        private Outlook.Application GetApplication()
        {
            Outlook.Application ret = GetFromProcess();
            if (ret == null)
            {
                ret = CreateOutlookApplication();
            }

            return ret;
        }

        private Outlook.Application GetFromProcess()
        {
            Outlook.Application ret = null;
            try
            {
                var outlook = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutlookProcessName));
                if (outlook.Count() > 0)
                {
                    ret = Marshal.GetActiveObject(OutlookApplication) as Outlook.Application;
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }

            return ret;
        }

        private Outlook.Application CreateOutlookApplication()
        {

            Outlook.Application ret = null;
            try
            {
                ret = new Outlook.Application(); 
                if (ret == null)
                    return ret;
                Outlook.NameSpace ns = ret.GetNamespace("MAPI");
                ns.Logon(ret.DefaultProfileName, "", Type.Missing, Type.Missing);//ret.DefaultProfileName
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }

            return ret;
        }

        private Outlook.MailItem GetMailItem(string entryID)
        {
            if (_app == null)
                return null;
            Outlook.MailItem mi = null;
            try
            {
                if (!IsOutlookAlive())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = _app.GetNamespace("MAPI");
                
           
                mi = ns.GetItemFromID(entryID, Type.Missing);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return mi;
        }

        private Outlook.Attachment GetAttacment(Outlook.MailItem mail,string filename)
        {
            Outlook.Attachment att = null;
            if (mail == null)
                return null;
            try
            {
                if (mail.Attachments.Count == 0)
                    return null;
                var listAttachment = mail.Attachments.OfType<Outlook.Attachment>().Where(a => a.FileName == filename).ToList();
                if (listAttachment.Count == 0)
                    return null;
                att = listAttachment[0];
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return att;
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
                WSSqlLogger.Instance.LogError(ex.Message);
            }

            return res;
        }

        private void ReopenOutlook(ref Outlook.Application app)
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
                    if (_app != null)
                    {
                        Marshal.ReleaseComObject(_app);
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
                    res.Add(subfolder.Name);
                    GetOutlookFolders(subfolder, res);
                }
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(e.Message);
            }
        }


        private Outlook.ContactItem GetContact(ContactSearchData data)
        {
            if (_app == null)
                return null;
            Outlook.ContactItem ci = null;
            try
            {
                if (!IsOutlookAlive())
                    ReopenOutlook(ref _app);
                Outlook.NameSpace ns = _app.GetNamespace("MAPI");
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
                WSSqlLogger.Instance.LogError(ex.Message);
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

        #endregion

    }
}
