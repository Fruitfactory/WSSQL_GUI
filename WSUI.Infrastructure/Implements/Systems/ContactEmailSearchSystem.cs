using OF.Core.Core.Search;
using OF.Core.Extensions;
using OF.Infrastructure.Implements.Rules;

namespace OF.Infrastructure.Implements.Systems
{
    public class ContactEmailSearchSystem : BaseSearchSystem
    {
        public ContactEmailSearchSystem(object Lock)
            :base(Lock)
        {
            
        }

        public override void Init()
        {
            AddRule(new ContactEmailSearchRule(Lock1));
            base.Init();
        }

        public override void SetProcessingRecordCount(int first, int second)
        {
            GetRules().ForEach(r => r.SetProcessingRecordCount(first,second));
        }
    }
}