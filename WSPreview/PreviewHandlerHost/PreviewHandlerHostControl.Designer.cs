namespace WSPreview.PreviewHandler.PreviewHandlerHost
{
    partial class PreviewHandlerHostControl
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
            this.webMessage = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // webMessage
            // 
            this.webMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webMessage.Location = new System.Drawing.Point(0, 0);
            this.webMessage.MinimumSize = new System.Drawing.Size(20, 20);
            this.webMessage.Name = "webMessage";
            this.webMessage.Size = new System.Drawing.Size(150, 150);
            this.webMessage.TabIndex = 0;
            // 
            // PreviewHandlerHostControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.webMessage);
            this.DoubleBuffered = true;
            this.Name = "PreviewHandlerHostControl";
            this.Resize += new System.EventHandler(this.Control_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser webMessage;



    }
}
