namespace CSharp
{
    partial class Form1
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.mainMenu1 = new System.Windows.Forms.MainMenu(this.components);
            this.menuItem1 = new System.Windows.Forms.MenuItem();
            this.menuItem2 = new System.Windows.Forms.MenuItem();
            this.mnuHelp = new System.Windows.Forms.MenuItem();
            this.mnuHelpContents = new System.Windows.Forms.MenuItem();
            this.mnuActDeact = new System.Windows.Forms.MenuItem();
            this.txtMain = new System.Windows.Forms.TextBox();
            this.lblTrialMessage = new System.Windows.Forms.Label();
            this.btnExtendTrial = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem1,
            this.mnuHelp});
            // 
            // menuItem1
            // 
            this.menuItem1.Index = 0;
            this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItem2});
            this.menuItem1.Text = "&File";
            // 
            // menuItem2
            // 
            this.menuItem2.Index = 0;
            this.menuItem2.Text = "New";
            // 
            // mnuHelp
            // 
            this.mnuHelp.Index = 1;
            this.mnuHelp.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.mnuHelpContents,
            this.mnuActDeact});
            this.mnuHelp.Text = "&Help";
            // 
            // mnuHelpContents
            // 
            this.mnuHelpContents.Index = 0;
            this.mnuHelpContents.Text = "Help contents";
            // 
            // mnuActDeact
            // 
            this.mnuActDeact.Index = 1;
            this.mnuActDeact.Text = "Deactivate...";
            this.mnuActDeact.Click += new System.EventHandler(this.mnuActDeact_Click);
            // 
            // txtMain
            // 
            this.txtMain.Location = new System.Drawing.Point(12, 12);
            this.txtMain.Multiline = true;
            this.txtMain.Name = "txtMain";
            this.txtMain.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMain.Size = new System.Drawing.Size(384, 140);
            this.txtMain.TabIndex = 0;
            this.txtMain.Text = resources.GetString("txtMain.Text");
            // 
            // lblTrialMessage
            // 
            this.lblTrialMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblTrialMessage.AutoSize = true;
            this.lblTrialMessage.Location = new System.Drawing.Point(9, 169);
            this.lblTrialMessage.Name = "lblTrialMessage";
            this.lblTrialMessage.Size = new System.Drawing.Size(95, 13);
            this.lblTrialMessage.TabIndex = 1;
            this.lblTrialMessage.Text = "Your trial expires in";
            this.lblTrialMessage.Visible = false;
            // 
            // btnExtendTrial
            // 
            this.btnExtendTrial.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExtendTrial.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.btnExtendTrial.Location = new System.Drawing.Point(315, 165);
            this.btnExtendTrial.Name = "btnExtendTrial";
            this.btnExtendTrial.Size = new System.Drawing.Size(81, 25);
            this.btnExtendTrial.TabIndex = 2;
            this.btnExtendTrial.Text = "Extend trial";
            this.btnExtendTrial.UseVisualStyleBackColor = true;
            this.btnExtendTrial.Visible = false;
            this.btnExtendTrial.Click += new System.EventHandler(this.btnExtendTrial_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(408, 199);
            this.Controls.Add(this.btnExtendTrial);
            this.Controls.Add(this.lblTrialMessage);
            this.Controls.Add(this.txtMain);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Menu = this.mainMenu1;
            this.Name = "Form1";
            this.Text = "Text Editor Plus";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MainMenu mainMenu1;
        private System.Windows.Forms.MenuItem menuItem1;
        private System.Windows.Forms.MenuItem menuItem2;
        private System.Windows.Forms.MenuItem mnuHelp;
        private System.Windows.Forms.MenuItem mnuHelpContents;
        private System.Windows.Forms.MenuItem mnuActDeact;
        private System.Windows.Forms.TextBox txtMain;
        private System.Windows.Forms.Label lblTrialMessage;
        private System.Windows.Forms.Button btnExtendTrial;
    }
}

