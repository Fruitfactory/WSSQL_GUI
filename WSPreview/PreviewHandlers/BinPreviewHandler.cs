// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using WSPreview.Controls.BinControl;
using WSPreview.PreviewHandler.PreviewHandlerFramework;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Binary Preview Handler", ".bin;.dat", "{E722C141-DA78-414D-B031-EED646E9B991}")]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.BinaryPreviewHandler")]
    [Guid("01EFABEE-0E35-410C-9B57-6C2A9ADE12AE")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class BinaryPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new BinaryPreviewHandlerControl();
        }

        private sealed class BinaryPreviewHandlerControl : FileBasedPreviewHandlerControl
        {

            protected override Control GetCustomerPreviewControl()
            {
                BinPreviewControl ctrl = new BinPreviewControl();
                return ctrl;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Bin;
            }

        }
    }
}