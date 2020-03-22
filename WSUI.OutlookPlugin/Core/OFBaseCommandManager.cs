using Microsoft.Practices.Prism.Events;
using OF.Core.Events;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Core
{
    public abstract class OFBaseCommandManager 
    {
        private IEventAggregator _eventAggregator;

        protected OFBaseCommandManager(IEventAggregator eventAggregator)
        {
	        _eventAggregator = eventAggregator;
	        _eventAggregator.GetEvent<OFMenuEnabling>().Subscribe(MenuEnabling);
        }

        protected virtual void MenuEnabling(bool obj)
        {   
        }

        protected IEventAggregator GetAggregator()
        {
            return _eventAggregator;
        }

    }
}