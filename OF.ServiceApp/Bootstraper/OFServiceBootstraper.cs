using System;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Threading;
using Microsoft.Practices.Prism.Events;
using OF.Core;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.NamedPipes;
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Service.Index;
using OF.ServiceApp.Controllers;
using OF.ServiceApp.Core;
using OF.ServiceApp.Events;
using OF.ServiceApp.Interfaces;
using OF.ServiceApp.Rest;

namespace OF.ServiceApp.Bootstraper
{
    public class OFServiceBootstraper : IOFServiceBootstraper, IOFNamedPipeObserver<OFServiceApplicationMessage>
    {
        #region [needs]



        private IEventAggregator _eventAggregator;

        private AutoResetEvent _stopEvent;
        private IOFNamedPipeServer<OFServiceApplicationMessage> _namedPipeServer;
        private OFBaseController _controller;

        private readonly object _lock = new object();

        #endregion

        #region [public methods]

        public bool IsApplicationAlreadyWorking()
        {
            var currentExecutablel = (typeof(OFServiceBootstraper)).Assembly.GetName().Name.ToUpperInvariant();
            var processList = Process.GetProcesses();
            var count = processList.Count(p => p.ProcessName.ToUpperInvariant().Equals(currentExecutablel));
            return count > 1;
        }

        public void Initialize()
        {
            _eventAggregator = new EventAggregator();
            _namedPipeServer = new OFNamedPipeServer<OFServiceApplicationMessage>(GlobalConst.ServiceApplicationServer);
            _namedPipeServer.Attach(this);
            var metaSettingsProvider = new OFRiverMetaSettingsProvider();
            var currentSettings = metaSettingsProvider.GetCurrentSettings();
            _controller = OFControllerFactory.Instance.GetController(currentSettings.Pst.Schedule.ScheduleType, metaSettingsProvider, _eventAggregator);
            _stopEvent = new AutoResetEvent(false);
        }

        public void Update(OFServiceApplicationMessage message)
        {
            switch (message.MessageType)
            {
                case ofServiceApplicationMessageType.StartIndexing:
                    if (_controller.IsNotNull() && _controller.Status == OFRiverStatus.InitialIndexing)
                    {
                        _controller.Start();
                    }
                    break;
                case ofServiceApplicationMessageType.ForceIndexing:
                    _eventAggregator.GetEvent<OFForcedEvent>().Publish(message.Payload);
                    break;
            }
        }

        public void Run()
        {
            System.Diagnostics.Debug.WriteLine("Service App have been started...");
            if (_controller.Status > OFRiverStatus.InitialIndexing)
            {
                _controller.Start();
            }
            _namedPipeServer.Start();
            _stopEvent.WaitOne();
        }

        public void Exit()
        {
            _controller.Dispose();
            _namedPipeServer.Stop();
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