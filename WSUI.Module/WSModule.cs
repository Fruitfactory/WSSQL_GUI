using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Microsoft.Practices.ObjectBuilder2;
using WSUI.Infrastructure;
using WSUI.Module.Interface;
using WSUI.Module.View;
using WSUI.Module.ViewModel;

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
            //TODO add view to regions
            var mmv = _unityContainer.Resolve<MainViewModel>();
            mmv.Init();
            IRegion region = _regionManager.Regions[RegionNames.StrategyRegion];
            region.Add(mmv.KindsView);
            region = _regionManager.Regions[RegionNames.PreviewRegion];
            region.Add(mmv.PreviewView);
        }

        private void RegistreInterfaces()
        {
            //TODO add interfaces and classes
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
        }

    }
}
