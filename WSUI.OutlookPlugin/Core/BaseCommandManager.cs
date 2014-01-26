using Microsoft.Practices.Prism.Events;
using WSUIOutlookPlugin.Events;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin.Core
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