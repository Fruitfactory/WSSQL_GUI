using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSSQLGUI.Services.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WSSQLGUI.Services.Helpers
{
    class OutlookHelper : IOutlookHelper, IDisposable
    {
#region static
        private string OutlookProcessName = "OUTLOOK";
        private string OutlookApplication = "Outlook.Application";
        private static OutlookHelper _instance= null;
        private static readonly object _lockObject = new object();
#endregion
        
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
        
        public string GetEMailTempFileName(WSSQLGUI.Models.SearchItem itemsearch)
        {
            if (itemsearch == null)
                return null;
            string mapiUrl = itemsearch.FileName;
            string entryID = EIDFromEncodeStringWDS30(mapiUrl.Substring(mapiUrl.LastIndexOf('/') + 1));
            Outlook.MailItem mailItem = GetMailItem(entryID);
            if (mailItem == null)
                return null;
            string tempFilename = TempFileManager.Instance.GenerateTempFileName(itemsearch);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            mailItem.SaveAs(tempFilename, Type.Missing);

            return tempFilename;
        }

        public string GetAttachmentTempFileName(WSSQLGUI.Models.SearchItem item)
        {
            throw new NotImplementedException();
        }
       
        public void Logoff()
        {
            if (_app == null)
                return;
            Outlook.NameSpace ns = _app.GetNamespace("MAPI");
            ns.Logoff();
        }

        public void Dispose()
        {
            Dispose(true);
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
                MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                ns.Logon("", "", Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return ret;
        }

        private Outlook.MailItem GetMailItem(string entryID)
        {
            if (_app == null)
                return null;
            Outlook.NameSpace ns = _app.GetNamespace("MAPI");
            Outlook.MailItem mi = null;
            try
            {
                mi = ns.GetItemFromID(entryID, Type.Missing);
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("Error: {0}", ex.Message), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return mi;
        }

        private object GetAttacment(string entryID, string filename)
        {
            throw new System.NotImplementedException();
        }

        private string SaveTempAttachment(object attach)
        {
            throw new System.NotImplementedException();
        }

        private string EIDFromEncodeStringWDS30(string mapi)
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

        #endregion

    }
}
