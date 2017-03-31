using System;
using Newtonsoft.Json;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.Settings.ControllerSettings;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.ServiceApp.Core;

namespace OF.ServiceApp.Controllers
{
    public class OFRepeatController : OFBaseController
    {
        private OFEveryHourPeriodSettings _localSettings;

        public OFRepeatController(IOFRiverMetaSettingsProvider metaSettingsProvider) : base(metaSettingsProvider)
        {
        }

        protected override void ParseSettings(OFRiverMeta settings)
        {
            try
            {
                _localSettings = JsonConvert.DeserializeObject(settings.Pst.Schedule.Settings, typeof(OFEveryHourPeriodSettings)) as OFEveryHourPeriodSettings;
            }
            catch (Exception e)
            {
                
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        protected override int OnRun(DateTime? lastDateTime)
        {
            Status = OFRiverStatus.StandBy;
            GetReader().Start(lastDateTime);
            Status = OFRiverStatus.Busy;
            GetReader().Join();

            return _localSettings.IsNotNull() ? _localSettings.HourPeriod : 1;
        }
    }
}