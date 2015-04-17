namespace OFOutlookPlugin
{
    partial class OFSidebar
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
            this.wpfSidebarHost = new System.Windows.Forms.Integration.ElementHost();
            this.SuspendLayout();
            // 
            // wpfSidebarHost
            // 
            this.wpfSidebarHost.BackColor = System.Drawing.Color.White;
            this.wpfSidebarHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfSidebarHost.Location = new System.Drawing.Point(0, 0);
            this.wpfSidebarHost.Name = "wpfSidebarHost";
            this.wpfSidebarHost.Size = new System.Drawing.Size(313, 483);
            this.wpfSidebarHost.TabIndex = 0;
            this.wpfSidebarHost.Child = null;
            // 
            // OFSidebar
            // 
            this.ClientSize = new System.Drawing.Size(313, 483);
            this.Controls.Add(this.wpfSidebarHost);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "OFSidebar";
            this.Text = "OFSidebar";
            this.ResumeLayout(false);

        }
        #endregion

        private System.Windows.Forms.Integration.ElementHost wpfSidebarHost;
    }
}
