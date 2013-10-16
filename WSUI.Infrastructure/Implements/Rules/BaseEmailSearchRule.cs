///////////////////////////////////////////////////////////
//  BaseEmailSearchRule.cs
//  Implementation of the Class BaseEmailSearchRule
//  Generated by Enterprise Architect
//  Created on:      03-Oct-2013 8:49:17 PM
//  Original author: Yariki
///////////////////////////////////////////////////////////


using System.Collections.Generic;
using System.Linq;
using WSUI.Core.Core.Search;
using WSUI.Core.Data;
using WSUI.Core.Logger;

namespace WSUI.Infrastructure.Implements.Rules 
{
	public class BaseEmailSearchRule : BaseSearchRule<EmailSearchObject>
    {
        #region [needs]

        private readonly List<string> _listID = new List<string>();

        #endregion

        public BaseEmailSearchRule()
        {
		}

	    public override void Init()
	    {
	        CountFirstProcess = 300;
	        CountSecondProcess = 100;
	        base.Init();
	    }

	    public override void Reset()
	    {
            _listID.Clear();
	        base.Reset();
	    }

	    protected override void ProcessResult()
        {
	        var groped = Result.GroupBy(e => e.ConversationId);
	        var result = new List<EmailSearchObject>();
            foreach (var group in groped)
	        {
                var convIndex = group.GroupBy(i => i.ConversationIndex);
                if(!convIndex.Any())
                    continue;
	            var data = convIndex.FirstOrDefault().First(); // FirstOrDefault = group, First = email
                if(data == null || string.IsNullOrEmpty(data.ConversationId) || _listID.Contains(data.ConversationId))
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
                WSSqlLogger.Instance.LogInfo("{0}: {1}",RuleName,result.Count);
                Result = result;
                LastDate = Result.Last().DateReceived;    
	        }
	    }
    }//end BaseEmailSearchRule

}//end namespace Implements