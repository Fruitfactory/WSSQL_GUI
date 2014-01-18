namespace WSUI.Infrastructure.Implements.Rules
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

        public override void Init()
        {
            RuleName = "Attachment (Filename)";
            base.Init();
        }
    }
}