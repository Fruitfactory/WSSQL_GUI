using Microsoft.Practices.Prism.Events;
using WSUIOutlookPlugin.Events;
using WSUIOutlookPlugin.Interfaces;

namespace WSUIOutlookPlugin.Core
{
    public abstract class BaseCommandManager : IWSUICommandManager
    {
        private IEventAggregator _eventAggregator;
        
        public void SetEventAggregator(IEventAggregator aggregator)
        {
            _eventAggregator = aggregator;
        }

        public virtual void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {
            
        }

        protected virtual void InternalShowPublish()
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<WSUIOpenWindow>().Publish(true);
        }

        protected virtual void InternalHidePublish()
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<WSUIHideWindow>().Publish(true);
        }

        protected virtual void InternalSearchPublich(string searchString)
        {
            if(_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<WSUISearch>().Publish(searchString);
        }

    }
}