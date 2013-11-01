using System;
using Microsoft.Practices.Prism.Events;

namespace WSUIOutlookPlugin.Interfaces
{
    public interface IWSUICommandManager
    {
        void SetEventAggregator(IEventAggregator aggregator);
        void SetShowHideButtonsEnabling(bool isShowButtonEnable, bool isHideButtonEnable);
    }
}