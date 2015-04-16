using System;
using System.Linq.Expressions;
using OF.Core.Data.ElasticSearch;
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