using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using WSPreview.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Infrastructure.Models;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.People, 1)]
    public class ContactViewModel : KindViewModelBase,IUView<ContactViewModel>, IScrollableView
    {
        private const string QueryContactEmail = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' {0}AND CONTAINS(System.Message.FromAddress,'\"*{1}*\"')  ORDER BY System.Message.DateReceived DESC) ";
        private const string FilterByFolder = "AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";
        private const string EmailPattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";//@"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        private string _currentEmail = string.Empty;
        private string _folder = string.Empty;
        private ContactSearchData _contactData = null;
        private ContactSuggestingService _contactSuggesting;

        private readonly List<BaseSearchData> _listContacts = new List<BaseSearchData>(); 

        private int _countAdded = 0;
        private int _countProcess;
        private DateTime _lastDate;



        public ContactViewModel(IUnityContainer container, ISettingsView<ContactViewModel> settingsView, IDataView<ContactViewModel> dataView)
            :base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            QueryTemplate = "GROUP ON System.DateCreated  ORDER BY System.DateCreated DESC  OVER ( SELECT System.ItemName, System.Contact.FirstName, System.Contact.LastName,System.Contact.EmailAddress,System.Contact.EmailAddress2,System.Contact.EmailAddress3, System.Subject,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Kind,System.Message.FromAddress, System.DateCreated  FROM SystemIndex WHERE ";
            QueryAnd = " OR ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') ))) ";
            ID = 1;
            _name = "People";
            UIName = _name;
            _prefix = "Contact";

            Folder = OutlookHelper.AllFolders;

            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
            _contactSuggesting = new ContactSuggestingService();
            _contactSuggesting.Suggest += (o, e) =>
                                              {
                                                  DataSourceSuggest = new ObservableCollection<string>();
                                                  if (e.Value != null)
                                                  {
                                                      Application.Current.Dispatcher.BeginInvoke(
                                                          new Action(
                                                              () =>
                                                                  {
                                                                      e.Value.ForEach(s => DataSourceSuggest.Add(s));
                                                                      OnPropertyChanged(() => DataSourceSuggest);
                                                                  }),
                                                          null);
                                                      
                                                  }
                                              };
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            _lastDate = DateTime.Now;
        }

        public ICommand EmailClickCommand { get; protected set; }

        public ContactSearchData Contact
        {
            get { return _contactData; }
        }

        public ObservableCollection<string> DataSourceSuggest { get; set; }

        public Visibility Visible
        {
            get { return _contactData != null ? Visibility.Visible : Visibility.Collapsed; }
        }

        protected override void ReadData(IDataReader reader)
        {
            var item = ReadContactData(reader);
            _listContacts.Add(item);
        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            string res = ContactHelpers.GetContactQuery(SearchString.Trim(), FormatDate(ref _lastDate), TopQueryResult);
            return res;
        }

        protected override void OnStart()
        {
            _listContacts.Clear();
            base.OnStart();
        }

        protected override void OnComplete(bool res)
        {
            if (_listContacts.Count > 0)
            {
                var list = _listContacts.OfType<BaseSearchData>().ToList();
                ProcessContactData(list);
                if(list.Count > 0)
                    _lastDate = list.ElementAt(list.Count - 1).DateModified;
            }

            ListData.OrderBy(b => b.Type);
            base.OnComplete(res);
            OnPropertyChanged(() => Contact);
            OnPropertyChanged(() => Visible);
            _countProcess = ScrollBehavior.CountSecondProcess;
            TopQueryResult = ScrollBehavior.CountSecondProcess;
        }

        protected override void OnSearchStringChanged()
        {
            _countProcess = ScrollBehavior.CountFirstProcess;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            _lastDate = DateTime.Now;
            if (string.IsNullOrEmpty(SearchString))
                return;
            ClearDataSource();
            //_contactSuggesting.StartSuggesting(SearchString);
        }

        protected override void OnFilterData()
        {
            base.OnFilterData();
            _countProcess = ScrollBehavior.CountFirstProcess;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            _lastDate = DateTime.Now;
        }
        private void EmailClick (object address)
        {
            string adr = string.Empty;
            if (address is string)
                adr = (string) address;
            else if(address is EmailSearchData)
            {
                adr = (address as EmailSearchData).From;
            }
            else if (address is ContactSearchData)
            {
                var contact = (ContactSearchData) address;
                adr = contact.EmailList != null && contact.EmailList.Count > 0 ? contact.EmailList[0] : string.Empty;
            }

            if(string.IsNullOrEmpty(adr))
                return;
            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = adr;
            email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);
        }

        protected override void OnInit()
        {
            base.OnInit();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ScrollBehavior = new ScrollBehavior() { CountFirstProcess = 400, CountSecondProcess = 100, LimitReaction = 99 };
            ScrollBehavior.SearchGo += () =>
            {
                ShowMessageNoMatches = false;
                Search();
            };
        }


        #region IUIView

        public ISettingsView<ContactViewModel> SettingsView
        {
            get; set;
        }

        public IDataView<ContactViewModel> DataView
        {
            get; set;
        }

        #endregion
        #region Implementation of IScrollableView

        public ICommand ScrollChangeCommand { get; private set; }

        #endregion

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }


        private BaseSearchData ReadContactData(IDataReader reader)
        {
            if (reader == null)
                return null;
            ContactItem item = new ContactItem();
            ReadGroupData(reader, item);


            switch (item.Kind[0])
            {
                case "contact":
                    ContactSearchData data = new ContactSearchData()
                    {
                        Name = item.EmailAddress,
                        Path = string.Empty,
                        FirstName = item.FirstName,
                        LastName = item.LastName,
                        ID = Guid.NewGuid(),
                        Type = TypeSearchItem.Contact
                    };
                    data.EmailList.Add(item.EmailAddress);
                    data.EmailList.Add(item.EmailAddress2);
                    data.EmailList.Add(item.EmailAddress3);
                    data.Foto = OutlookHelper.Instance.GetContactFotoTempFileName(data);
                    return data;
                case "email":
                    string fromAddress = ContactHelpers.GetEmailAddress(item.FromAddress, SearchString) ?? ContactHelpers.GetEmailAddress(item.CcAddress, SearchString) ?? ContactHelpers.GetEmailAddress(item.ToAddress, SearchString);
                    if (string.IsNullOrEmpty(fromAddress))
                        break;
                    EmailSearchData si = new EmailSearchData()
                    {
                        Subject = item.Subject,
                        Recepient = string.Format("{0}",
                        item.ToAddress != null && item.ToAddress.Length > 0 ? item.ToAddress[0] : string.Empty),
                        Name = fromAddress,
                        Path = item.ItemUrl,
                        Date = item.DateReceived,
                        Count = string.Empty,
                        Type = TypeSearchItem.Contact,
                        ID = Guid.NewGuid(),
                        From = fromAddress,
                        Tag = "Click to email recipient"

                    };

                    return si;
            }
            return null;
        }

        private void ProcessContactData(IEnumerable<BaseSearchData> listData)
        {
            var groups = listData.GroupBy(c => !string.IsNullOrEmpty(c.Name) ?  c.Name.ToLower() : string.Empty );//

            foreach (var group in groups)
            {
                if (string.IsNullOrEmpty(group.Key) || !group.Any())
                    continue;
                ListData.Add(group.ElementAt(0));
                _countAdded++;
            }
        }

    }
}
