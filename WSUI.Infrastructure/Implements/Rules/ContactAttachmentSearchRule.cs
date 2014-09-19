﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Core.Rules;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Implements.Rules.BaseRules;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class ContactAttachmentSearchRule : BaseAttachmentSearchRule
    {

        private const char Separator = ';';

        private string _name;
        private string _to;


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
            string addCritetis = GetContactCriteria();
            return string.Format(WhereTemplate, dateString, addCritetis);
        }


        private string GetContactCriteria()
        {
            var criteriaForField = GetFieldCriteria(_to);
            return
                string.Format(
                    "  Contains(*,'{0}') OR Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}') ",
                    criteriaForField);
        }

        private string GetFieldCriteria(string email)
        {
            var parts = email.SplitEmail();
            if (parts == null)
                return string.Empty;
            var builder = new StringBuilder();
            if (parts.Item1.Length > 0)
            {
                builder.AppendFormat(" \"{0}*\" ", parts.Item1[0]);
                foreach (var item in parts.Item1.Skip(1))
                {
                    builder.AppendFormat(" AND \"{0}*\" ", item);
                }
            }
            return string.Format(" {0} AND \"{1}*\" ", builder.ToString(), parts.Item2);
        }

    }
}