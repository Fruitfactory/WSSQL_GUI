using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security;
using System.Windows;
using System.Windows.Threading;
using OF.Downloader.Interfaces;

namespace OF.Downloader.ViewModel
{
    public class OFDownloaderViewModel : IMainViewModel, IDisposable, INotifyPropertyChanged
    {

        private static readonly string BASE_URL = "http://outlookfinder.com/downloads/clicktwice/full/1033/";
        private static string TempFolder = "OutlookFinder";
        private static string InstallerFileName = "OutlookFinderSetup.exe";

        private static readonly string LOG = "Application";
        private static readonly string SOURCE = "outlookfinder";

        private IMainView _view;
        private WebClient _webClient;
        private string _tempFileName;

        public OFDownloaderViewModel()
        {
        }


        private string _label;
        public string Label
        {
            get { return _label; }
            set
            {
                _label = value; 
                OnPropertyChanged("Label");
            }
        }

        private double _progress;
        public double Progress
        {
            get { return _progress; }
            set
            {
                _progress = value;
                OnPropertyChanged("Progress");
            }
        }


        public void Initialize()
        {
            string urlFile;
            _tempFileName = TempFile(out urlFile);
            if (string.IsNullOrEmpty(urlFile) || string.IsNullOrEmpty(_tempFileName))
            {
                return;
            }

            Dispatcher.CurrentDispatcher.BeginInvoke((Action) (() =>
            {
                _webClient = new WebClient();
                _webClient.DownloadFileCompleted += WebClientOnDownloadFileCompleted;
                _webClient.DownloadProgressChanged += WebClientOnDownloadProgressChanged;
                _webClient.DownloadFileAsync(new Uri(urlFile), _tempFileName);
            }));
            
            _view = new MainWindow();
            _view.Model = this;
        }

        private string TempFile(out string urlFile)
        {
            var tempFolder = Path.Combine(Path.GetTempPath(), TempFolder);
            if (Directory.Exists(tempFolder))
            {
                var dirInfo = new DirectoryInfo(tempFolder);
                dirInfo.Delete(true);
            }
            Directory.CreateDirectory(tempFolder);

            var version = GetCurrentVersion();

            if (string.IsNullOrEmpty(version))
            {
                urlFile = "";
                return "";
            }

            var installerFolder = Path.Combine(tempFolder, version);
            if (!Directory.Exists(installerFolder))
            {
                Directory.CreateDirectory(installerFolder);
            }
            else if (Directory.EnumerateFiles(installerFolder).Any())
            {
                foreach (var enumerateFile in Directory.EnumerateFiles(installerFolder))
                {
                    try
                    {
                        File.Delete(enumerateFile);
                    }
                    catch (Exception ex)
                    {
                        Log(ex.Message);
                    }
                }
            }

            var tempFile = Path.Combine(installerFolder, InstallerFileName);
            urlFile = BASE_URL + version + "/" + InstallerFileName;
            return tempFile;
        }


        public void Show()
        {
            if (_view == null)
            {
                return;
            }
            _view.Closed += ViewOnClosed;
            _view.ShowModal();         
              
        }

        private void ViewOnClosed(object sender, EventArgs eventArgs)
        {
            _view.Closed -= ViewOnClosed;
        }

        public IMainView View
        {
            get { return _view; }
        }

        public void Dispose()
        {
            _view.Closed -= ViewOnClosed;
            _view = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,new PropertyChangedEventArgs(property));
            }
        }

        private string GetCurrentVersion()
        {
            var currentAssembly = this.GetType().Assembly;
            return currentAssembly != null ? currentAssembly.GetName().Version.ToString() : "";
        }

        private void WebClientOnDownloadProgressChanged(object sender, DownloadProgressChangedEventArgs args)
        {
            Label = "Progress: " +  args.ProgressPercentage + "%";
            Progress = args.ProgressPercentage;
        }

        private void WebClientOnDownloadFileCompleted(object sender, AsyncCompletedEventArgs args)
        {
            _webClient.DownloadFileCompleted -= WebClientOnDownloadFileCompleted;
            _webClient.DownloadProgressChanged -= WebClientOnDownloadProgressChanged;
            if (args.Error != null)
            {
                Label = args.Error.Message;
                Log(args.Error.Message);
            }
            else if (!args.Cancelled && File.Exists(_tempFileName))
            {
                try
                {
                    Process.Start(_tempFileName);
                }
                catch (Exception ex)
                {
                    Log(ex.Message);
                }
                Application.Current.Shutdown(0);
            }
        }


        private static void Log(string message)
        {
            if (!EventLog.SourceExists(SOURCE))
            {
                EventLog.CreateEventSource(SOURCE,LOG);
            }
            EventLog.WriteEntry(SOURCE,message,EventLogEntryType.Error,0x01);
        }

    }
}