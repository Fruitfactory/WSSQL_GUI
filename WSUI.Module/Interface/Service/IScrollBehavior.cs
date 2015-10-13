using System;
using OF.Infrastructure.Service;

namespace OF.Module.Interface.Service
{
    public interface IScrollBehavior
    {
        int CountFirstProcess { get; set; }
        int CountSecondProcess { get; set; }
        int LimitReaction { get; set; }
        event Action SearchGo;
        void NeedSearch(OFScrollData sd);
    }
}
