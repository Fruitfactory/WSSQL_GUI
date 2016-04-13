using System;
using System.Windows;
using OF.Downloader.Interfaces;

namespace OF.Downloader.ViewModel
{
    public class OFDownloaderViewModel : IMainViewModel, IDisposable
    {

        private static readonly string BASE_URL = "http://outlookfinder.com/downloads/clicktwice/full/1033/{0}";

        private IMainView _view;

        public OFDownloaderViewModel()
        {
        }

        public void Initialize()
        {
            _view = new MainWindow();
            _view.Model = this;
        }


        public void Show()
        {
            _view.Closed += ViewOnClosed;
            _view.ShowModal();         
              
        }

        private void ViewOnClosed(object sender, EventArgs eventArgs)
        {
            _view.Closed -= ViewOnClosed;
        }

        public IMainView View
        {
            get { return _view; }
        }

        public void Dispose()
        {
            _view.Closed -= ViewOnClosed;
            _view = null;
        }
    }
}