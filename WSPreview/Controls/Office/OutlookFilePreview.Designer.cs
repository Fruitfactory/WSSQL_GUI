using System.Windows.Forms;

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OutlookFilePreview));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.textBoxSubject = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.webBrowserContent = new System.Windows.Forms.WebBrowser();
            this.textBoxTo = new System.Windows.Forms.TextBox();
            this.textBoxSend = new System.Windows.Forms.TextBox();
            this.webFrom = new System.Windows.Forms.WebBrowser();
            this.label1 = new System.Windows.Forms.Label();
            this.labelCC = new System.Windows.Forms.Label();
            this.textBoxCC = new System.Windows.Forms.TextBox();
            this.listViewAttachments = new System.Windows.Forms.ListView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 13.86736F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 86.13264F));
            this.tableLayoutPanel.Controls.Add(this.textBoxSubject, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.label2, 0, 4);
            this.tableLayoutPanel.Controls.Add(this.label3, 0, 5);
            this.tableLayoutPanel.Controls.Add(this.webBrowserContent, 0, 6);
            this.tableLayoutPanel.Controls.Add(this.textBoxTo, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxSend, 1, 5);
            this.tableLayoutPanel.Controls.Add(this.webFrom, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.label1, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.labelCC, 0, 3);
            this.tableLayoutPanel.Controls.Add(this.textBoxCC, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.listViewAttachments, 1, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 7;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 50F));
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
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.Gray;
            this.label2.Location = new System.Drawing.Point(3, 120);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 26);
            this.label2.TabIndex = 3;
            this.label2.Text = "Attachments:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.Gray;
            this.label3.Location = new System.Drawing.Point(3, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Send:";
            // 
            // webBrowserContent
            // 
            this.tableLayoutPanel.SetColumnSpan(this.webBrowserContent, 2);
            this.webBrowserContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.webBrowserContent.Location = new System.Drawing.Point(3, 193);
            this.webBrowserContent.MinimumSize = new System.Drawing.Size(20, 20);
            this.webBrowserContent.Name = "webBrowserContent";
            this.webBrowserContent.Size = new System.Drawing.Size(617, 256);
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
            this.textBoxSend.Location = new System.Drawing.Point(89, 173);
            this.textBoxSend.Multiline = true;
            this.textBoxSend.Name = "textBoxSend";
            this.textBoxSend.ReadOnly = true;
            this.textBoxSend.Size = new System.Drawing.Size(531, 14);
            this.textBoxSend.TabIndex = 7;
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
            // labelCC
            // 
            this.labelCC.AutoSize = true;
            this.labelCC.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.labelCC.ForeColor = System.Drawing.Color.Gray;
            this.labelCC.Location = new System.Drawing.Point(3, 100);
            this.labelCC.Name = "labelCC";
            this.labelCC.Size = new System.Drawing.Size(27, 13);
            this.labelCC.TabIndex = 10;
            this.labelCC.Text = "CC:";
            // 
            // textBoxCC
            // 
            this.textBoxCC.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBoxCC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxCC.Location = new System.Drawing.Point(86, 100);
            this.textBoxCC.Margin = new System.Windows.Forms.Padding(0);
            this.textBoxCC.Multiline = true;
            this.textBoxCC.Name = "textBoxCC";
            this.textBoxCC.Size = new System.Drawing.Size(537, 20);
            this.textBoxCC.TabIndex = 11;
            // 
            // listViewAttachments
            // 
            this.listViewAttachments.Activation = System.Windows.Forms.ItemActivation.OneClick;
            this.listViewAttachments.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listViewAttachments.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewAttachments.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listViewAttachments.HotTracking = true;
            this.listViewAttachments.HoverSelection = true;
            this.listViewAttachments.LargeImageList = this.imageList;
            this.listViewAttachments.Location = new System.Drawing.Point(86, 120);
            this.listViewAttachments.Margin = new System.Windows.Forms.Padding(0);
            this.listViewAttachments.MultiSelect = false;
            this.listViewAttachments.Name = "listViewAttachments";
            this.listViewAttachments.Size = new System.Drawing.Size(537, 50);
            this.listViewAttachments.SmallImageList = this.imageList;
            this.listViewAttachments.StateImageList = this.imageList;
            this.listViewAttachments.TabIndex = 12;
            this.listViewAttachments.UseCompatibleStateImageBehavior = false;
            this.listViewAttachments.View = System.Windows.Forms.View.SmallIcon;
            this.listViewAttachments.ItemActivate += new System.EventHandler(this.listViewAttachments_ItemActivate);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "_blank.png");
            this.imageList.Images.SetKeyName(1, "_page.png");
            this.imageList.Images.SetKeyName(2, "aac.png");
            this.imageList.Images.SetKeyName(3, "ai.png");
            this.imageList.Images.SetKeyName(4, "aiff.png");
            this.imageList.Images.SetKeyName(5, "avi.png");
            this.imageList.Images.SetKeyName(6, "bmp.png");
            this.imageList.Images.SetKeyName(7, "c.png");
            this.imageList.Images.SetKeyName(8, "cpp.png");
            this.imageList.Images.SetKeyName(9, "css.png");
            this.imageList.Images.SetKeyName(10, "dat.png");
            this.imageList.Images.SetKeyName(11, "dmg.png");
            this.imageList.Images.SetKeyName(12, "doc.png");
            this.imageList.Images.SetKeyName(13, "docx.png");
            this.imageList.Images.SetKeyName(14, "dotx.png");
            this.imageList.Images.SetKeyName(15, "dwg.png");
            this.imageList.Images.SetKeyName(16, "dxf.png");
            this.imageList.Images.SetKeyName(17, "eps.png");
            this.imageList.Images.SetKeyName(18, "exe.png");
            this.imageList.Images.SetKeyName(19, "flv.png");
            this.imageList.Images.SetKeyName(20, "gif.png");
            this.imageList.Images.SetKeyName(21, "h.png");
            this.imageList.Images.SetKeyName(22, "hpp.png");
            this.imageList.Images.SetKeyName(23, "html.png");
            this.imageList.Images.SetKeyName(24, "ics.png");
            this.imageList.Images.SetKeyName(25, "iso.png");
            this.imageList.Images.SetKeyName(26, "java.png");
            this.imageList.Images.SetKeyName(27, "jpg.png");
            this.imageList.Images.SetKeyName(28, "key.png");
            this.imageList.Images.SetKeyName(29, "mid.png");
            this.imageList.Images.SetKeyName(30, "mp3.png");
            this.imageList.Images.SetKeyName(31, "mp4.png");
            this.imageList.Images.SetKeyName(32, "mpg.png");
            this.imageList.Images.SetKeyName(33, "odf.png");
            this.imageList.Images.SetKeyName(34, "ods.png");
            this.imageList.Images.SetKeyName(35, "odt.png");
            this.imageList.Images.SetKeyName(36, "otp.png");
            this.imageList.Images.SetKeyName(37, "ots.png");
            this.imageList.Images.SetKeyName(38, "ott.png");
            this.imageList.Images.SetKeyName(39, "pdf.png");
            this.imageList.Images.SetKeyName(40, "php.png");
            this.imageList.Images.SetKeyName(41, "png.png");
            this.imageList.Images.SetKeyName(42, "ppt.png");
            this.imageList.Images.SetKeyName(43, "psd.png");
            this.imageList.Images.SetKeyName(44, "py.png");
            this.imageList.Images.SetKeyName(45, "qt.png");
            this.imageList.Images.SetKeyName(46, "rar.png");
            this.imageList.Images.SetKeyName(47, "rb.png");
            this.imageList.Images.SetKeyName(48, "rtf.png");
            this.imageList.Images.SetKeyName(49, "sql.png");
            this.imageList.Images.SetKeyName(50, "tga.png");
            this.imageList.Images.SetKeyName(51, "tgz.png");
            this.imageList.Images.SetKeyName(52, "tiff.png");
            this.imageList.Images.SetKeyName(53, "txt.png");
            this.imageList.Images.SetKeyName(54, "wav.png");
            this.imageList.Images.SetKeyName(55, "xls.png");
            this.imageList.Images.SetKeyName(56, "xlsx.png");
            this.imageList.Images.SetKeyName(57, "xml.png");
            this.imageList.Images.SetKeyName(58, "yml.png");
            this.imageList.Images.SetKeyName(59, "zip.png");
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
        private System.Windows.Forms.WebBrowser webFrom;
        private System.Windows.Forms.Label labelCC;
        private System.Windows.Forms.TextBox textBoxCC;
        private System.Windows.Forms.ListView listViewAttachments;
        private System.Windows.Forms.ImageList imageList;

    }
}
