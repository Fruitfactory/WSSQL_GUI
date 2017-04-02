using System;
using Microsoft.Practices.Prism.Events;
using OF.Core.Enums;
using OF.Core.Extensions;
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
            IOFRiverMetaSettingsProvider metaSettingsProvider, IEventAggregator eventAggregator)
        {
            OFBaseController controller = null;
            switch (riverSchedule)
            {
                case OFRiverSchedule.EveryNightOrIdle:
                    controller = new OFNightOrIdleTrackingController(metaSettingsProvider,eventAggregator);
                    break;
                case OFRiverSchedule.OnlyAt:
                    controller = new OFOnlyAtParseController(metaSettingsProvider);
                    break;
                case OFRiverSchedule.EveryHours:
                    controller = new OFRepeatController(metaSettingsProvider);
                    break;
            }
            if (controller.IsNotNull())
            {
                controller.Initialize();
            }
            return controller;
        }
    }
}