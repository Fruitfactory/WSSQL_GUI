﻿using System.Collections.Generic;
using System.Linq;
using WSUI.Core.Core.Search;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;

namespace WSUI.Infrastructure.Implements.Systems.Core
{
    public class BaseAllEmailSearchSystem : BaseSearchSystem
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