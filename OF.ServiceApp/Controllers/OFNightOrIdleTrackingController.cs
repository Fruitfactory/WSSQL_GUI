using System;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.Settings.ControllerSettings;
using OF.Core.Interfaces;
using OF.Infrastructure.Service.Helpers;
using OF.ServiceApp.Core;

namespace OF.ServiceApp.Controllers
{
    public class OFNightOrIdleTrackingController : OFBaseController
    {
        private OFNightIdleSettings _localSettings;

        private IUserActivityTracker _activityTracker;

        public OFNightOrIdleTrackingController(IOFRiverMetaSettingsProvider metaSettingsProvider) : base(metaSettingsProvider)
        {
        }

        protected override void ParseSettings(OFRiverMeta settings)
        {
            _localSettings = JsonConvert.DeserializeObject(settings.Pst.Schedule.Settings, typeof(OFNightIdleSettings)) as OFNightIdleSettings;
        }

        protected override int OnRun(DateTime? lastDateTime)
        {
            // TODO: need to pass items reader
            _activityTracker = new OFUserActivityTracker(_localSettings.IdleTime,lastDateTime);
            _activityTracker.Start(GetReader());
            GetReader().Start(lastDateTime);

            GetReader().Join();
            _activityTracker.Stop();

            return 1;
        }

        protected override void OnStop()
        {
            base.OnStop();
            _activityTracker.Stop();
        }
    }
}