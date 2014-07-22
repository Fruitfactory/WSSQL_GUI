using System;
using AddinExpress.OL;
using WSUI.Core.Enums;

namespace WSUIOutlookPlugin.Interfaces
{
    public interface ISidebarForm : IMainForm
    {
        void Hide();
        void Show();
        void Minimize();
        void SendAction(WSActionType actionType);
    }
}