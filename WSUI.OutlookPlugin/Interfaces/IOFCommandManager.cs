
using Microsoft.Practices.Prism.Events;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFCommandManager
    {
        void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable);

        void SetEventAggregator(IEventAggregator eventAggregator);
    }
}