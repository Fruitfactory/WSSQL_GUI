using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using WSUI.Module;
using WSUI.Module.Interface;

namespace WSUI
{
    public class WSBootStraper : UnityBootstrapper
    {
        protected override DependencyObject CreateShell()
        {
            MainWindow shell = Container.Resolve<MainWindow>();
            return shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            App.Current.MainWindow = (Window) this.Shell;
            App.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof (WSUI.Module.WSModule));
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            base.ConfigureModuleCatalog();
            var catalog = (ModuleCatalog) this.ModuleCatalog;
        }

        protected override void InitializeModules()
        {
            //base.InitializeModules();
            IModule module = Container.Resolve<WSUI.Module.WSModule>();
            module.Initialize();
            var mvv = Container.Resolve<WSUI.Module.ViewModel.MainViewModel>();
            (this.Shell as IMainView).Model = mvv;
        }

    }
}
