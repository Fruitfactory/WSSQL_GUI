using System;
using System.Diagnostics;
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
using OF.Core.Extensions;
using OF.Core.Logger;

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
            if (_localSettings.IsNotNull())
            {
                _activityTracker = new OFUserActivityTracker(_localSettings.IdleTime);
            }
        }

        protected override int OnRun(DateTime? lastDateTime)
        {
            OFLogger.Instance.LogDebug($"Run controller...");

            _activityTracker.SetLastDate(lastDateTime);
            GetReader().Start(lastDateTime);
            _activityTracker.Start(GetReader());
            
            GetReader().Join();
            _activityTracker.Stop();

            return 1;
        }

        protected override void OnEventProcessing()
        {
            base.OnEventProcessing();
            OFLogger.Instance.LogDebug($"Update Activity Tracker...");
            _activityTracker?.Update(IsForced);
        }

        protected override void OnStop()
        {
            base.OnStop();
            _activityTracker?.Stop();
        }
    }
}