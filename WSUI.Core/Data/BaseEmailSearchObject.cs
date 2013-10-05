///////////////////////////////////////////////////////////
//  BaseEmailSearchObject.cs
//  Implementation of the Class BaseEmailSearchObject
//  Generated by Enterprise Architect
//  Created on:      27-Sep-2013 1:33:39 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Globalization;
using WSUI.Core.Core.Attributes;
namespace WSUI.Core.Data 
{
	public class BaseEmailSearchObject : BaseSearchObject 
    {
        [Field("System.Message.ConversationID", 7, false)]
		public string ConversationId{ get;  set;}

        [Field("System.Message.ToAddress", 8, false)]
		public string[] ToAddress{ get;  set;}

        [Field("System.Message.DateReceived", 9, false)]
		public DateTime DateReceived{ get;  set;}

	    public string Recepient
	    {
	        get
	        {
	            return ToAddress != null && ToAddress.Length > 0
	                ? ToAddress[0]
	                : string.Empty;
	        }
	    }

		public BaseEmailSearchObject()
        {

		}

	    public override void SetValue(int index, object value)
	    {
	        base.SetValue(index, value);
	        switch (index)
	        {
	            case 7:
	                ConversationId = value as string;
	                break;
                case 8:
	                ToAddress = value as string[];
	                break;
                case 9:
	                DateReceived = (DateTime) Convert.ChangeType(value, typeof (DateTime), CultureInfo.InvariantCulture);
	                break;
	        }
	    }
    }//end BaseEmailSearchObject

}//end namespace Data