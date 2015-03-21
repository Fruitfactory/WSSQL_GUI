///////////////////////////////////////////////////////////
//  AttachmentFilenameSearchRule.cs
//  Implementation of the Class AttachmentFilenameSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:43 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Nest;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Core.Utils;
using WSUI.Infrastructure.Service.Helpers;

namespace WSUI.Infrastructure.Implements.Rules.BaseRules 
{
	public class BaseAttachmentSearchRule : BaseSearchRule<AttachmentContentSearchObject,WSUIAttachmentContent>
	{

		public BaseAttachmentSearchRule()
            :base(null,true)
		{
		    
		}

        public BaseAttachmentSearchRule(object lockOject)
            :base(lockOject,true)
        {
            
        }

        protected virtual string GetSearchProperty()
        {
            return "*"; // searching in all property
        }

        protected override QueryContainer BuildQuery(QueryDescriptor<WSUIAttachmentContent> queryDescriptor)
        {
            var preparedCriterias = GetProcessingSearchCriteria();
            if (preparedCriterias.Count > 1)
            {
                return queryDescriptor.Bool(descriptor =>
                {
                    descriptor.Must(preparedCriterias.Select(preparedCriteria => (Func<QueryDescriptor<WSUIAttachmentContent>, QueryContainer>)(descriptor1 => descriptor1.Term(GetSearchedProperty(), preparedCriteria))).ToArray());
                });
            }
            return queryDescriptor.Term(GetSearchedProperty(), Query);
        }

        protected virtual Expression<Func<WSUIAttachmentContent, string>> GetSearchedProperty()
        {
            return null;
        }

	    protected override bool NeedSorting
	    {
	        get { return false; }
	    }

	    public override void Init()
	    {
            InitCounts();
            ObjectType = RuleObjectType.File;
	        base.Init();
	    }

        protected virtual void InitCounts()
        {
            CountFirstProcess = 50;
            CountSecondProcess = 20;
        }

        protected override void ProcessCountAdded()
	    {
            CountAdded = Result.GroupBy(i => new { Name = i.ItemNameDisplay, Size = i.Size }).Count();
	    }

	    protected override void ProcessResult()
	    {
            var groups = Result.GroupBy(i => new { Name = i.ItemNameDisplay, Size = i.Size });
	        var result = new List<AttachmentContentSearchObject>();
            WSSqlLogger.Instance.LogInfo(string.Format("Count attachments: {0}", groups.Count()));
            foreach (var group in groups)
            {
                var item = group.FirstOrDefault();
                TypeSearchItem type = SearchItemHelper.GetTypeItem(item.ItemUrl, item.Kind != null && item.Kind.Length > 0 ? item.Kind[0].ToString() : string.Empty);
                foreach (var attachmentSearchObject in group.Skip(1))
                {
                    item.AddItem(attachmentSearchObject);
                }
                result.Add(item);
            }
            Result.Clear();
            if (result.Count > 0)
            {
                WSSqlLogger.Instance.LogInfo("{0}: {1}",RuleName,result.Count);
                Result = result;
                //LastDate = Result.Min(d => d.DateReceived); // TODO should be re-factored
            }
	    }

	    protected virtual IEnumerable<AttachmentSearchObject> GetSortedAttachmentSearchObjects(IEnumerable<AttachmentSearchObject> list)
	    {
	        return list.OrderByDescending(d => d.DateReceived);
	    }

	}//end AttachmentFilenameSearchRule

}//end namespace Implements