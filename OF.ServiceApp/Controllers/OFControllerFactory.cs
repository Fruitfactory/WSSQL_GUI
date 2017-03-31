using System;
using OF.Core.Enums;
using OF.Core.Interfaces;
using OF.ServiceApp.Core;

namespace OF.ServiceApp.Controllers
{
    public class OFControllerFactory
    {


        private static Lazy<OFControllerFactory> _instance = new Lazy<OFControllerFactory>(() => new OFControllerFactory());

        public static OFControllerFactory Instance
        {
            get { return _instance.Value; }
        }

        public OFBaseController GetController(OFRiverSchedule riverSchedule,
            IOFRiverMetaSettingsProvider metaSettingsProvider)
        {
            switch (riverSchedule)
            {
                case OFRiverSchedule.EveryNightOrIdle:
                    return new OFNightOrIdleTrackingController(metaSettingsProvider);
                case OFRiverSchedule.OnlyAt:
                    return new OFOnlyAtParseController(metaSettingsProvider);
                case OFRiverSchedule.EveryHours:
                    return new OFRepeatController(metaSettingsProvider);
            }
            return null;
        }
    }
}