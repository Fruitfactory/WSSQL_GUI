using System;
using AddinExpress.OL;

namespace WSUIOutlookPlugin.Interfaces
{
    public interface ISidebarForm : IMainForm
    {
        event EventHandler OnLoaded;
        void Hide();
        void Show();
        void Minimize();
    }
}