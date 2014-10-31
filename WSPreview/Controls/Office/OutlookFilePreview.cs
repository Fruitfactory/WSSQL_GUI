using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using WSPreview.PreviewHandler.Controls.Office.WebUtils;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.EventArguments;
using WSUI.Core.Extensions;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;
using mshtml;
using WSPreview.PreviewHandler.Service.OutlookPreview;

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Outlook)]
    public partial class OutlookFilePreview : ExtWebBrowser, IPreviewControl, ICommandPreviewControl
    {

        private string _hitString;
        private string _fileName;
        private dynamic _outlookItem;

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
            _outlookItem = app.CreateItemFromTemplate(filename);

            if (_outlookItem == null)
                return;

            string page = string.Empty;

            if (_outlookItem is Outlook.MailItem)
            {
                page = OutlookPreviewHelper.Instance.GetPreviewForEmail(_outlookItem as Outlook.MailItem, filename);
            }
            else if (_outlookItem is Outlook.AppointmentItem)
            {
                page = OutlookPreviewHelper.Instance.GetPreviewForAppointment(_outlookItem as Outlook.AppointmentItem, filename);
            }
            else if (_outlookItem is Outlook.MeetingItem)
            {
                page = OutlookPreviewHelper.Instance.GetPreviewForMeeting(_outlookItem as Outlook.MeetingItem, filename);
            }
            DocumentText = page;
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
            if (_outlookItem != null)
            {
                Marshal.ReleaseComObject(_outlookItem);
            }
        }

        #endregion

        public void CopySelectedText()
        {
            if (!Focused)
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
                    RaisePreviewCommandExecuted(WSPreviewCommand.ShowFolder, args.Url.LocalPath);
                    break;
                case "fax":
                    SendShowContactCommand(args.Url.LocalPath);
                    return;
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

        private void SendShowContactCommand(string localPath)
        {
            if (string.IsNullOrEmpty(localPath))
            {
                return;
            }
            if (localPath.Contains(":"))
            {
                ProcessFullContactInformation(localPath);
                return;
            }
            if (localPath.Contains(" "))
            {
                ProcessHalfContactInformation(localPath);
                return;
            }
        }


        private void ProcessFullContactInformation(string info)
        {
            var data = info.Split(':');
            if (data.Length < 2)
            {
                return;
            }

            if (data[1].IsEmail() || data.All(s => s.IsEmail()))
            {
                var tag = new EmailContactSearchObject() { ContactName = !data[0].IsEmail() ? data[0] : string.Empty, EMail = data[1] };
                RaisePreviewCommandExecuted(WSPreviewCommand.ShowContact, tag);
                return;
            }
            if (!string.IsNullOrEmpty(data[0]) && data[0].Contains(" "))
            {
                var names = data[0].Split(' ');
                var tag = new ContactSearchObject() { FirstName = names[0], LastName = names[1] };
                RaisePreviewCommandExecuted(WSPreviewCommand.ShowContact, tag);
                return;
            }
        }

        private void ProcessHalfContactInformation(string info)
        {
            var names = info.Split(' ');
            var tag = new ContactSearchObject() { FirstName = names[0], LastName = names[1] };
            RaisePreviewCommandExecuted(WSPreviewCommand.ShowContact, tag);
            return;
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
                temp(this, new WSUIPreviewCommandArgs(cmd, tag));
            }
        }
    }

}
