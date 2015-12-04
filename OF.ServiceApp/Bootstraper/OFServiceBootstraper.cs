using System;
using System.Threading;
using Microsoft.Practices.Prism.Events;
using OF.Core.Interfaces;
using OF.Infrastructure.Service.Helpers;
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
        private OFRestHosting _restHosting;
        private AutoResetEvent _stopEvent;


        #endregion

        #region [public methods]

        public void Initialize()
        {

            _eventAggregator = new EventAggregator();
            _userActivityTracker = new OFUserActivityTracker();
            _restHosting = new OFRestHosting(new OFNancyBootstraper(_eventAggregator));
            _eventAggregator.GetEvent<OFStopEvent>().Subscribe(StopExecute);
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

        #endregion



    }
}