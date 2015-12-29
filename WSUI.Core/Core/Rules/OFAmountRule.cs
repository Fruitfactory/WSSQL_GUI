using OF.Core.Enums;

namespace OF.Core.Core.Rules
{
    public class OFAmountRule : OFBaseRule
    {
        public OFAmountRule()
        {   
        }

        public override void InitRule()
        {
            base.InitRule();
            Rule = @"(\$ *\d{1,3}(?:,?\d{3})*\.\d{2})";
            Priority = 2;
        }

        protected override ofRuleType Type
        {
            get { return ofRuleType.Amount; }
        }
    }
}