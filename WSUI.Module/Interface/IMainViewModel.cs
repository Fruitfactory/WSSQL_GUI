using System;
using System.Collections.Generic;
using C4F.DevKit.PreviewHandler.Service;
using WSUI.Infrastructure.Core;

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
    }
}
