using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;
using mshtml;
using OFPreview.PreviewHandler.Controls.Office.WebUtils;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OFPreview.PreviewHandler.Service.OutlookPreview;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.EventArguments;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace OFPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Outlook)]
    public partial class OutlookFilePreview : ExtWebBrowser, IPreviewControl, ICommandPreviewControl
    {
        private string _hitString;
        private string _fileName;
        private dynamic _outlookItem;
        private ISearchObject _searchObject;

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

        public void LoadObject(OFBaseSearchObject obj)
        {
            //
            string page = string.Empty;

            if (obj is OFEmailSearchObject)
            {
                _searchObject = obj;
                
                page = OutlookPreviewHelper.Instance.GetPreviewForEmail(obj as OFEmailSearchObject);
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

        #endregion public

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
                    if (!string.IsNullOrEmpty(_fileName))
                    {
                        path = OutlookPreviewHelper.Instance.GetPathForEmail(args.Url, _fileName);
                    }
                    if (_searchObject != null && args.Url.LocalPath != "blank")
                    {
                        path = string.Format("{0}\\{1}", OFTempFileManager.Instance.GenerateTempFolderForObject(_searchObject),args.Url.LocalPath);
                    }
                    break;

                case "uuid":
                    RaisePreviewCommandExecuted(OFPreviewCommand.ShowFolder, args.Url.LocalPath);
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
                    OFLogger.Instance.LogError(ex.Message);
                }
            }
            else
            {
                OFLogger.Instance.LogDebug("Path is empty. Outlook Preview");
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
                var tag = new OFEmailContactSearchObject() { ContactName = !data[0].IsEmail() ? data[0] : string.Empty, EMail = data[1] };
                RaisePreviewCommandExecuted(OFPreviewCommand.ShowContact, tag);
                return;
            }
            if (!string.IsNullOrEmpty(data[0]) && data[0].Contains(" "))
            {
                var names = data[0].Split(' ');
                var tag = new OFContactSearchObject() { FirstName = names[0], LastName = names[1] };
                RaisePreviewCommandExecuted(OFPreviewCommand.ShowContact, tag);
                return;
            }
        }

        private void ProcessHalfContactInformation(string info)
        {
            var names = info.Split(' ');
            var tag = new OFContactSearchObject() { FirstName = names[0], LastName = names[1] };
            RaisePreviewCommandExecuted(OFPreviewCommand.ShowContact, tag);
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

        public event EventHandler<OFPreviewCommandArgs> PreviewCommandExecuted;

        private void RaisePreviewCommandExecuted(OFPreviewCommand cmd, object tag)
        {
            var temp = PreviewCommandExecuted;
            if (temp != null)
            {
                temp(this, new OFPreviewCommandArgs(cmd, tag));
            }
        }
    }
}