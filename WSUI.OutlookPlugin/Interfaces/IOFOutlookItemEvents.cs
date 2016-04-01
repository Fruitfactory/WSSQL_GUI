using System;
using AddinExpress.MSO;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFOutlookItemEvents : IDisposable
    {
        void ConnectTo(Microsoft.Office.Interop.Outlook.MailItem Mail);
    }
}