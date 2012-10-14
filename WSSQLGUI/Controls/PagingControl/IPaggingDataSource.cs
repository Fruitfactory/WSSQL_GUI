using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WSSQLGUI.Controls.PagingControl
{
    interface IPaggingDataSource<T>
    {
        bool IsLoading { get; }
        void CompleteLoading();
        void StartLoading();


        void AddData(T data);
        void AddData(List<T> data);
        T Selected { get; }
        List<T> VisibleItems { get; }
        List<T> AllItems { get; }
        int CurrentPage { get; }
        void MoveLeft();
        void MoveRight();
        void MoveToFirst();
        void MoveToLast();

        void AddColumn(DataGridViewColumn col);
        int ColumnCount { get; }
        void DeleteColumn(int index);
        void DeleteColumnAll();
        void ClearRows();
    }
}
