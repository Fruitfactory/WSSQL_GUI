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
using C4F.DevKit.PreviewHandler.Service.Logger;
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
        private string _queryByAddress =
            " OR (System.Kind = 'email' AND CONTAINS(System.Message.FromAddress, '\"*{0}*\"')"; //CONTAINS(System.Message.FromAddress,'*{0}*')  AND CONTAINS(System.ItemPathDisplay,'{0}*',1033))

        private string _queryContactWhere =
            " (System.Kind = 'contact' AND {1}( CONTAINS(System.Contact.FirstName,'\"*{0}*\"') OR CONTAINS(System.Contact.LastName,'\"*{0}*\"') ){2}";
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
                ProcessContactData(_listContacts.OfType<ContactSearchData>());
                ProcessEmailData(_listContacts.OfType<EmailSearchData>());
                var list = _listContacts.OfType<BaseSearchData>().ToList();
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

        private void GetEmailsForContact(ContactSearchData data)
        {
            if (string.IsNullOrEmpty(_currentEmail))
            {
                WSSqlLogger.Instance.LogWarning("Email address not found.");
                //base.DoAdditionalQuery();
                return;
            }
            if (!Regex.IsMatch(_currentEmail, EmailPattern, RegexOptions.IgnoreCase))
            {
                WSSqlLogger.Instance.LogWarning("Not Email address.");
                return;
            }
            _folder = Folder;
            var query = string.Format(QueryContactEmail, _folder != OutlookHelper.AllFolders ? string.Format(FilterByFolder,_folder) : string.Empty, _currentEmail);
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(ConnectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(query, myOleDbConnection);
            try
            {
                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();
                while (myDataReader.Read())
                {
                    data.Emails.Add(ReadContactEmail(myDataReader));
                }

            }
            finally
            {
                if (myDataReader != null)
                {
                    myDataReader.Close();
                    myDataReader.Dispose();
                }
                if (myOleDbConnection.State == System.Data.ConnectionState.Open)
                {
                    myOleDbConnection.Close();
                }

            }
        }


        private EmailSearchData ReadContactEmail(IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return null;
            var item = groups.Items[0];
            TypeSearchItem type = SearchItemHelper.GetTypeItem(item.Path);
            EmailSearchData si = new EmailSearchData()
            {
                Subject = item.Subject,
                Recepient = string.Format("{0}",
                item.Recepient),
                Name = item.Name,
                Path = item.Path,
                Date = item.Date,
                Count = groups.Items.Count.ToString(),
                Type = type,
                ID = Guid.NewGuid()
            };
            return si;
            //TODO: paste item to datacontroller;
            //_listData.Add(si);
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
            ScrollBehavior = new ScrollBehavior() { CountFirstProcess = 300, CountSecondProcess = 100, LimitReaction = 75 };
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


        private string GetEmailAddress(string[] from)
        {
            string fromAddress = string.Empty;
            if (from != null)
            {
                var arr = SearchString.Trim().Split(' ');
                if (arr != null && arr.Length > 0)
                {
                    foreach (var s in arr)
                    {
                        fromAddress =
                        from.FirstOrDefault(
                            str =>
                            str.IndexOf(s, StringComparison.CurrentCultureIgnoreCase) > -1 &&
                            Regex.IsMatch(str, EmailPattern, RegexOptions.IgnoreCase));
                        if (!string.IsNullOrEmpty(fromAddress))
                            break;
                    }
                }
                else
                    fromAddress =
                        from.FirstOrDefault(
                            str =>
                            str.IndexOf(SearchString.Trim(), StringComparison.CurrentCultureIgnoreCase) > -1 &&
                            Regex.IsMatch(str, EmailPattern, RegexOptions.IgnoreCase));
            }
            return fromAddress;
        }

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
                        Name = item.ItemName,
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
                    string fromAddress = GetEmailAddress(item.FromAddress);
                    if (string.IsNullOrEmpty(fromAddress))
                        break;
                    EmailSearchData si = new EmailSearchData()
                    {
                        Subject = item.Subject,
                        Recepient = string.Format("{0}",
                        item.ToAddress != null && item.ToAddress.Length > 0 ? item.ToAddress[0] : string.Empty),
                        Name = item.ItemName,
                        Path = item.ItemUrl,
                        Date = item.DateReceived,
                        Count = string.Empty,
                        Type = TypeSearchItem.Email,
                        ID = Guid.NewGuid(),
                        From = fromAddress
                    };

                    return si;
            }
            return null;
        }

        private void ProcessContactData(IEnumerable<ContactSearchData> listData)
        {
            var groups = listData.GroupBy(c => c.LastName);

            foreach (var group in groups)
            {
                var item = group.ElementAt(0);
                _currentEmail = item.EmailList.Count > 0 ? item.EmailList[0] : string.Empty;
                _contactData = item;
                if(!string.IsNullOrEmpty(_currentEmail))
                    GetEmailsForContact(item);
                item.Count = group.Count().ToString();
                if (!DataSource.OfType<ContactSearchData>().Any(c => c.FirstName == item.FirstName && c.LastName == item.LastName))
                {
                    ListData.Add(item);
                    _countAdded++;
                }
            }
        }

        private void ProcessEmailData(IEnumerable<EmailSearchData> listData )
        {
            var groups = listData.GroupBy(e => e.Subject);
            foreach (var group in groups)
            {
                var item = group.ElementAt(0);
                item.Count = group.Count().ToString();
                ListData.Add(item);
                _countAdded++;
            }
        }
    }
}
