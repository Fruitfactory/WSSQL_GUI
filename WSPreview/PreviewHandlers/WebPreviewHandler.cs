using System.Runtime.InteropServices;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.Office;
using WSPreview.PreviewHandler.PreviewHandlerFramework;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Web Preview Handler", ".html;.htm;", "{9267AE2E-AE50-4F5D-BAA3-8AC9CDC1A9BF}")]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.WebPreviewHandler")]
    [Guid("1CF296E0-1519-4504-A5C7-E600E6DA6CFF")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class WebPreviewHandler : FileBasedPreviewHandler
    {
        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new WebPreviewHandlerControl();
        }

        private sealed class WebPreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            protected override Control GetCustomerPreviewControl()
            {
                return new WebFilePreview();
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Web;
            }
        }
    }
}