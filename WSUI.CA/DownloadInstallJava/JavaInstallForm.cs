using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

namespace OF.CA.DownloadInstallJava
{
    public partial class JavaInstallForm : Form
    {
        private static string JAVA_RUNTIME_86 =
           "http://javadl.sun.com/webapps/download/AutoDL?BundleId=111687";

        private static string JAVA_RUNTIME_64 = "http://javadl.sun.com/webapps/download/AutoDL?BundleId=111689";

        private Session _session;
        private WebClient _webClient;
        private List<string> _messages;

        public JavaInstallForm(Session session)
        {
            InitializeComponent();
            Application.EnableVisualStyles();
            this.TopMost = true;
            _messages = new List<string>();
            this._session = session;
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            //Debugger.Launch();
            //Debugger.Break();

            var url = Environment.Is64BitOperatingSystem ? JAVA_RUNTIME_64 : JAVA_RUNTIME_86;

            string path =
                Path.Combine(Path.GetTempPath(), "jre.exe");
            try
            {
                btnDownloadInstall.Enabled = false;
                buttonCancel.Enabled = false;
                _webClient = new WebClient();
                _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
                _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
                _webClient.DownloadFileAsync(new Uri(url), path, path);
                labelStatusText.Text = "Downloading JRE...";
                Log("Downloading JRE...");
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs downloadProgressChangedEventArgs)
        {
            progressBar.Value = downloadProgressChangedEventArgs.ProgressPercentage / 2;
            Application.DoEvents();
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs asyncCompletedEventArgs)
        {
            try
            {
                _webClient.DownloadProgressChanged -= WebClientOnDownloadProgressChanged;
                _webClient.DownloadFileCompleted -= WebClientOnDownloadFileCompleted;
                var path = asyncCompletedEventArgs.UserState as string;
                if (string.IsNullOrEmpty(path))
                {
                    Log("Java Runtime path is empty.");
                    return;
                }
                if (File.Exists(path))
                {
                    this.Invoke(new Action<string>(UpdateStatus), "Installing JRE...");
                    Log("Installing JRE...");
                    Application.DoEvents();

                    this.Invoke(new Action<int>(UpdateProgress), 100);
                    Application.DoEvents();
                    ProcessStartInfo startInfo = new ProcessStartInfo(path, " /s SPONSORS=0 /L C:\\install_jre.log ");
                    startInfo.Verb = "runas";
                    Process instalation = Process.Start(startInfo);
                    instalation.WaitForExit();
                    Thread.Sleep(2500);
                    this.Invoke(new Action<DialogResult>(CloseApplication), DialogResult.OK);
                }
                else
                {
                    this.Invoke(new Action<DialogResult>(CloseApplication), DialogResult.Cancel);
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                this.Invoke(new Action<DialogResult>(CloseApplication), DialogResult.Cancel);
            }
        }

        public List<String> GetMessages()
        {
            return _messages.ToList();
        } 

        private void Log(string message)
        {
            _messages.Add(message);
        }

        private void UpdateStatus(string message)
        {
            labelStatusText.Text = message;
        }

        private void UpdateProgress(int value)
        {
            progressBar.Value = value;
        }

        private void CloseApplication(DialogResult result)
        {
            DialogResult = result;
            Close();
        }
    }
}
