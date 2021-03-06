///////////////////////////////////////////////////////////
//  ContactSearchSystem.cs
//  Implementation of the Class ContactSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:45 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using Microsoft.Practices.Unity;
using OF.Core.Core.Search;
using OF.Infrastructure.Implements.Rules;

namespace OF.Infrastructure.Implements.Systems 
{
	public class OFContactSearchSystem : OFBaseSearchSystem 
    {

		public OFContactSearchSystem()
        {
            
		}

        public override void Init(IUnityContainer container)
	    {
            AddRule(new OFGeneralContactRule(750,75,container));
	        base.Init(container);
	    }
    }//end ContactSearchSystem

}//end namespace Implements