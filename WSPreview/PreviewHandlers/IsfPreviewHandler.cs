// Stephen Toub
// Coded and published in January 2007 issue of MSDN Magazine 
// http://msdn.microsoft.com/msdnmag/issues/07/01/PreviewHandlers/default.aspx

using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Ink;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("MSDN Magazine Serialized Ink Preview Handler", ".isf", "{B85299A9-2E8D-4D5F-AE4D-13C1FD6FF999}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.IsfPreviewHandler")]
    [Guid("8FD7B59B-A960-4D76-A89E-35C359BC123E")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class IsfPreviewHandler : StreamBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new IsfPreviewHandlerControl();
        }

        private sealed class IsfPreviewHandlerControl : StreamBasedPreviewHandlerControl
        {
            public override void Load(Stream s)
            {
                Panel p = new Panel();
                p.Dock = DockStyle.Fill;
                
                byte[] inkData = new byte[s.Length];
                s.Read(inkData, 0, inkData.Length);

                InkOverlay overlay = new InkOverlay(p);
                overlay.Ink.Load(inkData);

                Controls.Add(p);
            }
        }
    }
}