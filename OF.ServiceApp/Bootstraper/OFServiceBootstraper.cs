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
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Service.Index;
using OF.ServiceApp.Controllers;
using OF.ServiceApp.Core;
using OF.ServiceApp.Events;
using OF.ServiceApp.Interfaces;
using OF.ServiceApp.Rest;

namespace OF.ServiceApp.Bootstraper
{
    public class OFServiceBootstraper : IOFServiceBootstraper
    {
        #region [needs]

        private IEventAggregator _eventAggregator;
        
        private OFRestHosting _restHosting;
        private AutoResetEvent _stopEvent;

        private OFBaseController _controller;

        private readonly object _lock = new object();

        #endregion

        #region [public methods]

        public bool IsApplicationAlreadyWorking()
        {
            var currentExecutablel = (typeof (OFServiceBootstraper)).Assembly.GetName().Name.ToUpperInvariant();
            var processList = Process.GetProcesses();
            var count = processList.Count(p => p.ProcessName.ToUpperInvariant().Equals(currentExecutablel));
            return count > 1;
        }

        public void Initialize()
        {
            _eventAggregator = new EventAggregator();
            
            _restHosting = new OFRestHosting(new OFNancyBootstraper(_eventAggregator));
            var metaSettingsProvider = new OFRiverMetaSettingsProvider();
            var currentSettings = metaSettingsProvider.GetCurrentSettings();
            _controller = OFControllerFactory.Instance.GetController(currentSettings.Pst.Schedule.ScheduleType, metaSettingsProvider);
            
            
            //_eventAggregator.GetEvent<OFStopEvent>().Subscribe(StopExecute);
            //_eventAggregator.GetEvent<OFStartReadEvent>().Subscribe(StartReadExecute);
            //_eventAggregator.GetEvent<OFStopReadEvent>().Subscribe(StopReadExecute);
            //_eventAggregator.GetEvent<OFSuspendReadEvent>().Subscribe(SuspenReadExecute);
            //_eventAggregator.GetEvent<OFResumeReadEvent>().Subscribe(ResumeReadExecute);

            _stopEvent = new AutoResetEvent(false);
        }


        public void Run()
        {
            System.Diagnostics.Debug.WriteLine("Service App have been started...");
            _restHosting.Start();
            _controller.Start();
            _stopEvent.WaitOne();
        }

        public void Exit()
        {
            _controller.Dispose();

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

        #endregion



    }
}