///////////////////////////////////////////////////////////
//  BaseFileSearchRule.cs
//  Implementation of the Class BaseFileSearchRule
//  Generated by Enterprise Architect
//  Created on:      03-Oct-2013 8:53:03 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Linq;
using OF.Core.Core.Rules;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;

namespace OF.Infrastructure.Implements.Rules.BaseRules 
{
	public class BaseFileSearchRule : BaseSearchRule<FileSearchObject,OFStub> 
    {
        protected string WhereTemplate = string.Empty;

		public BaseFileSearchRule()
        {

        }

        public BaseFileSearchRule(object lockObject)
            :base(lockObject,false)
        {

        }

	    public override void Init()
	    {
	        CountFirstProcess = 50;
	        CountSecondProcess = 25;
            ObjectType = RuleObjectType.File;
	        base.Init();
	    }


	    protected override void ProcessResult()
	    {
            var groups = GetSortedFileSearchObjects(Result).GroupBy(i => i.ItemNameDisplay);
	        var result = new List<FileSearchObject>();
	        foreach (var item in groups)
	        {
	            var first = item.First();
	            foreach (var file in item.Skip(1))
	            {
	                first.AddItem(file);
	            }
                TypeSearchItem type = SearchItemHelper.GetTypeItem(first.ItemUrl,first.Kind != null && first.Kind.Length > 0 ? first.Kind[0] : string.Empty);
	            first.TypeItem = type;
                result.Add(first);
	        }
            Result.Clear();
	        if (result.Count > 0)
	        {
                OFLogger.Instance.LogDebug("{0}: {1}",RuleName,result.Count);   
	            Result = result;
	            LastDate = Result.Last().DateCreated;
	        }
	    }

	    protected virtual IEnumerable<FileSearchObject> GetSortedFileSearchObjects(IEnumerable<FileSearchObject> list)
	    {
	        return list.OrderByDescending(d => d.DateCreated);
	    }


    }//end BaseFileSearchRule

}//end namespace Implements