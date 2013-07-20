namespace WSPreview.PreviewHandler.Controls.CsvControl
{
    partial class CsvPreview
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
            this.dataGridCsv = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCsv)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridCsv
            // 
            this.dataGridCsv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridCsv.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridCsv.Location = new System.Drawing.Point(0, 0);
            this.dataGridCsv.Name = "dataGridCsv";
            this.dataGridCsv.ReadOnly = true;
            this.dataGridCsv.Size = new System.Drawing.Size(150, 150);
            this.dataGridCsv.TabIndex = 0;
            // 
            // CsvPreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridCsv);
            this.Name = "CsvPreview";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridCsv)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridCsv;
    }
}
