///////////////////////////////////////////////////////////
//  EmailContentSearchRule.cs
//  Implementation of the Class EmailContentSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:46 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using WSUI.Core.Core.Rules;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class EmailContentSearchRule : BaseEmailSearchRule 
    {
        private string WhereTemplate =
            "WHERE CONTAINS(System.Kind,'email') AND System.Message.DateReceived < '{0}' AND CONTAINS(*,{1},1033) ORDER BY System.Message.DateReceived DESC";//System.Search.Contents


		public EmailContentSearchRule()
		{
		    Priority = 3;
		}

        public EmailContentSearchRule(object lockObject)
        :base(lockObject)
        {
            Priority = 3;
        }

        //TODO refactore
	    protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
	    {
            var dateString = FormatDate(ref LastDate);
            var and = GetProcessingSearchCriteria();
            return string.Format(WhereTemplate, dateString, and);
	    }

	    public override void Init()
	    {
            RuleName = "EmailContent";
	        base.Init();
	    }
    }//end EmailContentSearchRule

}//end namespace Implements