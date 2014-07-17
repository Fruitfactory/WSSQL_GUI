///////////////////////////////////////////////////////////
//  EmailSearchObject.cs
//  Implementation of the Class EmailSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:36 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;

namespace WSUI.Core.Data 
{
	public class EmailSearchObject : BaseEmailSearchObject 
    {
        [Field("System.Subject", 15, false)]    
		public string Subject{ get;  set;}

		public EmailSearchObject()
        {
            TypeItem = TypeSearchItem.Email;
		}

	    public override void SetValue(int index, object value)
	    {
	        base.SetValue(index, value);
	        switch (index)
	        {
	            case 15:
                    Subject = value as string;
                    break;
	        }
	    }
    }//end EmailSearchObject

}//end namespace Data