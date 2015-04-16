using System;
using System.IO;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Globalization;
using OFPreview.PreviewHandler.Controls.Calendar;
using OFPreview.PreviewHandler.PreviewHandlerFramework;

namespace OFPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL ICS Preview Handler", ".ics", "{EFB96ABA-99FF-4B53-9209-6154D9177DD1}")]
    [ProgId("OFPreview.PreviewHandler.PreviewHandlers.IcsPreviewHandler")]
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
            
            protected override Control GetCustomerPreviewControl()
            {
                CalendarIcsPreview preview = new CalendarIcsPreview();
                return preview;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Calendar;
            }

        }

    }
}