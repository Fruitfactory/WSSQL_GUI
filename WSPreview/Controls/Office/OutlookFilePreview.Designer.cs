using System.Windows.Forms;

namespace C4F.DevKit.PreviewHandler.Controls.Office
{
    partial class OutlookFilePreview
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
            this.webEmail = new C4F.DevKit.PreviewHandler.Controls.PreviewBrowser();
            this.SuspendLayout();
            // 
            // webEmail
            // 
            this.webEmail.AllowNavigation = false;
            this.webEmail.AllowWebBrowserDrop = false;
            this.webEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webEmail.Location = new System.Drawing.Point(0, 0);
            this.webEmail.MinimumSize = new System.Drawing.Size(20, 20);
            this.webEmail.Name = "webEmail";
            this.webEmail.Size = new System.Drawing.Size(623, 452);
            this.webEmail.TabIndex = 1;
            // 
            // OutlookFilePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.webEmail);
            this.DoubleBuffered = true;
            this.Name = "OutlookFilePreview";
            this.Size = new System.Drawing.Size(623, 452);
            this.ResumeLayout(false);

        }

        #endregion

        private PreviewBrowser webEmail;

    }
}
