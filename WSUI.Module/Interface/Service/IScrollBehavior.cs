using System;
using WSUI.Infrastructure.Service;

namespace WSUI.Module.Interface.Service
{
    public interface IScrollBehavior
    {
        int CountFirstProcess { get; set; }
        int CountSecondProcess { get; set; }
        int LimitReaction { get; set; }
        event Action SearchGo;
        void NeedSearch(ScrollData sd);
    }
}
