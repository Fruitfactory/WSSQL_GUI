using System.Windows.Forms;
using WSPreview.PreviewHandler.Controls.Office.WebUtils;

namespace WSPreview.PreviewHandler.Controls.Office
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
            this.components = new System.ComponentModel.Container();
            this.outlookPreviewContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.webEmail = new WSPreview.PreviewHandler.Controls.Office.WebUtils.ExtWebBrowser();
            this.outlookPreviewContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // outlookPreviewContextMenu
            // 
            this.outlookPreviewContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyMenuItem});
            this.outlookPreviewContextMenu.Name = "outlookPreviewContextMenu";
            this.outlookPreviewContextMenu.Size = new System.Drawing.Size(153, 48);
            // 
            // copyMenuItem
            // 
            this.copyMenuItem.Name = "copyMenuItem";
            this.copyMenuItem.Size = new System.Drawing.Size(152, 22);
            this.copyMenuItem.Text = "Copy";
            this.copyMenuItem.Click += new System.EventHandler(this.copyMenuItem_Click);
            // 
            // webEmail
            // 
            this.webEmail.AllowWebBrowserDrop = false;
            this.webEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webEmail.IsWebBrowserContextMenuEnabled = false;
            this.webEmail.Location = new System.Drawing.Point(0, 0);
            this.webEmail.MinimumSize = new System.Drawing.Size(20, 20);
            this.webEmail.Name = "webEmail";
            this.webEmail.Size = new System.Drawing.Size(623, 452);
            this.webEmail.TabIndex = 1;
            // 
            // OutlookFilePreview
            // 
            //this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            //this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            //this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.White;
            this.ContextMenuStrip = this.outlookPreviewContextMenu;
            this.Controls.Add(this.webEmail);
            this.DoubleBuffered = true;
            this.Name = "OutlookFilePreview";
            this.Size = new System.Drawing.Size(623, 452);
            this.outlookPreviewContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ExtWebBrowser webEmail;
        private ContextMenuStrip outlookPreviewContextMenu;
        private ToolStripMenuItem copyMenuItem;

    }
}
