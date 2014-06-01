using System.Net.Configuration;
using System.Windows.Forms;
using WSUI.CA.Core;

namespace WSUI.CA.EmailValidate
{
    public class EmailValidateApplication : CoreSetupApplication
    {
        private EmailForm _form;

        public EmailValidateApplication(string productName) 
            : base(productName)
        {
        }

        public bool PromtEmail()
        {
            _form = new EmailForm();
            return ShowDialog();
        }

        private bool ShowDialog()
        {
            return _form.ShowDialog(new WindowWrapper(GetMainWindowHandle())) == DialogResult.OK;
        }

        public string GetEmail()
        {
            return _form.GetEmail();
        }
    }
}