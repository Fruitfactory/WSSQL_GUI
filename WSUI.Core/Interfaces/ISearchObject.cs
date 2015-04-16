using System;
using System.Collections.Generic;
using OF.Core.Enums;

namespace OF.Core.Interfaces
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