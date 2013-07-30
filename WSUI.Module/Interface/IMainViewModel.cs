using System;
using System.Collections.Generic;
using WSPreview.PreviewHandler.Service;
using WSUI.Core.Enums;
using WSUI.Infrastructure.Core;
using WSUI.Module.Core;

namespace WSUI.Module.Interface
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
        List<BaseSearchData> MainDataSource { get; }
        void Clear();
        void SelectKind(string name);
        void PassActionForPreview(WSActionType actionType);
        HostType Host { get; }
    }
}
