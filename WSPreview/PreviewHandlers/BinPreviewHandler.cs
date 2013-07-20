// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
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
            public override void Load(FileInfo file)
            {
                ByteViewer viewer = new ByteViewer();
                viewer.Dock = DockStyle.Fill;
                viewer.SetFile(file.FullName);
                Controls.Add(viewer);
            }
        }
    }
}