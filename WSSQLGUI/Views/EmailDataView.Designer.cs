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
            this.ColumnRecepient = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSubject = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEmail)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewEmail
            // 
            this.dataGridViewEmail.AllowUserToAddRows = false;
            this.dataGridViewEmail.AllowUserToDeleteRows = false;
            this.dataGridViewEmail.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridViewEmail.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridViewEmail.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridViewEmail.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewEmail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewEmail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnRecepient,
            this.ColumnSubject,
            this.ColumnDate});
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Menu;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.DarkGray;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewEmail.DefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewEmail.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewEmail.MultiSelect = false;
            this.dataGridViewEmail.Name = "dataGridViewEmail";
            this.dataGridViewEmail.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridViewEmail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewEmail.Size = new System.Drawing.Size(247, 185);
            this.dataGridViewEmail.TabIndex = 0;
            // 
            // ColumnRecepient
            // 
            this.ColumnRecepient.HeaderText = "Recepient";
            this.ColumnRecepient.Name = "ColumnRecepient";
            this.ColumnRecepient.ReadOnly = true;
            this.ColumnRecepient.Width = 81;
            // 
            // ColumnSubject
            // 
            this.ColumnSubject.HeaderText = "Subject";
            this.ColumnSubject.Name = "ColumnSubject";
            this.ColumnSubject.ReadOnly = true;
            this.ColumnSubject.Width = 68;
            // 
            // ColumnDate
            // 
            this.ColumnDate.HeaderText = "Date";
            this.ColumnDate.Name = "ColumnDate";
            this.ColumnDate.ReadOnly = true;
            this.ColumnDate.Width = 55;
            // 
            // EmailDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewEmail);
            this.Name = "EmailDataView";
            this.Size = new System.Drawing.Size(247, 185);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewEmail)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewEmail;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnRecepient;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnSubject;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnDate;

    }
}
