///////////////////////////////////////////////////////////
//  AllSearchSystem.cs
//  Implementation of the Class AllSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:41 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using OF.Infrastructure.Implements.Rules;
using OF.Infrastructure.Implements.Systems.Core;

namespace OF.Infrastructure.Implements.Systems 
{
	public class AllSearchSystem : BaseAllEmailSearchSystem
    {

		public AllSearchSystem()
        {

		}

	    public override void Init()
	    {
            AddRule(new GeneralContactRule(100, 0, Lock1));
            AddRule(new EmailSubjectSearchRule(Lock1));
            AddRule(new EmailContentSearchRule(Lock1));
            AddRule(new AttachmentFilenameSearchRule(Lock1));
            AddRule(new AttachmentContentSearchRule(Lock1));
	        base.Init();
	    }


    }//end AllSearchSystem

}//end namespace Implements