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
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Base;
using OF.Core.Data.ElasticSearch.Request.Contact;
using OF.Core.Enums;

namespace OF.Infrastructure.Implements.Rules 
{
	public class OFContactSearchRule : BaseSearchRule<OFContactSearchObject,OFContact>
	{
        public OFContactSearchRule(IUnityContainer container)
            :this(null,container)
		{
		    Priority = 1;
		}

        public OFContactSearchRule(object lockObject, IUnityContainer container)
        :base(lockObject,false,container)
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

	    protected override OFBody GetSearchBody()
	    {
            var preparedCriterias = GetKeywordsList();
	        var body = new OFBody();
            
            if (preparedCriterias.Count > 1)
            {

                var queries = new OFQueryBoolConditions();
                body.query = queries;
                foreach (var preparedCriteria in preparedCriterias)
                {

                    var should = new OFQueryBoolShould<OFBaseTerm>();

                    var fn = new OFFirstNameTerm();
                    fn.SetValue(preparedCriteria);

                    var ln = new OFLastNameTerm();
                    ln.SetValue(preparedCriteria);
                    var ea11 = new OFEmailaddress1Term();
                    ea11.SetValue(preparedCriteria);
                    var ea22 = new OFEmailaddress2Term();
                    ea22.SetValue(preparedCriteria);
                    var ea33 = new OFEmailaddress3Term();
                    ea33.SetValue(preparedCriteria);

                    should._bool.should.Add(fn);
                    should._bool.should.Add(ln);
                    should._bool.should.Add(ea11);
                    should._bool.should.Add(ea22);
                    should._bool.should.Add(ea33);

                    var must = new OFMustCondition<object>(){Value = should};

                    queries._bool.Add(must);
                }
                return body;
            }
            var query = new OFQueryBoolShould<OFBaseTerm>();
            body.query = query;
            var firstName = new OFFirstNameTerm();
            firstName.SetValue(Query);
	        var lastName = new OFLastNameTerm();
            lastName.SetValue(Query);
	        var ea1 = new OFEmailaddress1Term();
            ea1.SetValue(Query);
            var ea2 = new OFEmailaddress2Term();
            ea2.SetValue(Query);
            var ea3 = new OFEmailaddress3Term();
            ea3.SetValue(Query);
            query._bool.should.Add(firstName);
            query._bool.should.Add(lastName);
            query._bool.should.Add(ea1);
            query._bool.should.Add(ea2);
            query._bool.should.Add(ea3);
	        return body;
	    }
	}//end ContactSearchRule

}//end namespace Implements