using System;
using System.Windows.Forms;
using GflWrapper;


namespace C4F.DevKit.PreviewHandler.Controls.ImageControl
{
    public partial class PicturePreviewControl : UserControl
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
            GflImageWrapper image = new GflImageWrapper(filename);

            if (!image.IsSupportExt())
                return;

            zoomPictureBox.Image = image.GetImage();
            labelZoomCurrent.Text = string.Format("{0}x", zoomPictureBox.Zoom.ToString());
            trackZoom.Value = (int)zoomPictureBox.Zoom;
        }

        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            zoomPictureBox.Zoom = trackZoom.Value;
            labelZoomCurrent.Text = string.Format("{0}x", zoomPictureBox.Zoom.ToString());
        }

    }
}
