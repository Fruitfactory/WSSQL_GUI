using System.Collections.Generic;
using System.Text;
using WSUI.Core.Core.Rules;
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
            return
                string.Format(
                    "Contains(*,'\"{0}\"') AND Contains(*,'\"{1}\"') ",
                    _name, _to);

        }
    }
}