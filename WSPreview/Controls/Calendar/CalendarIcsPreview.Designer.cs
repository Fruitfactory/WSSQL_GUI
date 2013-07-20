namespace WSPreview.PreviewHandler.Controls.Calendar
{
    partial class CalendarIcsPreview
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
            this.browserIcs = new System.Windows.Forms.WebBrowser();
            this.SuspendLayout();
            // 
            // browserIcs
            // 
            this.browserIcs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browserIcs.Location = new System.Drawing.Point(0, 0);
            this.browserIcs.MinimumSize = new System.Drawing.Size(20, 20);
            this.browserIcs.Name = "browserIcs";
            this.browserIcs.Size = new System.Drawing.Size(150, 150);
            this.browserIcs.TabIndex = 0;
            // 
            // CalendarIcsPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.browserIcs);
            this.Name = "CalendarIcsPreview";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.WebBrowser browserIcs;
    }
}
