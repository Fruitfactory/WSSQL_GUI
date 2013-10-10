///////////////////////////////////////////////////////////
//  AllSearchSystem.cs
//  Implementation of the Class AllSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:41 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////




using WSUI.Core.Core.Search;
using WSUI.Infrastructure.Implements.Rules;

namespace WSUI.Infrastructure.Implements.Systems 
{
	public class AllSearchSystem : BaseSearchSystem 
    {

		public AllSearchSystem()
        {

		}

	    public override void Init()
	    {
            AddRule(new EmailContactSearchRule());
            AddRule(new ContactSearchRule());
            AddRule(new EmailSubjectSearchRule());
            AddRule(new EmailContentSearchRule());
            AddRule(new FileFilenameSearchRule());
            AddRule(new FileContentSearchRule());
            AddRule(new AttachmentFilenameSearchRule());
	        base.Init();
	    }
    }//end AllSearchSystem

}//end namespace Implements