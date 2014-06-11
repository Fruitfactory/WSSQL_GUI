using System;
using System.Diagnostics;
using System.Windows.Threading;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using WSUI.Core.Logger;
using WSUI.Core.Utils;
using WSUI.Infrastructure;
using WSUI.Module.Interface;
using WSUI.Module.View;
using WSUI.Module.ViewModel;
using Action = System.Action;

namespace WSUI.Module
{
    public class WSModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _unityContainer;

        public WSModule(IUnityContainer container, IRegionManager region)
        {
            _regionManager = region;
            _unityContainer = container;
        }

        public void Initialize()
        {
            RegistreInterfaces();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //TODO add view to regions
            var mmv = _unityContainer.Resolve<MainViewModel>();
            mmv.Init();
            watch.Stop();
            WSSqlLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "Initialize <mmv.Init()>", watch.ElapsedMilliseconds));
            watch = new Stopwatch();
            watch.Start();
            IRegion region = _regionManager.Regions[RegionNames.StrategyRegion];
            region.Add(mmv.KindsView);
            region = _regionManager.Regions[RegionNames.PreviewRegion];
            region.Add(mmv.PreviewView);
            mmv.PreviewView.Init();
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => FieldCash.Instance.Initialize()));
            watch.Stop();
            WSSqlLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "Initialize <PreviewView.Init()>", watch.ElapsedMilliseconds));
        }

        private void RegistreInterfaces()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //TODO add interfaces and classes
            _unityContainer.RegisterType<MainViewModel>(new ContainerControlledLifetimeManager());

            _unityContainer.RegisterType<IKindsView, KindsView>();
            _unityContainer.RegisterType<IPreviewView, PreviewView>();
            //all files;
            _unityContainer.RegisterType<ISettingsView<AllFilesViewModel>, AllFilesSettingsView>();
            _unityContainer.RegisterType<IDataView<AllFilesViewModel>, AllFilesDataView>();
            //email;
            _unityContainer.RegisterType<ISettingsView<EmailViewModel>, EmailSettingsView>();
            _unityContainer.RegisterType<IDataView<EmailViewModel>, EmailDataView>();
            //contact;
            _unityContainer.RegisterType<ISettingsView<ContactViewModel>, ContactSettingsView>();
            _unityContainer.RegisterType<IDataView<ContactViewModel>, ContactDataView>();
            //attachment
            _unityContainer.RegisterType<ISettingsView<AttachmentViewModel>, AttachmentSettingsView>();
            _unityContainer.RegisterType<IDataView<AttachmentViewModel>, AttachmentDataView>();
            watch.Stop();
            WSSqlLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "RegistreInterfaces", watch.ElapsedMilliseconds));
        }
    }
}