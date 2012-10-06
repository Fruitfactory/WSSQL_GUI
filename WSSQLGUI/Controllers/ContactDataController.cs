using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core.CommandManager;
using MVCSharp.Core.Tasks;
using MVCSharp.Core.Configuration.Tasks;
using WSSQLGUI.Core;
using WSSQLGUI.Models;
using WSSQLGUI.Services.Helpers;
using WSSQLGUI.Views;

namespace WSSQLGUI.Controllers
{
    internal class ContactDataController : BaseDataController
    {
        protected DelegateCommand _createNewMail;
        protected DelegateCommand _createNewMail2;
        protected DelegateCommand _createNewMail3;


        public ICommand NewMailCommand
        {
            get
            {
                if(_createNewMail == null)
                    _createNewMail = new DelegateCommand("Create Email",CanExecute,Execute1);
                return _createNewMail;
            }
        }

        public ICommand NewMailCommand2
        {
            get
            {
                if (_createNewMail2 == null)
                    _createNewMail2 = new DelegateCommand("Create Email", CanExecute2, Execute2);
                return _createNewMail2;

            }
        }

        public ICommand NewMailCommand3
        {
            get
            {
                if (_createNewMail3 == null)
                    _createNewMail3 = new DelegateCommand("Create Email", CanExecute3, Execute3);
                return _createNewMail3;

            }
        }



        public override void SetData(BaseSearchData item)
        {
            if (item == null || !(item is EmailSearchData))
                return;
            EmailSearchData email = item as EmailSearchData;
            var value = new object[] { email.Recepient, email.Subject, email.Date };
            (View as IDataView).SetData(value, email);
        }

        public void SetContactData(ContactSearchData data)
        {
            if(View == null)
                return;
            (View as ContactDataView).SetContact(data);
        }



        #region private

        private bool CanExecute()
        {
            if (View == null || (View as IContactDataView).CurrentContact ==  null)
                return false;
            return !string.IsNullOrEmpty((View as IContactDataView).CurrentContact.EmailAddress);
        }

        private void Execute(string emailAddress)
        {
            var contactView = View as IContactDataView;

            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = emailAddress;
            email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);
        }

        private bool CanExecute2()
        {
            if (View == null || (View as IContactDataView).CurrentContact == null)
                return false;
            return !string.IsNullOrEmpty((View as IContactDataView).CurrentContact.EmailAddress2);
        }

        private bool CanExecute3()
        {
            if (View == null || (View as IContactDataView).CurrentContact == null)
                return false;
            return !string.IsNullOrEmpty((View as IContactDataView).CurrentContact.EmailAddress3);
        }

        private void Execute1()
        {
            Execute((View as IContactDataView).CurrentContact.EmailAddress);
        }

        private void Execute2()
        {
            Execute((View as IContactDataView).CurrentContact.EmailAddress2);
        }

        private void Execute3()
        {
            Execute((View as IContactDataView).CurrentContact.EmailAddress3);
        }


        #endregion


    }
}
