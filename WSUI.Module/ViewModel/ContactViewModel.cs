using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Infrastructure.Models;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    public class ContactViewModel : KindViewModelBase,IUView<ContactViewModel>
    {
        private const string QueryContactEmail = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) AND CONTAINS(System.Message.FromAddress,'{1}*')  ORDER BY System.Message.DateReceived DESC) ";
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";
        private string _currentEmail = string.Empty;
        private string _folder = string.Empty;
        private ContactSearchData _contactData = null;
        private string _queryByAddress =
            " OR (System.Kind = 'email' AND System.Message.FromAddress LIKE '%{0}%')"; //CONTAINS(System.Message.FromAddress,'*{0}*')  AND CONTAINS(System.ItemPathDisplay,'{0}*',1033))

        private string _queryContactWhere =
            " (System.Kind = 'contact' AND {1}( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') ){2}";
        private ContactSuggestingService _contactSuggesting;
        

        public ContactViewModel(IUnityContainer container, ISettingsView<ContactViewModel> settingsView, IDataView<ContactViewModel> dataView)
            :base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            _queryTemplate = "SELECT TOP 10 System.ItemName, System.Contact.FirstName, System.Contact.LastName,System.Contact.EmailAddress,System.Contact.EmailAddress2,System.Contact.EmailAddress3, System.Subject,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Kind,System.Message.FromAddress FROM SystemIndex WHERE ";
            _queryAnd = " OR ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') ))) ";
            ID = 1;
            _name = "People";
            UIName = _name;
            _prefix = "Contact";
            
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
            if (reader == null)
                return;
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
                    _currentEmail = em1;
                    data.Foto = OutlookHelper.Instance.GetContactFotoTempFileName(data);
                    _contactData = data;
                    GetEmailsForContact(data);
                    _listData.Add(data);
                    break;
                case "email":
                    string fromAddress = string.Empty;
                    if(from != null)
                        fromAddress = from.FirstOrDefault(str => str.IndexOf(SearchString,StringComparison.CurrentCultureIgnoreCase) > -1 && Regex.IsMatch(str,EmailPattern,RegexOptions.IgnoreCase));
                    if (string.IsNullOrEmpty(fromAddress))
                        break;
                    EmailSearchData si = new EmailSearchData()
                    {
                        Subject = subject,
                        Recepient = string.Format("{0}",
                        to != null && to.Length > 0? to[0] : string.Empty),
                        Name = itemname,
                        Path = url,
                        Date = date,
                        Count = string.Empty,
                        Type = TypeSearchItem.Email,
                        ID = Guid.NewGuid(),
                        From = fromAddress
                    };
                    _listData.Add(si);
                    break;
            }

        }

        protected override string CreateQuery()
        {
            var searchCriteria = SearchString;
            string res = String.Empty;

            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder strBuilder = new StringBuilder();
                var arr = searchCriteria.Split(' ').ToList();
                if (arr == null || arr.Count == 1)
                {
                    var where1 = string.Format(_queryContactWhere, arr[0], "", ")");
                    return string.Format("{0}{1}", _queryTemplate, where1) + string.Format(_queryByAddress, arr[0]);
                }
                var where2 = string.Format(_queryContactWhere, arr[0], "(", "");
                res = string.Format(_queryTemplate, arr[0]);
                for (int i = 1; i < arr.Count; i++)
                {
                    strBuilder.Append(string.Format(_queryAnd, arr[i]));
                }
                res += strBuilder.ToString();
            }
            else
            {
                var where = string.Format(_queryContactWhere, SearchString, "", ")");
                res = string.Format("{0}{1}", _queryTemplate, where);
            }
            return res + string.Format(_queryByAddress, SearchString);
        }

        protected override void OnStart()
        {
            ClearDaraSource();
            if(_parentViewModel != null)
                _parentViewModel.MainDataSource.Clear();
            base.OnStart();
        }

        protected override void OnComplete(bool res)
        {
            _listData.OrderBy(b => b.Type);
            base.OnComplete(res);
            OnPropertyChanged(() => Contact);
            OnPropertyChanged(() => Visible);
        }

        protected override void OnSearchStringChanged()
        {
            if (string.IsNullOrEmpty(SearchString))
                return;
            _contactSuggesting.StartSuggesting(SearchString);
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
            var query = string.Format(QueryContactEmail, _folder, _currentEmail);
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(_connectionString);
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
            _commandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
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

    }
}
