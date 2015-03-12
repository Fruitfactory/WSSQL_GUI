using System;
using System.Collections.Generic;
using WSUI.Core.Enums;

namespace WSUI.Core.Interfaces
{
    public interface ISearchObject
    {
        Guid Id { get; }

        string ItemName { get; set; }
        string EntryID { get; set; }

        TypeSearchItem TypeItem { get; set; }

        IEnumerable<ISearchObject> Items { get; }

        void AddItem(ISearchObject item);

        void SetDataObject(object data);
    }
}