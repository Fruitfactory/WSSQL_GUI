using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.Controls.ImageControl;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.PreviewHandlers
{
    [PreviewHandler("WSSQL Image Preview Handler", ".jpg;.png;.bmp;.psd;.gif;.tiff;.jpeg;.ico;.tga;.wmf;.cur;.dib", "{69D9AF89-B056-433B-8F92-E9A3585E533E}")]
    [ProgId("C4F.DevKit.PreviewHandler.PreviewHandlers.ImagePreviewHandler")]
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

            public override void Load(FileInfo file)
            {
                try
                {
                    PicturePreviewControl ctrl = new PicturePreviewControl();
                    ctrl.LoadFile(file.FullName);
                    ctrl.Dock = DockStyle.Fill;
                    Controls.Add(ctrl);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error Rendering Preview - this handler requires Image Preview", ex);
                }
                finally
                {
                    
                }
            }

            #endregion
        }

    }
}
