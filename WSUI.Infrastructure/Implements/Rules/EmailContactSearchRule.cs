using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Logger;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class EmailContactSearchRule : BaseSearchRule<EmailContactSearchObject,WSUIStub>
    {
        
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

        public override void Init()
        {
            RuleName = "EmailContact";
            CountFirstProcess = 150;
            CountSecondProcess = 50;
            base.Init();
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
                string email = "";
                    //GetEmailAddress(item.SenderName, item.SenderAddress, arr, ref item)
                    //?? GetEmailAddress(item.FromName, item.FromAddress, arr, ref item)
                    //?? GetEmailAddress(item.ToName, item.ToAddress, arr, ref item)
                    //?? GetEmailAddress(item.CcName,item.CcAddress, arr,ref item);
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

    }
}