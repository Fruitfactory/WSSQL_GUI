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
    [PreviewHandler("MSDN Magazine PDF Preview Handler", ".pdf", "{E1B9A916-BD9E-45e6-9266-E9BD0AF00CB7}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.PdfPreviewHandler")]
    [Guid("574FFFAA-17F6-44b1-A1B4-177AB5900A51")]
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
                    throw new Exception("Error Rendering Preview- this handler requires Acrobat Reader", ex);
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