using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using OFPreview.PreviewHandler;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OFPreview.PreviewHandler.Controls.Office;
using OFPreview.PreviewHandler.Controls.RtfControl;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL RTF Preview Handler", ".rtf", "{FE214723-C17C-4A05-BDFB-FDE44DBA689B}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.RtfPreviewHandler")]
    [Guid("27F11A7F-9DD2-4A75-AAD5-5C6F70868224")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class RtfPreviewHandler : FileBasedPreviewHandler
    {
        #region Overrides of PreviewHandler

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new RtfPreviewHandlerControl();
        }

        #endregion

        private sealed class RtfPreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            #region Overrides of PreviewHandlerControl

            protected override Control GetCustomerPreviewControl()
            {
                RtfPreviewControl ctrl = new RtfPreviewControl();
                return ctrl;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Rtf;
            }

            #endregion
        }
    }
}