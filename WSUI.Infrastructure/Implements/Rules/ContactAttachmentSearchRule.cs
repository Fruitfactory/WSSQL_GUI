using System.Collections.Generic;
using System.Linq;
using System.Text;
using WSUI.Core.Core.Rules;
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
        }

        protected override string OnGenerateWherePart(IList<IRule> listCriterisRules)
        {
            var dateString = FormatDate(ref LastDate);
            string addCritetis = GetContactCriteria();
            return string.Format(WhereTemplate, dateString, addCritetis);
        }


        private string GetContactCriteria()
        {
            var CurrentUserInfo = OutlookHelper.Instance.GetCurrentyUserInfo();


            var criteriaCurrentUserName = CriteriaHelpers.Instance.GetFieldCriteriaForName(CurrentUserInfo.Item1);
            var criteriaCurrentUserEmail = CriteriaHelpers.Instance.GetFieldCriteriaForEmail(CurrentUserInfo.Item2);
            
            var criteriaForField = CriteriaHelpers.Instance.GetFieldCriteriaForEmail(_to);
            var criteriaForName1 = CriteriaHelpers.Instance.GetFieldCriteriaForName(_name,_to);
            var criteriaForName2 = CriteriaHelpers.Instance.GetFieldCriteriaForName(_name);

            var searchedContactCtiteria = string.Format(
                    " Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}') ",
                    criteriaForField);

            if (!string.IsNullOrEmpty(criteriaForName1))
            {
                searchedContactCtiteria = string.Format("{0} OR Contains(System.Message.ToAddress,'{1}') OR Contains(System.Message.FromAddress,'{1}') OR Contains(System.Message.CcAddress,'{1}') OR Contains(System.Message.BccAddress,'{1}') ",
                    searchedContactCtiteria, criteriaForName1);
            }
            if (!string.IsNullOrEmpty(criteriaForName2))
            {
                searchedContactCtiteria = string.Format("{0} OR Contains(System.Message.ToName,'{1}') OR Contains(System.Message.FromName,'{1}') OR Contains(System.Message.CcName,'{1}') OR Contains(System.Message.BccName,'{1}') ",
                    searchedContactCtiteria, criteriaForName2);
            }

            searchedContactCtiteria = string.Format("( {0} OR CONTAINS(*,'{1}',1033))", searchedContactCtiteria, criteriaForField);

            const string templateTwo = "(( {0} OR {1} ) AND {2} )";
            const string templateOne = "(( {0}) AND {1} )";


            //if (!string.IsNullOrEmpty(criteriaCurrentUserName) && !string.IsNullOrEmpty(criteriaCurrentUserEmail))
            //{
            //    return string.Format(templateTwo, criteriaCurrentUserEmail, criteriaCurrentUserName, searchedContactCtiteria);
            //}
            //if (!string.IsNullOrEmpty(criteriaCurrentUserName))
            //{
            //    var temp = string.Format("Contains(System.Message.ToName,'{0}') OR Contains(System.Message.FromName,'{0}') OR Contains(System.Message.CcName,'{0}') OR Contains(System.Message.BccName,'{0}')",
            //        criteriaCurrentUserName);
            //    return string.Format(templateOne, temp, searchedContactCtiteria);
            //}
            //if (!string.IsNullOrEmpty(criteriaCurrentUserEmail))
            //{
            //    var temp = string.Format("Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}')",
            //        criteriaCurrentUserEmail);
            //    return string.Format(templateOne, temp, searchedContactCtiteria);
            //}
            return searchedContactCtiteria;
        }

    }
}