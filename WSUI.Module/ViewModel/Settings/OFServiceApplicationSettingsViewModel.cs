using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Input;
using System.Windows.Interactivity;
using Microsoft.Practices.Unity;
using OF.Core.Core.MVVM;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Utils.Dialog;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;

namespace OF.Module.ViewModel.Settings
{
    public class OFServiceApplicationSettingsViewModel : OFViewModelBase, IServiceApplicationSettingsViewModel
    {

        private readonly string SERVICE_APP = "serviceapp";

        private IUnityContainer _container;
        
        public OFServiceApplicationSettingsViewModel(IUnityContainer container, IServiceApplicationSettingsView view)
        {
            _container = container;
            ((IServiceApplicationSettingsView) view).Model = this;
            View = view;
        }

        public void ApplySettings()
        {
            
        }

        public object View { get; private set; }

        public void Initialize()
        {
            IsServiceAppRunning = CheckServiceAppIsRunning();
            IsServiceAppAutoStartExist = OFRegistryHelper.Instance.IsServiceApplicationAutoRunExist();

            ServiceAppRunCommand = new OFRelayCommand(ServiceAppAppRunCommandExecute, o => !IsServiceAppRunning);
            ServiceAppAutoStartCommand = new OFRelayCommand(ServiceAppAutoStartCommandExecute, o => !IsServiceAppAutoStartExist);
        }

        

        public bool HasDetailsChanges { get; private set; }

        public ICommand ServiceAppAutoStartCommand { get; private set; }

        public ICommand ServiceAppRunCommand { get; private set; }

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


    }
}