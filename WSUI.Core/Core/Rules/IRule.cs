using System.Collections.Generic;

namespace OF.Core.Core.Rules
{
    public interface IRule
    {
        void InitRule();

        string Rule { get; }

        int Priority { get; }

        IEnumerable<OFRuleToken> ApplyRule(string criteria);

        string ClearCriteriaAccordingRule(string criteria);
    }
}