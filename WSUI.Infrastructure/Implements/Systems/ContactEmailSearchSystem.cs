using WSUI.Core.Core.Search;
using WSUI.Infrastructure.Implements.Rules;

namespace WSUI.Infrastructure.Implements.Systems
{
    public class ContactEmailSearchSystem : BaseSearchSystem
    {
        public ContactEmailSearchSystem()
        {
            
        }

        public override void Init()
        {
            AddRule(new ContactEmailSearchRule());
            base.Init();
        }
    }
}