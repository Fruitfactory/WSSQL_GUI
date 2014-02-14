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
using System.IO.IsolatedStorage;
using System.Linq;
using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;

namespace WSUI.Core.Data 
{
	public class BaseSearchObject :  ISearchObject
    {
        #region [needs]
        private const int IncludeParentCount = 1; 
	    private readonly IList<ISearchObject> _internaList = new List<ISearchObject>();

        #endregion


        [Field("System.ItemName", 1, false)]
		public string ItemName{ get;  set;}

	    [Field("System.ItemUrl", 2, false)]
		public string ItemUrl{ get;  set;}

        [Field("System.Kind", 3, false)]
		public string[] Kind{ get;  set;}

        [Field("System.DateCreated", 4, false)]
		public DateTime DateCreated{ get;  set;}

        [Field("System.ItemNameDisplay", 5, false)]
		public string ItemNameDisplay{ get;  set;}

        [Field("System.Size", 6, false)]
		public int Size{ get;  set;} 

		public Guid Id {get;private set;}
		
		protected BaseSearchObject()
        {
			Id = Guid.NewGuid();
		}

	    public virtual void SetValue(int index, object value)
	    {
	        switch (index)
	        {
	            case 1:
	                ItemName = value as string;
	                break;
                case 2:
	                ItemUrl = value as string;
	                break;
                case 3:
	                Kind = value as string[];
	                break;
                case 4:
	                DateCreated = (DateTime)Convert.ChangeType(value, typeof (DateTime), CultureInfo.InvariantCulture);
	                break;
                case 5:
	                ItemNameDisplay = value as string;
	                break;
                case 6:
	                Size = (int)Convert.ChangeType(value,typeof(int),CultureInfo.InvariantCulture);
	                break;
	        }
	    }

	    public IEnumerable<ISearchObject> Items
	    {
	        get { return _internaList; }
	    }

	    public string Count
	    {
	        get { return Items.Any() ? (Items.Count() + IncludeParentCount).ToString(CultureInfo.InvariantCulture) : 0.ToString(); }
	    }

	    public void AddItem(ISearchObject item)
	    {
	        if (!_internaList.Contains(item)) 
                _internaList.Add(item);
	    }

	    public TypeSearchItem TypeItem {get; set;}

        public object Tag { get; set; }

        public override string ToString()
        {
            return string.Format("{0}:{1}",ItemName,Id.ToString());
        }
    }//end BaseSearchObject

}//end namespace Data