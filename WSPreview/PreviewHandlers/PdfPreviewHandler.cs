// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine PDF Preview Handler", ".pdf", "{480EE062-3BC0-4311-A18F-CACB7D3FBE74}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.PdfPreviewHandler")]
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
            public override void Load(FileInfo file)
            {
                try
                {
                    PdfAxHost viewer = new PdfAxHost();
                    Controls.Add(viewer);
                    viewer.Dock = DockStyle.Fill;

                    // file = MakeTemporaryCopy(file); // to avoid file sharing problems with the PDF control, make a copy first
                    viewer.LoadFile(file.FullName);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Rendering Preview - this handler requires Acrobat Reader", ex);
                }
            }
        }

        private class PdfAxHost : AxHost
        {
            object _ocx;

            public PdfAxHost()
                : base("ca8a9780-280d-11cf-a24d-444553540000") { }

            protected override void AttachInterfaces()
            {
                _ocx = base.GetOcx();
            }

            public void LoadFile(string fileName)
            {
                IntPtr forceCreation = Handle; // necessary for OCX instance to be created
                _ocx.GetType().InvokeMember(
                    "LoadFile", BindingFlags.InvokeMethod, null,
                    _ocx, new object[] { fileName }, CultureInfo.InvariantCulture);
            }
        }
    }
}