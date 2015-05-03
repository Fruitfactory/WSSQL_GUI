using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Data.ElasticSearch.Request;
using OF.Core.Data.ElasticSearch.Request.Attachment;
using OF.Infrastructure.Implements.Rules.BaseRules;

namespace OF.Infrastructure.Implements.Rules
{
    public class AttachmentFilenameSearchRule : BaseAttachmentSearchRule
    {
        public AttachmentFilenameSearchRule()
            : base()
        {
            CreateInit();
        }

        public AttachmentFilenameSearchRule(object lockObject)
            : base(lockObject)
        {
            CreateInit();
        }

        private void CreateInit()
        {
            Priority = 6;
        }

        protected override string GetSearchProperty()
        {
            return "System.ItemUrl";
        }

        protected override Expression<Func<OFAttachmentContent, string>> GetSearchedProperty()
        {
            return a => a.Filename;
        }

        public override void Init()
        {
            RuleName = "Attachment (Filename)";
            base.Init();
        }

        protected override OFBody GetSearchBody()
        {
            var preparedCriterias = GetKeywordsList();

            var body = new OFBody();
            if (preparedCriterias.Count > 1)
            {
                var query = new OFQueryBoolMust<OFTerm<OFAttachmentSimpleFilenameTerm>>();
                body.query = query;
                foreach (var preparedCriteria in preparedCriterias)
                {
                    var term = new OFTerm<OFAttachmentSimpleFilenameTerm>(preparedCriteria);
                    query._bool.must.Add(term);
                }
                return body;                   
            }

            body.query = new OFQuerySimpleTerm<OFAttachmentSimpleFilenameTerm>(Query);
            return body;
        }


        protected override IEnumerable<AttachmentSearchObject> GetSortedAttachmentSearchObjects(IEnumerable<AttachmentSearchObject> list)
        {
            var words = Query.Split(' ');
            return list.OrderBy(d => GetMinContainsIndex(d.ItemNameDisplay, words));
        }

        private int GetMinContainsIndex(string itemName, IEnumerable<string> words)
        {
            if (string.IsNullOrEmpty(itemName) || words == null)
                return int.MaxValue;
            int min = words.Min(w => itemName.IndexOf(w, StringComparison.InvariantCultureIgnoreCase));
            System.Diagnostics.Debug.WriteLine(string.Format("Min: {0}", min));
            return min == -1 ? int.MaxValue : min;
        }
    }
}