using Microsoft.Practices.Unity;
using OF.Core.Core.Search;
using OF.Core.Extensions;
using OF.Infrastructure.Implements.Rules;

namespace OF.Infrastructure.Implements.Systems
{
    public class OFContactEmailSearchSystem : OFBaseSearchSystem
    {
        public OFContactEmailSearchSystem(object Lock)
            :base(Lock)
        {
            
        }

        public override void Init(IUnityContainer container)
        {
            AddRule(new OFContactEmailSearchRule(Lock1,container));
            base.Init(container);
        }

        public override void SetProcessingRecordCount(int first, int second)
        {
            GetRules().ForEach(r => r.SetProcessingRecordCount(first,second));
        }
    }
}