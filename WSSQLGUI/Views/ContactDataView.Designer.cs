namespace WSSQLGUI.Views
{
    partial class ContactDataView
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tableLayout = new System.Windows.Forms.TableLayoutPanel();
            this.panelCellZeroZero = new System.Windows.Forms.Panel();
            this.panelFoto = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.panelLabel = new System.Windows.Forms.Panel();
            this.labelFoto = new System.Windows.Forms.Label();
            this.panelEmails = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.linkLabelEmail3 = new System.Windows.Forms.LinkLabel();
            this.linkLabelEmail2 = new System.Windows.Forms.LinkLabel();
            this.linkLabelEmail = new System.Windows.Forms.LinkLabel();
            this.panelForName = new System.Windows.Forms.Panel();
            this.labelName = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBoxEmails = new System.Windows.Forms.GroupBox();
            this.dataGridViewContactEmails = new System.Windows.Forms.DataGridView();
            this.ColumnRecep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tableLayout.SuspendLayout();
            this.panelCellZeroZero.SuspendLayout();
            this.panelFoto.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.panelLabel.SuspendLayout();
            this.panelEmails.SuspendLayout();
            this.panelForName.SuspendLayout();
            this.groupBoxEmails.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContactEmails)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayout
            // 
            this.tableLayout.ColumnCount = 2;
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayout.Controls.Add(this.panelCellZeroZero, 0, 0);
            this.tableLayout.Controls.Add(this.panelEmails, 1, 0);
            this.tableLayout.Controls.Add(this.panelForName, 0, 1);
            this.tableLayout.Controls.Add(this.groupBoxEmails, 0, 2);
            this.tableLayout.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayout.Location = new System.Drawing.Point(0, 0);
            this.tableLayout.Name = "tableLayout";
            this.tableLayout.RowCount = 3;
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
            this.tableLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayout.Size = new System.Drawing.Size(321, 278);
            this.tableLayout.TabIndex = 0;
            // 
            // panelCellZeroZero
            // 
            this.panelCellZeroZero.Controls.Add(this.panelFoto);
            this.panelCellZeroZero.Controls.Add(this.panelLabel);
            this.panelCellZeroZero.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCellZeroZero.Location = new System.Drawing.Point(3, 3);
            this.panelCellZeroZero.Name = "panelCellZeroZero";
            this.panelCellZeroZero.Size = new System.Drawing.Size(186, 105);
            this.panelCellZeroZero.TabIndex = 0;
            // 
            // panelFoto
            // 
            this.panelFoto.Controls.Add(this.pictureBox);
            this.panelFoto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFoto.Location = new System.Drawing.Point(0, 23);
            this.panelFoto.Name = "panelFoto";
            this.panelFoto.Size = new System.Drawing.Size(186, 82);
            this.panelFoto.TabIndex = 1;
            // 
            // pictureBox
            // 
            this.pictureBox.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(186, 82);
            this.pictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            // 
            // panelLabel
            // 
            this.panelLabel.Controls.Add(this.labelFoto);
            this.panelLabel.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelLabel.Location = new System.Drawing.Point(0, 0);
            this.panelLabel.Name = "panelLabel";
            this.panelLabel.Size = new System.Drawing.Size(186, 23);
            this.panelLabel.TabIndex = 0;
            // 
            // labelFoto
            // 
            this.labelFoto.AutoSize = true;
            this.labelFoto.Location = new System.Drawing.Point(3, 7);
            this.labelFoto.Name = "labelFoto";
            this.labelFoto.Size = new System.Drawing.Size(31, 13);
            this.labelFoto.TabIndex = 0;
            this.labelFoto.Text = "Foto:";
            // 
            // panelEmails
            // 
            this.panelEmails.Controls.Add(this.label3);
            this.panelEmails.Controls.Add(this.label2);
            this.panelEmails.Controls.Add(this.label1);
            this.panelEmails.Controls.Add(this.linkLabelEmail3);
            this.panelEmails.Controls.Add(this.linkLabelEmail2);
            this.panelEmails.Controls.Add(this.linkLabelEmail);
            this.panelEmails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelEmails.Location = new System.Drawing.Point(195, 3);
            this.panelEmails.Name = "panelEmails";
            this.panelEmails.Size = new System.Drawing.Size(123, 105);
            this.panelEmails.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(47, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "E-mail 3:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "E-mail 2:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "E-mail:";
            // 
            // linkLabelEmail3
            // 
            this.linkLabelEmail3.AutoSize = true;
            this.linkLabelEmail3.Location = new System.Drawing.Point(59, 68);
            this.linkLabelEmail3.Name = "linkLabelEmail3";
            this.linkLabelEmail3.Size = new System.Drawing.Size(36, 13);
            this.linkLabelEmail3.TabIndex = 2;
            this.linkLabelEmail3.TabStop = true;
            this.linkLabelEmail3.Text = "<n/a>";
            // 
            // linkLabelEmail2
            // 
            this.linkLabelEmail2.AutoSize = true;
            this.linkLabelEmail2.Location = new System.Drawing.Point(59, 39);
            this.linkLabelEmail2.Name = "linkLabelEmail2";
            this.linkLabelEmail2.Size = new System.Drawing.Size(36, 13);
            this.linkLabelEmail2.TabIndex = 1;
            this.linkLabelEmail2.TabStop = true;
            this.linkLabelEmail2.Text = "<n/a>";
            // 
            // linkLabelEmail
            // 
            this.linkLabelEmail.AutoSize = true;
            this.linkLabelEmail.Location = new System.Drawing.Point(59, 10);
            this.linkLabelEmail.Name = "linkLabelEmail";
            this.linkLabelEmail.Size = new System.Drawing.Size(36, 13);
            this.linkLabelEmail.TabIndex = 0;
            this.linkLabelEmail.TabStop = true;
            this.linkLabelEmail.Text = "<n/a>";
            // 
            // panelForName
            // 
            this.tableLayout.SetColumnSpan(this.panelForName, 2);
            this.panelForName.Controls.Add(this.labelName);
            this.panelForName.Controls.Add(this.label4);
            this.panelForName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelForName.Location = new System.Drawing.Point(3, 114);
            this.panelForName.Name = "panelForName";
            this.panelForName.Size = new System.Drawing.Size(315, 21);
            this.panelForName.TabIndex = 2;
            // 
            // labelName
            // 
            this.labelName.AutoSize = true;
            this.labelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelName.ForeColor = System.Drawing.Color.Blue;
            this.labelName.Location = new System.Drawing.Point(82, 5);
            this.labelName.Name = "labelName";
            this.labelName.Size = new System.Drawing.Size(0, 15);
            this.labelName.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 5);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(76, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Contact name:";
            // 
            // groupBoxEmails
            // 
            this.tableLayout.SetColumnSpan(this.groupBoxEmails, 2);
            this.groupBoxEmails.Controls.Add(this.dataGridViewContactEmails);
            this.groupBoxEmails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxEmails.Location = new System.Drawing.Point(3, 141);
            this.groupBoxEmails.Name = "groupBoxEmails";
            this.groupBoxEmails.Size = new System.Drawing.Size(315, 134);
            this.groupBoxEmails.TabIndex = 3;
            this.groupBoxEmails.TabStop = false;
            this.groupBoxEmails.Text = "E-mails";
            // 
            // dataGridViewContactEmails
            // 
            this.dataGridViewContactEmails.AllowUserToAddRows = false;
            this.dataGridViewContactEmails.AllowUserToDeleteRows = false;
            this.dataGridViewContactEmails.AllowUserToResizeRows = false;
            this.dataGridViewContactEmails.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewContactEmails.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewContactEmails.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewContactEmails.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewContactEmails.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewContactEmails.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnRecep,
            this.ColumnSubject,
            this.ColumnDate});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Menu;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewContactEmails.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewContactEmails.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewContactEmails.Location = new System.Drawing.Point(3, 16);
            this.dataGridViewContactEmails.MultiSelect = false;
            this.dataGridViewContactEmails.Name = "dataGridViewContactEmails";
            this.dataGridViewContactEmails.ReadOnly = true;
            this.dataGridViewContactEmails.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewContactEmails.Size = new System.Drawing.Size(309, 115);
            this.dataGridViewContactEmails.TabIndex = 0;
            this.dataGridViewContactEmails.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewContactEmails_CellLeave);
            this.dataGridViewContactEmails.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewContactEmails_RowEnter);
            this.dataGridViewContactEmails.SelectionChanged += new System.EventHandler(this.dataGridViewContactEmails_SelectionChanged);
            // 
            // ColumnRecep
            // 
            this.ColumnRecep.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnRecep.HeaderText = "Recepient";
            this.ColumnRecep.Name = "ColumnRecep";
            this.ColumnRecep.ReadOnly = true;
            // 
            // ColumnSubject
            // 
            this.ColumnSubject.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnSubject.HeaderText = "Subject";
            this.ColumnSubject.Name = "ColumnSubject";
            this.ColumnSubject.ReadOnly = true;
            // 
            // ColumnDate
            // 
            this.ColumnDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnDate.HeaderText = "Date";
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            // 
            // ContactDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayout);
            this.Name = "ContactDataView";
            this.Size = new System.Drawing.Size(321, 278);
            this.tableLayout.ResumeLayout(false);
            this.panelCellZeroZero.ResumeLayout(false);
            this.panelFoto.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.panelLabel.ResumeLayout(false);
            this.panelLabel.PerformLayout();
            this.panelEmails.ResumeLayout(false);
            this.panelEmails.PerformLayout();
            this.panelForName.ResumeLayout(false);
            this.panelForName.PerformLayout();
            this.groupBoxEmails.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewContactEmails)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayout;
        private System.Windows.Forms.Panel panelCellZeroZero;
        private System.Windows.Forms.Panel panelFoto;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Panel panelLabel;
        private System.Windows.Forms.Label labelFoto;
        private System.Windows.Forms.Panel panelEmails;
        private System.Windows.Forms.LinkLabel linkLabelEmail;
        private System.Windows.Forms.LinkLabel linkLabelEmail3;
        private System.Windows.Forms.LinkLabel linkLabelEmail2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelForName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label labelName;
        private System.Windows.Forms.GroupBox groupBoxEmails;
        private System.Windows.Forms.DataGridView dataGridViewContactEmails;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRecep;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;

    }
}
