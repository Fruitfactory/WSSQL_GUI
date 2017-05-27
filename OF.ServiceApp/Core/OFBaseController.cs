using System;
using System.Threading;
using OF.Core;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.Helpers.AttachedProperty;
using OF.Infrastructure.NamedPipes;
using OF.Infrastructure.Service.Index;
using Microsoft.Practices.Prism.Events;
using Newtonsoft.Json;
using OF.Infrastructure.Implements.ElasticSearch.Clients;
using OF.ServiceApp.Events;
using OF.ServiceApp.Service;

namespace OF.ServiceApp.Core
{
    public abstract class OFBaseController : IDisposable, IOFIOutlookItemsReaderObserver
    {
        private Thread _thread;
        private readonly IOFRiverMetaSettingsProvider _metaSettingsProvider;
        private IOutlookItemsReader _outlookItemsReader;
        private IElasticSearchCloseJavaClient _elasticSearchCloseJavaClient;
        private readonly IEventAggregator _eventAggregator;
        
        private readonly object LOCK = new object();
        private bool _stoped = false;
        private bool _isOFPluginStarted = false;

        protected OFBaseController(IOFRiverMetaSettingsProvider metaSettingsProvider)
        {
            _metaSettingsProvider = metaSettingsProvider;
            _outlookItemsReader = new OFOutlookItemsReader();
            _outlookItemsReader.Attach(this);
            _elasticSearchCloseJavaClient = new OFElasticSeachIndexOutlookItemsClient();
        }

        protected OFBaseController(IOFRiverMetaSettingsProvider metaSettingsProvider, IEventAggregator eventAggregator) 
            :this(metaSettingsProvider)
        {
            _eventAggregator = eventAggregator;
        }
        
        public void Initialize()
        {
            Status = OFRiverStatus.None;
            var settings = _metaSettingsProvider.GetCurrentSettings();
            ParseSettings(settings);
            SubscribeEvents(_eventAggregator);
            Status = !settings.LastDate.HasValue ? OFRiverStatus.InitialIndexing : OFRiverStatus.StandBy;
        }

        protected virtual void SubscribeEvents(IEventAggregator eventAggregator)
        {
            _eventAggregator.GetEvent<OFForcedEvent>().Subscribe(OnForsedEvent);
        }

        protected virtual void OnEventProcessing()
        {
            
        }

        private void OnForsedEvent(object o)
        {
            var isForsed = JsonConvert.DeserializeObject<OFIsForcedMessage>(o.ToString());
            if (isForsed.IsNotNull())
            {
                IsForced = isForsed.IsForced;
            }
            OnEventProcessing();
        }

        public void Start()
        {
            lock (LOCK)
            {
                if (_thread.IsNull())
                {
                    _thread = new Thread(run)
                    {
                        Priority =
                            Status == OFRiverStatus.InitialIndexing ? ThreadPriority.Highest : ThreadPriority.Lowest
                    };
                    _thread.Start();
                }
                OnStart();
            }
        }

        public void Close()
        {
            lock (LOCK)
            {
                Volatile.Write(ref _stoped,true);
                OnStop();
                if (_outlookItemsReader.IsNotNull())
                {
                    _outlookItemsReader.Stop();
                }
                if (_thread.IsNotNull())
                {
                    _thread.Join();
                    _thread = null;
                }

            }
        }

        public void SetOfPluginStatus(bool status)
        {
            Volatile.Write(ref _isOFPluginStarted,status);
        }

        protected bool IsStoped
        {
            get
            {
                bool isStoped;
                lock (LOCK)
                    isStoped = Volatile.Read(ref _stoped);
                return isStoped;
            }
        }

        protected bool IsForced { get; private set; }

        private OFRiverStatus _status;
        public OFRiverStatus Status
        {
            get
            {
                return _status;
            }
            protected set
            {
                _status = value;
                SendStatus(value,_outlookItemsReader.Status,_outlookItemsReader.Count);
            }
        }

        public PstReaderStatus ReaderStatus
        {
            get { return _outlookItemsReader.IsSuspended ? PstReaderStatus.Suspended :_outlookItemsReader.Status; }
        }

        public string CurrentFolder
        {
            get { return _outlookItemsReader.Folder; }
        }

        public int Count
        {
            get { return _outlookItemsReader.Count; }
        }

        protected IOutlookItemsReader GetReader()
        {
            return _outlookItemsReader;
        }

        protected abstract void ParseSettings(OFRiverMeta settings);

        protected virtual void OnStart()
        {
            
        }

        protected virtual void OnStop()
        {
            
        }

        protected abstract int OnRun(DateTime? lastDateTime);

        private void run()
        {
            while (true)
            {
                if (IsStoped)
                {
                    OFLogger.Instance.LogInfo("Controller was closed...");
                    break;
                }
                var settings = _metaSettingsProvider.GetCurrentSettings();
                //OFParserApplicationHandler.Instance.StartParser();
                var delay = OnRun(settings.LastDate);
                _metaSettingsProvider.UpdateLastIndexingDateTime(DateTime.Now);

                Status = OFRiverStatus.StandBy;
                OFLogger.Instance.LogDebug($"Parser/Sender has been finshed: {_elasticSearchCloseJavaClient.Stop()}");
                //OFParserApplicationHandler.Instance.StopParser();
                if (IsStoped)
                {
                    OFLogger.Instance.LogInfo("Controller was closed...");
                    break;
                }
                if (delay > 0)
                {
                    Thread.Sleep(TimeSpan.FromHours(delay));
                }
            }
        }


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_thread.IsNotNull() && !IsStoped)
                {
                    try
                    {
                        Close();
                    }
                    catch (Exception e)
                    {
                        OFLogger.Instance.LogError(e.ToString());
                    }
                }
                if (_outlookItemsReader.IsNotNull())
                {
                    _outlookItemsReader.Stop();
                    _outlookItemsReader = null;    
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateStatus(PstReaderStatus newStatus)
        {
            SendStatus(Status,newStatus,_outlookItemsReader.Count);            
        }


        private void SendStatus(OFRiverStatus ctrlStatus, PstReaderStatus readerStatus, int count)
        {
            var ofstatus = Volatile.Read(ref _isOFPluginStarted);
            if (!ofstatus)
            {
                return;
            }
            try
            {
                var status = new OFReaderStatus()
                {
                    ControllerStatus = ctrlStatus, ReaderStatus = readerStatus, Count  = count
                };
                OFNamedPipeClient<OFReaderStatus> client = new OFNamedPipeClient<OFReaderStatus>(GlobalConst.PluginServer);
                client.Send(status);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

    }
}