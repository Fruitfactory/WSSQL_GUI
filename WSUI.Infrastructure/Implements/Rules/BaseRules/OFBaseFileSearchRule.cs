///////////////////////////////////////////////////////////
//  BaseFileSearchRule.cs
//  Implementation of the Class BaseFileSearchRule
//  Generated by Enterprise Architect
//  Created on:      03-Oct-2013 8:53:03 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Practices.Unity;
using OF.Core.Core.Rules;
using OF.Core.Core.Search;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch;
using OF.Core.Enums;
using OF.Core.Logger;
using OF.Infrastructure.Service.Helpers;

namespace OF.Infrastructure.Implements.Rules.BaseRules 
{
	public class OFBaseFileSearchRule : BaseSearchRule<OFFileSearchObject,OFStub> 
    {
        protected string WhereTemplate = string.Empty;

        public OFBaseFileSearchRule(IUnityContainer container)
            :this(null,container)
        {

        }

        public OFBaseFileSearchRule(object lockObject, IUnityContainer container)
            :base(lockObject,false,container)
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
	        var result = new List<OFFileSearchObject>();
	        foreach (var item in groups)
	        {
	            var first = item.First();
	            foreach (var file in item.Skip(1))
	            {
	                first.AddItem(file);
	            }
                OFTypeSearchItem type = OFSearchItemHelper.GetTypeItem(first.ItemUrl,first.Kind != null && first.Kind.Length > 0 ? first.Kind[0] : string.Empty);
	            first.TypeItem = type;
                result.Add(first);
	        }
            Result.Clear();
	        if (result.Count > 0)
	        {
                OFLogger.Instance.LogDebug("{0}: {1}",RuleName,result.Count);   
	            Result = result;
                var temp = Result.Last().DateCreated;
	            LastDate = temp.HasValue ? temp.Value : DateTime.Today;
	        }
	    }

	    protected virtual IEnumerable<OFFileSearchObject> GetSortedFileSearchObjects(IEnumerable<OFFileSearchObject> list)
	    {
	        return list.OrderByDescending(d => d.DateCreated);
	    }


    }//end BaseFileSearchRule

}//end namespace Implements