using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.ImageControl;
using WSPreview.PreviewHandler.PreviewHandlerFramework;

namespace WSPreview.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Image Preview Handler", ".jpg;.png;.bmp;.psd;.gif;.tiff;.jpeg;.ico;.tga;.wmf;.cur;.dib", "{69D9AF89-B056-433B-8F92-E9A3585E533E}")]
    [ProgId("WSPreview.PreviewHandler.PreviewHandlers.ImagePreviewHandler")]
    [Guid("9251B469-BF27-410E-ABD4-D3F7A47AD7C6")]
    [ClassInterface(ClassInterfaceType.None)]
    [ComVisible(true)]
    public sealed class ImagePreviewHandler : FileBasedPreviewHandler
    {
        #region Overrides of PreviewHandler

        protected override PreviewHandlerControl CreatePreviewHandlerControl()
        {
            return new ImagePreviewHandlerControl();
        }

        #endregion

        private sealed class ImagePreviewHandlerControl : FileBasedPreviewHandlerControl
        {
            #region Overrides of PreviewHandlerControl

            protected override Control GetCustomerPreviewControl()
            {
                PicturePreviewControl ctrl = new PicturePreviewControl();
                return ctrl;
            }

            protected override ControlsKey GetControlsKey()
            {
                return ControlsKey.Image;
            }

            #endregion
        }

    }
}
