namespace OF.Core.Core.Rules
{
    public class OFPriceAnountRule : BaseRule
    {
        public override void InitRule()
        {
            base.InitRule();
            Rule = @"(\d{1,3}(?:,?\d{3})*\.\d{2})";
            Priority = 3;
        }
    }
}