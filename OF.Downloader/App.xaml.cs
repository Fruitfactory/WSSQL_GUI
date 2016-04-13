using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using OF.Downloader.Bootstraper;
using OF.Downloader.Interfaces;

namespace OF.Downloader
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IBootstraper _bootstraper = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            _bootstraper = new OFBootstraperDownloader();
            _bootstraper.Run();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _bootstraper.Exit();
        }
    }
}
