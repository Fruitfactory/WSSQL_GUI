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
            AddRule(new GeneralContactRule(75,0,Lock1));
            AddRule(new EmailSubjectSearchRule(Lock1));
            AddRule(new EmailContentSearchRule(Lock1));
            AddRule(new FileFilenameSearchRule(Lock1));
            AddRule(new FileContentSearchRule(Lock1));
            AddRule(new BaseAttachmentSearchRule(Lock1));
	        base.Init();
	    }
    }//end AllSearchSystem

}//end namespace Implements