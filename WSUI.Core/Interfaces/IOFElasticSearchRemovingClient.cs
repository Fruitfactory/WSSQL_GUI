using System;

namespace OF.Core.Interfaces
{
    public interface IOFElasticSearchRemovingClient : IDisposable
    {
        void RemoveEmail(string entryId);
    }
}