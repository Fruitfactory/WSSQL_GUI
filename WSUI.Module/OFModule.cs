using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using OF.Core.Core.LimeLM;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Utils;
using OF.Infrastructure;
using OF.Infrastructure.Implements.ElasticSearch.Clients;
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Service.Index;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.View;
using OF.Module.View.Settings;
using OF.Module.View.Windows;
using OF.Module.ViewModel;
using OF.Module.ViewModel.LogFileManager;
using OF.Module.ViewModel.Settings;
using OF.Module.ViewModel.Suggest;
using Action = System.Action;

namespace OF.Module
{
    public class OFModule : IModule
    {
        private readonly IRegionManager _regionManager;
        private readonly IUnityContainer _unityContainer;

        public OFModule(IUnityContainer container, IRegionManager region)
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
            Task.Factory.StartNew(new Action(() => OFFieldCash.Instance.Initialize()));
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

            _unityContainer.RegisterType<INavigationService, Service.OFNavigationService>();
            _unityContainer.RegisterType<IContactDetailsView, ContactDetailsView>();
            _unityContainer.RegisterType<IContactDetailsViewModel, ContactDetailsViewModel>();

            _unityContainer.RegisterType<ISettingsView<AdvancedSearchViewModel>, AdvancedSearchSettingsView>();
            _unityContainer.RegisterType<IDataView<AdvancedSearchViewModel>, AdvancedSearchDataView>();

            _unityContainer.RegisterType<IElasticSearchInitializationIndex, OFElasticInitializingClient>();
            
            _unityContainer.RegisterType<IElasticSearchClient, OFElasticSearchClient>();
            _unityContainer.RegisterType<IElasticSearchRiverStatus, OFElasticRiverStatus>();           
           

            _unityContainer.RegisterType<IElasticSearchView, ElasticSearchView>();
            _unityContainer.RegisterType<IElasticSearchViewModel, ElasticSearchViewModel>();

            _unityContainer.RegisterType<IUserActivityTracker, OFUserActivityTracker>();

            _unityContainer.RegisterType<IMainSettingsWindow, OFMainSettingsWindow>();
            _unityContainer.RegisterType<IElasticSearchRiverSettingsView, ElasticSearchRiverSettingsView>();
            _unityContainer.RegisterType<IElasticSearchRiverSettingsViewModel, OFElasticSearchRiverSettingsViewModel>();
            
            _unityContainer.RegisterType<IElasticSearchMonitoringView, ElasticSearchMonitoringView>();
            _unityContainer.RegisterType<IElasticSearchMonitoringViewModel, ElasticSearchMonitoringViewModel>();

            _unityContainer.RegisterType <IMainSettingsViewModel,OFMainSettingsViewModel>();
            _unityContainer.RegisterType<IMainSettingsView, OFMainSettingsView>();
            _unityContainer.RegisterType<ILoggingSettingsViewModel, OFLoggingSettingsViewModel>();
            _unityContainer.RegisterType<ILoggingSettingsView, OFLoggingSettingsView>();

            _unityContainer.RegisterType<IElasticSearchIndexOutlookItemsClient, OFElasticSeachIndexOutlookItemsClient>();

            _unityContainer.RegisterType<IServiceAppOFPluginStatusClient, OfServiceAppOfPluginStatusClient>();
            _unityContainer.RegisterType<IForceClient, OFForceClient>();

            _unityContainer.RegisterType<IServiceApplicationSettingsViewModel, OFServiceApplicationSettingsViewModel>();
            _unityContainer.RegisterType<IServiceApplicationSettingsView, OFServiceApplicationSettingsView>();

            _unityContainer.RegisterType<IOutlookSecuritySettingsViewModel, OFOutlookSecutirySettingsViewModel>();
            _unityContainer.RegisterType<IOutlookSecuritySettingsView, OFOutlookSecuritySettingsView>();

            _unityContainer.RegisterType<IOFElasticSearchRemovingClient, OFElasticSearchRemovingClient>();

            _unityContainer.RegisterType<IOFEmailSuggestWindow, OFEmailSuggestWindow>();
            _unityContainer.RegisterType<IOFEmailSuggestViewModel, OFEmailSuggestViewModel>();
            _unityContainer.RegisterType<IOFLogFilesSenderManager, OFLogFilesSenderManager>();

            _unityContainer.RegisterType<IOFTurboLimeActivate, TurboLimeActivate>();

            _unityContainer.RegisterType<IOFRiverMetaSettingsProvider, OFRiverMetaSettingsProvider>();
            

            _unityContainer.RegisterType<IOutlookAutoCompleteSettingsView, OFOutlookAutoCompleteSettingsView>();
            _unityContainer.RegisterType<IOutlookAutoCompleteSettingsViewModel, OFOutlookAutoCompleteSettingsViewModel>();
            
            watch.Stop();
            OFLogger.Instance.LogError(string.Format("Elapsed ({0}): {1}", "RegistreInterfaces", watch.ElapsedMilliseconds));
        }
    }
}