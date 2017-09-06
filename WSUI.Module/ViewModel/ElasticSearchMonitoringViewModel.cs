using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using Newtonsoft.Json;
using OF.Core.Core.ElasticSearch;
using OF.Core.Core.MVVM;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure;
using OF.Infrastructure.Services;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using Timer = System.Timers.Timer;

namespace OF.Module.ViewModel
{
    public class ElasticSearchMonitoringViewModel : OFViewModelBase, IElasticSearchMonitoringViewModel, IOFNamedPipeObserver<OFReaderStatus>
    {

        private static string UPDATING = "Updating...";
        private static string READY = "Ready.";

        private IElasticSearchRiverStatus _riverStatusClient;
        private IEventAggregator _eventAggregator;
        private IUnityContainer _unityContainer;
        private IRegionManager _regionManager;
        private IOFRiverMetaSettingsProvider _metaSettingsProvider;

        private readonly  object _lock = new object();

        private OFReaderStatus _currentStatus;

        private Timer _timer;


        public event EventHandler<EventArgs<OFRiverStatus>> StatusChanged;

        public ElasticSearchMonitoringViewModel(IElasticSearchRiverStatus riverStatusClient, 
            IEventAggregator eventAggregator, 
            IRegionManager regionManager,
            IUnityContainer unityContainer,
            IOFRiverMetaSettingsProvider metaSettingsProvider,
            IElasticSearchMonitoringView view)
        {
            _riverStatusClient = riverStatusClient;
            _eventAggregator = eventAggregator;
            _regionManager = regionManager;
            _unityContainer = unityContainer;
            _metaSettingsProvider = metaSettingsProvider;
            View = view;
            view.Model = this;
        }

        #region [properties]

        [Dependency]
        public IElasticSearchInitializationIndex ElasticSearchClient { get; set; }

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
            OFReaderStatus current = null;
            lock (_lock)
            {
                current = _currentStatus;
            }
            var metaSettings = _metaSettingsProvider.GetCurrentSettings();
            if (current.IsNull())
            {
                var response = ElasticSearchClient.GetRiverStatus();
                current = JsonConvert.DeserializeObject<IEnumerable<OFReaderStatus>>(response.Body.ToString()).FirstOrDefault();
            }

            var emailCount = _riverStatusClient.GetTypeCount<OFEmail>();
            var attachmentCount = _riverStatusClient.GetTypeCount<OFAttachmentContent>();

            if (current.IsNotNull())
            {
                if (Status != current.ControllerStatus)
                {
                    Status = current.ControllerStatus;
                    OnStatusChanged(Status);
                }
            }
            else
            {
                Status = OFRiverStatus.None;
            }
            StatusText = Status == OFRiverStatus.Busy || Status == OFRiverStatus.InitialIndexing ? UPDATING : READY;
            LastUpdated = metaSettings.LastDate ?? DateTime.MinValue;
            EmailCount = emailCount;
            AttachmentCount = attachmentCount;
        }

        public void Stop()
        {
            if (_timer.IsNotNull())
            {
                _timer.Enabled = false;
            }
        }

        public bool IsRunning { get { return _timer.IsNotNull() && _timer.Enabled; } }

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

        public object Update(OFReaderStatus message)
        {
            lock (_lock)
            {
                _currentStatus = message;
            }
            return new object();
        }

        private void OnStatusChanged(OFRiverStatus status)
        {
            var temp = StatusChanged;
            if (temp.IsNotNull())
            {
                temp(this,new EventArgs<OFRiverStatus>(status));
            }
        }
    }
}