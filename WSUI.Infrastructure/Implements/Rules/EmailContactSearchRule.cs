using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class EmailContactSearchRule : BaseSearchRule<EmailContactSearchObject>
    {
        private const string WhereTemplate = " WHERE System.Kind = 'email' AND ";

        private const string NamesTemplate = "(CONTAINS(System.Message.FromAddress,'\"{0}*\"') OR CONTAINS(System.Message.CcAddress,'\"{0}*\"') OR CONTAINS(System.Message.ToAddress,'\"{0}*\"') OR CONTAINS(System.Search.Contents,'\"{0}*\"'))";
        private const string CollapseTemplate = "( {0} )";

        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        private readonly List<string> _listEmails = new List<string>(); 

        public EmailContactSearchRule()
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
                result = WhereTemplate + string.Format(NamesTemplate, Query);
            }
            return result;
        }

        public override void Init()
        {
            RuleName = "EmailContact";
            base.Init();
        }

        private string ProcessAndBuildWhereQueryPart(string query)
        {
            StringBuilder strBuid = new StringBuilder();
            var arr = query.Split(' ').ToList();
            strBuid.Append(string.Format(NamesTemplate, arr[0]));
            foreach (var item in arr.Skip(1))
            {
                strBuid.Append(" AND " + string.Format(NamesTemplate, item));
            }
            return string.Format(CollapseTemplate, strBuid.ToString());
        }

        protected override void ProcessResult()
        {
            var groups = Result.GroupBy(i => i.ConversationId);
            var result = new List<EmailContactSearchObject>();
            foreach (var group in groups)
            {
                var item = group.First();
                string email = GetEmailAddress(item.FromAddress, Query) 
                    ?? GetEmailAddress(item.CcAddress, Query) 
                    ?? GetEmailAddress(item.ToAddress, Query);
                if(string.IsNullOrEmpty(email) || _listEmails.Contains(email))
                    continue;
                _listEmails.Add(email);
                item.EMail = email;
                result.Add(item);
            }
            Result.Clear();
            if (result.Count > 0)
            {
                Result = result;
            }
            _listEmails.Clear();
        }

        private string GetEmailAddress(string[] from, string searchCriteria)
        {
            string fromAddress = null;
            if (from != null)
            {
                var arr = searchCriteria.Trim().Split(' ');
                if (arr != null && arr.Length > 0)
                {
                    foreach (var s in arr)
                    {
                        fromAddress =
                        from.FirstOrDefault(
                            str =>
                            str.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1 &&
                            Regex.IsMatch(str, EmailPattern, RegexOptions.IgnoreCase));
                        if (!string.IsNullOrEmpty(fromAddress))
                            break;
                    }
                }
                else
                    fromAddress =
                        from.FirstOrDefault(
                            str =>
                            str.IndexOf(searchCriteria.Trim(), StringComparison.CurrentCultureIgnoreCase) > -1 &&
                            Regex.IsMatch(str, EmailPattern, RegexOptions.IgnoreCase));
            }
            return fromAddress;
        }


    }
}