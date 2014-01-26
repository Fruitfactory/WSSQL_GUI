using Microsoft.Practices.Prism.Events;
using WSUIOutlookPlugin.Events;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin.Core
{
    public abstract class SearchCommandManager : BaseCommandManager, IWSUICommandManager
    {

        public virtual void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {

        }

        protected virtual void InternalShowPublish()
        {
            var aggregator = GetAggregator();
            if (aggregator == null)
                return;
            aggregator.GetEvent<WSUIOpenWindow>().Publish(true);
        }

        protected virtual void InternalHidePublish()
        {
            var aggregator = GetAggregator();
            if (aggregator == null)
                return;
            aggregator.GetEvent<WSUIHideWindow>().Publish(true);
        }

        protected virtual void InternalSearchPublich(string searchString)
        {
            var aggregator = GetAggregator();
            if (aggregator == null)
                return;
            aggregator.GetEvent<WSUISearch>().Publish(searchString);
        }
    }
}