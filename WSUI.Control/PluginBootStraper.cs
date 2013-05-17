using System.Windows;
using C4F.DevKit.PreviewHandler.Service;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Prism.UnityExtensions;
using Microsoft.Practices.Unity;
using WSUI.Module.Interface;
using System.Windows.Forms.Integration;

namespace WSUI.Control
{
    public class PluginBootStraper : UnityBootstrapper, IPluginBootStraper
    {
        private ElementHost _elementHost = null;
        private IMainViewModel _mainViewModel;
        
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
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof(WSUI.Module.WSModule));
            return base.CreateModuleCatalog();
        }

        protected override void InitializeModules()
        {
           
            //base.InitializeModules();
            IModule module = Container.Resolve<WSUI.Module.WSModule>();
            module.Initialize();
            _mainViewModel mvv = Container.Resolve<WSUI.Module.ViewModel.MainViewModel>();
            (this.Shell as IMainView).Model = _mainViewModel;
        }

        public void PassAction(WSActionType actionType)
        {
            if(ReferenceEquals(_mainViewModel,null))
                return;
            _mainViewModel.PassActionForPreview(actionType);
        }
    }
}