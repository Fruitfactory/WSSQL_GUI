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
    [KindNameId("People", 1)]
    public class ContactViewModel : KindViewModelBase,IUView<ContactViewModel>, IScrollableView
    {
        private const string QueryContactEmail = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' {0}AND CONTAINS(System.Message.FromAddress,'{1}*')  ORDER BY System.Message.DateReceived DESC) ";
        private const string FilterByFolder = "AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        private string _currentEmail = string.Empty;
        private string _folder = string.Empty;
        private ContactSearchData _contactData = null;
        private string _queryByAddress =
            " OR (System.Kind = 'email' AND System.Message.FromAddress LIKE '%{0}%')"; //CONTAINS(System.Message.FromAddress,'*{0}*')  AND CONTAINS(System.ItemPathDisplay,'{0}*',1033))

        private string _queryContactWhere =
            " (System.Kind = 'contact' AND {1}( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') ){2}";
        private ContactSuggestingService _contactSuggesting;

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

            var group = new List<BaseSearchData>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).ToString() != "System.Data.IDataReader")
                    continue;
                OleDbDataReader itemsReader = reader.GetValue(i) as OleDbDataReader;

                while (itemsReader.Read())
                {
                    var item = ReadGroupData(itemsReader);
                    if (item != null)
                        group.Add(item);
                }
            }

            if (group.Count > 0)
            {
                ProcessContactData(group.OfType<ContactSearchData>());
                ProcessEmailData(group.OfType<EmailSearchData>());
            }
            if (_countAdded == _countProcess)
                IsInterupt = true;
        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            var searchCriteria = SearchString.Trim();
            string res = String.Empty;

            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder strBuilder = new StringBuilder();
                var arr = searchCriteria.Split(' ').ToList();
                if (arr == null || arr.Count == 1)
                {
                    var where1 = string.Format(_queryContactWhere, arr[0], "", ")");
                    return string.Format("{0}{1}", QueryTemplate, where1) + string.Format(_queryByAddress, arr[0]);
                }
                var address = new StringBuilder(string.Format(_queryByAddress, arr[0]));
                var where2 = string.Format(_queryContactWhere, arr[0], "(", "");
                res += QueryTemplate + where2;
                for (int i = 1; i < arr.Count; i++)
                {
                    strBuilder.Append(string.Format(QueryAnd, arr[i]));
                    address.Append(string.Format(_queryByAddress, arr[i]));
                }
                res += strBuilder.ToString() + address.ToString();
            }
            else
            {
                var where = string.Format(_queryContactWhere, searchCriteria, "", ")");
                res = string.Format("{0}{1}", QueryTemplate, where) + string.Format(_queryByAddress, searchCriteria);
            }
            res += string.Format( " AND System.Message.DateReceived < '{0}'  ORDER BY System.DateCreated DESC )", FormatDate(ref _lastDate));
            return res;
        }

        protected override void OnComplete(bool res)
        {
            ListData.OrderBy(b => b.Type);
            base.OnComplete(res);
            OnPropertyChanged(() => Contact);
            OnPropertyChanged(() => Visible);
            _countProcess = ScrollBehavior.CountSecondProcess;
        }

        protected override void OnSearchStringChanged()
        {
            _countProcess = ScrollBehavior.CountFirstProcess;
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
            _lastDate = DateTime.Now;
        }

        private void GetEmailsForContact(ContactSearchData data)
        {
            if (string.IsNullOrEmpty(_currentEmail))
            {
                WSSqlLogger.Instance.LogWarning("Email address not found.");
                base.DoAdditionalQuery();
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
            ScrollBehavior = new ScrollBehavior() { CountFirstProcess = 45, CountSecondProcess = 5, LimitReaction = 75 };
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


        private BaseSearchData ReadGroupData(IDataReader reader)
        {
            if (reader == null)
                return null;
            string itemname = reader[0].ToString();
            string first = reader[1].ToString();
            string last = reader[2].ToString();
            string em1 = reader[3].ToString();
            string em2 = reader[4].ToString();
            string em3 = reader[5].ToString();
            string subject = reader[6].ToString();
            string url = reader[7].ToString();
            var to = reader[8] as string[];
            string d = reader[9].ToString();
            DateTime date = DateTime.Today;
            DateTime.TryParse(d, out date);
            var kind = reader[10] as string[];
            var from = reader[11] as string[];
            var created = reader[12].ToString();
            DateTime.TryParse(created, out _lastDate);



            switch (kind[0])
            {
                case "contact":
                    ContactSearchData data = new ContactSearchData()
                    {
                        Name = itemname,
                        Path = string.Empty,
                        FirstName = first,
                        LastName = last,
                        ID = Guid.NewGuid(),
                        Type = TypeSearchItem.Contact
                    };
                    data.EmailList.Add(em1);
                    data.EmailList.Add(em2);
                    data.EmailList.Add(em3);
                    //_currentEmail = em1;
                    data.Foto = OutlookHelper.Instance.GetContactFotoTempFileName(data);
                    //_contactData = data;
                    //GetEmailsForContact(data);

                    return data;
                case "email":
                    string fromAddress = GetEmailAddress(from);
                    if (string.IsNullOrEmpty(fromAddress))
                        break;
                    EmailSearchData si = new EmailSearchData()
                    {
                        Subject = subject,
                        Recepient = string.Format("{0}",
                        to != null && to.Length > 0 ? to[0] : string.Empty),
                        Name = itemname,
                        Path = url,
                        Date = date,
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
