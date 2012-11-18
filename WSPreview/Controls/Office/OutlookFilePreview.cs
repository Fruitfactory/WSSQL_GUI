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
using System.Text.RegularExpressions;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
    public partial class OutlookFilePreview : UserControl
    {
        private static string AfterStrongTemplate = "<font style='background-color: yellow'><strong>{0}</strong></font>";
        private static string OutlookProcessName = "OUTLOOK";
        private static string OutlookApplication = "Outlook.Application";

        private static string PageTemplate =
            "<html><body leftmargin='0' style='font: bold 11; font-family: Microsoft Sans Serif'>{0}</body></html>";

        private bool _IsExistProcess = false;

        public OutlookFilePreview()
        {
            InitializeComponent();
        }


        #region public 

        public string HitString { get; set; }

        public void LoadFile(string filename)
        {
            Outlook.Application app = GetApplication();
            if (app == null)
                return;

            Outlook.MailItem mail = (Outlook.MailItem)app.CreateItemFromTemplate(filename);

            if (mail == null)
                return;
            textBoxSubject.Text = mail.Subject;
            webFrom.DocumentText = HighlightSearchString(string.Format(PageTemplate, mail.SenderName));
            textBoxTo.Text = mail.To;
            textBoxSend.Text = mail.ReceivedTime.ToString();
            webBrowserContent.DocumentText = HighlightSearchString(mail.HTMLBody);

            if (mail.Attachments.Count > 0)
            {
                StringBuilder str = new StringBuilder();
                foreach (Outlook.Attachment att in mail.Attachments)
                {
                    str.AppendLine(string.Format("{0};", att.DisplayName));
                }
                textBoxAttachments.Text = str.ToString();
            }
            else
                tableLayoutPanel.RowStyles[3].Height = 0;

            Marshal.ReleaseComObject(mail);
            CloseApplication(app);


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
                    _IsExistProcess = true;
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

        private void CloseApplication(Outlook.Application app)
        {
            if (!_IsExistProcess)
            {
                app.Quit();
                Marshal.ReleaseComObject(app);
            }
        }

        private string HighlightSearchString(string inputString)
        {
            if (HitString == null)
                return inputString;
            string result = inputString;
            var itemArray = HitString.Trim().Split(' ');
            foreach (var s in itemArray)
            {
                var escape = Regex.Escape(s);
                result = Regex.Replace(result, Regex.Escape(s), string.Format(AfterStrongTemplate, s), RegexOptions.IgnoreCase);
            }
            return result;
        }

        #endregion



    }

}
