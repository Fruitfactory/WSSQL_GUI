using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using Timer = System.Timers.Timer;

namespace OF.Module.ViewModel
{
    public class ElasticSearchMonitoringViewModel : ViewModelBase, IElasticSearchMonitoringViewModel
    {

        private static string UPDATING = "Updating...";
        private static string READY = "Ready.";

        private IElasticSearchRiverStatus _riverStatusClient;
        private IEventAggregator _eventAggregator;
        private IRegionManager _regionManager;

        private Timer _timer;

        public ElasticSearchMonitoringViewModel(IElasticSearchRiverStatus riverStatusClient, IEventAggregator eventAggregator, IRegionManager regionManager, IElasticSearchMonitoringView view)
        {
            _riverStatusClient = riverStatusClient;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            View = view;
            view.Model = this;
        }

        #region [properties]

        public OFRiverStatus Status
        {
            get { return Get(() => Status); }
            set { Set(() => Status, value); }
        }

        public string StatusText
        {
            get { return Get(() => StatusText); }
            private set { Set(() => StatusText, value); }
        }

        public DateTime LastUpdated
        {
            get { return Get(() => LastUpdated); }
            set { Set(() => LastUpdated, value); }
        }

        public int EmailCount
        {
            get { return Get(() => EmailCount); }
            set { Set(() => EmailCount, value); }
        }

        public int AttachmentCount
        {
            get { return Get(() => AttachmentCount); }
            set { Set(() => AttachmentCount, value); }
        }


        #endregion

        #region [methods]

        public object View { get; private set; }

        public void Start()
        {
            if (_timer.IsNull())
            {
                _timer = new Timer(2000);
                _timer.Elapsed += TimerOnElapsed;    
            }

            Application.Current.Dispatcher.BeginInvoke(new Action(ShowUi));

            _timer.Enabled = true;
        }

        private void ShowUi()
        {
            if (_regionManager.IsNull())
            {
                return;
            }
            IRegion region = _regionManager.Regions[RegionNames.ElasticSearchMonitoring];

            if (region.IsNull())
            {
                return;
            }
            if (!region.Views.Contains(View))
            {
                region.Add(View);
            }
            region.Activate(View);
        }

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            var response = _riverStatusClient.GetRiverStatus();
            if (response.IsNull() || !response.Success)
            {
                return;
            }
            var result = response.Response;
            if (result.IsNull() || !result.Success)
            {
                return;
            }

            Status = result.Status;
            StatusText = Status == OFRiverStatus.Busy || Status == OFRiverStatus.InitialIndexing ? UPDATING : READY;
            LastUpdated = result.Lastupdated;
            EmailCount = result.Emailcount;
            AttachmentCount = result.Attachmentcount;
        }

        public void Stop()
        {
            if (_timer.IsNotNull())
            {
                _timer.Enabled = false;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if(!Disposed)
            {
                if (_timer.IsNotNull())
                {
                    _timer.Elapsed -= TimerOnElapsed;
                    _timer = null;
                }
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}