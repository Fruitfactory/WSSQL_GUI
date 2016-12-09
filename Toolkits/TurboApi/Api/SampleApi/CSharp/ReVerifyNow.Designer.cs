partial class ReVerifyNow
{
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnReverify = new System.Windows.Forms.Button();
            this.lblDescr = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExit.Location = new System.Drawing.Point(258, 56);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(174, 30);
            this.btnExit.TabIndex = 4;
            this.btnExit.Text = "Exit application";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnReverify
            // 
            this.btnReverify.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnReverify.Location = new System.Drawing.Point(19, 56);
            this.btnReverify.Margin = new System.Windows.Forms.Padding(4);
            this.btnReverify.Name = "btnReverify";
            this.btnReverify.Size = new System.Drawing.Size(123, 30);
            this.btnReverify.TabIndex = 3;
            this.btnReverify.Text = "Re-verify now";
            this.btnReverify.UseVisualStyleBackColor = true;
            this.btnReverify.Click += new System.EventHandler(this.btnReverify_Click);
            // 
            // lblDescr
            // 
            this.lblDescr.AutoSize = true;
            this.lblDescr.Location = new System.Drawing.Point(16, 11);
            this.lblDescr.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblDescr.Name = "lblDescr";
            this.lblDescr.Size = new System.Drawing.Size(358, 17);
            this.lblDescr.TabIndex = 5;
            this.lblDescr.Text = "You have X days to re-verify with the activation servers.";
            // 
            // ReVerifyNow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(458, 105);
            this.Controls.Add(this.lblDescr);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnReverify);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ReVerifyNow";
            this.Padding = new System.Windows.Forms.Padding(12, 11, 12, 11);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Re-verify with the activation servers";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    private System.Windows.Forms.Button btnExit;
    private System.Windows.Forms.Button btnReverify;
    private System.Windows.Forms.Label lblDescr;
}