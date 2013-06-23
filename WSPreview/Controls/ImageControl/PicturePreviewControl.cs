using System;
using System.IO;
using System.Windows.Forms;
using C4F.DevKit.PreviewHandler.Controls.ImageControl.Interface;
using C4F.DevKit.PreviewHandler.Controls.ImageControl.Service;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;

namespace C4F.DevKit.PreviewHandler.Controls.ImageControl
{
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

                if (!generator.IsSupportFofrmat())
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

        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            zoomPictureBox.Zoom = trackZoom.Value;
            labelZoomCurrent.Text = string.Format("{0}x", zoomPictureBox.Zoom.ToString());
        }

    }
}
