using System;
using OF.Core.Enums;

namespace OF.Infrastructure.Implements.Service
{
    public interface IAttachmentReader
    {
        PstReaderStatus Status { get; }
        void Start(DateTime? lastUpdate);
        void Stop();

        void Suspend();

        void Resume();

        void Close();

        bool IsSuspended { get; }

        int Count { get; }

    }
}