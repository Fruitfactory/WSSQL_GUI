using System.Collections.Generic;
using System.Text.RegularExpressions;
using OF.Core.Enums;

namespace OF.Core.Core.Rules
{
    public abstract class OFBaseRule : IRule
    {
        #region Implementation of IRule

        public string Rule { get; protected set; }

        public int Priority { get; protected set; }

        protected abstract ofRuleType Type { get; }

        public virtual void InitRule()
        { }

        public IEnumerable<OFRuleToken> ApplyRule(string criteria)
        {
            MatchCollection col = Regex.Matches(criteria, Rule);
            if (col.Count == 0)
                return null;
            var list = new List<OFRuleToken>();
            for (int i = 0; i < col.Count; i++)
            {
                var item = col[i];
                if (string.IsNullOrEmpty(item.Groups[1].Value))
                    continue;
                list.Add( new OFRuleToken(Type){Result = item.Groups[1].Value});
            }
            return list.ToArray();
        }

        public string ClearCriteriaAccordingRule(string criteria)
        {
            if (string.IsNullOrEmpty(criteria))
                return criteria;
            string result = Regex.Replace(criteria, Rule, string.Empty);
            return result;
        }

        #endregion Implementation of IRule
    }
}