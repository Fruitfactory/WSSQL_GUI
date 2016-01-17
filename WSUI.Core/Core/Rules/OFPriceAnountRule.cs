using OF.Core.Enums;

namespace OF.Core.Core.Rules
{
    public class OFPriceAnountRule : OFBaseRule
    {
        public override void InitRule()
        {
            base.InitRule();
            Rule = @"(\d{1,3}(?:,?\d{3})*\.\d{2})";
            Priority = 3;
        }

        protected override ofRuleType Type
        {
            get { return ofRuleType.Amount; }
        }
    }
}