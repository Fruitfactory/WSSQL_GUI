///////////////////////////////////////////////////////////
//  ContactSearchRule.cs
//  Implementation of the Class ContactSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:39 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;

namespace OF.Infrastructure.Implements.Rules 
{
	public class ContactSearchRule : BaseSearchRule<ContactSearchObject,OFContact>
	{
        public ContactSearchRule()
		{
		    Priority = 1;
		}
        
        public ContactSearchRule(object lockObject)
        :base(lockObject,false)
        {
            Priority = 1;
        }
        
        public override void Init()
        {
            RuleName = "Contact";
            CountFirstProcess = 100;
            CountSecondProcess = 50;
            ObjectType = RuleObjectType.Contact;
            base.Init();
        }

	    protected override bool NeedSorting
	    {
	        get { return false; }
	    }

	    protected override QueryContainer BuildQuery(QueryDescriptor<OFContact> queryDescriptor)
	    {
	        var preparedCriterias = GetKeywordsList();
            if (preparedCriterias.Count > 1)
            {

                var list = new List<Func<QueryDescriptor<OFContact>, QueryContainer>>();

                foreach (var criteria in preparedCriterias)
                {
                    list.Add(descriptor => descriptor.Term(c => c.Firstname, criteria));
                    list.Add(descriptor => descriptor.Term(c => c.Lastname, criteria));
                    list.Add(descriptor => descriptor.Term(c => c.Emailaddress1, criteria));
                    list.Add(descriptor => descriptor.Term(c => c.Emailaddress2, criteria));
                    list.Add(descriptor => descriptor.Term(c => c.Emailaddress3, criteria));
                }

                return queryDescriptor.Bool(bd => bd.Should(list.ToArray()));
            }
	        return queryDescriptor.Bool(bd => bd.Should(
                qd => qd.Term(c => c.Firstname,Query),
                qd => qd.Term(c => c.Lastname,Query),
                qd => qd.Term(c => c.Emailaddress1,Query),
                qd => qd.Term(c => c.Emailaddress2,Query),
                qd => qd.Term(c => c.Emailaddress3,Query)
	            ));
	    }
	}//end ContactSearchRule

}//end namespace Implements