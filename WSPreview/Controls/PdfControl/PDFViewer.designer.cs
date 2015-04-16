namespace OFPreview.PreviewHandler
{
    partial class PDFViewer
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PDFViewer));
            this.ttpLink = new System.Windows.Forms.ToolTip(this.components);
            this.bgLoadPages = new System.ComponentModel.BackgroundWorker();
            this.pageViewControl1 = new OFPreview.PreviewHandler.PageViewer();
            this.panelButton = new System.Windows.Forms.Panel();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelPage = new System.Windows.Forms.Label();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.panelButton.SuspendLayout();
            this.SuspendLayout();
            // 
            // bgLoadPages
            // 
            this.bgLoadPages.WorkerReportsProgress = true;
            this.bgLoadPages.WorkerSupportsCancellation = true;
            // 
            // pageViewControl1
            // 
            this.pageViewControl1.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.pageViewControl1.BorderColor = System.Drawing.Color.Black;
            resources.ApplyResources(this.pageViewControl1, "pageViewControl1");
            this.pageViewControl1.DrawBorder = true;
            this.pageViewControl1.DrawShadow = true;
            this.pageViewControl1.Name = "pageViewControl1";
            this.pageViewControl1.PageColor = System.Drawing.Color.White;
            this.pageViewControl1.PageSize = new System.Drawing.Size(0, 0);
            this.pageViewControl1.PaintMethod = OFPreview.PreviewHandler.PageViewer.DoubleBufferMethod.BuiltInOptimizedDoubleBuffer;
            this.pageViewControl1.ScrollPosition = new System.Drawing.Point(-10, -10);
            this.pageViewControl1.NextPage += new OFPreview.PreviewHandler.PageViewer.MovePageHandler(this.doubleBufferControl1_NextPage);
            this.pageViewControl1.PreviousPage += new OFPreview.PreviewHandler.PageViewer.MovePageHandler(this.doubleBufferControl1_PreviousPage);
            this.pageViewControl1.PaintControl += new OFPreview.PreviewHandler.PageViewer.PaintControlHandler(this.doubleBufferControl1_PaintControl);
            // 
            // panelButton
            // 
            this.panelButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.panelButton.Controls.Add(this.buttonZoomOut);
            this.panelButton.Controls.Add(this.buttonZoomIn);
            this.panelButton.Controls.Add(this.buttonNext);
            this.panelButton.Controls.Add(this.labelPage);
            this.panelButton.Controls.Add(this.buttonPrev);
            resources.ApplyResources(this.panelButton, "panelButton");
            this.panelButton.Name = "panelButton";
            // 
            // buttonZoomOut
            // 
            this.buttonZoomOut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            resources.ApplyResources(this.buttonZoomOut, "buttonZoomOut");
            this.buttonZoomOut.Image = global::OFPreview.PreviewHandler.Properties.Resources.ZoomOut;
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.UseVisualStyleBackColor = false;
            this.buttonZoomOut.Click += new System.EventHandler(this.tsbZoomOut_Click);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            resources.ApplyResources(this.buttonZoomIn, "buttonZoomIn");
            this.buttonZoomIn.Image = global::OFPreview.PreviewHandler.Properties.Resources.ZoomIn;
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.UseVisualStyleBackColor = false;
            this.buttonZoomIn.Click += new System.EventHandler(this.tsbZoomIn_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.Image = global::OFPreview.PreviewHandler.Properties.Resources.netshell_1611_1_16x16x32;
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.tsbNext_Click);
            // 
            // labelPage
            // 
            this.labelPage.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.labelPage, "labelPage");
            this.labelPage.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.labelPage.Name = "labelPage";
            // 
            // buttonPrev
            // 
            this.buttonPrev.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            resources.ApplyResources(this.buttonPrev, "buttonPrev");
            this.buttonPrev.Image = global::OFPreview.PreviewHandler.Properties.Resources.netshell_21611_1_16x16x32;
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.UseVisualStyleBackColor = false;
            this.buttonPrev.Click += new System.EventHandler(this.tsbPrev_Click);
            // 
            // PDFViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelButton);
            this.Controls.Add(this.pageViewControl1);
            this.DoubleBuffered = true;
            this.Name = "PDFViewer";
            this.panelButton.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolTip ttpLink;
        private PageViewer pageViewControl1;
        //private PDFImagesThumbView pdfImagesThumbView1;
        private System.ComponentModel.BackgroundWorker bgLoadPages;
        private System.Windows.Forms.Panel panelButton;
        private System.Windows.Forms.Button buttonNext;
        private System.Windows.Forms.Label labelPage;
        private System.Windows.Forms.Button buttonPrev;
        private System.Windows.Forms.Button buttonZoomOut;
        private System.Windows.Forms.Button buttonZoomIn;
    }
}

