using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Extensions;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.Rules
{
    public class EmailContactSearchRule : BaseSearchRule<EmailContactSearchObject,OFEmail>
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
            CountFirstProcess = 100;
            CountSecondProcess = 50;
            base.Init();
        }


        public override void Reset()
        {
            _listEmails.Clear();
            base.Reset();
        }

        protected override QueryContainer BuildQuery(QueryDescriptor<OFEmail> queryDescriptor)
        {
            var preparedCriterias = GetProcessingSearchCriteria();

            if (preparedCriterias.Count > 1)
            {

                var list = new List<Func<QueryDescriptor<OFEmail>, QueryContainer>>();

                foreach (var criteria in preparedCriterias)
                {
                    list.Add(descriptor => descriptor.Term("to.name", criteria));
                    list.Add(descriptor => descriptor.Term("to.address", criteria));
                    list.Add(descriptor => descriptor.Term("cc.name", criteria));
                    list.Add(descriptor => descriptor.Term("cc.address", criteria));
                    list.Add(descriptor => descriptor.Term("fromname", criteria));
                    list.Add(descriptor => descriptor.Term("fromaddress", criteria));

                }
                return queryDescriptor.Bool(bd => bd.Should(list.ToArray()));
            }
            return queryDescriptor.Bool(bd => bd.Should(
                qd => qd.Term("to.name", Query),
                qd => qd.Term("to.address", Query),
                qd => qd.Term("cc.name", Query),
                qd => qd.Term("cc.address", Query),
                qd => qd.Term("fromname", Query),
                qd => qd.Term("fromaddress", Query)
                ));


        }

        protected override bool NeedSorting
        {
            get { return false; }
        }

        protected override void ProcessResult()
        {
            var groups = Result.GroupBy(i => i.OutlookConversationId);
            var result = new List<EmailContactSearchObject>();
            var arr = SplitSearchCriteria(Query);
            foreach (var group in groups)
            {
                var item = group.First();
                OFRecipient recepient = GetRecepient(item.FromName, item.FromAddress, arr) ?? GetEmailAddress(item.To, arr) ?? GetEmailAddress(item.Cc, arr);

                if (recepient.IsNull() || string.IsNullOrEmpty(recepient.Address))
                {
                    continue;
                }

                _listEmails.Add(recepient.Address);
                item.EMail = recepient.Address;
                item.ContactName = recepient.Name;
                item.AddressType = recepient.Emailaddresstype;
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


        private OFRecipient GetRecepient(string name, string email, string[] criteries)
        {
            if (criteries.All(c => name.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) > -1))
            {
                return new OFRecipient(){Name=name, Address =  email};
            } else if (criteries.All(c => email.IndexOf(c, StringComparison.InvariantCultureIgnoreCase) > -1))
            {
                return new OFRecipient(){Name = name, Address = email};
            }
            return null;
        }

        
        private OFRecipient GetEmailAddress(OFRecipient[] recepients, string[] searchCriteria)
        {
            if (recepients == null || recepients.Length == 0)
            {
                return null;
            }
            var contact = recepients.FirstOrDefault(n => searchCriteria.All(s => n.Name.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1));
            if (contact == null)
            {
                var emailContact = recepients.FirstOrDefault(n => searchCriteria.All(s => n.Address.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1));
                return emailContact;
            }
            return contact;
        }

    }
}