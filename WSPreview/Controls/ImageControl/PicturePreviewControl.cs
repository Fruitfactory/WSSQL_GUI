using System;
using System.IO;
using System.Windows.Forms;
using OFPreview.PreviewHandler.Controls.ImageControl.Interface;
using OFPreview.PreviewHandler.Controls.ImageControl.Service;
using OFPreview.PreviewHandler.PreviewHandlerFramework;
using OF.Core.Data;

namespace OFPreview.PreviewHandler.Controls.ImageControl
{
    [KeyControl(ControlsKey.Image)]
    public partial class PicturePreviewControl : UserControl, IPreviewControl
    {
        public PicturePreviewControl()
        {
            InitializeComponent();
        }

        public void LoadFile(string filename)
        {
            if (string.IsNullOrEmpty(filename))
            {
                zoomPictureBox.Image = null;
                return;
            }
            try
            {
                IImagePreviewGenerator generator = ImagePreviewGenerator.Instance;
                generator.SetFileName(filename);

                if (!generator.IsSupportFormat())
                    return;

                zoomPictureBox.Image = generator.GetImage();
                labelZoomCurrent.Text = string.Format("{0}x", zoomPictureBox.Zoom.ToString());
                trackZoom.Value = (int)zoomPictureBox.Zoom;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);        
            }
            
        }

        public void LoadFile(Stream stream)
        {
        }

        public void LoadObject(OFBaseSearchObject obj)
        {
            
        }

        public void Clear()
        {
            if (zoomPictureBox != null)
            {
                zoomPictureBox.Image = null;
            }
        }

        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            zoomPictureBox.Zoom = trackZoom.Value;
            labelZoomCurrent.Text = string.Format("{0}x", zoomPictureBox.Zoom.ToString());
        }

    }
}
