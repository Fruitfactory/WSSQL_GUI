using System;
using System.Threading;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.Helpers.AttachedProperty;
using OF.Infrastructure.Service.Index;

namespace OF.ServiceApp.Core
{
    public abstract class OFBaseController : IDisposable
    {
        private Thread _thread;
        private IOFRiverMetaSettingsProvider _metaSettingsProvider;
        private IOutlookItemsReader _outlookItemsReader;

        private object LOCK = new object();
        private volatile bool _stoped = false;

        protected OFBaseController(IOFRiverMetaSettingsProvider metaSettingsProvider)
        {
            _metaSettingsProvider = metaSettingsProvider;
            _outlookItemsReader = new OFOutlookItemsReader();
        }

        public void Initialize()
        {
            Status = OFRiverStatus.None;
            var settings = _metaSettingsProvider.GetCurrentSettings();
            ParseSettings(settings);
            Status = !settings.LastDate.HasValue ? OFRiverStatus.InitialIndexing : OFRiverStatus.StandBy;
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
                _stoped = true;
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

        protected bool IsStoped
        {
            get
            {
                bool isStoped;
                lock (LOCK)
                    isStoped = _stoped;
                return isStoped;
            }
        }

        protected OFRiverStatus Status { get; set; }

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
                var delay = OnRun(settings.LastDate);
                _metaSettingsProvider.UpdateLastIndexingDateTime(DateTime.Now);
                Status = OFRiverStatus.StandBy;

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
    }
}