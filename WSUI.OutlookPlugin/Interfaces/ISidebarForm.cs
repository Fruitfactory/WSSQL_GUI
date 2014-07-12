using System;
using AddinExpress.OL;
using WSUI.Core.Enums;

namespace WSUIOutlookPlugin.Interfaces
{
    public interface ISidebarForm : IMainForm
    {
        event EventHandler OnLoaded;
        void Hide();
        void Show();
        void Minimize();
        void SendAction(WSActionType actionType);
    }
}