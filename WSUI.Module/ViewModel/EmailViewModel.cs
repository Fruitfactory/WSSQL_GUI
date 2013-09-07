using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Attributes;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Module.Service;
using WSUI.Module.Strategy;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Email,2)]
    public class EmailViewModel : KindViewModelBase, IUView<EmailViewModel>, IScrollableView
    {
        
        private const string OrderTemplate = " ORDER BY System.Message.DateReceived DESC";//, System.Search.EntryID DECS
        private const string FilterByFolder = " AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";
        private const int CountFirstProcess = 45;
        private const int CountSecondAndOtherProcess = 7;
        private readonly List<string> _listID = new List<string>();
        private readonly List<EmailGroupData> _listEmails = new List<EmailGroupData>(); 
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
//            QueryTemplate = "GROUP ON TOP {3} System.Message.ConversationID OVER( SELECT TOP 75  System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) {4})";

            QueryTemplate = " SELECT TOP {3}  System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) {4}";


            QueryAnd = " AND \"{0}\"";
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            Folder = OutlookHelper.AllFolders;
            TopQueryResult = 50;
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
        }

        protected override void ReadData(IDataReader reader)
        {
            EmailGroupData item = new EmailGroupData();
            ReadGroupData(reader, item);
            if (item != null && !string.IsNullOrEmpty(item.Subject))
            {
                _listEmails.Add(item);
            }
        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            IsInterupt = false;
            var folder = Folder;
            var searchCriteria = SearchString.Trim();
            string res = string.Empty;

            ProcessSearchCriteria(searchCriteria);

            res = string.Format(QueryTemplate, folder != OutlookHelper.AllFolders ? string.Format(FilterByFolder, folder) : string.Empty, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause, FormatDate(ref _lastDate), TopQueryResult, OrderTemplate);

            return res;

        }

        protected override void OnInit()
        {
            base.OnInit();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 300,CountSecondProcess = 100 ,LimitReaction = 85};
            ScrollBehavior.SearchGo += () =>
                                           {
                                               ShowMessageNoMatches = false;
                                               Search();
                                               
                                           };
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        protected override void OnSearchStringChanged()
        {
            base.OnSearchStringChanged();
            ClearDataSource();
            ClearMainDataSource();
            _countProcess = ScrollBehavior.CountFirstProcess;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            lock (_lock)
                _listID.Clear();
            _lastDate = DateTime.Now;
            ShowMessageNoMatches = true;
        }

        protected override void OnFilterData()
        {
            _countProcess = ScrollBehavior.CountFirstProcess;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            lock(_lock)
                _listID.Clear();
            _lastDate = DateTime.Now;
            ShowMessageNoMatches = true;
            base.OnFilterData();
        }

        protected override void OnComplete(bool res)
        {
            var watchGroup = new Stopwatch();
            watchGroup.Start();
            var groups = _listEmails.GroupBy(e => e.ConversationId);
            lock (_lock)
            {
                foreach (var group in groups)
                {
                    var data = group.FirstOrDefault();
                    if (string.IsNullOrEmpty(data.ConversationId) || _listID.Any(s => s == data.ConversationId))
                        continue;
                    _listID.Add(data.ConversationId);
                    string tag = string.Empty;
                    EmailSearchData newValue = new EmailSearchData()
                    {
                        Subject = data.Subject,
                        ConversationId = data.ConversationId,
                        Count = group.Count().ToString(),
                        Date = data.DateReceived,
                        DateModified = data.DateReceived,
                        Recepient = data.ToAddress != null && data.ToAddress.Length > 0 ? data.ToAddress[0] : string.Empty,
                        Display = data.ItemName,
                        Path = data.ItemUrl,
                        ID = Guid.NewGuid(),
                        Name = data.Subject
                    };

                    TypeSearchItem type = SearchItemHelper.GetTypeItem(data.ItemUrl);
                    newValue.Type = type;
                    ListData.Add(newValue);
                    _countAdded++;
                }
                if (_listEmails.Count > 0)
                    _lastDate = _listEmails[_listEmails.Count - 1].DateReceived;
            }
            watchGroup.Stop();
            WSSqlLogger.Instance.LogInfo("Grouping (Email) Elapsed: " + watchGroup.ElapsedMilliseconds.ToString());
            base.OnComplete(res);
            TopQueryResult = ScrollBehavior.CountSecondProcess;
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

        class EmailGroupData : ISearchData
        {
            [FieldIndex(0)]
            public string Subject { get; set; }
            [FieldIndex(1)]
            public string ItemName { get; set; }
            [FieldIndex(2)]
            public string ItemUrl { get; set; }
            [FieldIndex(3)]
            public string[] ToAddress { get; set; }
            [FieldIndex(4)]
            public DateTime DateReceived { get; set; }
            [FieldIndex(5)]
            public string ConversationId { get; set; }
        }

    }
}
