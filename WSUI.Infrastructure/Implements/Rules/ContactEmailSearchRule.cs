using System.Collections.Generic;
using WSUI.Core.Core.Rules;
using WSUI.Core.Enums;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class ContactEmailSearchRule : BaseEmailSearchRule
    {
        private string WhereTemplate =
            "WHERE CONTAINS(System.Kind,'email') AND System.Message.DateReceived < '{0}' AND {1} ORDER BY System.Message.DateReceived DESC";//System.Search.Contents

        private const char Separator = ';';

        private string _name;
        private string _to;

        #region [ctor]

        public ContactEmailSearchRule():base()
        {   
        }

        public ContactEmailSearchRule(object lockObject)
            : base(lockObject)
        {
        }

        #endregion

        public override void SetSearchCriteria(string criteria)
        {
            base.SetSearchCriteria(criteria);
            if (string.IsNullOrEmpty(criteria) || criteria.IndexOf(Separator) == -1)
                return;
            var arrStr = criteria.Split(new[] { Separator });
            _name = arrStr.Length > 0 ? arrStr[0] : string.Empty;
            _to = arrStr.Length > 1 ? arrStr[1] : string.Empty;
        }

        protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            var dateString = FormatDate(ref LastDate);
            string addCritetis = GetEmailsCriteria();
            return string.Format(WhereTemplate, dateString, addCritetis);
        }


        private string GetEmailsCriteria()
        {
            return
                string.Format(
                    " Contains(*,'\"{0}\"') OR Contains(*,'\"{1}\"') ",
                    _name,
                    _to); //Contains(System.Message.FromAddress,'\"{0}\"') AND 

        }

    }
}