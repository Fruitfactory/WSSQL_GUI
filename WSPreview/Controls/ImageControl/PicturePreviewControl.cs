using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
            zoomPictureBox.Image = new Bitmap(filename);
            labelZoomCurrent.Text = string.Format("{0}x", zoomPictureBox.Zoom.ToString());
            trackZoom.Value = (int)zoomPictureBox.Zoom;
        }

        private void trackZoom_ValueChanged(object sender, EventArgs e)
        {
            zoomPictureBox.Zoom = trackZoom.Value;
            labelZoomCurrent.Text = string.Format("{0}x", trackZoom.Value.ToString());
        }

    }
}
