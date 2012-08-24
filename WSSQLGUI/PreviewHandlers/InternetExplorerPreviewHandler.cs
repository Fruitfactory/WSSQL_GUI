// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Internet Explorer Preview Handler", "xmlfile;.xml;.xps;.config;.psq", "{88235ab2-bfce-4be8-9ed0-0408cd8da792}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.InternetExplorerPreviewHandler")]
    [Guid("8fd75842-96ae-4ac9-a029-b57f7ef961a8")]
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