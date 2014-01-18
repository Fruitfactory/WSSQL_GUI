///////////////////////////////////////////////////////////
//  ContactSearchSystem.cs
//  Implementation of the Class ContactSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:45 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////




using WSUI.Core.Core.Search;
using WSUI.Infrastructure.Implements.Rules;

namespace WSUI.Infrastructure.Implements.Systems 
{
	public class ContactSearchSystem : BaseSearchSystem 
    {

		public ContactSearchSystem()
        {
            
		}

	    public override void Init()
	    {
            AddRule(new GeneralContactRule(200,75));
	        base.Init();
	    }
    }//end ContactSearchSystem

}//end namespace Implements