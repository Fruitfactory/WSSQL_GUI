using System;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using WSUI.Core.Core.LimeLM;

namespace WSUI.CA.EmailValidate
{
    public partial class EmailForm : Form
    {
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        public EmailForm()
        {
            InitializeComponent();
            button1.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (IsEmailPresentOnLimeServer())
            {
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                button1.Enabled = false;
                textBoxEmail.Text = string.Empty;
                MessageBox.Show("Try another email.");
            }
        }

        public string GetEmail()
        {
            return textBoxEmail.Text;
        }

        private bool IsEmailPresentOnLimeServer()
        {
            bool isPresent = LimeLMApi.IsEmailPresent(textBoxEmail.Text);
            return isPresent;
        }

        private void textBoxEmail_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = Regex.IsMatch(textBoxEmail.Text, EmailPattern, RegexOptions.IgnoreCase);
        }
    }
}