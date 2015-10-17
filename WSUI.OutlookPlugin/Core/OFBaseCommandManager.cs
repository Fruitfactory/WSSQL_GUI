using Microsoft.Practices.Prism.Events;
using OF.Core.Events;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Core
{
    public abstract class OFBaseCommandManager : ICommandManager
    {
        private IEventAggregator _eventAggregator;
        
        public void SetEventAggregator(IEventAggregator aggregator)
        {
            _eventAggregator = aggregator;
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