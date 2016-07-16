///////////////////////////////////////////////////////////
//  EmailSearchSystem.cs
//  Implementation of the Class EmailSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:47 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using Microsoft.Practices.Unity;
using OF.Infrastructure.Implements.Rules;
using OF.Infrastructure.Implements.Systems.Core;

namespace OF.Infrastructure.Implements.Systems
{
    public class OFEmailSearchSystem : OFBaseAllEmailSearchSystem
    {
        public OFEmailSearchSystem()
        {
		}

        public override void Init(IUnityContainer container)
	    {
            //AddRule(new OFEmailSubjectSearchRule(container));
            AddRule(new OFEmailContentSearchRule(container));
	        base.Init(container);
	    }


    }//end EmailSearchSystem

}//end namespace Implements