namespace WSSQLGUI.Views
{
    partial class ContactSettingsView
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
            this.panelSearch = new System.Windows.Forms.Panel();
            this.textSearchComplete = new WSSQLGUI.Controls.TextComplete.TextSearchComplete();
            this.tabPageSearch.SuspendLayout();
            this.panelForText.SuspendLayout();
            this.panelSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.Size = new System.Drawing.Size(268, 99);
            // 
            // tabPageAdditional
            // 
            this.tabPageAdditional.Size = new System.Drawing.Size(262, 121);
            // 
            // panelForText
            // 
            this.panelForText.Controls.Add(this.panelSearch);
            this.panelForText.Size = new System.Drawing.Size(262, 80);
            // 
            // panelSearch
            // 
            this.panelSearch.Controls.Add(this.textSearchComplete);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Margin = new System.Windows.Forms.Padding(0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(262, 80);
            this.panelSearch.TabIndex = 0;
            // 
            // textSearchComplete
            // 
            this.textSearchComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textSearchComplete.BackColor = System.Drawing.Color.Silver;
            this.textSearchComplete.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textSearchComplete.DataSource = null;
            this.textSearchComplete.Location = new System.Drawing.Point(5, 5);
            this.textSearchComplete.Margin = new System.Windows.Forms.Padding(5, 5, 25, 5);
            this.textSearchComplete.Name = "textSearchComplete";
            this.textSearchComplete.Size = new System.Drawing.Size(235, 20);
            this.textSearchComplete.TabIndex = 0;
            // 
            // ContactSettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "ContactSettingsView";
            this.Size = new System.Drawing.Size(346, 125);
            this.tabPageSearch.ResumeLayout(false);
            this.tabPageSearch.PerformLayout();
            this.panelForText.ResumeLayout(false);
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelSearch;
        private Controls.TextComplete.TextSearchComplete textSearchComplete;

    }
}
