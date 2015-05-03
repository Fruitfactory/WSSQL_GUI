///////////////////////////////////////////////////////////
//  EmailContentSearchRule.cs
//  Implementation of the Class EmailContentSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:46 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OF.Core.Core.Rules;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Email;
using OF.Infrastructure.Implements.Rules.BaseRules;

namespace OF.Infrastructure.Implements.Rules 
{
	public class EmailContentSearchRule : BaseEmailSearchRule 
    {

		public EmailContentSearchRule()
		{
		    Priority = 3;
		}

        public EmailContentSearchRule(object lockObject)
        :base(lockObject)
        {
            Priority = 3;
        }

	    public override void Init()
	    {
            RuleName = "EmailContent";
	        base.Init();
	    }

        protected override OFBody GetSearchBody()
        {
            var preparedCriterias = GetKeywordsList();
            var body = new OFBodySort();
            body.sort = new OFSortDateCreated();
            if (preparedCriterias.Count > 1)
            {
                var query = new OFQueryBoolMust<OFTerm<OFSimpleContentTerm>>();
                body.query = query;
                foreach (var preparedCriteria in preparedCriterias)
                {
                    var term = new OFTerm<OFSimpleContentTerm>(preparedCriteria);
                    query._bool.must.Add(term);
                }
                return body;
            }
            body.query = new OFQuerySimpleTerm<OFSimpleContentTerm>(Query);
            return body;
        }

	    protected override Expression<Func<OFEmail, string>> GetSearchedProperty()
	    {
	        return e => e.Analyzedcontent;
	    }
    }//end EmailContentSearchRule

}//end namespace Implements