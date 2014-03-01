///////////////////////////////////////////////////////////
//  AllSearchSystem.cs
//  Implementation of the Class AllSearchSystem
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:41 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using WSUI.Core.Core.Search;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Infrastructure.Implements.Rules;

namespace WSUI.Infrastructure.Implements.Systems 
{
	public class AllSearchSystem : BaseSearchSystem 
    {

		public AllSearchSystem()
        {

		}

	    public override void Init()
	    {
            AddRule(new GeneralContactRule(75,0,Lock1));
            AddRule(new EmailSubjectSearchRule(Lock1));
            AddRule(new EmailContentSearchRule(Lock1));
            AddRule(new FileFilenameSearchRule(Lock1));
            AddRule(new FileContentSearchRule(Lock1));
            AddRule(new AttachmentFilenameSearchRule(Lock1));
            AddRule(new AttachmentContentSearchRule(Lock1));
	        base.Init();
	    }

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
	    }

    }//end AllSearchSystem

}//end namespace Implements