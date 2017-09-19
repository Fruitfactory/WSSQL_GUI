using System;

namespace OF.Core.Interfaces
{
    public interface IUserActivityTracker
    {
        void Start(IOutlookItemsReader reader);
        void Stop();
        void Update(bool isForced);
        void SetLastDate(DateTime? lastDateTime);
    }
}