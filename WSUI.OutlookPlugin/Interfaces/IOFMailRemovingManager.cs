using System;

namespace OFOutlookPlugin.Interfaces
{
    public interface IOFMailRemovingManager : IDisposable
    {
        void Initialize();

        void RemoveMail(Microsoft.Office.Interop.Outlook.MailItem Mail);

        void ConnectTo(Microsoft.Office.Interop.Outlook.MailItem Mail);

        void ConnectTo(Microsoft.Office.Interop.Outlook.Folder Folder);

        void RemoveConnection();
    }
}