using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Microsoft.Practices.Prism.Events;
using OF.Core.Extensions;
using OF.Core.Interfaces;
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
        private IAttachmentReader _attachmentReader;
        private OFRestHosting _restHosting;
        private AutoResetEvent _stopEvent;

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
            _eventAggregator = new EventAggregator();
            _userActivityTracker = new OFUserActivityTracker();
            _restHosting = new OFRestHosting(new OFNancyBootstraper(_eventAggregator));
            
            _eventAggregator.GetEvent<OFStopEvent>().Subscribe(StopExecute);
            _eventAggregator.GetEvent<OFStartReadEvent>().Subscribe(StartReadExecute);
            _eventAggregator.GetEvent<OFStopReadEvent>().Subscribe(StopReadExecute);
            _eventAggregator.GetEvent<OFSuspendReadEvent>().Subscribe(SuspenReadExecute);
            _eventAggregator.GetEvent<OFResumeReadEvent>().Subscribe(ResumeReadExecute);


            _stopEvent = new AutoResetEvent(false);
        }


        public void Run()
        {
            Console.WriteLine("Service App have been started...");
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
            Console.WriteLine("Service App have been stopped...");
        }

        #endregion


        #region [protected private]

        private void StopExecute(bool b)
        {
            _stopEvent.Set();
        }

        private void ResumeReadExecute(bool b)
        {
            if (_attachmentReader.IsNotNull() && _attachmentReader.IsSuspended)
            {
                _attachmentReader.Resume();
            }
        }

        private void SuspenReadExecute(bool b)
        {
            if (_attachmentReader.IsNotNull() && !_attachmentReader.IsSuspended)
            {
                _attachmentReader.Suspend();
            }
        }

        private void StopReadExecute(bool b)
        {
            if (_attachmentReader.IsNotNull() && !_attachmentReader.IsSuspended)
            {
                _attachmentReader.Stop();
                _attachmentReader = null;
            }
        }

        private void StartReadExecute(DateTime? date)
        {
            _attachmentReader = new OFAttachmentReader();
            _attachmentReader.Start(date);
        }
        #endregion



    }
}