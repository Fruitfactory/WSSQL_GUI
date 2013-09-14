using System;
using System.Diagnostics;
using System.Windows;

namespace WSUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //[STAThread]
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            AppDomain.CurrentDomain.UnhandledException +=
                (sender, args) => Debug.WriteLine("Unhandled exception: " + args.ExceptionObject);
            Application.Current.DispatcherUnhandledException += (sender, args) => Debug.WriteLine("Unhandled exception: " + args.Exception);

            var boots = new WSBootStraper();
            boots.Run();
        }

    }
}
