namespace WSSQLGUI.Views
{
    partial class SearchForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SearchForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.splitPanels = new System.Windows.Forms.SplitContainer();
            this.previewControl = new WSPreview.PreviewHandler.PreviewHandlerHost.PreviewHandlerHostControl();
            this.buttonPreview = new System.Windows.Forms.Button();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.panelSettings = new System.Windows.Forms.Panel();
            this.comboBoxKinds = new System.Windows.Forms.ComboBox();
            this.labelKinds = new System.Windows.Forms.Label();
            this.tableLayoutPanelHeader = new System.Windows.Forms.TableLayoutPanel();
            this.contextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitPanels)).BeginInit();
            this.splitPanels.Panel2.SuspendLayout();
            this.splitPanels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.tableLayoutPanelHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 3);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(141, 25);
            this.label1.TabIndex = 0;
            this.label1.Text = "WSQL - GUI";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 204);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "Search Results";
            // 
            // contextMenu
            // 
            this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemOpen});
            this.contextMenu.Name = "contextMenu";
            this.contextMenu.Size = new System.Drawing.Size(168, 26);
            // 
            // toolStripMenuItemOpen
            // 
            this.toolStripMenuItemOpen.Name = "toolStripMenuItemOpen";
            this.toolStripMenuItemOpen.Size = new System.Drawing.Size(167, 22);
            this.toolStripMenuItemOpen.Text = "Open Current File";
            // 
            // splitPanels
            // 
            this.splitPanels.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitPanels.Location = new System.Drawing.Point(43, 225);
            this.splitPanels.Name = "splitPanels";
            // 
            // splitPanels.Panel2
            // 
            this.splitPanels.Panel2.Controls.Add(this.previewControl);
            this.splitPanels.Size = new System.Drawing.Size(887, 319);
            this.splitPanels.SplitterDistance = 485;
            this.splitPanels.TabIndex = 8;
            // 
            // previewControl
            // 
            this.previewControl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.previewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewControl.FilePath = null;
            this.previewControl.Location = new System.Drawing.Point(0, 0);
            this.previewControl.Name = "previewControl";
            this.previewControl.SearchCriteria = null;
            this.previewControl.Size = new System.Drawing.Size(398, 319);
            this.previewControl.TabIndex = 0;
            // 
            // buttonPreview
            // 
            this.buttonPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.buttonPreview.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonPreview.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(153)))), ((int)(((byte)(153)))), ((int)(((byte)(153)))));
            this.buttonPreview.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.buttonPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPreview.ForeColor = System.Drawing.Color.Black;
            this.buttonPreview.Location = new System.Drawing.Point(845, 194);
            this.buttonPreview.Name = "buttonPreview";
            this.buttonPreview.Size = new System.Drawing.Size(85, 24);
            this.buttonPreview.TabIndex = 10;
            this.buttonPreview.Text = "Preview";
            this.buttonPreview.UseVisualStyleBackColor = false;
            // 
            // errorProvider
            // 
            this.errorProvider.ContainerControl = this;
            // 
            // panelSettings
            // 
            this.panelSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelSettings.Location = new System.Drawing.Point(150, 3);
            this.panelSettings.Name = "panelSettings";
            this.tableLayoutPanelHeader.SetRowSpan(this.panelSettings, 3);
            this.panelSettings.Size = new System.Drawing.Size(734, 157);
            this.panelSettings.TabIndex = 11;
            // 
            // comboBoxKinds
            // 
            this.comboBoxKinds.BackColor = System.Drawing.Color.Silver;
            this.comboBoxKinds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.comboBoxKinds.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKinds.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxKinds.FormattingEnabled = true;
            this.comboBoxKinds.Location = new System.Drawing.Point(3, 70);
            this.comboBoxKinds.Name = "comboBoxKinds";
            this.comboBoxKinds.Size = new System.Drawing.Size(141, 23);
            this.comboBoxKinds.TabIndex = 12;
            this.comboBoxKinds.SelectedIndexChanged += new System.EventHandler(this.comboBoxKinds_SelectedIndexChanged);
            // 
            // labelKinds
            // 
            this.labelKinds.AutoSize = true;
            this.labelKinds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelKinds.Location = new System.Drawing.Point(3, 34);
            this.labelKinds.Margin = new System.Windows.Forms.Padding(3);
            this.labelKinds.Name = "labelKinds";
            this.labelKinds.Size = new System.Drawing.Size(141, 30);
            this.labelKinds.TabIndex = 13;
            this.labelKinds.Text = "Choose kind of file searching:";
            // 
            // tableLayoutPanelHeader
            // 
            this.tableLayoutPanelHeader.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanelHeader.ColumnCount = 2;
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 16.66667F));
            this.tableLayoutPanelHeader.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 83.33334F));
            this.tableLayoutPanelHeader.Controls.Add(this.labelKinds, 0, 1);
            this.tableLayoutPanelHeader.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.panelSettings, 1, 0);
            this.tableLayoutPanelHeader.Controls.Add(this.comboBoxKinds, 0, 2);
            this.tableLayoutPanelHeader.Location = new System.Drawing.Point(43, 12);
            this.tableLayoutPanelHeader.Name = "tableLayoutPanelHeader";
            this.tableLayoutPanelHeader.RowCount = 3;
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanelHeader.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanelHeader.Size = new System.Drawing.Size(887, 163);
            this.tableLayoutPanelHeader.TabIndex = 14;
            // 
            // SearchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.ClientSize = new System.Drawing.Size(969, 574);
            this.Controls.Add(this.tableLayoutPanelHeader);
            this.Controls.Add(this.splitPanels);
            this.Controls.Add(this.buttonPreview);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(985, 612);
            this.Name = "SearchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "WSSQL GUI";
            this.contextMenu.ResumeLayout(false);
            this.splitPanels.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitPanels)).EndInit();
            this.splitPanels.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.tableLayoutPanelHeader.ResumeLayout(false);
            this.tableLayoutPanelHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.SplitContainer splitPanels;
        private WSPreview.PreviewHandler.PreviewHandlerHost.PreviewHandlerHostControl previewControl;
        private System.Windows.Forms.Button buttonPreview;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemOpen;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.Panel panelSettings;
        private System.Windows.Forms.ComboBox comboBoxKinds;
        public System.Windows.Forms.ContextMenuStrip contextMenu;
        private System.Windows.Forms.Label labelKinds;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelHeader;
    }
}

