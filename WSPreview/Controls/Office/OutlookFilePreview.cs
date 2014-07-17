using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using WSPreview.PreviewHandler.Controls.Office.WebUtils;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSUI.Core.Enums;
using WSUI.Core.EventArguments;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;
using mshtml;
using WSPreview.PreviewHandler.Service.OutlookPreview;

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Outlook)]
    public partial class OutlookFilePreview : ExtWebBrowser,IPreviewControl,ICommandPreviewControl
    {

        private string _hitString;
        private string _fileName;

        public OutlookFilePreview()
        {
            BeforeNavigate += WebEmailOnBeforeNavigate;
            Navigating += OnNavigating;
            Navigated += OnNavigated;
            ScriptErrorsSuppressed = true;
        }

        private void OnNavigated(object sender, WebBrowserNavigatedEventArgs webBrowserNavigatedEventArgs)
        {
            
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

        public string FullFolderPath
        {
            set { OutlookPreviewHelper.Instance.FullFolderPath = value; }
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
            if(!Focused)
                return;
            
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

        private void OnNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {

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
                case "uuid":
                    RaisePreviewCommandExecuted(WSPreviewCommand.ShowFolder,args.Url.LocalPath);
                    break;
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    System.Diagnostics.Debug.WriteLine(GetDefaultBrowserPath());
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

        public static string GetDefaultBrowserPath()
        {
            string defaultBrowserPath = null;
            RegistryKey regkey;

            // Check if we are on Vista or Higher
            OperatingSystem OS = Environment.OSVersion;
            if ((OS.Platform == PlatformID.Win32NT) && (OS.Version.Major >= 6))
            {
                regkey = Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command", false);
                if (regkey != null)
                {
                    defaultBrowserPath = regkey.GetValue("").ToString();
                }
                else
                {
                    regkey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Classes\\IE.HTTP\\shell\\open\\command", false);
                    defaultBrowserPath = regkey.GetValue("").ToString();
                }
            }
            else
            {
                regkey = Registry.ClassesRoot.OpenSubKey("http\\shell\\open\\command", false);
                defaultBrowserPath = regkey.GetValue("").ToString();
            }

            return defaultBrowserPath;
        }

        public event EventHandler<WSUIPreviewCommandArgs> PreviewCommandExecuted;

        private void RaisePreviewCommandExecuted(WSPreviewCommand cmd, object tag)
        {
            var temp = PreviewCommandExecuted;
            if (temp != null)
            {
                temp(this,new WSUIPreviewCommandArgs(cmd,tag));
            }
        }
    }

}
