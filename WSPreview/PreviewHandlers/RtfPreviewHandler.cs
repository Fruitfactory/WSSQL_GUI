using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using C4F.DevKit.PreviewHandler;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;
using C4F.DevKit.PreviewHandler.Controls.Office;
using C4F.DevKit.PreviewHandler.Controls.RtfControl;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL RTF Preview Handler", ".rtf", "{FE214723-C17C-4A05-BDFB-FDE44DBA689B}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.RtfPreviewHandler")]
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

            public override void Load(FileInfo file)
            {
                RtfPreviewControl ctrl = new RtfPreviewControl();
                ((IPreviewControl)ctrl).LoadFile(file.FullName);
                ctrl.Dock = DockStyle.Fill;
                Controls.Add(ctrl);
            }

            #endregion
        }
    }
}