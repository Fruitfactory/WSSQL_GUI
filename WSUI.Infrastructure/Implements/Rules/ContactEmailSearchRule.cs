using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Infrastructure.Implements.Rules.BaseRules;

namespace OF.Infrastructure.Implements.Rules
{
    public class ContactEmailSearchRule : BaseEmailSearchRule
    {

        private const char Separator = ';';
        private static string[] AddressProperties = { "to.address", "cc.address", "bcc.address" };
        private static string[] NameProperties = { "to.name", "cc.name", "bcc.name" };

        private string _name;
        private string _to;
        private string _keyWord;

        #region [ctor]

        public ContactEmailSearchRule()
            : base()
        {
        }

        public ContactEmailSearchRule(object lockObject)
            : base(lockObject)
        {
        }

        #endregion

        public override void SetSearchCriteria(string criteria)
        {
            base.SetSearchCriteria(criteria);
            if (string.IsNullOrEmpty(criteria) || criteria.IndexOf(Separator) == -1)
                return;
            var arrStr = criteria.Split(new[] { Separator });
            _name = arrStr.Length > 0 ? arrStr[0].ToLowerInvariant() : string.Empty;
            _to = arrStr.Length > 1 ? arrStr[1].ToLowerInvariant() : string.Empty;
            _keyWord = arrStr.Length > 2 ? arrStr[2].ToLowerInvariant() : string.Empty;
        }

        protected override Expression<Func<OFEmail, string>> GetSearchedProperty()
        {
            return null;
        }

        protected override QueryContainer BuildQuery(QueryDescriptor<OFEmail> queryDescriptor)
        {
            var emailCriterias = _to.SplitEmail();
            var nameCriteria = _name.SplitString();
            var keywordsCriteria = GetProcessingSearchCriteria(_keyWord);

            var listShould = AddressProperties.Select(property => (Func<QueryDescriptor<OFEmail>, QueryContainer>) (descriptor => descriptor.Bool(boolQueryDescriptor => boolQueryDescriptor.Must(emailCriterias.Select(email => (Func<QueryDescriptor<OFEmail>, QueryContainer>) (des => des.Term(property.ToString(), email.ToString()))).ToArray())))).ToList();
            listShould.AddRange(NameProperties.Select(property => (Func<QueryDescriptor<OFEmail>, QueryContainer>) (descriptor => descriptor.Bool(boolQueryDescriptor => boolQueryDescriptor.Must(nameCriteria.Select(name => (Func<QueryDescriptor<OFEmail>, QueryContainer>) (des => des.Term(property.ToString(), name.ToString()))).ToArray())))));

            if (!string.IsNullOrEmpty(_keyWord) && keywordsCriteria != null && keywordsCriteria.Count > 0)
            {

                var listKeyWordShould = new List<Func<QueryDescriptor<OFEmail>, QueryContainer>>();
                listKeyWordShould.AddRange(keywordsCriteria.Select((temp => (Func<QueryDescriptor<OFEmail>, QueryContainer>)(d => d.Term(e => e.Analyzedcontent, temp)))).ToArray());
                listKeyWordShould.AddRange(keywordsCriteria.Select((temp => (Func<QueryDescriptor<OFEmail>, QueryContainer>)(d => d.Term(e => e.Subject, temp)))).ToArray());

                var l = new List<Func<QueryDescriptor<OFEmail>, QueryContainer>>();
                l.Add(d => d.Bool(s => s.Should(listKeyWordShould.ToArray())));
                l.Add(d => d.Bool(s => s.Should(listShould.ToArray())));

                return
                    queryDescriptor.Bool(
                        bd =>
                            bd.Must(l.ToArray()));
            }

            return queryDescriptor.Bool(bd => bd.Should(listShould.ToArray()));
        }
    }
}