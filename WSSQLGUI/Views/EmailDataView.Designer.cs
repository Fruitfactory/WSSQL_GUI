namespace WSSQLGUI.Views
{
    partial class EmailDataView
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
            this.dataGridViewEmail = new System.Windows.Forms.DataGridView();
            this.panelCheck = new System.Windows.Forms.Panel();
            this.checkBoxAttachments = new System.Windows.Forms.CheckBox();
            this.ColumnRecepient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnAttachment = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ColumnPreview = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEmail)).BeginInit();
            this.panelCheck.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridViewEmail
            // 
            this.dataGridViewEmail.AllowUserToAddRows = false;
            this.dataGridViewEmail.AllowUserToDeleteRows = false;
            this.dataGridViewEmail.AllowUserToResizeRows = false;
            this.dataGridViewEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewEmail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataGridViewEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewEmail.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewEmail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEmail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnRecepient,
            this.ColumnSubject,
            this.ColumnDate,
            this.ColumnAttachment,
            this.ColumnPreview});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Menu;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEmail.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEmail.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewEmail.MultiSelect = false;
            this.dataGridViewEmail.Name = "dataGridViewEmail";
            this.dataGridViewEmail.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridViewEmail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEmail.Size = new System.Drawing.Size(247, 155);
            this.dataGridViewEmail.TabIndex = 0;
            this.dataGridViewEmail.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewEmail_CellClick);
            this.dataGridViewEmail.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewEmail_RowEnter);
            this.dataGridViewEmail.RowLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewEmail_RowLeave);
            // 
            // panelCheck
            // 
            this.panelCheck.Controls.Add(this.checkBoxAttachments);
            this.panelCheck.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panelCheck.Location = new System.Drawing.Point(0, 158);
            this.panelCheck.Name = "panelCheck";
            this.panelCheck.Size = new System.Drawing.Size(247, 27);
            this.panelCheck.TabIndex = 1;
            // 
            // checkBoxAttachments
            // 
            this.checkBoxAttachments.AutoSize = true;
            this.checkBoxAttachments.Location = new System.Drawing.Point(3, 3);
            this.checkBoxAttachments.Name = "checkBoxAttachments";
            this.checkBoxAttachments.Size = new System.Drawing.Size(115, 17);
            this.checkBoxAttachments.TabIndex = 0;
            this.checkBoxAttachments.Text = "Show Attachments";
            this.checkBoxAttachments.UseVisualStyleBackColor = true;
            this.checkBoxAttachments.CheckedChanged += new System.EventHandler(this.checkBoxAttachments_CheckedChanged);
            // 
            // ColumnRecepient
            // 
            this.ColumnRecepient.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnRecepient.HeaderText = "Recepient";
            this.ColumnRecepient.Name = "ColumnRecepient";
            this.ColumnRecepient.ReadOnly = true;
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
            // ColumnAttachment
            // 
            this.ColumnAttachment.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnAttachment.HeaderText = "Attachments";
            this.ColumnAttachment.Name = "ColumnAttachment";
            this.ColumnAttachment.Visible = false;
            // 
            // ColumnPreview
            // 
            this.ColumnPreview.HeaderText = "...";
            this.ColumnPreview.Name = "ColumnPreview";
            this.ColumnPreview.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnPreview.Text = "...";
            this.ColumnPreview.Visible = false;
            // 
            // EmailDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelCheck);
            this.Controls.Add(this.dataGridViewEmail);
            this.Name = "EmailDataView";
            this.Size = new System.Drawing.Size(247, 185);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEmail)).EndInit();
            this.panelCheck.ResumeLayout(false);
            this.panelCheck.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewEmail;
        private System.Windows.Forms.Panel panelCheck;
        private System.Windows.Forms.CheckBox checkBoxAttachments;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRecepient;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;
        private System.Windows.Forms.DataGridViewComboBoxColumn ColumnAttachment;
        private System.Windows.Forms.DataGridViewButtonColumn ColumnPreview;

    }
}
