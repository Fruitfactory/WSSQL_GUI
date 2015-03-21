﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nest;
using WSUI.Core.Core.Rules;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class ContactAttachmentSearchRule : BaseAttachmentSearchRule
    {

        private const char Separator = ';';

        private string _name;
        private string _to;
        private string _keyWord;

        #region [ctor]

        public ContactAttachmentSearchRule()
            : base()
        {

        }

        public ContactAttachmentSearchRule(object lockObject)
            : base(lockObject)
        {

        }

        #endregion

        protected override QueryContainer BuildQuery(QueryDescriptor<WSUIAttachmentContent> queryDescriptor)
        {
            var emailCriterias = _to.SplitEmail();
            var nameCriteria = _name.SplitString();
            var keywordCriteria = GetProcessingSearchCriteria(_keyWord);

            var listShould = new List<Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>>();

            var listAnd = emailCriterias.Select(emailCriteria => (Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>) (descriptor => descriptor.Term(a => a.Analyzedcontent, emailCriteria))).ToList();
            var and = listAnd;
            listShould.Add(descriptor => descriptor.Bool(boolQueryDescriptor => boolQueryDescriptor.Must(and.ToArray())));

            
            listAnd = nameCriteria.Select(name => (Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>) (descriptor => descriptor.Term(a => a.Analyzedcontent, name))).ToList();
            listShould.Add(descriptor => descriptor.Bool(boolQueryDescriptor => boolQueryDescriptor.Must(listAnd.ToArray())));

            if (!string.IsNullOrEmpty(_keyWord) && keywordCriteria != null && keywordCriteria.Count > 0)
            {

                var listKeyWordShould = new List<Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>>();
                listKeyWordShould.AddRange(keywordCriteria.Select(temp => (Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>)(d => d.Term(a => a.Analyzedcontent, temp))).ToArray());
                listKeyWordShould.AddRange(keywordCriteria.Select(temp => (Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>)(d => d.Term(a => a.Filename, temp))).ToArray());

                var l = new List<Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>>();
                l.Add(descriptor => descriptor.Bool(s => s.Should(listKeyWordShould.ToArray())));
                l.Add(descriptor => descriptor.Bool(s => s.Should(listShould.ToArray())));

                return queryDescriptor.Bool(bd => bd.Must(l.ToArray()));
            }

            return queryDescriptor.Bool(bd => bd.Should(listShould.ToArray())); 
        }

        protected override void InitCounts()
        {
            CountFirstProcess = 250;
            CountSecondProcess = 100;
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
        
    }
}