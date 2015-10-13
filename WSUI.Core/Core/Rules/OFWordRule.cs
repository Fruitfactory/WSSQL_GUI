namespace OF.Core.Core.Rules
{
    public class OFWordRule : OFBaseRule
    {
        public OFWordRule()
        {
        }

        public override void InitRule()
        {
            base.InitRule();
            Rule = @"(?<group>\w+)";
            Priority = 100;
        }
    }
}