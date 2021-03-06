///////////////////////////////////////////////////////////
//  AllSearchSystem.cs
//  Implementation of the Class AllSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:41 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using Microsoft.Practices.Unity;
using OF.Infrastructure.Implements.Rules;
using OF.Infrastructure.Implements.Systems.Core;

namespace OF.Infrastructure.Implements.Systems 
{
	public class OFAllSearchSystem : OFBaseAllEmailSearchSystem
    {

		public OFAllSearchSystem()
        {

		}

        public override void Init(IUnityContainer container)
	    {
            AddRule(new OFGeneralContactRule(100, 0, Lock1,container));
            AddRule(new OFEmailSubjectSearchRule(Lock1,container));
            AddRule(new OFEmailContentSearchRule(Lock1,container));
            AddRule(new OFAttachmentFilenameSearchRule(Lock1,container));
            AddRule(new OFAttachmentContentSearchRule(Lock1,container));
	        base.Init(container);
	    }


    }//end AllSearchSystem

}//end namespace Implements