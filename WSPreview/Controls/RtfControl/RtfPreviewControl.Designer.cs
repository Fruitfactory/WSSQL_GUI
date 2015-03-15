using WSUI.Core.Data;

namespace WSPreview.PreviewHandler.Controls.RtfControl
{
    partial class RtfPreviewControl
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
            this.rtfPreview = new System.Windows.Forms.RichTextBox();
            this.SuspendLayout();
            // 
            // rtfPreview
            // 
            this.rtfPreview.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.rtfPreview.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rtfPreview.Location = new System.Drawing.Point(0, 0);
            this.rtfPreview.Name = "rtfPreview";
            this.rtfPreview.Size = new System.Drawing.Size(150, 150);
            this.rtfPreview.TabIndex = 0;
            this.rtfPreview.Text = "";
            // 
            // RtfPreviewControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rtfPreview);
            this.Name = "RtfPreviewControl";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox rtfPreview;
        public void LoadObject(BaseSearchObject obj)
        {
            
        }
    }
}
