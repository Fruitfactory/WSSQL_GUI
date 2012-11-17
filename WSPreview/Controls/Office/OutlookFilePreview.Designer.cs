namespace C4F.DevKit.PreviewHandler.Controls.Office
{
    partial class OutlookFilePreview
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxSubject = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.webBrowserContent = new System.Windows.Forms.WebBrowser();
            this.textBoxTo = new System.Windows.Forms.TextBox();
            this.textBoxSend = new System.Windows.Forms.TextBox();
            this.textBoxAttachments = new System.Windows.Forms.TextBox();
            this.webFrom = new System.Windows.Forms.WebBrowser();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.86736F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.13264F));
            this.tableLayoutPanel.Controls.Add(this.textBoxSubject, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.webBrowserContent, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.textBoxTo, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxSend, 1, 4);
            this.tableLayoutPanel.Controls.Add(this.textBoxAttachments, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.webFrom, 0, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(623, 452);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // textBoxSubject
            // 
            this.textBoxSubject.BackColor = System.Drawing.Color.White;
            this.textBoxSubject.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.tableLayoutPanel.SetColumnSpan(this.textBoxSubject, 2);
            this.textBoxSubject.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSubject.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.textBoxSubject.Location = new System.Drawing.Point(5, 5);
            this.textBoxSubject.Margin = new System.Windows.Forms.Padding(5);
            this.textBoxSubject.Multiline = true;
            this.textBoxSubject.Name = "textBoxSubject";
            this.textBoxSubject.ReadOnly = true;
            this.textBoxSubject.Size = new System.Drawing.Size(613, 20);
            this.textBoxSubject.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.Gray;
            this.label1.Location = new System.Drawing.Point(3, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "To:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(3, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Attacments:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(3, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Send:";
            // 
            // webBrowserContent
            // 
            this.tableLayoutPanel.SetColumnSpan(this.webBrowserContent, 2);
            this.webBrowserContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserContent.Location = new System.Drawing.Point(3, 153);
            this.webBrowserContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserContent.Name = "webBrowserContent";
            this.webBrowserContent.Size = new System.Drawing.Size(617, 296);
            this.webBrowserContent.TabIndex = 5;
            // 
            // textBoxTo
            // 
            this.textBoxTo.BackColor = System.Drawing.Color.White;
            this.textBoxTo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxTo.Dock = System.Windows.Forms.DockStyle.Top;
            this.textBoxTo.Location = new System.Drawing.Point(89, 83);
            this.textBoxTo.Multiline = true;
            this.textBoxTo.Name = "textBoxTo";
            this.textBoxTo.ReadOnly = true;
            this.textBoxTo.Size = new System.Drawing.Size(531, 14);
            this.textBoxTo.TabIndex = 6;
            // 
            // textBoxSend
            // 
            this.textBoxSend.BackColor = System.Drawing.Color.White;
            this.textBoxSend.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxSend.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxSend.Location = new System.Drawing.Point(89, 133);
            this.textBoxSend.Multiline = true;
            this.textBoxSend.Name = "textBoxSend";
            this.textBoxSend.ReadOnly = true;
            this.textBoxSend.Size = new System.Drawing.Size(531, 14);
            this.textBoxSend.TabIndex = 7;
            // 
            // textBoxAttachments
            // 
            this.textBoxAttachments.BackColor = System.Drawing.Color.White;
            this.textBoxAttachments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxAttachments.Location = new System.Drawing.Point(89, 103);
            this.textBoxAttachments.Multiline = true;
            this.textBoxAttachments.Name = "textBoxAttachments";
            this.textBoxAttachments.ReadOnly = true;
            this.textBoxAttachments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textBoxAttachments.Size = new System.Drawing.Size(531, 24);
            this.textBoxAttachments.TabIndex = 8;
            // 
            // webFrom
            // 
            this.tableLayoutPanel.SetColumnSpan(this.webFrom, 2);
            this.webFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webFrom.Location = new System.Drawing.Point(3, 33);
            this.webFrom.MinimumSize = new System.Drawing.Size(20, 20);
            this.webFrom.Name = "webFrom";
            this.webFrom.Size = new System.Drawing.Size(617, 44);
            this.webFrom.TabIndex = 9;
            // 
            // OutlookFilePreview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "OutlookFilePreview";
            this.Size = new System.Drawing.Size(623, 452);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.TextBox textBoxSubject;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.WebBrowser webBrowserContent;
        private System.Windows.Forms.TextBox textBoxTo;
        private System.Windows.Forms.TextBox textBoxSend;
        private System.Windows.Forms.TextBox textBoxAttachments;
        private System.Windows.Forms.WebBrowser webFrom;
    }
}
