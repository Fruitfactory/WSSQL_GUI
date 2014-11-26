using System.Management.Instrumentation;
using WSUI.Core.Core.Search;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Implements.Rules;

namespace WSUI.Infrastructure.Implements.Systems
{
    public class ContactAttachmentSearchSystem : BaseSearchSystem
    {
        public ContactAttachmentSearchSystem()
        {   
        }

        public override void Init()
        {
            AddRule(new ContactAttachmentSearchRule());
            base.Init();
        }

        public override void SetProcessingRecordCount(int first, int second)
        {
            GetRules().ForEach(r => r.SetProcessingRecordCount(first,second));
        }
    }
}