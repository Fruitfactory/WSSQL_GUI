using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Module.Service;
using WSUI.Module.Strategy;
using System.Collections.ObjectModel;

namespace WSUI.Module.ViewModel
{
    [KindNameId("Email",2)]
    public class EmailViewModel : KindViewModelBase, IUView<EmailViewModel>, IScrollableView
    {
        
        private const string OrderTemplate = " ORDER BY System.Message.DateReceived DESC";//, System.Search.EntryID DECS
        private const string FilterByFolder = " AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";
        private const int CountFirstProcess = 45;
        private const int CountSecondAndOtherProcess = 7;
        private readonly List<string> _listID = new List<string>();
        private readonly List<EmailLessData> _listEmails = new List<EmailLessData>(); 
        private int _countAdded;
        private int _countProcess;
        private DateTime _lastDate;
        private object _lock = new object();


        public ICommand ScrollChangeCommand { get; protected set; }

        public EmailViewModel(IUnityContainer container, ISettingsView<EmailViewModel> settingsView, IDataView<EmailViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            //QueryTemplate = "SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) ";//¬ход€щие  //Inbox
            QueryTemplate = "SELECT System.Message.ConversationID,System.Message.DateReceived FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) ";//¬ход€щие  //Inbox
            QueryAnd = " AND \"{0}\"";
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            Folder = OutlookHelper.AllFolders;
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
        }

        protected override void ReadData(IDataReader reader)
        {
            var item = GetData(reader);
          

            //TODO: paste item to datacontroller;
            
            //ListData.Add(item);
            _listEmails.Add(item);
            _countAdded = _listEmails.GroupBy(e => e.ConversationId).Count();
            if (_countAdded == _countProcess)
                IsInterupt = true;
        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            IsInterupt = false;
            var folder = Folder;
            var searchCriteria = SearchString.Trim();
            string res = string.Empty;

            ProcessSearchCriteria(searchCriteria);

            res = string.Format(QueryTemplate, folder != OutlookHelper.AllFolders ? string.Format(FilterByFolder, folder) : string.Empty, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause, FormatDate(ref _lastDate)) + OrderTemplate;

            return res;

        }

        protected override void OnInit()
        {
            base.OnInit();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 45,CountSecondProcess = 7 ,LimitReaction = 85};
            ScrollBehavior.SearchGo += () =>
                                           {
                                               ShowMessageNoMatches = false;
                                               Search();
                                               
                                           };
        }

        protected override void OnSearchStringChanged()
        {
            base.OnSearchStringChanged();
            ClearDataSource();
            ClearMainDataSource();
            _countProcess = ScrollBehavior.CountFirstProcess;
            lock (_lock)
                _listID.Clear();
            _lastDate = DateTime.Now;
            ShowMessageNoMatches = true;
        }

        protected override void OnFilterData()
        {
            _countProcess = ScrollBehavior.CountFirstProcess;
            lock(_lock)
                _listID.Clear();
            _lastDate = DateTime.Now;
            ShowMessageNoMatches = true;
            base.OnFilterData();
        }

        protected override void OnComplete(bool res)
        {

            var groups = _listEmails.GroupBy(e => e.ConversationId);
            lock (_lock)
            {
                foreach (var group in groups)
                {
                    var data = group.FirstOrDefault();
                    if (_listID.Any(s => s == data.ConversationId))
                        continue;
                    var email = EmailGroupReaderHelpers.GroupEmail(data.ConversationId);

                    _listID.Add(email.ConversationId);
                    email.Type = SearchItemHelper.GetTypeItem(email.Path);
                    WSSqlLogger.Instance.LogError(string.Format("ConversationIndex = {0}",email.ConversationId));
                    try
                    {
                        email.Attachments = OutlookHelper.Instance.GetAttachments(email);
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "OnComplete - Email", ex.Message));
                    }
                    ListData.Add(email);
                }
            }
            base.OnComplete(res);
            _countProcess = ScrollBehavior.CountSecondProcess;
        }

        protected override void OnStart()
        {
            ListData.Clear();
            _listEmails.Clear();
            FireStart();
        }

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }

        private EmailLessData GetData(IDataReader reader)
        {
            string conversationid = reader[0].ToString();
            var datetime = reader[1];
            DateTime res;
            DateTime.TryParse(datetime.ToString(), out res);

            var item = new EmailLessData() {ConversationId = conversationid};
            _lastDate = res;
            return item;
        }


        #region IUIView

        public ISettingsView<EmailViewModel> SettingsView
        {
            get;
            set;
        }

        public IDataView<EmailViewModel> DataView
        {
            get;
            set;
        }

        #endregion

        class EmailLessData
        {
            public string ConversationId { get; set; }
        }

    }
}
