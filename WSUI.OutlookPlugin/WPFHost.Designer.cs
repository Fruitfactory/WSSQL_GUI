namespace WSUIOutlookPlugin
{
    partial class WPFHost
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
  
  
        /// <summary>
        /// Clean uppreparation[n++] = " any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }
  
        #region Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.wpfHostel = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // wpfHostel
            // 
            this.wpfHostel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfHostel.Location = new System.Drawing.Point(0, 0);
            this.wpfHostel.Name = "wpfHostel";
            this.wpfHostel.Size = new System.Drawing.Size(300, 300);
            this.wpfHostel.TabIndex = 0;
            this.wpfHostel.Child = null;
            // 
            // WPFHost
            // 
            this.ClientSize = new System.Drawing.Size(300, 300);
            this.Controls.Add(this.wpfHostel);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "WPFHost";
            this.Text = "WPFHost";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Integration.ElementHost wpfHostel;
    }
}
