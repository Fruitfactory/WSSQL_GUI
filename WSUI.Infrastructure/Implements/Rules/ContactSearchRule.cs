///////////////////////////////////////////////////////////
//  ContactSearchRule.cs
//  Implementation of the Class ContactSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:39 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Enums;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class ContactSearchRule : BaseSearchRule<ContactSearchObject>
	{
        private const string WhereTemplate =
            " WHERE CONTAINS(System.Kind,'contact') AND ";
	    private const string NamesTemplate =
            "CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"')  OR CONTAINS(System.Contact.EmailAddress,'\"{0}*\"')" +
            " OR CONTAINS(System.Contact.EmailAddress2,'\"{0}*\"') OR CONTAINS(System.Contact.EmailAddress3,'\"{0}*\"')";

	    private const string CollapseTemplate = "( {0} )";

		public ContactSearchRule()
		{
		    Priority = 1;
		}
        
        public ContactSearchRule(object lockObject)
        :base(lockObject,false)
        {
            Priority = 1;
        }
        

	    protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
	    {
	        string result = string.Empty;
	        if (Query.IndexOf(' ') > -1)
	        {
	            result = WhereTemplate + ProcessAndBuildWhereQueryPart(Query);
	        }
	        else
	        {
	            var template = string.Format(CollapseTemplate, NamesTemplate);
                result = WhereTemplate + string.Format(template, Query);
	        }
	        return result;
	    }

	    private string ProcessAndBuildWhereQueryPart(string query)
	    {
	        StringBuilder strBuid = new StringBuilder();
	        var arr = query.Split(' ').ToList();
	        var temp = string.Format(NamesTemplate, arr[0]);
	        strBuid.Append(string.Format("({0})",temp));
	        foreach (var item in arr.Skip(1))
	        {
	            strBuid.Append(" AND (" + string.Format(NamesTemplate, item) + ")");
	        }
	        return string.Format(CollapseTemplate, strBuid.ToString());
	    }

        public override void Init()
        {
            RuleName = "Contact";
            CountFirstProcess = 100;
            CountSecondProcess = 50;
            ObjectType = RuleObjectType.Contact;
            base.Init();
        }

	}//end ContactSearchRule

}//end namespace Implements