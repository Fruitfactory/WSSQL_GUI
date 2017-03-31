using System;
using System.Threading;
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
    public class OFOnlyAtParseController : OFBaseController
    {
        private OFOnlyAtSettings _localSettings;
        private readonly String AM_CONST = "am";
        private readonly String PM_CONST = "pm";
        private readonly int HOUR_SHIFT = 12;

        private int _hour;

        private bool _isForce;

        public OFOnlyAtParseController(IOFRiverMetaSettingsProvider metaSettingsProvider)
            : base(metaSettingsProvider)
        {
        }


        protected override void ParseSettings(OFRiverMeta settings)
        {
            try
            {
                _localSettings =
                    JsonConvert.DeserializeObject(settings.Pst.Schedule.Settings, typeof(OFOnlyAtSettings)) as
                        OFOnlyAtSettings;
                if (_localSettings.IsNotNull())
                {
                    _hour = GetTimeAccordingSettings(_localSettings);
                }
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        protected override int OnRun(DateTime? lastDateTime)
        {
            Status = OFRiverStatus.StandBy;
            while (true && !_isForce)
            {
                DateTime now = DateTime.Now;
                int currentHour = now.Hour;
                int currentMinutes = now.Minute;
                if (_hour == currentHour && currentMinutes == 0)
                {
                    break;
                }
                Thread.Sleep(1250);

            }

            Status = OFRiverStatus.Busy;


            GetReader().Start(lastDateTime);

            GetReader().Join();

            return 0;
        }

        private int GetTimeAccordingSettings(OFOnlyAtSettings settings)
        {
            String type = settings.HourType.ToLowerInvariant();
            if (settings.HourOnlyAt == 12 && type.Equals(AM_CONST))
            {
                return 0;
            }
            if (settings.HourOnlyAt == 12 && type.Equals(PM_CONST))
            {
                return settings.HourOnlyAt;
            }

            if (type.Equals(AM_CONST))
            {
                return settings.HourOnlyAt;
            }
            else if (type.Equals(PM_CONST))
            {
                return settings.HourOnlyAt + HOUR_SHIFT;
            }
            return 0;
        }
    }
}