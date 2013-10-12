using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using MahApps.Metro.Controls;
using Microsoft.Practices.Prism.Commands;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Attributes;
using WSUI.Infrastructure.Implements.Systems;
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
    [KindNameId(KindsConstName.Email,0)]
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
        private object _lock = new object();

        public ICommand ScrollChangeCommand { get; protected set; }

        public EmailViewModel(IUnityContainer container, ISettingsView<EmailViewModel> settingsView, IDataView<EmailViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            QueryTemplate = " SELECT TOP {3}  System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) {4}";
            QueryAnd = " AND \"{0}\"";
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            Folder = OutlookHelper.AllFolders;
            TopQueryResult = 50;
            SearchSystem = new EmailSearchSystem();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
        }

       

        protected override void OnInit()
        {
            base.OnInit();
            SearchSystem.Init();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 300,CountSecondProcess = 100 ,LimitReaction = 85};
            ScrollBehavior.SearchGo += OnScroolNeedSearch;
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
            ShowMessageNoMatches = true;
        }

        protected override void OnFilterData()
        {
            _countProcess = ScrollBehavior.CountFirstProcess;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            lock(_lock)
                _listID.Clear();
            ShowMessageNoMatches = true;
            base.OnFilterData();
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
