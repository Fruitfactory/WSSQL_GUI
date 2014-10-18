using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Logger;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class EmailContactSearchRule : BaseSearchRule<EmailContactSearchObject>
    {
        private const string WhereTemplate = " WHERE CONTAINS(System.Kind,'email') AND ";

        private const string NamesTemplate =
            "(CONTAINS(System.Message.SenderName,'\"{0}*\"') OR CONTAINS(System.Message.SenderAddress,'\"{0}*\"') OR CONTAINS(System.Message.FromName,'\"{0}*\"') OR CONTAINS(System.Message.FromAddress,'\"{0}*\"') OR CONTAINS(System.Message.CcName,'\"{0}*\"') OR CONTAINS(System.Message.CcAddress,'\"{0}*\"') OR CONTAINS(System.Message.ToAddress,'\"{0}*\"') OR CONTAINS(System.Message.ToName,'\"{0}*\"') OR CONTAINS(System.Search.Contents,'\"{0}*\"') )";
        private const string CollapseTemplate = "( {0} )";
        private const string DateTemplate = " AND System.Message.DateReceived < '{0}' ORDER BY System.Message.DateReceived DESC";
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        private readonly List<string> _listEmails = new List<string>(); 

        public EmailContactSearchRule()
        {
            Priority = 0;
        }

        public EmailContactSearchRule(object lockObject)
        :base(lockObject,false)
        {
            Priority = 0;
        }

        protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            string result = string.Empty;
            var date = FormatDate(ref LastDate);
            string dateTemplate = string.Format(DateTemplate, date);
            if (Query.IndexOf(' ') > -1)
            {
                result = WhereTemplate + ProcessAndBuildWhereQueryPart(Query) + dateTemplate;
            }
            else
            {
                result = WhereTemplate + "(" + string.Format(NamesTemplate, Query) + ")" + dateTemplate;
            }
            return result;
        }

        public override void Init()
        {
            RuleName = "EmailContact";
            CountFirstProcess = 150;
            CountSecondProcess = 50;
            base.Init();
        }

        private string ProcessAndBuildWhereQueryPart(string query)
        {
            StringBuilder strBuid = new StringBuilder();
            var arr = query.Split(' ').ToList();
            var temp = string.Format(NamesTemplate, arr[0]);
            strBuid.Append(string.Format("({0})", temp));
            foreach (var item in arr.Skip(1))
            {
                strBuid.Append(" AND (" + string.Format(NamesTemplate, item)+")");
            }
            return string.Format(CollapseTemplate, strBuid.ToString());
        }

        public override void Reset()
        {
            _listEmails.Clear();
            base.Reset();
        }

        protected override void ProcessResult()
        {
            var groups = Result.GroupBy(i => i.ConversationId);
            var result = new List<EmailContactSearchObject>();
            foreach (var group in groups)
            {
                var item = group.First();
                var arr = SplitSearchCriteria(Query);
                string email =
                    GetEmailAddress(item.SenderName, item.SenderAddress, arr, ref item)
                    ?? GetEmailAddress(item.FromName, item.FromAddress, arr, ref item)
                    ?? GetEmailAddress(item.ToName, item.ToAddress, arr, ref item)
                    ?? GetEmailAddress(item.CcName,item.CcAddress, arr,ref item);
                if(string.IsNullOrEmpty(email) || _listEmails.Contains(email))
                    continue;
                _listEmails.Add(email);
                item.EMail = email;
                result.Add(item);
            }
            if (Result.Any())
            {
                LastDate = Result.Last().DateReceived;
            }
            Result.Clear();
            if (result.Count > 0)
            {
                Result = result;
            }
        }

        private string[] SplitSearchCriteria(string searchCriteria)
        {
            return searchCriteria.Trim().Split(' ');
        }

        private string GetEmailAddress(string[] from, string[] searchCriteria)
        {
            if (from == null)
                return null;
            string fromAddress = null;
            foreach (var email in from)
            {
                if (searchCriteria.All(str => email.IndexOf(str, StringComparison.InvariantCultureIgnoreCase) > -1) && Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
                {
                    fromAddress = email;
                    break;
                }
            }
            return fromAddress;
        }

        private string GetEmailAddress(string[] names, string[] emails, string[] searchCriteria, ref EmailContactSearchObject item)
        {
            if (names == null || names.Length == 0)
            {
                return GetEmailAddress(emails, searchCriteria);
            }
            string contactName = names.FirstOrDefault(n => searchCriteria.All(s => n.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1));
            if (string.IsNullOrEmpty(contactName))
            {
                return GetEmailAddress(emails, searchCriteria);
            }
            else
            {
                int index = Array.IndexOf(names, contactName);
                if (index < emails.Length)
                {
                    item.ContactName = contactName;
                    return emails[index];
                }
            }
            return GetEmailAddress(emails, searchCriteria);
        }
    }
}