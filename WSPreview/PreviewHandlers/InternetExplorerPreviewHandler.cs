// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using OFPreview.PreviewHandler.PreviewHandlerFramework;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Internet Explorer Preview Handler", "xmlfile;.xml;.xps;.config;.psq", "{5C92DC69-2154-4974-8937-C9A2158D1ADC}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.InternetExplorerPreviewHandler")]
    [Guid("77A309CD-9901-400A-93FC-460B97C4FF88")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class InternetExplorerPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new InternetExplorerPreviewHandlerControl();
        }

        private sealed class InternetExplorerPreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            public override void Load(FileInfo file)
            {
                WebBrowser browser = new WebBrowser();
                browser.Dock = DockStyle.Fill;
                browser.Navigate(file.FullName);
                Controls.Add(browser);
            }
        }
    }
}