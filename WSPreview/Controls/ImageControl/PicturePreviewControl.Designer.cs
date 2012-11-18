namespace C4F.DevKit.PreviewHandler.Controls.ImageControl
{
    partial class PicturePreviewControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelBottom = new System.Windows.Forms.Panel();
            this.panel = new System.Windows.Forms.Panel();
            this.zoomPictureBox = new C4F.DevKit.PreviewHandler.Controls.ImageControl.ZoomPictureBox();
            this.trackZoom = new System.Windows.Forms.TrackBar();
            this.labelZoom = new System.Windows.Forms.Label();
            this.labelZoomCurrent = new System.Windows.Forms.Label();
            this.panelBottom.SuspendLayout();
            this.panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.labelZoomCurrent);
            this.panelBottom.Controls.Add(this.labelZoom);
            this.panelBottom.Controls.Add(this.trackZoom);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 262);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(304, 56);
            this.panelBottom.TabIndex = 0;
            // 
            // panel
            // 
            this.panel.Controls.Add(this.zoomPictureBox);
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 0);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(304, 262);
            this.panel.TabIndex = 1;
            // 
            // zoomPictureBox
            // 
            this.zoomPictureBox.AutoScroll = true;
            this.zoomPictureBox.AutoScrollMargin = new System.Drawing.Size(304, 288);
            this.zoomPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomPictureBox.Image = null;
            this.zoomPictureBox.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.High;
            this.zoomPictureBox.Location = new System.Drawing.Point(0, 0);
            this.zoomPictureBox.Name = "zoomPictureBox";
            this.zoomPictureBox.Size = new System.Drawing.Size(304, 262);
            this.zoomPictureBox.TabIndex = 0;
            this.zoomPictureBox.Text = "zoomPictureBox1";
            this.zoomPictureBox.Zoom = 1F;
            // 
            // trackZoom
            // 
            this.trackZoom.Location = new System.Drawing.Point(47, 6);
            this.trackZoom.Name = "trackZoom";
            this.trackZoom.Size = new System.Drawing.Size(138, 45);
            this.trackZoom.TabIndex = 0;
            this.trackZoom.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.trackZoom.ValueChanged += new System.EventHandler(this.trackZoom_ValueChanged);
            // 
            // labelZoom
            // 
            this.labelZoom.AutoSize = true;
            this.labelZoom.Location = new System.Drawing.Point(4, 7);
            this.labelZoom.Name = "labelZoom";
            this.labelZoom.Size = new System.Drawing.Size(37, 13);
            this.labelZoom.TabIndex = 1;
            this.labelZoom.Text = "Zoom:";
            // 
            // labelZoomCurrent
            // 
            this.labelZoomCurrent.AutoSize = true;
            this.labelZoomCurrent.Location = new System.Drawing.Point(191, 7);
            this.labelZoomCurrent.Name = "labelZoomCurrent";
            this.labelZoomCurrent.Size = new System.Drawing.Size(0, 13);
            this.labelZoomCurrent.TabIndex = 2;
            // 
            // PicturePreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel);
            this.Controls.Add(this.panelBottom);
            this.Name = "PicturePreviewControl";
            this.Size = new System.Drawing.Size(304, 318);
            this.panelBottom.ResumeLayout(false);
            this.panelBottom.PerformLayout();
            this.panel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackZoom)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panel;
        private ZoomPictureBox zoomPictureBox;
        private System.Windows.Forms.Label labelZoom;
        private System.Windows.Forms.TrackBar trackZoom;
        private System.Windows.Forms.Label labelZoomCurrent;
    }
}
