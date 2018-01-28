using System;
using AddinExpress.MSO;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFEmailSuggesterManager : IDisposable
    {
        void SubscribeMailWindow();

        void UnsubscribeMailWindow();

        void ProcessKeyDown(ADXKeyDownEventArgs Key);

        void SuggestedEmail(Tuple<IntPtr, string> data);

        bool IsSuggestWindowVisible();
    }
}