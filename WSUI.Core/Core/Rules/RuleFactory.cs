using System;
using System.Collections.Generic;
using System.Linq;

namespace WSUI.Core.Core.Rules
{
    public class RuleFactory
    {
        private static Lazy<RuleFactory> _instance = new Lazy<RuleFactory>(() => new RuleFactory());

        public static RuleFactory Instance
        {
            get { return _instance.Value; }
        }

        public IRule GetQueteRule()
        {
            return new QuoteRule();
        }

        public IRule GetWordRule()
        {
            return new WordRule();
        }

        public IList<IRule> GetAllRules()
        {
            var list = new List<IRule>() { GetQueteRule(), GetWordRule() };
            return list.OrderBy(rule => rule.Priority).ToList();
        }
    }
}