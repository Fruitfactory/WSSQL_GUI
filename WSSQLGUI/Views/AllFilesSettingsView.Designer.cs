using System.Windows.Forms;

namespace WSSQLGUI.Views
{
    partial class AllFilesSettingsView
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
            this.components = new System.ComponentModel.Container();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.textBoxSearch = new System.Windows.Forms.TextBox();
            this.panelSearch = new System.Windows.Forms.Panel();
            this.tabPageSearch.SuspendLayout();
            this.panelForText.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.panelSearch.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.Size = new System.Drawing.Size(320, 113);
            // 
            // tabPageAdditional
            // 
            this.tabPageAdditional.Size = new System.Drawing.Size(262, 121);
            // 
            // panelForText
            // 
            this.panelForText.Controls.Add(this.panelSearch);
            this.panelForText.Size = new System.Drawing.Size(314, 94);
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // textBoxSearch
            // 
            this.textBoxSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxSearch.BackColor = System.Drawing.Color.Silver;
            this.textBoxSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBoxSearch.Location = new System.Drawing.Point(5, 5);
            this.textBoxSearch.Margin = new System.Windows.Forms.Padding(5, 5, 25, 5);
            this.textBoxSearch.Name = "textBoxSearch";
            this.textBoxSearch.Size = new System.Drawing.Size(287, 20);
            this.textBoxSearch.TabIndex = 0;
            // 
            // panelSearch
            // 
            this.panelSearch.AutoSize = true;
            this.panelSearch.Controls.Add(this.textBoxSearch);
            this.panelSearch.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelSearch.Location = new System.Drawing.Point(0, 0);
            this.panelSearch.Margin = new System.Windows.Forms.Padding(0);
            this.panelSearch.Name = "panelSearch";
            this.panelSearch.Size = new System.Drawing.Size(314, 30);
            this.panelSearch.TabIndex = 1;
            // 
            // AllFilesSettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Name = "AllFilesSettingsView";
            this.Size = new System.Drawing.Size(410, 139);
            this.tabPageSearch.ResumeLayout(false);
            this.tabPageSearch.PerformLayout();
            this.panelForText.ResumeLayout(false);
            this.panelForText.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.panelSearch.ResumeLayout(false);
            this.panelSearch.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider;
        private TextBox textBoxSearch;
        private Panel panelSearch;
        
    }
}
