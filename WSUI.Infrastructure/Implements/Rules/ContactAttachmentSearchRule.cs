using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WSUI.Core.Core.Rules;
using WSUI.Core.Data.ElasticSearch;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Infrastructure.Implements.Rules.BaseRules;
using WSUI.Infrastructure.Implements.Rules.Helpers;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class ContactAttachmentSearchRule : BaseAttachmentSearchRule
    {

        private const char Separator = ';';

        private string _name;
        private string _to;
        private string _keyWord;

        #region [ctor]

        public ContactAttachmentSearchRule()
            : base()
        {

        }

        public ContactAttachmentSearchRule(object lockObject)
            : base(lockObject)
        {

        }

        #endregion

        protected override Expression<Func<WSUIAttachmentContent, string>> GetSearchedProperty()
        {
            return a => a.Analyzedcontent;
        }

        protected override void InitCounts()
        {
            CountFirstProcess = 250;
            CountSecondProcess = 100;
        }

        public override void SetSearchCriteria(string criteria)
        {
            base.SetSearchCriteria(criteria);
            if (string.IsNullOrEmpty(criteria) || criteria.IndexOf(Separator) == -1)
                return;
            var arrStr = criteria.Split(new[] { Separator });
            _name = arrStr.Length > 0 ? arrStr[0] : string.Empty;
            _to = arrStr.Length > 1 ? arrStr[1] : string.Empty;
            _keyWord = arrStr.Length > 2 ? arrStr[2] : string.Empty;
        }
        
    }
}