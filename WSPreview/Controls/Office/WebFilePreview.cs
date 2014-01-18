using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.Office.WebUtils;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using WSUI.Core.Logger;

namespace WSPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Web)]
    public class WebFilePreview : ExtWebBrowser, IPreviewControl
    {

        public WebFilePreview()
        {
            BeforeNavigate += OnBeforeNavigate;
            Navigating += OnNavigating;
            ScriptErrorsSuppressed = true;
        }

        public void LoadFile(string filename)
        {
            if (!File.Exists(filename))
                return;
            try
            {
                var content = File.ReadAllText(filename);
                DocumentText = content;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Web: {0}",ex.Message);
            }
        }

        public void LoadFile(Stream stream)
        {
            throw new System.NotImplementedException();
        }

        public void Clear()
        {
            DocumentText = string.Empty;
        }

        #region [private]

        private void OnNavigating(object sender, WebBrowserNavigatingEventArgs e)
        {
            
        }


        private void OnBeforeNavigate(object sender, WebBrowserNavigatingEventArgs e)
        {
            if( e == null)
                return;
            string path = String.Empty;
            
            switch (e.Url.Scheme)
            {
                case "https":
                case "http":
                    path = e.Url.AbsoluteUri;
                    break;
            }

            if (string.IsNullOrEmpty(path))
                return;
            try
            {
                e.Cancel = true;
                Process.Start(e.Url.AbsoluteUri);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Web: {0}", ex.Message);
            }
        }


        #endregion
    }
}