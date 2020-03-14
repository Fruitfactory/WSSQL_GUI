using System;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFEmailSuggesterManager : IDisposable
    {
        void SubscribeMailWindow();

        void UnsubscribeMailWindow();

        void ProcessKeyDown(EventArgs Key);

        void SuggestedEmail(Tuple<IntPtr, string> data);

        bool IsSuggestWindowVisible();
    }
}