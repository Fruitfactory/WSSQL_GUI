using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Logging;
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
            Stopwatch watch =  new Stopwatch();
            watch.Start();
            MainWindow shell = Container.Resolve<MainWindow>();
            watch.Stop();
            Logger.Log(string.Format("Elapsed (CreateShell): {0}", watch.ElapsedMilliseconds),Category.Info,Priority.High);
            return shell;
        }

        protected override void InitializeShell()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            base.InitializeShell();
            App.Current.MainWindow = (Window) this.Shell;
            App.Current.MainWindow.Show();
            watch.Stop();
            Logger.Log(string.Format("Elapsed (InitializeShell): {0}", watch.ElapsedMilliseconds), Category.Info, Priority.High);
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            var catalog = new ModuleCatalog();
            catalog.AddModule(typeof (WSUI.Module.WSModule));
            watch.Stop();
            Logger.Log(string.Format("Elapsed (CreateModuleCatalog): {0}", watch.ElapsedMilliseconds), Category.Info, Priority.High);
            return base.CreateModuleCatalog();
        }

        protected override void ConfigureModuleCatalog()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            base.ConfigureModuleCatalog();
            var catalog = (ModuleCatalog) this.ModuleCatalog;
            watch.Stop();
            Logger.Log(string.Format("Elapsed (ConfigureModuleCatalog): {0}", watch.ElapsedMilliseconds), Category.Info, Priority.High);
        }

        protected override void InitializeModules()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            //base.InitializeModules();
            IModule module = Container.Resolve<WSUI.Module.WSModule>();
            module.Initialize();
            var mvv = Container.Resolve<WSUI.Module.ViewModel.MainViewModel>();
            (this.Shell as IMainView).Model = mvv;
            watch.Stop();
            Logger.Log(string.Format("Elapsed (InitializeModules): {0}", watch.ElapsedMilliseconds), Category.Info, Priority.High);
        }

    }
}
