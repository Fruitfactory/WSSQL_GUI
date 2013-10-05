using System;
using System.Collections.Generic;
using WSPreview.PreviewHandler.Service;
using WSUI.Core.Core;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;

namespace WSUI.Module.Interface
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
        List<BaseSearchObject> MainDataSource { get; }
        void Clear();
        void SelectKind(string name);
        void PassAction(IWSAction action);
        HostType Host { get; }
        void ForceClosePreview();
    }
}
