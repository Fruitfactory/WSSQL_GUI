using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Data.Settings.ControllerSettings;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Win32;
using OF.Infrastructure.Implements.ElasticSearch.Clients;

namespace OF.Infrastructure.Service.Helpers
{
    public class OFUserActivityTracker : IUserActivityTracker
    {
        private readonly OFElasticTrackingClient _elasticSearchClient;
        private IOutlookItemsReader _reader;
        private readonly Thread _userActivityThread;
        private volatile bool _stop;
        private readonly int _onlineTime;
        private DateTime? _lastDate;
        private ofUserActivityState oldState;
        private volatile bool _isForce;

        private readonly object LOCK = new object();

        private readonly DateTime _startNight = new DateTime(0, 0, 0, 0, 0, 0);
        private readonly DateTime _finishNight = new DateTime(0, 0, 0, 6, 0, 0);

        public OFUserActivityTracker(int onlineTime, DateTime? lastDate)
        {
            _elasticSearchClient = new OFElasticTrackingClient();
            _userActivityThread = new Thread(UserActivityProcess);
            _onlineTime = onlineTime;
            _lastDate = lastDate;
        }


        public void Start(IOutlookItemsReader reader)
        {
            _reader = reader;
            _userActivityThread.Start();
        }

        public void Stop()
        {
            if (!_userActivityThread.IsAlive)
            {
                return;
            }
            _stop = true;
            _userActivityThread.Join();
        }

        public void Update(bool isForced)
        {
            _isForce = isForced;
        }

        private void UserActivityProcess(object arg)
        {
            IIndexExistsRequest request = new IndexExistsRequest(OFElasticSearchClientBase.DefaultInfrastructureName);

            while (!_stop)
            {
                try
                {
                    var resp = _elasticSearchClient.IndexExists(request);
                    if (!resp.Exists)
                    {
                        OFLogger.Instance.LogDebug("ES Exist = " + resp.Exists);
                        Thread.Sleep(2000);
                        continue;
                    }
                    var idle = WindowsFunction.GetIdleTime();
                    uint idleTimeSec = idle / 1000;
                    System.Diagnostics.Debug.WriteLine(string.Format("{0} - {1}", idle, idleTimeSec));

                    var newState = IsNight() || _isForce
                        ? ofUserActivityState.Night
                        : idleTimeSec < _onlineTime ? ofUserActivityState.Online : ofUserActivityState.Away;
                    if (newState != oldState)
                    {
                        lock (LOCK)
                        {
                            ProcessState(newState);    
                        }
                    }
                    oldState = newState;
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                Thread.Sleep(1000);
            }
        }

        private void ProcessState(ofUserActivityState newState)
        {
            if (_reader.IsNull())
            {
                return;
            }
            switch (newState)
            {
                case ofUserActivityState.Online:
                    if (_reader.Status == PstReaderStatus.Busy && !_reader.IsSuspended)
                    {
                        _reader.Suspend();
                    }

                    break;
                case ofUserActivityState.Away:
                case ofUserActivityState.Night:
                    if (_reader.Status == PstReaderStatus.Busy && _reader.IsSuspended)
                    {
                        _reader.Resume(_lastDate);
                    }
                    break;
            }
        }

        private bool IsNight()
        {
            DateTime current = DateTime.Now;
            return _startNight.TimeOfDay < current.TimeOfDay && current.TimeOfDay < _finishNight.TimeOfDay;
        }




    }
}