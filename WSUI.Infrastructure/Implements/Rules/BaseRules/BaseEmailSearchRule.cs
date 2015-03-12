///////////////////////////////////////////////////////////
//  BaseEmailSearchRule.cs
//  Implementation of the Class BaseEmailSearchRule
//  Generated by Enterprise Architect
//  Created on:      03-Oct-2013 8:49:17 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Nest;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;

namespace WSUI.Infrastructure.Implements.Rules.BaseRules
{
    public class BaseEmailSearchRule : BaseSearchRule<EmailSearchObject, WSUIEmail>, IEmailSearchRule
    {


        #region [needs]

        private readonly List<string> _listID = new List<string>();

        #endregion

        public BaseEmailSearchRule()
        {
        }

        public BaseEmailSearchRule(object lockObject)
            : base(lockObject, false)
        {
        }

        public override void Init()
        {
            CountFirstProcess = 150;
            CountSecondProcess = 75;
            ObjectType = RuleObjectType.Email;
            base.Init();
        }

        public override void Reset()
        {
            _listID.Clear();
            base.Reset();
        }

        protected override QueryContainer BuildQuery(QueryDescriptor<WSUIEmail> queryDescriptor)
        {
            var preparedCriterias = GetProcessingSearchCriteria();
            if (preparedCriterias.Count > 1)
            {
                return queryDescriptor.Bool(descriptor =>
                {
                    descriptor.Must(preparedCriterias.Select(preparedCriteria => (Func<QueryDescriptor<WSUIEmail>, QueryContainer>) (descriptor1 => descriptor1.Term(e => e.Subject, preparedCriteria))).ToArray());
                });
            }
            return queryDescriptor.Term(e => e.Subject, Query);
        }

        protected override void ProcessResult()
        {
            IEnumerable<IGrouping<string, EmailSearchObject>> groped = null;

            if (!IsAdvancedMode)
                groped = Result.OrderByDescending(i => i.DateReceived).GroupBy(e => e.ConversationId);
            else
                groped = GetSortedResult(Result);

            var result = new List<EmailSearchObject>();
            foreach (var group in groped)
            {
                var convIndex = group.GroupBy(i => i.ConversationIndex);
                if (!convIndex.Any())
                    continue;
                var data = convIndex.FirstOrDefault().First();
                if (data == null || string.IsNullOrEmpty(data.ConversationId) || _listID.Contains(data.ConversationId))
                    continue;
                _listID.Add(data.ConversationId);
                foreach (var emailSearchObject in convIndex.Skip(1))
                {
                    data.AddItem(emailSearchObject.First());
                }
                result.Add(data);
            }
            Result.Clear();
            if (result.Count > 0)
            {
                WSSqlLogger.Instance.LogInfo("{0}: {1}", RuleName, result.Count);
                Result = result;
                LastDate = Result.Last().DateReceived;
            }
        }

        private IEnumerable<IGrouping<string, EmailSearchObject>> GetSortedResult(IList<EmailSearchObject> result)
        {
            if (AdvancedSearchCriterias.All(c => c.CriteriaType != AdvancedSearchCriteriaType.SortBy))
                return result.GroupBy(e => e.ConversationId);
            var sort = (AdvancedSearchSortByType)AdvancedSearchCriterias.First(c => c.CriteriaType == AdvancedSearchCriteriaType.SortBy).Value;
            switch (sort)
            {
                case AdvancedSearchSortByType.NewestToOldest:
                    return result.OrderByDescending(i => i.DateReceived).GroupBy(e => e.ConversationId);
                case AdvancedSearchSortByType.OldestToNewest:
                    return result.OrderBy(i => i.DateReceived).GroupBy(e => e.ConversationId);
                default:
                    return result.GroupBy(e => e.ConversationId);
            }
        }

        public IEnumerable<string> GetConversationId()
        {
            return Result.Select(e => e.ConversationId);
        }

        public void ApplyFilter(IEnumerable<string> conversationIds)
        {
            if (conversationIds == null || !conversationIds.Any())
                return;
            for (int i = 0; i < Result.Count; i++)
            {
                var obj = Result[i];
                if (conversationIds.Contains(obj.ConversationId))
                {
                    Result.Remove(obj);
                    i--;
                }
            }
        }

    }//end BaseEmailSearchRule

}//end namespace Implements