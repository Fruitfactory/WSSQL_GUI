using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Threading;
using Microsoft.Practices.Prism.Events;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.Implements.Service;
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Service.Index;
using OF.ServiceApp.Events;
using OF.ServiceApp.Interfaces;
using OF.ServiceApp.Rest;

namespace OF.ServiceApp.Bootstraper
{
    public class OFServiceBootstraper : IOFServiceBootstraper
    {
        #region [needs]

        private IEventAggregator _eventAggregator;
        private IUserActivityTracker _userActivityTracker;
        private IOutlookItemsReader _outlookItemsReader;
        private OFRestHosting _restHosting;
        private AutoResetEvent _stopEvent;
        private DateTime? _lastDateTime;

        private readonly object _lock = new object();

        #endregion

        #region [public methods]

        public bool IsApplicationAlreadyWorking()
        {
            var currentExecutablel = (typeof (OFServiceBootstraper)).Assembly.GetName().Name.ToUpperInvariant();
            var processList = Process.GetProcesses();
            var count = processList.Count(p => p.ProcessName.ToUpperInvariant().Contains(currentExecutablel));
            return count > 1;
        }

        public void Initialize()
        {
            DisableAccessPrompt();

            _eventAggregator = new EventAggregator();
            _userActivityTracker = new OFUserActivityTracker();
            _restHosting = new OFRestHosting(new OFNancyBootstraper(_eventAggregator));
            _outlookItemsReader = new OFOutlookItemsReader();
            
            _eventAggregator.GetEvent<OFStopEvent>().Subscribe(StopExecute);
            _eventAggregator.GetEvent<OFStartReadEvent>().Subscribe(StartReadExecute);
            _eventAggregator.GetEvent<OFStopReadEvent>().Subscribe(StopReadExecute);
            _eventAggregator.GetEvent<OFSuspendReadEvent>().Subscribe(SuspenReadExecute);
            _eventAggregator.GetEvent<OFResumeReadEvent>().Subscribe(ResumeReadExecute);

            _stopEvent = new AutoResetEvent(false);
        }


        public void Run()
        {
            System.Diagnostics.Debug.WriteLine("Service App have been started...");
            _userActivityTracker.Start();
            _restHosting.Start();
            _stopEvent.WaitOne();

        }

        public void Exit()
        {
            _userActivityTracker.Stop();
            _userActivityTracker = null;

            StopReadExecute(true);

            _restHosting.Dispose();

            _stopEvent.Dispose();
            _stopEvent = null;
            System.Diagnostics.Debug.WriteLine("Service App have been stopped...");
        }

        #endregion


        #region [protected private]

        private void StopExecute(bool b)
        {
            _stopEvent.Set();
        }

        private void ResumeReadExecute(DateTime? date)
        {
            StartReadExecute(date);
            lock (_lock)
            {
                if (_outlookItemsReader.IsNotNull() && _outlookItemsReader.IsStarted && _outlookItemsReader.IsSuspended)
                {
                    _outlookItemsReader.Resume(date);
                }    
            }
        }

        private void SuspenReadExecute(bool b)
        {
            lock (_lock)
            {
                if (_outlookItemsReader.IsNotNull() && _outlookItemsReader.IsStarted && !_outlookItemsReader.IsSuspended)
                {
                    _outlookItemsReader.Suspend();
                }    
            }
        }

        private void StopReadExecute(bool b)
        {
            lock (_lock)
            {
                if (_outlookItemsReader.IsNotNull() && !_outlookItemsReader.IsSuspended)
                {
                    _outlookItemsReader.Stop();
                }    
            }
        }

        private void StartReadExecute(DateTime? date)
        {
            lock (_lock)
            {
                if (_outlookItemsReader.IsNotNull() && !_outlookItemsReader.IsStarted)
                {
                    OFLogger.Instance.LogInfo("Start Reading Attachment...");
                    _outlookItemsReader.Start(date);
                }    
            }
        }


        private void DisableAccessPrompt()
        {
            try
            {
                var versions = (new OFOfficeVersionFinder()).GetOfficeVersion();
                OFRegistryHelper.Instance.DisableOutlookSecurityWarning(versions.Item2);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            
        }

        #endregion



    }
}