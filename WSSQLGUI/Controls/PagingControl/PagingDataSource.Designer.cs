using WSSQLGUI.Core;

namespace WSSQLGUI.Controls.PagingControl
{
    partial class PagingDataSource<T>
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
            this.panelBottom = new System.Windows.Forms.Panel();
            this.tableStatus = new System.Windows.Forms.TableLayoutPanel();
            this.labelCurrent = new System.Windows.Forms.Label();
            this.labelCount = new System.Windows.Forms.Label();
            this.labelCurrentText = new System.Windows.Forms.Label();
            this.labelCountText = new System.Windows.Forms.Label();
            this.tableLinks = new System.Windows.Forms.TableLayoutPanel();
            this.buttonLeft = new System.Windows.Forms.Button();
            this.buttonRigth = new System.Windows.Forms.Button();
            this.link1 = new System.Windows.Forms.LinkLabel();
            this.link2 = new System.Windows.Forms.LinkLabel();
            this.link3 = new System.Windows.Forms.LinkLabel();
            this.link4 = new System.Windows.Forms.LinkLabel();
            this.link5 = new System.Windows.Forms.LinkLabel();
            this.buttonFirst = new System.Windows.Forms.Button();
            this.buttonLast = new System.Windows.Forms.Button();
            this.panelFill = new System.Windows.Forms.Panel();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.panelBottom.SuspendLayout();
            this.tableStatus.SuspendLayout();
            this.tableLinks.SuspendLayout();
            this.panelFill.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // panelBottom
            // 
            this.panelBottom.Controls.Add(this.tableStatus);
            this.panelBottom.Controls.Add(this.tableLinks);
            this.panelBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelBottom.Location = new System.Drawing.Point(0, 272);
            this.panelBottom.Name = "panelBottom";
            this.panelBottom.Size = new System.Drawing.Size(652, 33);
            this.panelBottom.TabIndex = 0;
            this.panelBottom.Resize += new System.EventHandler(this.panelBottom_Resize);
            // 
            // tableStatus
            // 
            this.tableStatus.ColumnCount = 2;
            this.tableStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableStatus.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableStatus.Controls.Add(this.labelCurrent, 0, 0);
            this.tableStatus.Controls.Add(this.labelCount, 1, 0);
            this.tableStatus.Controls.Add(this.labelCurrentText, 0, 1);
            this.tableStatus.Controls.Add(this.labelCountText, 1, 1);
            this.tableStatus.Dock = System.Windows.Forms.DockStyle.Left;
            this.tableStatus.Location = new System.Drawing.Point(0, 0);
            this.tableStatus.Name = "tableStatus";
            this.tableStatus.RowCount = 2;
            this.tableStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableStatus.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableStatus.Size = new System.Drawing.Size(161, 33);
            this.tableStatus.TabIndex = 1;
            // 
            // labelCurrent
            // 
            this.labelCurrent.AutoSize = true;
            this.labelCurrent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCurrent.Location = new System.Drawing.Point(3, 0);
            this.labelCurrent.Name = "labelCurrent";
            this.labelCurrent.Size = new System.Drawing.Size(74, 16);
            this.labelCurrent.TabIndex = 0;
            this.labelCurrent.Text = "Current page:";
            // 
            // labelCount
            // 
            this.labelCount.AutoSize = true;
            this.labelCount.Location = new System.Drawing.Point(83, 0);
            this.labelCount.Name = "labelCount";
            this.labelCount.Size = new System.Drawing.Size(70, 13);
            this.labelCount.TabIndex = 1;
            this.labelCount.Text = "Count pages:";
            // 
            // labelCurrentText
            // 
            this.labelCurrentText.AutoSize = true;
            this.labelCurrentText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCurrentText.Location = new System.Drawing.Point(3, 16);
            this.labelCurrentText.Name = "labelCurrentText";
            this.labelCurrentText.Size = new System.Drawing.Size(74, 17);
            this.labelCurrentText.TabIndex = 2;
            // 
            // labelCountText
            // 
            this.labelCountText.AutoSize = true;
            this.labelCountText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelCountText.Location = new System.Drawing.Point(83, 16);
            this.labelCountText.Name = "labelCountText";
            this.labelCountText.Size = new System.Drawing.Size(75, 17);
            this.labelCountText.TabIndex = 3;
            // 
            // tableLinks
            // 
            this.tableLinks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLinks.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLinks.ColumnCount = 9;
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLinks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 31F));
            this.tableLinks.Controls.Add(this.buttonLeft, 1, 0);
            this.tableLinks.Controls.Add(this.buttonRigth, 7, 0);
            this.tableLinks.Controls.Add(this.link1, 2, 0);
            this.tableLinks.Controls.Add(this.link2, 3, 0);
            this.tableLinks.Controls.Add(this.link3, 4, 0);
            this.tableLinks.Controls.Add(this.link4, 5, 0);
            this.tableLinks.Controls.Add(this.link5, 6, 0);
            this.tableLinks.Controls.Add(this.buttonFirst, 0, 0);
            this.tableLinks.Controls.Add(this.buttonLast, 8, 0);
            this.tableLinks.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
            this.tableLinks.Location = new System.Drawing.Point(249, 0);
            this.tableLinks.MaximumSize = new System.Drawing.Size(271, 32);
            this.tableLinks.MinimumSize = new System.Drawing.Size(271, 32);
            this.tableLinks.Name = "tableLinks";
            this.tableLinks.RowCount = 1;
            this.tableLinks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLinks.Size = new System.Drawing.Size(271, 32);
            this.tableLinks.TabIndex = 0;
            this.tableLinks.Visible = false;
            // 
            // buttonLeft
            // 
            this.buttonLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLeft.Image = global::WSSQLGUI.Properties.Resources.previous;
            this.buttonLeft.Location = new System.Drawing.Point(33, 3);
            this.buttonLeft.MaximumSize = new System.Drawing.Size(24, 24);
            this.buttonLeft.MinimumSize = new System.Drawing.Size(24, 24);
            this.buttonLeft.Name = "buttonLeft";
            this.buttonLeft.Size = new System.Drawing.Size(24, 24);
            this.buttonLeft.TabIndex = 0;
            this.buttonLeft.UseVisualStyleBackColor = true;
            this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
            // 
            // buttonRigth
            // 
            this.buttonRigth.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonRigth.Image = global::WSSQLGUI.Properties.Resources.next;
            this.buttonRigth.Location = new System.Drawing.Point(213, 3);
            this.buttonRigth.MaximumSize = new System.Drawing.Size(24, 24);
            this.buttonRigth.MinimumSize = new System.Drawing.Size(24, 24);
            this.buttonRigth.Name = "buttonRigth";
            this.buttonRigth.Size = new System.Drawing.Size(24, 24);
            this.buttonRigth.TabIndex = 1;
            this.buttonRigth.UseVisualStyleBackColor = true;
            this.buttonRigth.Click += new System.EventHandler(this.buttonRigth_Click);
            // 
            // link1
            // 
            this.link1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.link1.AutoSize = true;
            this.link1.Location = new System.Drawing.Point(63, 0);
            this.link1.Name = "link1";
            this.link1.Size = new System.Drawing.Size(24, 32);
            this.link1.TabIndex = 2;
            this.link1.TabStop = true;
            this.link1.Text = "1";
            this.link1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.link1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link2
            // 
            this.link2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.link2.AutoSize = true;
            this.link2.Location = new System.Drawing.Point(93, 0);
            this.link2.Name = "link2";
            this.link2.Size = new System.Drawing.Size(24, 32);
            this.link2.TabIndex = 3;
            this.link2.TabStop = true;
            this.link2.Text = "2";
            this.link2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.link2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link3
            // 
            this.link3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.link3.AutoSize = true;
            this.link3.Location = new System.Drawing.Point(123, 0);
            this.link3.Name = "link3";
            this.link3.Size = new System.Drawing.Size(24, 32);
            this.link3.TabIndex = 4;
            this.link3.TabStop = true;
            this.link3.Text = "3";
            this.link3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.link3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link4
            // 
            this.link4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.link4.AutoSize = true;
            this.link4.Location = new System.Drawing.Point(153, 0);
            this.link4.Name = "link4";
            this.link4.Size = new System.Drawing.Size(24, 32);
            this.link4.TabIndex = 5;
            this.link4.TabStop = true;
            this.link4.Text = "4";
            this.link4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.link4.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // link5
            // 
            this.link5.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.link5.AutoSize = true;
            this.link5.Location = new System.Drawing.Point(183, 0);
            this.link5.Name = "link5";
            this.link5.Size = new System.Drawing.Size(24, 32);
            this.link5.TabIndex = 6;
            this.link5.TabStop = true;
            this.link5.Text = "5";
            this.link5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.link5.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.link_LinkClicked);
            // 
            // buttonFirst
            // 
            this.buttonFirst.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonFirst.Image = global::WSSQLGUI.Properties.Resources.first;
            this.buttonFirst.Location = new System.Drawing.Point(3, 3);
            this.buttonFirst.MaximumSize = new System.Drawing.Size(24, 24);
            this.buttonFirst.MinimumSize = new System.Drawing.Size(24, 24);
            this.buttonFirst.Name = "buttonFirst";
            this.buttonFirst.Size = new System.Drawing.Size(24, 24);
            this.buttonFirst.TabIndex = 7;
            this.buttonFirst.UseVisualStyleBackColor = true;
            this.buttonFirst.Click += new System.EventHandler(this.buttonFirst_Click);
            // 
            // buttonLast
            // 
            this.buttonLast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.buttonLast.Image = global::WSSQLGUI.Properties.Resources.last;
            this.buttonLast.Location = new System.Drawing.Point(243, 3);
            this.buttonLast.MaximumSize = new System.Drawing.Size(24, 24);
            this.buttonLast.MinimumSize = new System.Drawing.Size(24, 24);
            this.buttonLast.Name = "buttonLast";
            this.buttonLast.Size = new System.Drawing.Size(24, 24);
            this.buttonLast.TabIndex = 8;
            this.buttonLast.UseVisualStyleBackColor = true;
            this.buttonLast.Click += new System.EventHandler(this.buttonLast_Click);
            // 
            // panelFill
            // 
            this.panelFill.Controls.Add(this.dataGridView);
            this.panelFill.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFill.Location = new System.Drawing.Point(0, 0);
            this.panelFill.Name = "panelFill";
            this.panelFill.Size = new System.Drawing.Size(652, 272);
            this.panelFill.TabIndex = 1;
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView.Location = new System.Drawing.Point(0, 0);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.ReadOnly = true;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.Size = new System.Drawing.Size(652, 272);
            this.dataGridView.TabIndex = 0;
            // 
            // PagingDataSource
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelFill);
            this.Controls.Add(this.panelBottom);
            this.Name = "PagingDataSource";
            this.Size = new System.Drawing.Size(652, 305);
            this.panelBottom.ResumeLayout(false);
            this.tableStatus.ResumeLayout(false);
            this.tableStatus.PerformLayout();
            this.tableLinks.ResumeLayout(false);
            this.tableLinks.PerformLayout();
            this.panelFill.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBottom;
        private System.Windows.Forms.Panel panelFill;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.TableLayoutPanel tableLinks;
        private System.Windows.Forms.Button buttonLeft;
        private System.Windows.Forms.Button buttonRigth;
        private System.Windows.Forms.LinkLabel link1;
        private System.Windows.Forms.LinkLabel link2;
        private System.Windows.Forms.LinkLabel link3;
        private System.Windows.Forms.LinkLabel link4;
        private System.Windows.Forms.LinkLabel link5;
        private System.Windows.Forms.Button buttonFirst;
        private System.Windows.Forms.Button buttonLast;
        private System.Windows.Forms.TableLayoutPanel tableStatus;
        private System.Windows.Forms.Label labelCurrent;
        private System.Windows.Forms.Label labelCount;
        private System.Windows.Forms.Label labelCurrentText;
        private System.Windows.Forms.Label labelCountText;
    }
}
