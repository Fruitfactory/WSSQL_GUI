///////////////////////////////////////////////////////////
//  AttachmentSearchObject.cs
//  Implementation of the Class AttachmentSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:31 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Globalization;
using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;

namespace WSUI.Core.Data 
{
	public class AttachmentSearchObject : BaseEmailSearchObject 
    {
        [Field("System.DateModified", 15, false)]
		public DateTime DateModified{ get;  set;} 

		public AttachmentSearchObject()
        {
            TypeItem = TypeSearchItem.Attachment;
		}

	    public override void SetValue(int index, object value)
	    {
	        base.SetValue(index, value);
	        switch (index)
	        {
                case 15:
	                DateModified = (DateTime) Convert.ChangeType(value, typeof (DateTime), CultureInfo.InvariantCulture);
	                break;
	        }
	    }
    }//end AttachmentSearchObject

}//end namespace Data