using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WSUI.Core.Core.Rules
{
    public abstract class BaseRule : IRule
    {
        #region Implementation of IRule

        public string Rule { get; protected set; }

        public int Priority { get; protected set; }

        public virtual void InitRule()
        { }

        public string[] ApplyRule(string criteria)
        {
            MatchCollection col = Regex.Matches(criteria, Rule);
            if (col.Count == 0)
                return new string[] { };
            var list = new List<string>();
            for (int i = 0; i < col.Count; i++)
            {
                var item = col[i];
                if (string.IsNullOrEmpty(item.Groups[1].Value))
                    continue;
                list.Add(item.Groups[1].Value);
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