using System;
using Newtonsoft.Json;
using OF.Core;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Data.Settings.ControllerSettings;
using OF.Core.Interfaces;
using OF.Infrastructure.NamedPipes;
using OF.Infrastructure.Service.Helpers;
using OF.ServiceApp.Core;
using Microsoft.Practices.Prism.Events;

namespace OF.ServiceApp.Controllers
{
    public class OFNightOrIdleTrackingController : OFBaseController
    {
        private OFNightIdleSettings _localSettings;
        private IUserActivityTracker _activityTracker;

        public OFNightOrIdleTrackingController(IOFRiverMetaSettingsProvider metaSettingsProvider,IEventAggregator eventAggregator) : base(metaSettingsProvider,eventAggregator)
        {
        }

        protected override void ParseSettings(OFRiverMeta settings)
        {
            _localSettings = JsonConvert.DeserializeObject(settings.Pst.Schedule.Settings, typeof(OFNightIdleSettings)) as OFNightIdleSettings;
        }

        protected override int OnRun(DateTime? lastDateTime)
        {
            GetReader().Start(lastDateTime);
            _activityTracker = new OFUserActivityTracker(_localSettings.IdleTime,lastDateTime);
            _activityTracker.Start(GetReader());
            
            GetReader().Join();
            _activityTracker.Stop();

            return 1;
        }

        protected override void OnEventProcessing()
        {
            base.OnEventProcessing();
            if (_activityTracker != null)
            {
                _activityTracker.Update(IsForced);
            }
        }

        protected override void OnStop()
        {
            base.OnStop();
            if (_activityTracker != null)
            {
                _activityTracker.Stop();
            }
        }
    }
}