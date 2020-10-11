using System;
using OF.Infrastructure.Payloads;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFEmailSuggesterManager : IDisposable
    {
        void SubscribeMailWindow();

        void UnsubscribeMailWindow();

        void ProcessKeyDown(OFKeyDownPayload payload);

        void SuggestedEmail(Tuple<IntPtr, string> data);

        bool IsSuggestWindowVisible();
    }
}