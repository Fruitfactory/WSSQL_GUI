using System.Diagnostics;
using System.Windows;
using C4F.DevKit.PreviewHandler.Service;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Logging;
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
        private Stopwatch _watch;
        
        public PluginBootStraper(ElementHost host)
        {
            _elementHost = host;
            _watch = new Stopwatch();
        }

        protected override Microsoft.Practices.Prism.Logging.ILoggerFacade CreateLogger()
        {
            return (ILoggerFacade) WSSqlLogger.Instance;
        }

        protected override DependencyObject CreateShell()
        {
            _watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("CreateModuleCatalog - CreateShell: {0}ms",_watch.ElapsedMilliseconds));

            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var shell = Container.Resolve<WSMainControl>();
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("Create shell (plugin): {0}ms",watch.ElapsedMilliseconds));
            (_watch = new Stopwatch()).Start();
            return shell;
        }

        protected override void InitializeShell()
        {
            _watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("CreateShell - (find and create regions) - InitializeShell: {0}ms", _watch.ElapsedMilliseconds));


            Stopwatch watch = new Stopwatch();
            watch.Start();
            base.InitializeShell();
            _elementHost.Child = (UIElement)this.Shell;
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("InitializeShell (plugin): {0}ms",watch.ElapsedMilliseconds));
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof(WSUI.Module.WSModule));
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("CreateModuleCatalog (plugin): {0}ms",watch.ElapsedMilliseconds));


            _watch.Start();
            return base.CreateModuleCatalog();
        }

        protected override void InitializeModules()
        {
            //base.InitializeModules();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            IModule module = Container.Resolve<WSUI.Module.WSModule>();
            module.Initialize();
            _mainViewModel = Container.Resolve<WSUI.Module.ViewModel.MainViewModel>();
            (this.Shell as IMainView).Model = _mainViewModel;
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("InitializeModules (plugin): {0}ms",watch.ElapsedMilliseconds));
        }

        public void PassAction(WSActionType actionType)
        {
            if(ReferenceEquals(_mainViewModel,null))
                return;
            _mainViewModel.PassActionForPreview(actionType);
        }
    }
}