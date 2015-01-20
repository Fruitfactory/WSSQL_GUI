using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Core.Rules;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Implements.Rules.BaseRules;
using WSUI.Infrastructure.Implements.Rules.Helpers;

namespace WSUI.Infrastructure.Implements.Rules
{
    public class ContactEmailSearchRule : BaseEmailSearchRule
    {

        private string WhereTemplate =
            "WHERE CONTAINS(System.Kind,'email') AND System.Message.DateReceived < '{0}' AND {1} ORDER BY System.Message.DateReceived DESC";//System.Search.Contents

        private const char Separator = ';';

        private string _name;
        private string _to;
        private string _keyWord;

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
            _keyWord = arrStr.Length > 2 ? arrStr[2] :string.Empty;
        }

        protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            var dateString = FormatDate(ref LastDate);
            var keyWordCriteria = GetKeywoardCriteria(listCriterisRules);
            string addCritetis = GetEmailsCriteria(keyWordCriteria);
            GetKeywoardCriteria(listCriterisRules);
            return string.Format(WhereTemplate, dateString, addCritetis);
        }

        private string GetKeywoardCriteria(IList<IRule> listCriterisRules)
        {
            var listW = new List<string>();
            var temp = _keyWord;
            foreach (var rule in listCriterisRules.OrderBy(i => i.Priority))
            {
                listW.AddRange(rule.ApplyRule(temp));
                temp = rule.ClearCriteriaAccordingRule(temp);
            }
            var builder = new StringBuilder();
            if (listW.Count > 0)
            {
                builder.Append(string.Format("\"{0}*\"", listW[0]));
                foreach (var item in listW.Skip(1))
                {
                    builder.Append(string.Format(" OR \"{0}*\"", item));
                }
            }

            return builder.ToString();
        }


        private string GetEmailsCriteria(string keyWordCriteria)
        {
            var emailPart = CriteriaHelpers.Instance.GetFieldCriteriaForEmail(_to);
            var namePart = CriteriaHelpers.Instance.GetFieldCriteriaForName(_name, _to);

            var searchedCriteria = string.Empty;
            if (!string.IsNullOrEmpty(emailPart))
            {
                searchedCriteria = string.Format("Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}')  ", //OR CONTAINS(*,'{0}',1033)
                    emailPart);
            }

            if (!string.IsNullOrEmpty(searchedCriteria) && !string.IsNullOrEmpty(namePart) && !string.IsNullOrEmpty(keyWordCriteria))
            {
                const string template = "( ({0} OR {1}) )"; // AND  Contains(*,'{2}')
                var nameCriteriaPart = string.Format("Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}') ",
                    namePart);
                return string.Format(template, nameCriteriaPart, searchedCriteria, keyWordCriteria);
            }
            if (!string.IsNullOrEmpty(searchedCriteria) && !string.IsNullOrEmpty(namePart))
            {
                const string template = "( {0} OR {1} )";
                var nameCriteriaPart = string.Format("Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}') ",
                    namePart);
                return string.Format(template, nameCriteriaPart, searchedCriteria);
            }
            if (!string.IsNullOrEmpty(namePart) && !string.IsNullOrEmpty(keyWordCriteria))
            {
                var nameCriteriaPart = string.Format("Contains(System.Message.FromName,'{0}') OR Contains(System.Message.ToName,'{0}') OR  Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcName,'{0}') OR  Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccName,'{0}') OR  Contains(System.Message.BccAddress,'{0}') ",
                        namePart);
                return string.Format("( ({0})  )", nameCriteriaPart, keyWordCriteria);//AND  Contains(*,'{1}')
            }
            if (!string.IsNullOrEmpty(namePart))
            {
                var nameCriteriaPart = string.Format("Contains(System.Message.FromName,'{0}') OR Contains(System.Message.ToName,'{0}') OR  Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcName,'{0}') OR  Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccName,'{0}') OR  Contains(System.Message.BccAddress,'{0}') ",
                        namePart);
                return string.Format("( {0} )", nameCriteriaPart);
            }
            return string.Format("( {0} )", searchedCriteria);
        }

    }
}