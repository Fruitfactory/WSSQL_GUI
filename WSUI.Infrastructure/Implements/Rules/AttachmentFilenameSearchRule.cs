///////////////////////////////////////////////////////////
//  AttachmentFilenameSearchRule.cs
//  Implementation of the Class AttachmentFilenameSearchRule
//  Generated by Enterprise Architect
//  Created on:      29-Sep-2013 10:41:43 AM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Core.Rules;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class AttachmentFilenameSearchRule : BaseSearchRule<AttachmentSearchObject>
	{
        private const string WhereTemplate = "WHERE Contains(System.ItemUrl,'at=') AND System.DateModified < '{0}'  AND ( {1} ) ORDER BY System.DateModified DESC";
        private readonly List<string> _listId = new List<string>(); 


		public AttachmentFilenameSearchRule()
		{
		    Priority = 6;
		}
        
	    protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
	    {
	        var dateString = FormatDate(ref LastDate);
	        var tuple = GetProcessingSearchCriteria(listCriterisRules);
	        string addCriteria = GetAdditionalCriteria(tuple.Item2);
	        return string.Format(WhereTemplate, dateString, addCriteria, tuple.Item1);
	    }

	    private string GetAdditionalCriteria(List<string> listWord)
	    {
            if (listWord.Count == 0)
                 return string.Empty;
             var temp = new StringBuilder();

             temp.Append(string.Format("Contains('\"{0}\"') ", listWord[0]));

             if (listWord.Count > 1)
                 for (int i = 1; i < listWord.Count; i++)
                     temp.Append(string.Format("AND Contains('\"{0}\"') ", listWord.ElementAt(i)));

             return temp.ToString();
	    }

	    public override void Init()
	    {
	        RuleName = "Attachment";
	        CountFirstProcess = 50;
	        CountSecondProcess = 20;
	        base.Init();
	    }

        protected override void ProcessCountAdded()
	    {
            CountAdded = Result.GroupBy(i => new { Name = i.ItemName, Size = i.Size }).Count();
            if (CountAdded == CountProcess)
                IsInterupt = true;
	    }

	    protected override void ProcessResult()
	    {
            var groups = Result.GroupBy(i => new { Name = i.ItemName, Size = i.Size });
	        var result = new List<AttachmentSearchObject>();
            lock (Lock)
            {
                WSSqlLogger.Instance.LogInfo(string.Format("Count attachments: {0}", groups.Count()));
                foreach (var group in groups)
                {
                    var item = group.FirstOrDefault();
                    if (_listId.Any(i => i == item.ConversationId))
                        continue;
                    TypeSearchItem type = SearchItemHelper.GetTypeItem(item.ItemUrl, item.Kind != null && item.Kind.Length > 0 ? item.Kind[0].ToString() : string.Empty);
                    foreach (var attachmentSearchObject in group.Skip(1))
                    {
                        item.AddItem(attachmentSearchObject);
                    }
                    _listId.Add(item.ConversationId);
                    result.Add(item);
                }
                Result.Clear();
                if (result.Count > 0)
                {
                    WSSqlLogger.Instance.LogInfo("{0}: {1}",RuleName,result.Count);
                    Result = result;
                    LastDate = Result.Last().DateModified;
                }
                _listId.Clear();
            }
	    }
	}//end AttachmentFilenameSearchRule

}//end namespace Implements