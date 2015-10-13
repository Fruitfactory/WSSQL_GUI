using System.Management.Instrumentation;
using Microsoft.Practices.Unity;
using OF.Core.Core.Search;
using OF.Core.Extensions;
using OF.Infrastructure.Implements.Rules;

namespace OF.Infrastructure.Implements.Systems
{
    public class OFContactAttachmentSearchSystem : OFBaseSearchSystem
    {
        public OFContactAttachmentSearchSystem(object Lock)
            :base(Lock)
        {   
        }

        public override void Init(IUnityContainer container)
        {
            AddRule(new OFContactAttachmentSearchRule(Lock1,container));
            base.Init(container);
        }

        public override void SetProcessingRecordCount(int first, int second)
        {
            GetRules().ForEach(r => r.SetProcessingRecordCount(first,second));
        }
    }
}