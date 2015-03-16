using System;
using System.Linq.Expressions;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules
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

        protected override Expression<Func<WSUIAttachmentContent, string>> GetSearchedProperty()
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