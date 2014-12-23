///////////////////////////////////////////////////////////
//  EmailSubjectSearchRule.cs
//  Implementation of the Class EmailSubjectSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:48 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using WSUI.Core.Core.Rules;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class EmailSubjectSearchRule : BaseEmailSearchRule
	{

	    private string WhereTemplate =
            "WHERE CONTAINS(System.Kind,'email') AND System.Message.DateReceived < '{0}' AND CONTAINS(System.Subject,{1},1033) ORDER BY System.Message.DateReceived DESC";

        private string WhereAdvancedTemplate = "WHERE CONTAINS(System.Kind,'email') AND System.Message.DateReceived < '{0}' AND {1} ORDER BY System.Message.DateReceived DESC";

		public EmailSubjectSearchRule()
		{
		    Priority = 2;
		}

        public EmailSubjectSearchRule(object lockObject)
            :base(lockObject)
        {
            Priority = 2;
        }

        protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            var dateString = FormatDate(ref LastDate);
            var and = GetProcessingSearchCriteria(listCriterisRules).Item1;
            return string.Format(WhereTemplate, dateString, and);
        }

	    protected override string OnGenerateAdvancedWherePart(string advancedCriteria)
	    {
            var dateString = FormatDate(ref LastDate);
	        var advancedPart = ProcessAdvancedCriteria(advancedCriteria);
            return string.Format(WhereAdvancedTemplate, dateString,advancedPart);
	    }

	    private string ProcessAdvancedCriteria(string advancedCriteria)
	    {
	        var arr = advancedCriteria.Split(' ');
	        if (arr.Length == 0)
	            return string.Empty;
	        var criterias = new List<string>();
	        foreach (var s in arr)
	        {
	            var temp = string.Empty;
	            if (s.Contains("to:"))
	            {
	                temp = s.Replace("to:", "");
	                temp = RemoveBracket(temp);
                    criterias.Add(string.Format("(CONTAINS(System.Message.ToAddress,'\"{0}*\"',1033) OR CONTAINS(System.Message.ToName,'\"{0}*\"',1033))", temp));
	            }
                else if (s.Contains("folder:"))
                {
                    temp = s.Replace("folder:", "");
                    temp = RemoveBracket(temp);
                    criterias.Add(string.Format("CONTAINS(System.ItemUrl,'\"{0}*\"',1033)", temp));
                }
                else if (s.Contains("body:"))
                {
                    temp = s.Replace("body:", "");
                    temp = RemoveBracket(temp);
                    criterias.Add(string.Format("CONTAINS(*,'\"{0}*\"',1033)", temp));
                }
	        }

	        return string.Join(" AND ", criterias);
	    }

	    private string RemoveBracket(string str)
	    {
	        var temp = str.Replace("(", "").Replace(")", "");
	        return temp;
	    }

	    protected override bool GetIncludedInAdvancedMode()
	    {
	        return true;
	    }

	    public override void Init()
	    {
            RuleName = "EmailSubject";
            base.Init();
	    }
	}//end EmailSubjectSearchRule

}//end namespace Implements