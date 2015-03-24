using System;
using System.Collections.Generic;
using WSUI.Core.Core.Attributes;
using WSUI.Core.Core.MVVM;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;

namespace WSUI.Core.Data
{
    public class AbstractSearchObject : DataViewModel,ISearchObject
    {
        protected AbstractSearchObject()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id 
        { 
            get;
            private set;
        }

        public virtual string ItemName
        {
            get
            {
                return Get(() => ItemName);
            }
            set { Set(() => ItemName, value); }
        }

        public string EntryID
        {
            get { return Get(() => EntryID); }
            set { Set(() => EntryID, value); }
        }

        public TypeSearchItem TypeItem
        {
            get; set;
        }

        public virtual IEnumerable<ISearchObject> Items
        {
            get{return null;}
        }

        public virtual void AddItem(ISearchObject item)
        {
            
        }

        public void SetDataObject(object data)
        {
            base.SetDataObject(data);
        }

        public object Tag { get; set; }
    }
}