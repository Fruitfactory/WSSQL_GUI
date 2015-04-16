﻿using System;
using AddinExpress.OL;
using OF.Core.Enums;

namespace OFOutlookPlugin.Interfaces
{
    public interface ISidebarForm : IMainForm
    {
        void Hide();
        void Show();
        void Minimize();
        void SendAction(WSActionType actionType);
        bool IsDisposed { get;}
    }
}