///////////////////////////////////////////////////////////
//  EmailSearchSystem.cs
//  Implementation of the Class EmailSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:47 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using OF.Core.Core.Search;
using OF.Infrastructure.Implements.Rules;
using OF.Infrastructure.Implements.Systems.Core;

namespace OF.Infrastructure.Implements.Systems
{
    public class EmailSearchSystem : BaseAllEmailSearchSystem
    {
        public EmailSearchSystem()
        {
		}

	    public override void Init()
	    {
            AddRule(new EmailSubjectSearchRule());
            AddRule(new EmailContentSearchRule());
	        base.Init();
	    }


    }//end EmailSearchSystem

}//end namespace Implements