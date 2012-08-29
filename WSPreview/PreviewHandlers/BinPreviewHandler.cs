// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Binary Preview Handler", ".bin;.dat", "{FDFA5AAF-8243-415d-B5E5-AF551336BE7B}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.BinaryPreviewHandler")]
    [Guid("DF9E65B0-7980-4053-9FCF-6E9AF953A9F4")]
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