using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Attachment;
using OF.Core.Data.ElasticSearch.Request.Base;
using OF.Core.Data.ElasticSearch.Request.Contact;
using OF.Core.Data.ElasticSearch.Request.Email;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Infrastructure.Implements.Rules.BaseRules;

namespace OF.Infrastructure.Implements.Rules
{
    public class OFContactAttachmentSearchRule : OFBaseAttachmentSearchRule
    {

        private const char Separator = ';';

        private string _name;
        private string _to;
        private string _keyWord;

        #region [ctor]

        public OFContactAttachmentSearchRule(IUnityContainer container)
            : base(container)
        {

        }

        public OFContactAttachmentSearchRule(object lockObject, IUnityContainer container)
            : base(lockObject,container)
        {

        }

        #endregion
        

        protected override void InitCounts()
        {
            CountFirstProcess = 100;
            CountSecondProcess = 50;
        }

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

        protected override OFBody GetSearchBody()
        {
            var emailCriterias = _to.SplitEmail();
            var nameCriterias = _name.SplitString();
            var keywordsCriterias = GetProcessingSearchCriteria(_keyWord);

            var body = new OFBody();

            if (!_keyWord.IsStringEmptyOrNull() && keywordsCriterias.IsNotEmpty())
            {
                var query = new OFQueryBoolMust<OFInternalBoolShould<object>>();
                body.query = query;
                var shouldEmailName = GetOfInternalBoolShould(emailCriterias, nameCriterias);
                var shouldKeywords = new OFInternalBoolShould<object>();
                foreach (var keywordsCriteria in keywordsCriterias)
                {
                    var analyzed = new OFContentTerm();
                    analyzed.SetValue(keywordsCriteria);
                    var subject = new OFFilenameTerm();
                    subject.SetValue(keywordsCriteria);
                    shouldKeywords.AddCondition(analyzed);
                    shouldKeywords.AddCondition(subject);
                }
                query._bool.must.Add(shouldKeywords);
                query._bool.must.Add(shouldEmailName);

            }
            else
            {
                var query = new OFQueryBoolShould<OFInternalBoolMust<OFBaseTerm>>();
                body.query = query;
                var should = GetOfBoolShould(emailCriterias, nameCriterias);
                query._bool = should;

            }

            return body;
        }

        private OFInternalBoolShould<object> GetOfInternalBoolShould(string[] emailCriterias, string[] nameCriterias)
        {

            IEnumerable<object> list = GetMustConditions(emailCriterias, nameCriterias);

            var should = new OFInternalBoolShould<object>();
            list.ForEach(c => should.AddCondition(c));
            return should;
        }

        private OFBoolShould<OFInternalBoolMust<OFBaseTerm>> GetOfBoolShould(string[] emailCriterias, string[] nameCriterias)
        {

            IEnumerable<object> list = GetMustConditions(emailCriterias, nameCriterias);

            var should = new OFBoolShould<OFInternalBoolMust<OFBaseTerm>>();
            list.ForEach(c => should.should.Add((OFInternalBoolMust<OFBaseTerm>)c));
            return should;
        }

        private IEnumerable<object> GetMustConditions(string[] emailCriterias, string[] nameCriterias)
        {
            var list = new List<Object>();
            if (emailCriterias.IsNotNull())
            {
                var mustEmail = new OFInternalBoolMust<OFBaseTerm>();

                foreach (var emailCriteria in emailCriterias)
                {
                    var term = new OFContentTerm();
                    term.SetValue(emailCriteria);
                    mustEmail.AddCondition(term);
                }    
                list.Add(mustEmail);
            }
            else if (nameCriterias.IsNotNull())
            {
                var mustName = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var nameCriteria in nameCriterias)
                {
                    var term = new OFContentTerm();
                    term.SetValue(nameCriteria);
                    mustName.AddCondition(term);
                }
                list.Add(mustName);
            }
            return list;
        }

    }
}