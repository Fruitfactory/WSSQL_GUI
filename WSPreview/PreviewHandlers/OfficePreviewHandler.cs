using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using OFPreview.PreviewHandler;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OFPreview.PreviewHandler.Controls.Office;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Office Preview Handler", ".doc;.docx;.xls;.xlsx", "{288E3293-B332-422C-BF5E-18BC8672C13A}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.OfficePreviewHandler")]
    [Guid("E289F0A5-EFFB-4B29-822B-CB1849D62426")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class OfficePreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
             return new OfficePreviewHandlerControl();
        }

        private sealed class OfficePreviewHandlerControl : FileBasedPreviewHandlerControl
        {

            protected override Control GetCustomerPreviewControl()
            {
                OfficeFilePreview ctrl = new OfficeFilePreview();
                return ctrl;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Office;
            }

        }

    }
}
