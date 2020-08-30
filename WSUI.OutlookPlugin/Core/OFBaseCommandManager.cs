using Microsoft.Practices.Prism.Events;
using OF.Core.Events;
using OF.Module.Interface.Service;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;

namespace OFOutlookPlugin.Core
{
    public abstract class OFBaseCommandManager :  IOFCommandManager
    {
        private IEventAggregator _eventAggregator;

        protected OFBaseCommandManager()
        {
            
        }

        protected virtual void MenuEnabling(bool obj)
        {   
        }

        protected IEventAggregator GetAggregator()
        {
            return _eventAggregator;
        }

        public virtual void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable)
        {

        }

        public void SetEventAggregator(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<OFMenuEnabling>().Subscribe(MenuEnabling);
        }
    }
}