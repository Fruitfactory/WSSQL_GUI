using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Practices.Prism.Logging;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using OF.Core.Enums;
using Transitionals.Controls;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.Service;

namespace OF.Control
{
    public class PluginBootStraper : UnityBootstrapper, IPluginBootStraper
    {
        private IMainViewModel _mainViewModel;

        public PluginBootStraper()
        {
        }

        public override void Run(bool runWithDefaultConfiguration)
        {
            this.Logger = this.CreateLogger();
            this.ModuleCatalog = this.CreateModuleCatalog();
            if (this.ModuleCatalog == null)
                throw new NullReferenceException("ModuleCatalog");
            this.ConfigureModuleCatalog();
            this.Container = this.CreateContainer();
            if (Container == null)
                throw new NullReferenceException("Container");
            this.ConfigureContainer();
            this.ConfigureServiceLocator();
            this.ConfigureRegionAdapterMappings();
            this.ConfigureDefaultRegionBehaviors();
            this.RegisterFrameworkExceptionTypes();
            this.Shell = this.CreateShell();
            if (this.Shell != null)
            {
                RegionManager.SetRegionManager(this.Shell, UnityContainerExtensions.Resolve<IRegionManager>(this.Container, new ResolverOverride[0]));
                RegionManager.UpdateRegions();
                this.InitializeShell();
            }
            if (UnityContainerExtensions.IsRegistered<IModuleManager>(this.Container))
            {
                this.InitializeModules();
            }
        }

        protected override Microsoft.Practices.Prism.Logging.ILoggerFacade CreateLogger()
        {
            return (ILoggerFacade)OFLogger.Instance;
        }

        protected override DependencyObject CreateShell()
        {
            var shell = Container.Resolve<WSSidebarControl>();
            return shell;
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof(OF.Module.OFModule));
            return catalog;
        }

        protected override void InitializeModules()
        {
            IModule module = Container.Resolve<OF.Module.OFModule>();
            module.Initialize();
            _mainViewModel = Container.Resolve<OF.Module.ViewModel.MainViewModel>();
            (this.View as ISidebarView).Model = _mainViewModel;
        }

        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            RegionAdapterMappings mappings = base.ConfigureRegionAdapterMappings();
            mappings.RegisterMapping(typeof(TransitionElement),
                Container.Resolve<OFTransitionElementAdaptor>());
            return mappings;
        }

        public void PassAction(IWSAction action)
        {
            if (ReferenceEquals(_mainViewModel, null))
                return;
            _mainViewModel.PassAction(action);
        }

        public DependencyObject View
        {
            get { return this.Shell; }
        }

        public bool IsPluginBusy
        {
            get { return _mainViewModel.IsBusy; }
        }

        public bool IsMainUiActive { get { return (this.Shell as UserControl).IsFocused; } }

        public bool IsMenuEnabled { get { return _mainViewModel.IsMenuEnabled; } }
    }
}