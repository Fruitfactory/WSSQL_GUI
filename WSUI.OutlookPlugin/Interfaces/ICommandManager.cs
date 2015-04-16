using Microsoft.Practices.Prism.Events;

namespace OFOutlookPlugin.Interfaces
{
    public interface ICommandManager
    {
        void SetEventAggregator(IEventAggregator aggregator);
    }
}