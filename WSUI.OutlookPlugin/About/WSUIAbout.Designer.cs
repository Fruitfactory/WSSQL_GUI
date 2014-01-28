namespace WSUIOutlookPlugin.About
{
    partial class WSUIAbout
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WSUIAbout));
            this.mainGrid = new System.Windows.Forms.TableLayoutPanel();
            this.boxLogo = new System.Windows.Forms.PictureBox();
            this.panelTop = new System.Windows.Forms.Panel();
            this.labelRights = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.panelMiddle = new System.Windows.Forms.Panel();
            this.linkSite = new System.Windows.Forms.LinkLabel();
            this.linkSupport = new System.Windows.Forms.LinkLabel();
            this.textBoxInfo = new System.Windows.Forms.TextBox();
            this.panelBottom = new System.Windows.Forms.Panel();
            this.buttonOk = new System.Windows.Forms.Button();
            this.mainGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.boxLogo)).BeginInit();
            this.panelTop.SuspendLayout();
            this.panelMiddle.SuspendLayout();
            this.panelBottom.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainGrid
            // 
            this.mainGrid.ColumnCount = 3;
            this.mainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.mainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainGrid.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.mainGrid.Controls.Add(this.boxLogo, 0, 0);
            this.mainGrid.Controls.Add(this.panelTop, 1, 0);
            this.mainGrid.Controls.Add(this.panelMiddle, 0, 1);
            this.mainGrid.Controls.Add(this.panelBottom, 0, 2);
            this.mainGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainGrid.Location = new System.Drawing.Point(0, 0);
            this.mainGrid.Name = "mainGrid";
            this.mainGrid.RowCount = 3;
            this.mainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 75F));
            this.mainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainGrid.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
            this.mainGrid.Size = new System.Drawing.Size(322, 318);
            this.mainGrid.TabIndex = 0;
            // 
            // boxLogo
            // 
            this.boxLogo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.boxLogo.Image = global::WSUIOutlookPlugin.Properties.Resources.logo_64;
            this.boxLogo.Location = new System.Drawing.Point(3, 3);
            this.boxLogo.Name = "boxLogo";
            this.boxLogo.Size = new System.Drawing.Size(69, 69);
            this.boxLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.boxLogo.TabIndex = 0;
            this.boxLogo.TabStop = false;
            // 
            // panelTop
            // 
            this.mainGrid.SetColumnSpan(this.panelTop, 2);
            this.panelTop.Controls.Add(this.labelRights);
            this.panelTop.Controls.Add(this.labelCopyright);
            this.panelTop.Controls.Add(this.labelProductName);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelTop.Location = new System.Drawing.Point(78, 3);
            this.panelTop.Name = "panelTop";
            this.panelTop.Padding = new System.Windows.Forms.Padding(5, 15, 5, 15);
            this.panelTop.Size = new System.Drawing.Size(241, 69);
            this.panelTop.TabIndex = 1;
            // 
            // labelRights
            // 
            this.labelRights.AutoEllipsis = true;
            this.labelRights.AutoSize = true;
            this.labelRights.Location = new System.Drawing.Point(8, 42);
            this.labelRights.Name = "labelRights";
            this.labelRights.Size = new System.Drawing.Size(35, 13);
            this.labelRights.TabIndex = 2;
            this.labelRights.Text = "label3";
            // 
            // labelCopyright
            // 
            this.labelCopyright.AutoEllipsis = true;
            this.labelCopyright.AutoSize = true;
            this.labelCopyright.Location = new System.Drawing.Point(8, 29);
            this.labelCopyright.Name = "labelCopyright";
            this.labelCopyright.Size = new System.Drawing.Size(35, 13);
            this.labelCopyright.TabIndex = 1;
            this.labelCopyright.Text = "label2";
            // 
            // labelProductName
            // 
            this.labelProductName.AutoEllipsis = true;
            this.labelProductName.AutoSize = true;
            this.labelProductName.Location = new System.Drawing.Point(8, 16);
            this.labelProductName.Name = "labelProductName";
            this.labelProductName.Size = new System.Drawing.Size(35, 13);
            this.labelProductName.TabIndex = 0;
            this.labelProductName.Text = "label1";
            // 
            // panelMiddle
            // 
            this.mainGrid.SetColumnSpan(this.panelMiddle, 3);
            this.panelMiddle.Controls.Add(this.linkSite);
            this.panelMiddle.Controls.Add(this.linkSupport);
            this.panelMiddle.Controls.Add(this.textBoxInfo);
            this.panelMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMiddle.Location = new System.Drawing.Point(3, 78);
            this.panelMiddle.Name = "panelMiddle";
            this.panelMiddle.Padding = new System.Windows.Forms.Padding(10);
            this.panelMiddle.Size = new System.Drawing.Size(316, 192);
            this.panelMiddle.TabIndex = 2;
            // 
            // linkSite
            // 
            this.linkSite.AutoSize = true;
            this.linkSite.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkSite.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkSite.Location = new System.Drawing.Point(9, 130);
            this.linkSite.Name = "linkSite";
            this.linkSite.Size = new System.Drawing.Size(55, 13);
            this.linkSite.TabIndex = 2;
            this.linkSite.TabStop = true;
            this.linkSite.Text = "linkLabel1";
            this.linkSite.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSite_LinkClicked);
            // 
            // linkSupport
            // 
            this.linkSupport.AutoSize = true;
            this.linkSupport.Cursor = System.Windows.Forms.Cursors.Hand;
            this.linkSupport.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.linkSupport.Location = new System.Drawing.Point(9, 113);
            this.linkSupport.Name = "linkSupport";
            this.linkSupport.Size = new System.Drawing.Size(55, 13);
            this.linkSupport.TabIndex = 1;
            this.linkSupport.TabStop = true;
            this.linkSupport.Text = "linkLabel1";
            this.linkSupport.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkSupport_LinkClicked);
            // 
            // textBoxInfo
            // 
            this.textBoxInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInfo.Location = new System.Drawing.Point(9, 13);
            this.textBoxInfo.Multiline = true;
            this.textBoxInfo.Name = "textBoxInfo";
            this.textBoxInfo.ReadOnly = true;
            this.textBoxInfo.Size = new System.Drawing.Size(298, 97);
            this.textBoxInfo.TabIndex = 0;
            this.textBoxInfo.TabStop = false;
            this.textBoxInfo.WordWrap = false;
            // 
            // panelBottom
            // 
            this.mainGrid.SetColumnSpan(this.panelBottom, 3);
            this.panelBottom.Controls.Add(this.buttonOk);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelBottom.Location = new System.Drawing.Point(3, 276);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(316, 39);
            this.panelBottom.TabIndex = 3;
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.AutoEllipsis = true;
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Location = new System.Drawing.Point(121, 7);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(75, 23);
            this.buttonOk.TabIndex = 0;
            this.buttonOk.Text = "Ok";
            this.buttonOk.UseVisualStyleBackColor = true;
            // 
            // WSUIAbout
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(322, 318);
            this.Controls.Add(this.mainGrid);
            this.DoubleBuffered = true;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(330, 345);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(330, 345);
            this.Name = "WSUIAbout";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "About";
            this.mainGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.boxLogo)).EndInit();
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panelMiddle.ResumeLayout(false);
            this.panelMiddle.PerformLayout();
            this.panelBottom.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel mainGrid;
        private System.Windows.Forms.PictureBox boxLogo;
        private System.Windows.Forms.Panel panelTop;
        private System.Windows.Forms.Label labelRights;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Panel panelMiddle;
        private System.Windows.Forms.TextBox textBoxInfo;
        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.LinkLabel linkSupport;
        private System.Windows.Forms.LinkLabel linkSite;
    }
}