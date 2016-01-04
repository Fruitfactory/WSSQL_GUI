using System;
using OF.Core.Enums;

namespace OF.Infrastructure.Implements.Service
{
    public interface IOutlookItemsReader
    {
        PstReaderStatus Status { get; }
        void Start(DateTime? lastUpdate);
        void Stop();

        void Suspend();

        void Resume(DateTime? lastUpdated);

        void Close();

        bool IsSuspended { get; }

        bool IsStarted { get; }


        int Count { get; }

    }
}