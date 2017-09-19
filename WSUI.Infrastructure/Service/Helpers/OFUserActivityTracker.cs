using System;
using System.Diagnostics;
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
        
        private IOutlookItemsReader _reader;
        private Thread _userActivityThread;
        private volatile bool _stop;
        private readonly int _onlineTime;
        private DateTime? _lastDate;
        private ofUserActivityState oldState;
        private bool _isForce = false;

        private readonly object LOCK = new object();

        private readonly DateTime _startNight = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, 0, 0, 0);
        private readonly DateTime _finishNight = new DateTime(DateTime.MinValue.Year, DateTime.MinValue.Month, DateTime.MinValue.Day, 6, 0, 0);

        public OFUserActivityTracker(int onlineTime)
        {
            _onlineTime = onlineTime;
        }


        public void Start(IOutlookItemsReader reader)
        {
            _userActivityThread = new Thread(UserActivityProcess);
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
            _userActivityThread = null;
        }

        public void SetLastDate(DateTime? lastDateTime)
        {
            _lastDate = lastDateTime;
        }


        public void Update(bool isForced)
        {
            OFLogger.Instance.LogDebug($"Update Force: {isForced}");
            Volatile.Write(ref _isForce, isForced);
        }

        private void UserActivityProcess(object arg)
        {
            while (!_stop)
            {
                try
                {

                    var idle = WindowsFunction.GetIdleTime();
                    uint idleTimeSec = idle / 1000;
                    System.Diagnostics.Debug.WriteLine(string.Format("{0}: {1} < {2}", idle, idleTimeSec,_onlineTime));
                    var isForce = Volatile.Read(ref _isForce);
                    var newState = IsNight() || isForce
                        ? ofUserActivityState.Night
                        : idleTimeSec < _onlineTime ? ofUserActivityState.Online : ofUserActivityState.Away;
                    System.Diagnostics.Debug.WriteLine(string.Format("Status: {0}\nOld Status: {1}", newState, oldState));
                    if (newState != oldState)
                    {
                        ProcessState(newState);
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
                    if (_reader.Status ==  PstReaderStatus.Suspended || (_reader.Status == PstReaderStatus.Busy && _reader.IsSuspended))
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