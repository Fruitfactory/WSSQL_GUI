using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using C4F.DevKit.PreviewHandler.Controls.Calendar;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL ICS Preview Handler", ".ics", "{EFB96ABA-99FF-4B53-9209-6154D9177DD1}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.IcsPreviewHandler")]
    [Guid("EFB96ABA-99FF-4B53-9209-6154D9177DD1")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class IcsPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new IcsPreviewHandlerControl();
        }

        private sealed class IcsPreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            public override void Load(FileInfo file)
            {
                try
                {
                    CalendarIcsPreview preview = new CalendarIcsPreview();
                    preview.LoadFile(file.FullName);
                    preview.Dock = DockStyle.Fill;
                    Controls.Add(preview);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}