// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using OFPreview.PreviewHandler;
using OFPreview.PreviewHandler.PreviewHandlerFramework;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine PDF Preview Handler", ".pdf", "{480EE062-3BC0-4311-A18F-CACB7D3FBE74}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.PdfPreviewHandler")]
    [Guid("092B96A3-5136-40DE-AA8B-13CB54084789")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class PdfPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new PdfPreviewHandlerControl();
        }

        private sealed class PdfPreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            protected override Control GetCustomerPreviewControl()
            {
                PDFViewer viewer = new PDFViewer();
                return viewer;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Pdf;
            }

        }
       
    }
}