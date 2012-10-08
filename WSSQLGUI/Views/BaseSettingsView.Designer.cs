namespace WSSQLGUI.Views
{
    partial class BaseSettingsView
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
            this.tableLayoutPanelBase = new System.Windows.Forms.TableLayoutPanel();
            this.tabControlBase = new System.Windows.Forms.TabControl();
            this.tabPageSearch = new System.Windows.Forms.TabPage();
            this.panelForText = new System.Windows.Forms.Panel();
            this.panelLabel = new System.Windows.Forms.Panel();
            this.labelSearch = new System.Windows.Forms.Label();
            this.tabPageAdditional = new System.Windows.Forms.TabPage();
            this.tableLayoutPanelAdditional = new System.Windows.Forms.TableLayoutPanel();
            this.labelChoose = new System.Windows.Forms.Label();
            this.comboBoxFolder = new System.Windows.Forms.ComboBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.tableLayoutPanelBase.SuspendLayout();
            this.tabControlBase.SuspendLayout();
            this.tabPageSearch.SuspendLayout();
            this.panelLabel.SuspendLayout();
            this.tabPageAdditional.SuspendLayout();
            this.tableLayoutPanelAdditional.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanelBase
            // 
            this.tableLayoutPanelBase.ColumnCount = 2;
            this.tableLayoutPanelBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanelBase.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelBase.Controls.Add(this.tabControlBase, 0, 0);
            this.tableLayoutPanelBase.Controls.Add(this.buttonSearch, 1, 1);
            this.tableLayoutPanelBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelBase.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanelBase.Name = "tableLayoutPanelBase";
            this.tableLayoutPanelBase.RowCount = 2;
            this.tableLayoutPanelBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayoutPanelBase.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 90F));
            this.tableLayoutPanelBase.Size = new System.Drawing.Size(338, 147);
            this.tableLayoutPanelBase.TabIndex = 0;
            // 
            // tabControlBase
            // 
            this.tabControlBase.Controls.Add(this.tabPageSearch);
            this.tabControlBase.Controls.Add(this.tabPageAdditional);
            this.tabControlBase.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlBase.Location = new System.Drawing.Point(0, 0);
            this.tabControlBase.Margin = new System.Windows.Forms.Padding(0);
            this.tabControlBase.Name = "tabControlBase";
            this.tableLayoutPanelBase.SetRowSpan(this.tabControlBase, 2);
            this.tabControlBase.SelectedIndex = 0;
            this.tabControlBase.Size = new System.Drawing.Size(270, 147);
            this.tabControlBase.TabIndex = 0;
            // 
            // tabPageSearch
            // 
            this.tabPageSearch.Controls.Add(this.panelForText);
            this.tabPageSearch.Controls.Add(this.panelLabel);
            this.tabPageSearch.Location = new System.Drawing.Point(4, 22);
            this.tabPageSearch.Name = "tabPageSearch";
            this.tabPageSearch.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSearch.Size = new System.Drawing.Size(262, 121);
            this.tabPageSearch.TabIndex = 0;
            this.tabPageSearch.Text = "Search settings";
            this.tabPageSearch.UseVisualStyleBackColor = true;
            // 
            // panelForText
            // 
            this.panelForText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForText.Location = new System.Drawing.Point(3, 16);
            this.panelForText.Name = "panelForText";
            this.panelForText.Size = new System.Drawing.Size(256, 102);
            this.panelForText.TabIndex = 1;
            // 
            // panelLabel
            // 
            this.panelLabel.AutoSize = true;
            this.panelLabel.Controls.Add(this.labelSearch);
            this.panelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLabel.Location = new System.Drawing.Point(3, 3);
            this.panelLabel.Name = "panelLabel";
            this.panelLabel.Size = new System.Drawing.Size(256, 13);
            this.panelLabel.TabIndex = 0;
            // 
            // labelSearch
            // 
            this.labelSearch.AutoSize = true;
            this.labelSearch.Location = new System.Drawing.Point(3, 0);
            this.labelSearch.Name = "labelSearch";
            this.labelSearch.Size = new System.Drawing.Size(135, 13);
            this.labelSearch.TabIndex = 0;
            this.labelSearch.Text = "Enter search criteria below:";
            // 
            // tabPageAdditional
            // 
            this.tabPageAdditional.Controls.Add(this.tableLayoutPanelAdditional);
            this.tabPageAdditional.Location = new System.Drawing.Point(4, 22);
            this.tabPageAdditional.Name = "tabPageAdditional";
            this.tabPageAdditional.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageAdditional.Size = new System.Drawing.Size(262, 121);
            this.tabPageAdditional.TabIndex = 1;
            this.tabPageAdditional.Text = "Additional settings";
            this.tabPageAdditional.UseVisualStyleBackColor = true;
            // 
            // tableLayoutPanelAdditional
            // 
            this.tableLayoutPanelAdditional.ColumnCount = 2;
            this.tableLayoutPanelAdditional.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 20F));
            this.tableLayoutPanelAdditional.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 80F));
            this.tableLayoutPanelAdditional.Controls.Add(this.labelChoose, 0, 0);
            this.tableLayoutPanelAdditional.Controls.Add(this.comboBoxFolder, 1, 0);
            this.tableLayoutPanelAdditional.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanelAdditional.Location = new System.Drawing.Point(3, 3);
            this.tableLayoutPanelAdditional.Name = "tableLayoutPanelAdditional";
            this.tableLayoutPanelAdditional.RowCount = 2;
            this.tableLayoutPanelAdditional.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelAdditional.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanelAdditional.Size = new System.Drawing.Size(250, 109);
            this.tableLayoutPanelAdditional.TabIndex = 0;
            // 
            // labelChoose
            // 
            this.labelChoose.AutoSize = true;
            this.labelChoose.Dock = System.Windows.Forms.DockStyle.Top;
            this.labelChoose.Location = new System.Drawing.Point(3, 0);
            this.labelChoose.Name = "labelChoose";
            this.labelChoose.Size = new System.Drawing.Size(44, 26);
            this.labelChoose.TabIndex = 0;
            this.labelChoose.Text = "Choose folder:";
            // 
            // comboBoxFolder
            // 
            this.comboBoxFolder.BackColor = System.Drawing.Color.DarkGray;
            this.comboBoxFolder.Dock = System.Windows.Forms.DockStyle.Top;
            this.comboBoxFolder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxFolder.FormattingEnabled = true;
            this.comboBoxFolder.Location = new System.Drawing.Point(53, 3);
            this.comboBoxFolder.Name = "comboBoxFolder";
            this.comboBoxFolder.Size = new System.Drawing.Size(194, 21);
            this.comboBoxFolder.TabIndex = 1;
            // 
            // buttonSearch
            // 
            this.buttonSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSearch.BackColor = System.Drawing.Color.DarkGray;
            this.buttonSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSearch.Location = new System.Drawing.Point(280, 24);
            this.buttonSearch.Margin = new System.Windows.Forms.Padding(10);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(48, 23);
            this.buttonSearch.TabIndex = 1;
            this.buttonSearch.UseVisualStyleBackColor = false;
            // 
            // BaseSettingsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelBase);
            this.Name = "BaseSettingsView";
            this.Size = new System.Drawing.Size(338, 147);
            this.tableLayoutPanelBase.ResumeLayout(false);
            this.tabControlBase.ResumeLayout(false);
            this.tabPageSearch.ResumeLayout(false);
            this.tabPageSearch.PerformLayout();
            this.panelLabel.ResumeLayout(false);
            this.panelLabel.PerformLayout();
            this.tabPageAdditional.ResumeLayout(false);
            this.tableLayoutPanelAdditional.ResumeLayout(false);
            this.tableLayoutPanelAdditional.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        protected System.Windows.Forms.TabControl tabControlBase;
        protected System.Windows.Forms.Button buttonSearch;
        protected System.Windows.Forms.ComboBox comboBoxFolder;
        protected System.Windows.Forms.TabPage tabPageSearch;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanelAdditional;
        protected System.Windows.Forms.TabPage tabPageAdditional;
        protected System.Windows.Forms.TableLayoutPanel tableLayoutPanelBase;
        protected System.Windows.Forms.Label labelChoose;
        private System.Windows.Forms.Panel panelLabel;
        private System.Windows.Forms.Label labelSearch;
        protected System.Windows.Forms.Panel panelForText;
    }
}
