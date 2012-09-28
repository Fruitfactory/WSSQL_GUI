namespace WSSQLGUI.Views
{
    partial class AllFilesDataView
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
            this.dataGridViewFiles = new System.Windows.Forms.DataGridView();
            this.columnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.columnPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFiles)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewFiles
            // 
            this.dataGridViewFiles.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewFiles.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.columnName,
            this.columnPath});
            this.dataGridViewFiles.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewFiles.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewFiles.Name = "dataGridViewFiles";
            this.dataGridViewFiles.Size = new System.Drawing.Size(247, 196);
            this.dataGridViewFiles.TabIndex = 0;
            // 
            // columnName
            // 
            this.columnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnName.HeaderText = "Name";
            this.columnName.MinimumWidth = 150;
            this.columnName.Name = "columnName";
            this.columnName.ReadOnly = true;
            this.columnName.Width = 150;
            // 
            // columnPath
            // 
            this.columnPath.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.columnPath.HeaderText = "Path";
            this.columnPath.MinimumWidth = 200;
            this.columnPath.Name = "columnPath";
            this.columnPath.ReadOnly = true;
            this.columnPath.Width = 200;
            // 
            // AllFilesDataView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewFiles);
            this.Name = "AllFilesDataView";
            this.Size = new System.Drawing.Size(247, 196);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewFiles)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewFiles;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn columnPath;
    }
}
