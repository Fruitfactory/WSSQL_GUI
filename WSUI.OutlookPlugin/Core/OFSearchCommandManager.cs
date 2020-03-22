using Microsoft.Practices.Prism.Events;
using OF.Core.Logger;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Core
{
    public abstract class OFSearchCommandManager : OFBaseCommandManager, IOFCommandManager
    {
	    protected OFSearchCommandManager(IEventAggregator eventAggregator) : base(eventAggregator)
	    {
	    }

	    public virtual void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {

        }

        protected virtual void InternalShowHidePublish()
        {
            GetAggregator()?.GetEvent<OFShowHideWindow>().Publish(true);
        }

        protected virtual void InternalSearchPublich(string searchString)
        {
            GetAggregator()?.GetEvent<OFSearch>().Publish(searchString);
        }
    }
}