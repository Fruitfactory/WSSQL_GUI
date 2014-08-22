using System.Management.Instrumentation;
using WSUI.Core.Core.Search;
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
    }
}