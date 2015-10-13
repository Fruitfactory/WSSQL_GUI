using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Documents;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Base;
using OF.Core.Data.ElasticSearch.Request.Contact;
using OF.Core.Extensions;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.Rules
{
    public class OFEmailContactSearchRule : BaseSearchRule<OFEmailContactSearchObject,OFEmail>
    {
        
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        private readonly List<string> _listEmails = new List<string>();

        public OFEmailContactSearchRule(IUnityContainer container)
            :this(null,container)
        {
            Priority = 0;
        }

        public OFEmailContactSearchRule(object lockObject, IUnityContainer container)
        :base(lockObject,false,container)
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

                    var tn = new OFToNameTerm();
                    tn.SetValue(preparedCriteria);
                    var ta = new OFToAddressTerm();
                    ta.SetValue(preparedCriteria);
                    var cn = new OFCcNameTerm();
                    cn.SetValue(preparedCriteria);
                    var sa = new OFCcAddressTerm();
                    sa.SetValue(preparedCriteria);
                    var fn = new OFFromNameTerm();
                    fn.SetValue(preparedCriteria);
                    var fa = new OFFromAddressTerm();
                    fa.SetValue(preparedCriteria);

                    should._bool.should.Add(tn);
                    should._bool.should.Add(ta);
                    should._bool.should.Add(cn);
                    should._bool.should.Add(sa);
                    should._bool.should.Add(fn);
                    should._bool.should.Add(fa);

                    var mustCond = new OFMustCondition<object> {Value = should};

                    queries._bool.Add(mustCond);
                }
                return body;
            }

            var query = new OFQueryBoolConditions(); //new OFQueryBoolShould<OFBaseTerm>();
            body.query = query;
            var should1 = new OFQueryBoolShould<OFBaseTerm>();
            var toName = new OFToNameTerm();
            toName.SetValue(Query);
            var toAddress = new OFToAddressTerm();
            toAddress.SetValue(Query);
            var ccName = new OFCcNameTerm();
            ccName.SetValue(Query);
            var ccAddress = new OFCcAddressTerm();
            ccAddress.SetValue(Query);
            var fromName = new OFFromNameTerm();
            fromName.SetValue(Query);
            var fromAddress = new OFFromAddressTerm();
            fromAddress.SetValue(Query);
            should1._bool.should.Add(toName);
            should1._bool.should.Add(toAddress);
            should1._bool.should.Add(ccName);
            should1._bool.should.Add(ccAddress);
            should1._bool.should.Add(fromName);
            should1._bool.should.Add(fromAddress);

            var must = new OFMustCondition<object>(){Value = should1};

            query._bool.Add(must);

            return body;
        }

        protected override bool NeedSorting
        {
            get { return false; }
        }

        protected override void ProcessResult()
        {
            var groups = Result.GroupBy(i => i.OutlookConversationId);
            var result = new List<OFEmailContactSearchObject>();
            var arr = SplitSearchCriteria(Query);
            foreach (var group in groups)
            {
                var item = group.First();

                var listResult = GetEmailAddress(item.To, arr); 
                listResult.AddRange(GetEmailAddress(item.Cc, arr));
                var recipient = GetRecepient(item.FromName, item.FromAddress, arr);
                if (recipient.IsNotNull())
                {
                    listResult.Add(recipient);
                }

                if (!listResult.Any())
                {
                    continue;
                }
                _listEmails.AddRange(listResult.Select(r => r.Address));
                result.AddRange(listResult.Select(r => new OFEmailContactSearchObject(){AddressType = r.Emailaddresstype,ContactName = r.Name,EMail = r.Address}));
            }
            if (Result.Any())
            {
                LastDate = Result.Last().DateReceived;
            }
            result = result.GroupBy(r => r.ContactName).Select(s => s.FirstOrDefault()).ToList();
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

        
        private List<OFRecipient> GetEmailAddress(OFRecipient[] recepients, string[] searchCriteria)
        {
            if (recepients == null || recepients.Length == 0)
            {
                return new List<OFRecipient>();
            }
            var result = recepients.Where(n => searchCriteria.All(s => n.Name.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1) 
                                                      || searchCriteria.All(s => n.Address.IndexOf(s, StringComparison.InvariantCultureIgnoreCase) > -1)).ToList();
            return result;
        }
        
    }
}