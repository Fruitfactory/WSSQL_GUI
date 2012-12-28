using System;
using System.Collections.Generic;
using WSUI.Infrastructure.Core;

namespace WSUI.Module.Interface
{
    public interface IMainViewModel
    {
        event EventHandler Start;
        event EventHandler Complete;
        List<BaseSearchData> MainDataSource { get; }
        void Clear();
    }
}
