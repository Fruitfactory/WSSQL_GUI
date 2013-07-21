using System;
using System.IO;
using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.ImageControl.Interface;
using WSPreview.PreviewHandler.Controls.ImageControl.Service;
using WSPreview.PreviewHandler.PreviewHandlerFramework;

namespace WSPreview.PreviewHandler.Controls.ImageControl
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
