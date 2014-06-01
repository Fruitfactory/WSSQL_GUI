using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using WSUI.CA.Core;

namespace WSUI.CA.ClosePromt
{
    public class PromptCloseApplication :CoreSetupApplication
    {
        
        private readonly string _processName;
        private readonly string _displayName;
        private System.Threading.Timer _timer;
        private Form _form;

        
        public PromptCloseApplication(string productName, string processName, string displayName)
            :base(productName)
        {
            _processName = processName;
            _displayName = displayName;
        }

        public bool Prompt()
        {
            if (IsRunning(_processName))
            {
                _form = new ClosePromptForm(String.Format("Please close running instances of {0} before running {1} setup.", _displayName, ProductName));
                _timer = new System.Threading.Timer(TimerElapsed, _form, 200, 200);
                return ShowDialog();
            }
            return true;
        }

        bool ShowDialog()
        {
            if (_form.ShowDialog(new WindowWrapper(GetMainWindowHandle())) == DialogResult.OK)
                return !IsRunning(_processName) || ShowDialog();
            return false;
        }

        private void TimerElapsed(object sender)
        {
            if (_form == null || IsRunning(_processName) || !_form.Visible)
                return;
            _form.DialogResult = DialogResult.OK;
            _form.Close();
        }

        static bool IsRunning(string processName)
        {
            return Process.GetProcesses().Any(p => p.ProcessName.ToUpper().StartsWith(processName.ToUpper()));
        }

        public override void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
            if (_form != null && _form.Visible)
                _form.Close();
            base.Dispose();
        }
    }
}