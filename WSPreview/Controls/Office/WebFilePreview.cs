using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using GDIDraw.Service;
using OFPreview.PreviewHandler.Controls.Office.WebUtils;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OF.Core.Data;
using OF.Core.Logger;
using Html = HtmlAgilityPack;

namespace OFPreview.PreviewHandler.Controls.Office
{
    [KeyControl(ControlsKey.Web)]
    public class WebFilePreview : ExtWebBrowser, IPreviewControl
    {

        public WebFilePreview()
        {
            BeforeNavigate += OnBeforeNavigate;
            ScriptErrorsSuppressed = true;
        }

        public void LoadFile(string filename)
        {
            if (!File.Exists(filename))
                return;
            try
            {
                var content = File.ReadAllText(filename);
                ClearAllTagsA(content);
                DocumentText = content;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("Web: {0}",ex.Message);
            }
        }

        public void LoadFile(Stream stream)
        {
           
        }

        public void LoadObject(BaseSearchObject obj)
        {
            
        }

        public void Clear()
        {
            DocumentText = string.Empty;
        }

        #region [private]

        private string ClearAllTagsA( string content )
        {
            if (string.IsNullOrEmpty(content))
                return string.Empty;
            Html.HtmlDocument doc = new Html.HtmlDocument();
            doc.LoadHtml(content);
            ClearAllA(doc.DocumentNode);
            return doc.DocumentNode.InnerText;
        }

        private void ClearAllA(Html.HtmlNode node)
        {
            if (node == null || node.ChildNodes.Count == 0) return;

            if (node.Name.ToLowerInvariant() == Extensions.LinkTag)
            {
                node.RemoveTargetFromTagA();
            }
            foreach (var child in node.ChildNodes)
            {
                ClearAllA(child);                
            }
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
                OFLogger.Instance.LogError("Web: {0}", ex.Message);
            }
        }


        #endregion
    }
}