using Microsoft.Practices.Prism.Events;
using OF.Core.Logger;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Core
{
    public abstract class SearchCommandManager : BaseCommandManager, IOFCommandManager
    {

        public virtual void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {

        }

        protected virtual void InternalShowPublish()
        {
            var aggregator = GetAggregator();
            if (aggregator == null)
                return;
            aggregator.GetEvent<OFOpenWindow>().Publish(true);
        }

        protected virtual void InternalHidePublish()
        {
            var aggregator = GetAggregator();
            if (aggregator == null)
                return;
            aggregator.GetEvent<OFHideWindow>().Publish(true);
        }

        protected virtual void InternalSearchPublich(string searchString)
        {
            var aggregator = GetAggregator();
            if (aggregator == null)
                return;
            WSSqlLogger.Instance.LogInfo("Edit Criteria (toolbox):{0}", searchString);
            aggregator.GetEvent<OFSearch>().Publish(searchString);
        }
    }
}