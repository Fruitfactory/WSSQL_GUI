using System;
using System.Collections.Generic;
using System.Text;
using WSUI.Infrastructure.Services;


namespace WSUI.Infrastructure.Core
{
    internal interface IDataView
    {
        bool IsLoading { get; set; }
        void StartLoading();
        void FinishLoading();

        event EventHandler<EventArgs<BaseSearchData>> SelectedItemChanged;
//        void SetContextMenu(ContextMenuStrip contextMenu);
        void SetData(object[] value, BaseSearchData data);
        void SetData(IList<BaseSearchData> listData);
        void Clear();

    }
}
