﻿using System;
using System.Linq.Expressions;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Attachment;
using OF.Infrastructure.Implements.Rules.BaseRules;

namespace OF.Infrastructure.Implements.Rules
{
    public class AttachmentContentSearchRule : BaseAttachmentSearchRule
    {
        public AttachmentContentSearchRule():base()
        {
            CreateInit();
        }

        public AttachmentContentSearchRule(object locObject):base(locObject)
        {
            CreateInit();
        }

        private void CreateInit()
        {
            Priority = 7;
        }

        protected override OFBody GetSearchBody()
        {
            var preparedCriterias = GetKeywordsList();

            var body = new OFBody();
            if (preparedCriterias.Count > 1)
            {
                var query = new OFQueryBoolMust<OFTerm<OFAttachmentSimpleContentTerm>>();
                body.query = query;
                foreach (var preparedCriteria in preparedCriterias)
                {
                    var term = new OFTerm<OFAttachmentSimpleContentTerm>(preparedCriteria);
                    query._bool.must.Add(term);
                }
                return body;
            }

            body.query = new OFQuerySimpleTerm<OFAttachmentSimpleContentTerm>(Query);
            return body;
        }

        protected override Expression<Func<OFAttachmentContent, string>> GetSearchedProperty()
        {
            return a => a.Analyzedcontent;
        }

        public override void Init()
        {
            RuleName = "Attachment (Content)";
            base.Init();
        }
    }
}