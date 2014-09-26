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
            var CurrentUserInfo = OutlookHelper.Instance.GetCurrentyUserEmail();


            var criteriaCurrentUserName = CriteriaHelpers.Instance.GetFieldCriteriaForName(CurrentUserInfo.Item1,CurrentUserInfo.Item2);
            var criteriaCurrentUserEmail = CriteriaHelpers.Instance.GetFieldCriteriaForEmail(CurrentUserInfo.Item2);
            
            var criteriaForField = CriteriaHelpers.Instance.GetFieldCriteriaForEmail(_to);
            var criteriaForName = CriteriaHelpers.Instance.GetFieldCriteriaForName(_name, _to);
            return
                string.Format(
                "( (Contains(System.Message.ToAddress,'{0}') OR Contains(System.Message.FromAddress,'{0}') OR Contains(System.Message.CcAddress,'{0}') OR Contains(System.Message.BccAddress,'{0}') " +
                    " OR " +
                    "Contains(System.Message.ToAddress,'{1}') OR Contains(System.Message.FromAddress,'{1}') OR Contains(System.Message.CcAddress,'{1}') OR Contains(System.Message.BccAddress,'{1}') )" +

                    " AND (" +

                    "Contains(System.Message.ToAddress,'{2}') OR Contains(System.Message.FromAddress,'{2}') OR Contains(System.Message.CcAddress,'{2}') OR Contains(System.Message.BccAddress,'{2}') " +
                    " OR " +
                    "Contains(System.Message.ToAddress,'{3}') OR Contains(System.Message.FromAddress,'{3}') OR Contains(System.Message.CcAddress,'{3}') OR Contains(System.Message.BccAddress,'{3}') ))",
                    criteriaCurrentUserName,criteriaCurrentUserEmail,
                    criteriaForField,criteriaForName);
        }

    }
}