using System;
using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface IOutlookItemsReader
    {
        PstReaderStatus Status { get; }
        void Start(DateTime? lastUpdate);
        void Stop();

        void Suspend();

        void Resume(DateTime? lastUpdated);

        void Join();

        void Close();

        bool IsSuspended { get; }

        bool IsStarted { get; }


        int Count { get; }

        void Attach(IOFIOutlookItemsReaderObserver observer);

        string Folder { get; }

    }
}