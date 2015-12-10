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


        //protected override void DoSearch()
        //{
        //    try
        //    {

        //        var watch = new Stopwatch();
        //        watch.Start();

        //        var rules = GetRules();

        //        foreach (var rule in rules.OrderBy(r => r.Priority))
        //        {
        //            var ev = rule.GetEvent();
        //            rule.Search();
        //            ev.WaitOne();
        //        }
        //        watch.Stop();
        //        OFLogger.Instance.LogInfo("------------------- searching is DONE!!!!--------------------- {0} ms", watch.ElapsedMilliseconds);

        //        if (_needStop)
        //        {
        //            RaiseSearchStopped();
        //            _IsSearching = false;
        //            OFLogger.Instance.LogError("Searching was stoped");
        //            return;
        //        }
        //        ProcessData();
        //        var items = IsAdvancedMode
        //            ? rules.Where(item => item.IncludedInAdvancedMode).OrderBy(i => i.Priority)
        //            : rules.OrderBy(i => i.Priority);
        //        foreach (var item in items)
        //        {
        //            var result = (item as ISearchRule).GetResults();
        //            if (result == null)
        //                continue;
        //            var itemResult = new SystemSearchResult(item.Priority, result.OperationResult, item.ObjectType);
        //            InternalResult.Add(itemResult);
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //        OFLogger.Instance.LogError("{0}", ex.ToString());
        //    }
        //    finally
        //    {
        //        _IsSearching = false;
        //        RaiseSearchFinished();
        //    }
        //}

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