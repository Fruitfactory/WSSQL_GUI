namespace OF.Core.Core.Rules
{
    public class QuoteRule : BaseRule
    {
        public QuoteRule()
        {
        }

        public override void InitRule()
        {
            base.InitRule();
            Rule = "\"(?<group>[\\w\u0020]*)\"";
            Priority = 1;
        }
    }
}