﻿using System;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using OF.Core.Core.ElasticSearch;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using Timer = System.Timers.Timer;

namespace OF.Module.ViewModel
{
    public class ElasticSearchMonitoringViewModel : OFViewModelBase, IElasticSearchMonitoringViewModel
    {

        private static string UPDATING = "Updating...";
        private static string READY = "Ready.";

        private IElasticSearchRiverStatus _riverStatusClient;
        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;
        private IAttachmentReader _attachmentReader;

        private readonly  object _lock = new object();

        private Timer _timer;

        public ElasticSearchMonitoringViewModel(IElasticSearchRiverStatus riverStatusClient, 
            IEventAggregator eventAggregator, 
            IRegionManager regionManager,
            IUnityContainer unityContainer,
            IElasticSearchMonitoringView view)
        {
            _riverStatusClient = riverStatusClient;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _unityContainer = unityContainer;
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

        public long EmailCount
        {
            get { return Get(() => EmailCount); }
            set { Set(() => EmailCount, value); }
        }

        public long AttachmentCount
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
            var emailCount = _riverStatusClient.GetTypeCount<OFEmail>();
            var attachmentCount = _riverStatusClient.GetTypeCount<OFAttachmentContent>();
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
            EmailCount = emailCount;
            AttachmentCount = attachmentCount;
            AttachmentMonitoring();
        }

        private void AttachmentMonitoring()
        {
            lock (_lock)
            {
                if (Status == OFRiverStatus.Busy && _attachmentReader.IsNull())
                {
                    _attachmentReader = _unityContainer.Resolve<IAttachmentReader>();
                    _attachmentReader.Start(LastUpdated);
                }
                else if (Status == OFRiverStatus.StandBy && _attachmentReader.IsNotNull())
                {
                    _attachmentReader.Stop();
                    _attachmentReader = null;
                }    
            }
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