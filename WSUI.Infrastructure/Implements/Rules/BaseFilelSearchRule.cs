///////////////////////////////////////////////////////////
//  BaseFilelSearchRule.cs
//  Implementation of the Class BaseFilelSearchRule
//  Generated by Enterprise Architect
//  Created on:      03-Oct-2013 8:53:03 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Linq;
using System.Windows.Documents;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using System.Collections.Generic;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class BaseFilelSearchRule : BaseSearchRule<FileSearchObject> 
    {
        protected string WhereTemplate = string.Empty;

		public BaseFilelSearchRule()
        {

        }

        public BaseFilelSearchRule(object lockObject)
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

	    protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            var dateFormat = FormatDate(ref LastDate);
            var and = GetProcessingSearchCriteria(listCriterisRules).Item1;
            return string.Format(WhereTemplate, and, dateFormat);
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
                WSSqlLogger.Instance.LogInfo("{0}: {1}",RuleName,result.Count);   
	            Result = result;
	            LastDate = Result.Last().DateCreated;
	        }
	    }

	    protected virtual IEnumerable<FileSearchObject> GetSortedFileSearchObjects(IEnumerable<FileSearchObject> list)
	    {
	        return list.OrderByDescending(d => d.DateCreated);
	    }


    }//end BaseFilelSearchRule

}//end namespace Implements