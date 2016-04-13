using System;
using OF.Downloader.Interfaces;
using OF.Downloader.ViewModel;

namespace OF.Downloader.Bootstraper
{
    internal class OFBootstraperDownloader :IBootstraper
    {
        private IMainViewModel _model;
        
        internal OFBootstraperDownloader()
        {
            _model = new OFDownloaderViewModel();
        }

        public void Run()
        {
            _model.Initialize();
            _model.Show();    
        }


        public void Exit()
        {
            _model.Dispose();
        }
        
    }
}