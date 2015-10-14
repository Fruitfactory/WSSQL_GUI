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
            this.panelButton = new System.Windows.Forms.Panel();
            this.buttonZoomOut = new System.Windows.Forms.Button();
            this.buttonZoomIn = new System.Windows.Forms.Button();
            this.buttonNext = new System.Windows.Forms.Button();
            this.labelPage = new System.Windows.Forms.Label();
            this.buttonPrev = new System.Windows.Forms.Button();
            this.pageViewControl1 = new OFPreview.PreviewHandler.PageViewer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panelButton.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // bgLoadPages
            // 
            this.bgLoadPages.WorkerReportsProgress = true;
            this.bgLoadPages.WorkerSupportsCancellation = true;
            // 
            // panelButton
            // 
            this.panelButton.BackColor = System.Drawing.SystemColors.ButtonHighlight;
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
            this.buttonZoomOut.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.buttonZoomOut, "buttonZoomOut");
            this.buttonZoomOut.FlatAppearance.BorderSize = 0;
            this.buttonZoomOut.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.buttonZoomOut.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.buttonZoomOut.Image = global::OFPreview.PreviewHandler.Properties.Resources.ZoomOut;
            this.buttonZoomOut.Name = "buttonZoomOut";
            this.buttonZoomOut.UseVisualStyleBackColor = false;
            this.buttonZoomOut.Click += new System.EventHandler(this.tsbZoomOut_Click);
            // 
            // buttonZoomIn
            // 
            this.buttonZoomIn.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.buttonZoomIn, "buttonZoomIn");
            this.buttonZoomIn.FlatAppearance.BorderSize = 0;
            this.buttonZoomIn.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.buttonZoomIn.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.buttonZoomIn.Image = global::OFPreview.PreviewHandler.Properties.Resources.ZoomIn;
            this.buttonZoomIn.Name = "buttonZoomIn";
            this.buttonZoomIn.UseVisualStyleBackColor = false;
            this.buttonZoomIn.Click += new System.EventHandler(this.tsbZoomIn_Click);
            // 
            // buttonNext
            // 
            this.buttonNext.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.buttonNext, "buttonNext");
            this.buttonNext.FlatAppearance.BorderSize = 0;
            this.buttonNext.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.buttonNext.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.buttonNext.Image = global::OFPreview.PreviewHandler.Properties.Resources.netshell_1611_1_16x16x32;
            this.buttonNext.Name = "buttonNext";
            this.buttonNext.UseVisualStyleBackColor = false;
            this.buttonNext.Click += new System.EventHandler(this.tsbNext_Click);
            // 
            // labelPage
            // 
            this.labelPage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            resources.ApplyResources(this.labelPage, "labelPage");
            this.labelPage.Name = "labelPage";
            // 
            // buttonPrev
            // 
            this.buttonPrev.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            resources.ApplyResources(this.buttonPrev, "buttonPrev");
            this.buttonPrev.FlatAppearance.BorderSize = 0;
            this.buttonPrev.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(158)))), ((int)(((byte)(218)))));
            this.buttonPrev.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(207)))), ((int)(((byte)(235)))), ((int)(((byte)(247)))));
            this.buttonPrev.Image = global::OFPreview.PreviewHandler.Properties.Resources.netshell_21611_1_16x16x32;
            this.buttonPrev.Name = "buttonPrev";
            this.buttonPrev.UseVisualStyleBackColor = false;
            this.buttonPrev.Click += new System.EventHandler(this.tsbPrev_Click);
            // 
            // pageViewControl1
            // 
            this.pageViewControl1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(251)))), ((int)(((byte)(253)))));
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
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.panel2);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panelButton);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.pageViewControl1);
            resources.ApplyResources(this.panel3, "panel3");
            this.panel3.Name = "panel3";
            // 
            // PDFViewer
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel1);
            this.DoubleBuffered = true;
            this.Name = "PDFViewer";
            this.panelButton.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
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
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel2;
    }
}

