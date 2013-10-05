///////////////////////////////////////////////////////////
//  EmailSubjectSearchRule.cs
//  Implementation of the Class EmailSubjectSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:48 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;

namespace WSUI.Infrastructure.Implements 
{
	public class EmailSubjectSearchRule : BaseEmailSearchRule
	{

	    private string WhereTemplate =
            "WHERE System.Kind = 'email' AND System.Message.DateReceived < '{0}' AND CONTAINS(System.Subject,{1}) ORDER BY System.Message.DateReceived DESC";

		public EmailSubjectSearchRule()
		{
		    Priority = 2;
		}

        protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            var dateString = FormatDate(ref LastDate);
            var and = GetProcessingSearchSriteria(listCriterisRules);
            return string.Format(WhereTemplate, dateString, and);
        }

	}//end EmailSubjectSearchRule

}//end namespace Implements