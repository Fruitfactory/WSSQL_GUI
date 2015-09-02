using System;
using OF.Core.Enums;

namespace OF.Module.Interface.Service
{
    public interface IAttachmentReader
    {
        PstReaderStatus Status { get; }
        void Start(DateTime? lastUpdate);
        void Stop();
    }
}