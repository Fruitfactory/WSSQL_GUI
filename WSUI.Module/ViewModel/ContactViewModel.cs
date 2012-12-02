using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
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
        private string _currentEmail = string.Empty;
        private string _folder = string.Empty;
        private ContactSearchData _contactData = null;

        private ContactSuggestingService _contactSuggesting;

        public ContactViewModel(IUnityContainer container, ISettingsView<ContactViewModel> settingsView, IDataView<ContactViewModel> dataView)
            :base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            _queryTemplate = "SELECT TOP 1 System.ItemName, System.Contact.FirstName, System.Contact.LastName,System.Contact.EmailAddress,System.Contact.EmailAddress2,System.Contact.EmailAddress3 FROM SystemIndex WHERE System.Kind = 'contact' AND ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') )";
            _queryAnd = " AND ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') ) ";
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


        protected override void DoAdditionalQuery()
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
                    ReadContactEmail(myDataReader);
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
            base.DoAdditionalQuery();
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
                    return string.Format(_queryTemplate, searchCriteria);
                res = string.Format(_queryTemplate, arr[0]);
                for (int i = 1; i < arr.Count; i++)
                {
                    strBuilder.Append(string.Format(_queryAnd, arr[i]));
                }
                res += strBuilder.ToString();
            }
            else
                res = string.Format(_queryTemplate, searchCriteria);

            return res;
        }

        protected override void OnStart()
        {
            ClearDaraSource();
            base.OnStart();
        }

        protected override void OnComplete(bool res)
        {
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

        private void ReadContactEmail(IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return;
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

            //TODO: paste item to datacontroller;
            _listData.Add(si);
        }

        private void EmailClick (object address)
        {
            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = (string)address;
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
