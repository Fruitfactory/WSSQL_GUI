using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Core.Utils.Dialog;
using OF.Module.Events;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.ViewModel.Settings
{
    public class OFServiceApplicationSettingsViewModel : OFCoreSettingsViewModel, IServiceApplicationSettingsViewModel
    {
        private readonly string ELASTICSEARCH_SERVICE = "elasticsearch";
        private readonly string SERVICE_APP = "serviceapp";

        private IUnityContainer _container;
        private IEventAggregator _eventAggregator;
        
        public OFServiceApplicationSettingsViewModel(IEventAggregator EventAggregator, IUnityContainer container, IServiceApplicationSettingsView view)
        {
            _container = container;
            _eventAggregator = EventAggregator;
            ((IServiceApplicationSettingsView) view).Model = this;
            View = view;
        }

        public override void Initialize()
        {
            IsServiceAppRunning = CheckServiceAppIsRunning();
            IsServiceAppAutoStartExist = OFRegistryHelper.Instance.IsServiceApplicationAutoRunExist();
            CheckElasticSearchServiceStatus();

            ServiceAppRunCommand = new OFRelayCommand(ServiceAppAppRunCommandExecute, o => !IsServiceAppRunning);
            ServiceAppAutoStartCommand = new OFRelayCommand(ServiceAppAutoStartCommandExecute, o => !IsServiceAppAutoStartExist);

            ElasticSearchServiceInstallCommand = new OFRelayCommand(ElasticSearchServiceInstallCommandExecute, o => !IsElasticSearchServiceInstalled);
            ElasticSearchServiceRunCommand = new OFRelayCommand(ElasticSearchServiceRunCommandExecute,o => !IsElasticSearchServiceRunning);
            WarmingVisibility = Visibility.Collapsed;
            WarmSecond = 10;
        }

        public ICommand ServiceAppAutoStartCommand { get; private set; }

        public ICommand ServiceAppRunCommand { get; private set; }

        public ICommand ElasticSearchServiceInstallCommand { get; private set; }

        public ICommand ElasticSearchServiceRunCommand { get; private set; }

        public bool IsServiceAppRunning
        {
            get { return Get(() => IsServiceAppRunning); }
            set { Set(() => IsServiceAppRunning, value); }
        }

        public bool IsServiceAppAutoStartExist
        {
            get { return Get(() => IsServiceAppAutoStartExist); }
            set { Set(() => IsServiceAppAutoStartExist, value); }
        }

        public bool IsElasticSearchServiceInstalled
        {
            get { return Get(() => IsElasticSearchServiceInstalled); }
            set { Set(() => IsElasticSearchServiceInstalled, value); }
        }

        public bool IsElasticSearchServiceRunning
        {
            get { return Get(() => IsElasticSearchServiceRunning); }
            set { Set(() => IsElasticSearchServiceRunning, value); }
        }

        public Visibility WarmingVisibility
        {
            get { return Get(() => WarmingVisibility); }
            set { Set(() => WarmingVisibility, value); }
        }

        public int WarmSecond
        {
            get { return Get(() => WarmSecond); }
            set { Set(() => WarmSecond, value); }
        }

        private bool CheckServiceAppIsRunning()
        {
            var process = Process.GetProcessesByName(SERVICE_APP);
            return process.Length > 0;
        }

        private void ServiceAppAutoStartCommandExecute(object o)
        {
            var ofPath = OFRegistryHelper.Instance.GetOfPath();
            OFRegistryHelper.Instance.SetAutoRunHelperApplication(ofPath);
            IsServiceAppAutoStartExist = OFRegistryHelper.Instance.IsServiceApplicationAutoRunExist();
        }

        private void ServiceAppAppRunCommandExecute(object o)
        {
            var ofPath = OFRegistryHelper.Instance.GetOfPath();
            var filename = Path.Combine(ofPath, "serviceapp.exe");
            if (File.Exists(filename))
            {
                Process.Start(filename);
                Thread.Sleep(1500);
                IsServiceAppRunning = CheckServiceAppIsRunning();
            }
            else
            {
                OFMessageBoxService.Instance.Show("Service Application", "Service Application doesn't exist.");
            }
        }

        private void CheckElasticSearchServiceStatus()
        {
            ServiceController sct =
                ServiceController.GetServices()
                    .FirstOrDefault(
                        s => s.ServiceName.IndexOf(ELASTICSEARCH_SERVICE, StringComparison.InvariantCultureIgnoreCase) > -1);
            if (sct == null)
            {
                IsElasticSearchServiceInstalled = false;
            }
            else
            {
                IsElasticSearchServiceInstalled = true;
                IsElasticSearchServiceRunning = sct.Status == ServiceControllerStatus.Running;
            }
        }


        private void ElasticSearchServiceInstallCommandExecute(object o)
        {
            ExecuteCommandForService("install");
            Thread.Sleep(1500);
            CheckElasticSearchServiceStatus();
        }

        private void ElasticSearchServiceRunCommandExecute(object o)
        {
            ExecuteCommandForService("start");
            WarmingVisibility = Visibility.Visible;

            Dispatcher disp = Dispatcher.CurrentDispatcher;


            var backGround = new BackgroundWorker();
            backGround.DoWork += (sender, args) =>
            {
                for (int i = WarmSecond - 1; i >= 0; i--)
                {
                    WarmSecond = i + 1;
                    Thread.Sleep(1000);
                }
                disp.BeginInvoke((Action)(() =>
                {
                    CheckElasticSearchServiceStatus();
                    _eventAggregator.GetEvent<OFElasticsearchServiceStartedEvent>().Publish(IsElasticSearchServiceRunning);
                }));
            };
            backGround.RunWorkerAsync();

        }


        private void ExecuteCommandForService(string command)
        {
            string path = OFRegistryHelper.Instance.GetElasticSearchpath();
            string javaHome = OFRegistryHelper.Instance.GetJavaInstallationPath();
            if (path.IsEmpty() || command.IsEmpty() || javaHome.IsEmpty())
            {
                OFLogger.Instance.LogDebug("Path or Command or JavaHome was empty");
                return;
            }
            const string serviceBat = "service.bat";
            try
            {
                Process p = new Process();
                p.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                p.StartInfo.FileName = string.Format("{0}\\bin\\{1}", path, serviceBat);
                p.StartInfo.Arguments = string.Format(" {0} \"{1}\"", command, javaHome);
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Verb = "runas";
                p.StartInfo.WorkingDirectory = string.Format("{0}{1}", path, "\\bin");
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;

                p.Start();
                p.WaitForExit();

                Debug.WriteLine(string.Format("!!!!!!!! {0}", p.StandardOutput != null ?  p.StandardOutput.ReadToEnd() : ""));
                Debug.WriteLine(string.Format("!!!!!!!! {0}", p.StandardError != null ? p.StandardError.ReadToEnd() : ""));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }
    }
}