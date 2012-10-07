using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
    public partial class OutlookFilePreview : UserControl
    {
        private static string OutlookProcessName = "OUTLOOK";
        private static string OutlookApplication = "Outlook.Application";

        public OutlookFilePreview()
        {
            InitializeComponent();
        }


        #region public 


        public void LoadFile(string filename)
        {
            Outlook.Application app = GetApplication();
            if (app == null)
                return;

            Outlook.MailItem mail = (Outlook.MailItem)app.Session.OpenSharedItem(filename);

            if(mail == null)
                return;
            textBoxSubject.Text = mail.Subject;
            textBoxFrom.Text = mail.SenderName;
            textBoxTo.Text = mail.To;
            textBoxSend.Text = mail.ReceivedTime.ToString();
            webBrowserContent.DocumentText = mail.HTMLBody;

            if (mail.Attachments.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                foreach (Outlook.Attachment att  in  mail.Attachments)
                {
                    str.AppendLine(string.Format("{0};", att.DisplayName));
                }
                textBoxAttachments.Text = str.ToString();
            }
            else
                tableLayoutPanel.RowStyles[3].Height = 0;

            Marshal.ReleaseComObject(app);
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

        #endregion



    }

}
