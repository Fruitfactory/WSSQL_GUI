﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Outlook = Microsoft.Office.Interop.Outlook;
using OF.CA.Core;
using OF.CA.Enums;
using OF.Core.Helpers;

namespace OF.CA.ClosePromt
{
    public class PromptCloseApplication : CoreSetupApplication
    {

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool TerminateProcess(IntPtr hProcess, uint uExitCode);

        private readonly string _processName;
        private readonly string _displayName;
        private System.Threading.Timer _timer;
        private Form _form;

        public PromptCloseApplication(string productName, string processName, string displayName)
            : base(productName)
        {
            _processName = processName;
            _displayName = displayName;
        }

        public bool Prompt()
        {
            if (IsRunning(_processName))
            {
                _form = new ClosePromptForm(String.Format("Please close running instances of {0} before running {1} setup.", _displayName, ProductName));//OutlookFinder needs to close Outlook to proceed. Please save your work, close the program and click Close Outlook and Continue.
                var res = ShowDialog();
                switch (res)
                {
                    case eClosePrompt.Continue:
                        CloseAllOutlookInstancesHard();
                        OFRegistryHelper.Instance.SetFlagClosedOutlookApplication();
                        return true;
                    case eClosePrompt.Cancel:
                        return false;
                }
            }
            return true;
        }

        eClosePrompt ShowDialog()
        {
            var r = _form.ShowDialog(new WindowWrapper(GetMainWindowHandle()));
            return (_form as IClosePromptForm).Result;
        }

        static bool IsRunning(string processName)
        {
            return Process.GetProcesses().Any(p => p.ProcessName.ToUpper() == processName.ToUpper());
        }

        public override void Dispose()
        {
            if (_timer != null)
                _timer.Dispose();
            if (_form != null && _form.Visible)
                _form.Close();
            base.Dispose();
        }

        private static void CloseAllOutlookInstancesHard()
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = Path.Combine(Environment.SystemDirectory, "taskkill.exe");
                info.Arguments = string.Format(" /F /IM OUTLOOK.EXE");
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = new Process() { StartInfo = info };
                p.Start();
                p.WaitForExit();    
            }
            finally { }
            
        }

    }
}