///////////////////////////////////////////////////////////
//  BaseSearchObject.cs
//  Implementation of the Class BaseSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:26 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using OF.Core.Core.Attributes;
using OF.Core.Core.MVVM;
using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Core.Data
{
    public class OFBaseSearchObject : OFAbstractSearchObject
    {
        #region [needs]

        private const int IncludeParentCount = 1;
        private readonly IList<ISearchObject> _internaList = new List<ISearchObject>();

        #endregion [needs]

        

        public virtual string ItemUrl
        {
            get
            {
                return Get(() => ItemUrl);
            }
            set { Set(() => ItemUrl, value); }
        }

        public string[] Kind { get; set; }

        public DateTime? DateCreated
        {
            get { return Get(() => DateCreated); }
            set { Set(() => DateCreated, value); }
        }

        public virtual string ItemNameDisplay
        {
            get { return Get(() => ItemNameDisplay); }
            set { Set(() => ItemNameDisplay, value); }
        }

        public long Size {
            get
            {
                return Get(() => Size);
            }
            set
            {
              Set(() => Size, value);  
            } 
        }

        public OFBaseSearchObject()
            :base()
        {
        }

        public override IEnumerable<ISearchObject> Items
        {
            get { return _internaList; }
        }

        public string Count
        {
            get { return Items.Any() ? (Items.Count() + IncludeParentCount).ToString(CultureInfo.InvariantCulture) : 0.ToString(); }
        }

        public override void AddItem(ISearchObject item)
        {
            if (!_internaList.Contains(item))
                _internaList.Add(item);
        }

        

        public override string ToString()
        {
            return string.Format("{0}:{1}", ItemName, Id.ToString());
        }
    }//end BaseSearchObject
}//end namespace Data