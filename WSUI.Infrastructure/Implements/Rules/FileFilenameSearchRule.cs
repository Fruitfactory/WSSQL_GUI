///////////////////////////////////////////////////////////
//  FileFilenameSearchRule.cs
//  Implementation of the Class FileFilenameSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:50 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System;
using System.Collections.Generic;
using System.Linq;
using WSUI.Core.Data;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class FileFilenameSearchRule : BaseFilelSearchRule
	{

		public FileFilenameSearchRule()
		{
		    ConstructorInit();
		}

        public FileFilenameSearchRule(object lockObject)
            :base(lockObject)
        {
            ConstructorInit();
        }

	    private void ConstructorInit()
	    {
	        Priority = 4;
	        WhereTemplate =
                " WHERE scope='file:' AND Contains(System.Kind,' NOT \"folder\"') AND Contains(System.ItemUrl,{0},1033) AND System.DateCreated < '{1}' ORDER BY System.DateCreated DESC";
	    }



	    public override void Init()
	    {
	        RuleName = "FileFilename";
	        base.Init();
	    }

	    protected override IEnumerable<FileSearchObject> GetSortedFileSearchObjects(IEnumerable<FileSearchObject> list)
	    {
            var words = Query.Split(' ');
            return list.OrderBy(d => GetMinContainsIndex(d.ItemNameDisplay, words));
	    }

        private int GetMinContainsIndex(string itemName, IEnumerable<string> words)
        {
            if (string.IsNullOrEmpty(itemName) || words == null)
                return int.MaxValue;
            int min = words.Min(w => itemName.IndexOf(w, StringComparison.InvariantCultureIgnoreCase));
            return min == -1 ? int.MaxValue : min;
        }
	}//end FileFilenameSearchRule

}//end namespace Implements