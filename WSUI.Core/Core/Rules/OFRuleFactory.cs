using System;
using System.Collections.Generic;
using System.Linq;

namespace OF.Core.Core.Rules
{
    public class OFRuleFactory
    {
        private static Lazy<OFRuleFactory> _instance = new Lazy<OFRuleFactory>(() => new OFRuleFactory());

        public static OFRuleFactory Instance
        {
            get { return _instance.Value; }
        }

        public IRule GetQueteRule()
        {
            return CreateRule(typeof(OFQuoteRule));
        }

        public IRule GetWordRule()
        {
            return CreateRule(typeof(OFWordRule));
        }

        public IRule GetAmountRule()
        {
            return CreateRule(typeof (OFAmountRule));
        }

        public IRule GetPriceRule()
        {
            return CreateRule(typeof (OFPriceAnountRule));
        }

        public IList<IRule> GetAllRules()
        {
            var list = new List<IRule>() { GetQueteRule(), GetWordRule(), GetAmountRule(), GetPriceRule() };
            return list.OrderBy(rule => rule.Priority).ToList();
        }

        private IRule CreateRule(Type type)
        {
            var rule = Activator.CreateInstance(type) as IRule;
            rule.InitRule();
            return rule;
        }
    }
}