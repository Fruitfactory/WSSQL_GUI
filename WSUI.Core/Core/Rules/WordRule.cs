namespace WSUI.Core.Core.Rules
{
    public class WordRule : BaseRule
    {
        public WordRule()
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