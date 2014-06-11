﻿using System;
using System.Windows.Forms;
using WSUI.CA.Enums;

namespace WSUI.CA.ClosePromt
{
    public partial class ClosePromptForm : Form,IClosePromptForm
    {
        public ClosePromptForm(string text)
        {
            InitializeComponent();
            messageText.Text = text;
        }

        public eClosePrompt Result { get; set; }

        private void OkButtonClick(object sender, EventArgs e)
        {
            Result = eClosePrompt.Continue;
            Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Result = eClosePrompt.Cancel;
            Close();
        }

        private void ClosePromptForm_Load(object sender, EventArgs e)
        {
            this.AcceptButton = okButton;
        }
    }
}
