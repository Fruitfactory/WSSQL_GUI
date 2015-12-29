using OF.Core.Enums;

namespace OF.Core.Core.Rules
{
    public class OFQuoteRule : OFBaseRule
    {
        public OFQuoteRule()
        {
        }

        public override void InitRule()
        {
            base.InitRule();
            Rule = "\"(?<group>[\\w\u0020]*)\"";
            Priority = 1;
        }

        protected override ofRuleType Type
        {
            get { return ofRuleType.Quote; }
        }
    }
}