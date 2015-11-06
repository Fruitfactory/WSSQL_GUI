using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;
using OF.CA.Core;

namespace OF.CA.DownloadInstallJava
{
    internal class JavaInstallApplication : CoreSetupApplication
    {
        private JavaInstallForm _form;
        private Session _session;


        public JavaInstallApplication(string productName, Session session) : base(productName)
        {
            _session = session;
        }

        public bool DownloadAndInstallJava()
        {
            _form = new JavaInstallForm(_session);
            return ShowDialog();
        }

        public IEnumerable<string> GetMessages()
        {
            return _form != null ? _form.GetMessages() : new List<string>();
        }

        private bool ShowDialog()
        {
            return _form.ShowDialog(new WindowWrapper(GetMainWindowHandle())) == DialogResult.OK;
        }


    }
}