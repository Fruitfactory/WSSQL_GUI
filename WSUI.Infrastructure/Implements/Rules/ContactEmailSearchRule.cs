using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Core.Rules;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
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

            var emailPart = GeneratePartsFromEmail(_to);
            return
                string.Format(
                    "( Contains(*,'\"{0}*\"') OR ( {1} ) )",
                    _name,
                    emailPart);
        }

        private string GeneratePartsFromEmail(string email)
        {
            var parts = GetEmailParts(email);
            if (parts == null)
                return string.Empty;
            var builder = new StringBuilder();
            if (parts.Item1.Length > 0)
            {
                builder.AppendFormat(" Contains(*, '\"{0}*\"') ", parts.Item1[0]);
                foreach (var item in parts.Item1.Skip(1))
                {
                    builder.AppendFormat(" AND Contains(*, '\"{0}*\"')", item);
                }
            }
            return string.Format(" {0} AND Contains(*, '\"{1}*\"')", builder.ToString(), parts.Item2);

        }

        private Tuple<string[], string> GetEmailParts(string email)
        {
            return email.SplitEmail();
        }



    }
}