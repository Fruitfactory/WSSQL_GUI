using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OF.Core.Core.Search;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Infrastructure.Implements.Systems.Core
{
    public class OFBaseAllEmailSearchSystem : OFBaseSearchSystem
    {
        protected override void ProcessData()
        {
            IEnumerable<ISearch> rules = GetRules();
            ProcessEmails(rules.Where(r => r.ObjectType == RuleObjectType.Email && r is IEmailSearchRule).OfType<IEmailSearchRule>());
        }

        private void ProcessEmails(IEnumerable<IEmailSearchRule> emailRules)
        {
            if (emailRules == null || !emailRules.Any())
                return;
            var listIds = (emailRules.First() as IEmailSearchRule).GetConversationId().ToList();
            foreach (var emailRule in emailRules.Skip(1))
            {
                emailRule.ApplyFilter(listIds);
                listIds.AddRange(emailRule.GetConversationId());
            }
            if (listIds.Any())
                listIds.Clear();
        }

    }
}