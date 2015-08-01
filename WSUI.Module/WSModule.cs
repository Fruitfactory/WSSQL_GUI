using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using OF.Core.ElasticSearch.Clients;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Utils;
using OF.Infrastructure;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.View;
using OF.Module.View.Windows;
using OF.Module.ViewModel;
using Action = System.Action;

namespace OF.Module
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
            OFLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "Initialize <mmv.Init()>", watch.ElapsedMilliseconds));
            watch = new Stopwatch();
            watch.Start();
            IRegion sidebarRegion = _regionManager.Regions[RegionNames.SidebarStrategyRegion];
            if (sidebarRegion != null)
            {
                sidebarRegion.Add(mmv.KindsView);
                sidebarRegion = null;
            }
            Task.Factory.StartNew(new Action(() => FieldCash.Instance.Initialize()));
            watch.Stop();
            OFLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "Initialize <PreviewView.Init()>", watch.ElapsedMilliseconds));
        }

        private void RegistreInterfaces()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //TODO add interfaces and classes
            _unityContainer.RegisterType<IMainViewModel, MainViewModel>(new ContainerControlledLifetimeManager());

            _unityContainer.RegisterType<IKindsView, KindsView>();
            //_unityContainer.RegisterType<IPreviewView, PreviewView>(new ContainerControlledLifetimeManager());
            _unityContainer.RegisterInstance(typeof (IPreviewView), new PreviewView(),
                new ContainerControlledLifetimeManager());

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

            _unityContainer.RegisterType<INavigationService, Service.NavigationService>();
            _unityContainer.RegisterType<IContactDetailsView, ContactDetailsView>();
            _unityContainer.RegisterType<IContactDetailsViewModel, ContactDetailsViewModel>();

            _unityContainer.RegisterType<ISettingsView<AdvancedSearchViewModel>, AdvancedSearchSettingsView>();
            _unityContainer.RegisterType<IDataView<AdvancedSearchViewModel>, AdvancedSearchDataView>();

            _unityContainer.RegisterType<IElasticSearchInitializationIndex, OFElasticInitializingClient>();
            _unityContainer.RegisterType<IElasticTrackingClient, OFElasticTrackingClient>();
            _unityContainer.RegisterType<IElasticSearchClient, OFElasticSearchClient>();
            _unityContainer.RegisterType<IElasticSearchRiverStatus, OFElasticRiverStatus>();           
           

            _unityContainer.RegisterType<IElasticSearchView, ElasticSearchView>();
            _unityContainer.RegisterType<IElasticSearchViewModel, ElasticSearchViewModel>();

            _unityContainer.RegisterType<IUserActivityTracker, OFUserActivityTracker>();

            _unityContainer.RegisterType<IElasticSearchRiverSettingsWindow, ElasticSearchRiverSettingsWindow>();
            _unityContainer.RegisterType<IElasticSearchRiverSettingsView, ElasticSearchRiverSettingsView>();
            _unityContainer.RegisterType<IElasticSearchRiverSettingsViewModel, ElasticSearchRiverSettingsViewModel>();
            _unityContainer.RegisterType<IElasticUpdateSettingsClient, OFElasticUpdateSettingsClient>();

            _unityContainer.RegisterType<IElasticSearchMonitoringView, ElasticSearchMonitoringView>();
            _unityContainer.RegisterType<IElasticSearchMonitoringViewModel, ElasticSearchMonitoringViewModel>();
            

            watch.Stop();
            OFLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "RegistreInterfaces", watch.ElapsedMilliseconds));
        }
    }
}