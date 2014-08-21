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

        public override void Init()
        {
            RuleName = "Attachment (Content)";
            base.Init();
        }
    }
}