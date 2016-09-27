using System;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFEmailSuggesterManager : IDisposable
    {
        void SubscribeMailWindow();
        void UnsubscribeMailWindow();
        //void ProcessKeyDown(int VirtualKey);
        void ProcessKeyDown(int Key);
    }
}