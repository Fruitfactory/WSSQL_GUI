using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.Office.WebUtils;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;
using mshtml;
using WSPreview.PreviewHandler.Service.OutlookPreview;

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Outlook)]
    public partial class OutlookFilePreview : ExtWebBrowser,IPreviewControl //UserControl, IPreviewControl
    {

        private string _hitString;
        private string _fileName;

        public OutlookFilePreview()
        {
            //InitializeComponent();
            BeforeNavigate += WebEmailOnBeforeNavigate;
            //ContextMenu = outlookPreviewContextMenu.ContextMenu;
        }

        #region public

        public string HitString
        {
            get { return _hitString; }
            set
            {
                _hitString = value;
                OutlookPreviewHelper.Instance.HitString = value;
            }
        }

        public void LoadFile(string filename)
        {
            _fileName = filename;
            Outlook._Application app = OutlookPreviewHelper.Instance.OutlookApp;
            if (app == null)
                return;
            var mail = app.CreateItemFromTemplate(filename);

            if (mail == null)
                return;

            string page = string.Empty;

            if (mail is Outlook.MailItem)
            {
                page = OutlookPreviewHelper.Instance.GetPreviewForEmail(mail as Outlook.MailItem, filename);
            }
            else if (mail is Outlook.AppointmentItem)
            {
                page = OutlookPreviewHelper.Instance.GetPreviewForAppointment(mail as Outlook.AppointmentItem, filename);
            }
            else if (mail is Outlook.MeetingItem)
            {
                page = OutlookPreviewHelper.Instance.GetPreviewForMeeting(mail as Outlook.MeetingItem, filename);
            }
            DocumentText = page;
            Marshal.ReleaseComObject(mail);
        }

        public void LoadFile(Stream stream)
        {
        }

        public void Clear()
        {
            //if (webEmail != null)
            //{
            //    webEmail.DocumentText = string.Empty;
            //}
            DocumentText = string.Empty;
            Unload();
        }

        public void Unload()
        {
            OutlookPreviewHelper.Instance.DeleteAllTempFile();
        }

        #endregion

        public void CopySelectedText()
        {
            IHTMLDocument2 htmlDocument = Document.DomDocument as IHTMLDocument2;
            IHTMLSelectionObject currentSelection = htmlDocument.selection;
            if (currentSelection != null)
            {
                IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;
                if (range != null && range.text != null)
                {
                    Clipboard.SetText(range.text);
                }
            }
        }

        public void ShowContextMenu(IWSAction action)
        {
            if (outlookPreviewContextMenu == null || !this.Visible)
                return;

            outlookPreviewContextMenu.Show(this,this.PointToClient((Point)action.Data));
        }




        private void WebEmailOnBeforeNavigate(object sender, WebBrowserNavigatingEventArgs args)
        {
            string path = string.Empty;
            switch (args.Url.Scheme)
            {
                case "https":
                case "mailto":
                case "http":
                    path = args.Url.AbsoluteUri;
                    break;
                case "about":
                case "re":
                    path = OutlookPreviewHelper.Instance.GetPathForEmail(args.Url, _fileName);
                    break;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    args.Cancel = true;
                    Process.Start(path);
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError(ex.Message);
                }
            }
            else
            {
                WSSqlLogger.Instance.LogInfo("Path is empty. Outlook Preview");
            }
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            CopySelectedText();
        }

    }

}
