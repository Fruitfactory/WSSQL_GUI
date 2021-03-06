﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Practices.Unity;
using Nest;
using OF.Core.Core.Rules;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Base;
using OF.Core.Data.ElasticSearch.Request.Contact;
using OF.Core.Data.ElasticSearch.Request.Email;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Infrastructure.Implements.Rules.BaseRules;

namespace OF.Infrastructure.Implements.Rules
{
    public class OFContactEmailSearchRule : OFBaseEmailSearchRule
    {

        private const char Separator = ';';
        private static string[] AddressProperties = { "to.address", "cc.address", "bcc.address" };
        private static string[] NameProperties = { "to.name", "cc.name", "bcc.name" };

        private string _name;
        private string _to;
        private string _keyWord;

        #region [ctor]

        public OFContactEmailSearchRule(IUnityContainer container)
            : base(container)
        {
        }

        public OFContactEmailSearchRule(object lockObject, IUnityContainer container)
            : base(lockObject,container)
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
       

        protected override OFBody GetSearchBody()
        {
            var emailCriterias = GetProcessingSearchCriteria(_to).Select(t => t.Result).ToArray();// _to.SplitEmail();
            var nameCriterias = _name.SplitString();
            var keywordsCriterias = GetProcessingSearchCriteria(_keyWord);

            var body = new OFBodySort();
            body.sort = new OFSortDateCreated();

            if (!_keyWord.IsStringEmptyOrNull() && keywordsCriterias.IsNotEmpty())
            {
                var query = new OFQueryBoolMust<OFInternalBoolShould<object>>();
                body.query = query;
                var shouldEmailName = GetOfInternalBoolShould(emailCriterias, nameCriterias,_to);
                var shouldKeywords = new OFInternalBoolShould<object>();
                foreach (var keywordsCriteria in keywordsCriterias)
                {
                    var analyzed = new OFContentTerm();
                    analyzed.SetValue(keywordsCriteria);
                    var subject = new OFSubjectTerm();
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
                var should = GetOfBoolShould(emailCriterias, nameCriterias,_to);
                query._bool = should;
            }

            return body;
        }

        private OFInternalBoolShould<object> GetOfInternalBoolShould(string[] emailCriterias, string[] nameCriterias, string email)
        {

            IEnumerable<object> list = GetMustConditions(emailCriterias, nameCriterias,email);

            var should = new OFInternalBoolShould<object>();
            list.ForEach(c => should.AddCondition(c));
            return should;
        }

        private OFBoolShould<OFInternalBoolMust<OFBaseTerm>> GetOfBoolShould(string[] emailCriterias, string[] nameCriterias, string email)
        {

            IEnumerable<object> list = GetMustConditions(emailCriterias, nameCriterias, email);

            var should = new OFBoolShould<OFInternalBoolMust<OFBaseTerm>>();
            list.ForEach(c => should.should.Add((OFInternalBoolMust<OFBaseTerm>)c));
            return should;
        }

        private IEnumerable<object> GetMustConditions(string[] emailCriterias, string[] nameCriterias,string email)
        {
            var list = new List<Object>();
            if (emailCriterias.IsNotNull() && emailCriterias.Length > 0)
            {
                var mustTo = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var emailCriteria in emailCriterias)
                {
                    var term = new OFToAddressTerm();
                    term.SetValue(emailCriteria);
                    mustTo.AddCondition(term);
                }
                var mustCc = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var emailCriteria in emailCriterias)
                {
                    var term = new OFCcAddressTerm();
                    term.SetValue(emailCriteria);
                    mustCc.AddCondition(term);
                }
                var mustBCc = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var emailCriteria in emailCriterias)
                {
                    var term = new OFBccAddressTerm();
                    term.SetValue(emailCriteria);
                    mustBCc.AddCondition(term);
                }
                var content = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var emailCriteria in emailCriterias)
                {
                    var term = new OFContactContentTerm();
                    term.SetValue(emailCriteria);
                    content.AddCondition(term);
                }
                list.Add(mustTo);
                list.Add(mustCc);
                list.Add(mustBCc);
                list.Add(content);
            }
            if (nameCriterias.IsNotNull() && nameCriterias.Length > 0)
            {
                var mustToName = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var nameCriteria in nameCriterias)
                {
                    var term = new OFToNameTerm();
                    term.SetValue(nameCriteria);
                    mustToName.AddCondition(term);
                }
                var mustCcName = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var nameCriteria in nameCriterias)
                {
                    var term = new OFCcNameTerm();
                    term.SetValue(nameCriteria);
                    mustCcName.AddCondition(term);
                }
                var mustBCcName = new OFInternalBoolMust<OFBaseTerm>();
                foreach (var nameCriteria in nameCriterias)
                {
                    var term = new OFBccNameTerm();
                    term.SetValue(nameCriteria);
                    mustBCcName.AddCondition(term);
                }
                list.Add(mustToName);
                list.Add(mustCcName);
                list.Add(mustBCcName);
            }

            return list;
        }


    }
}