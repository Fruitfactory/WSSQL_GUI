using Microsoft.Practices.Prism.Events;

namespace WSUIOutlookPlugin.Interfaces
{
    public interface ICommandManager
    {
        void SetEventAggregator(IEventAggregator aggregator);
    }
}