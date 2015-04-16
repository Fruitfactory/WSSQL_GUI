using Microsoft.Practices.Prism.Events;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Core
{
    public abstract class BaseCommandManager : ICommandManager
    {
        private IEventAggregator _eventAggregator;
        
        public void SetEventAggregator(IEventAggregator aggregator)
        {
            _eventAggregator = aggregator;
        }

        protected IEventAggregator GetAggregator()
        {
            return _eventAggregator;
        }

    }
}