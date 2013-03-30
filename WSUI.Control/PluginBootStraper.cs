using System.Windows;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using WSUI.Module.Interface;
using System.Windows.Forms.Integration;

namespace WSUI.Control
{
    public class PluginBootStraper : UnityBootstrapper
    {
        private ElementHost _elementHost = null;
        
        public PluginBootStraper(ElementHost host)
        {
            _elementHost = host;
        }

        protected override DependencyObject CreateShell()
        {
            var shell = Container.Resolve<WSMainControl>();
            return shell;
        }

        protected override void InitializeShell()
        {
            base.InitializeShell();
            _elementHost.Child = (UIElement)this.Shell;
            //Application.Current.RootVisual = this.Shell;
            //App.Current.MainWindow = (Window)this.Shell;
            //App.Current.MainWindow.Show();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof(WSUI.Module.WSModule));
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            var catalog = (ModuleCatalog)this.ModuleCatalog;
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