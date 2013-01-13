using System;
using System.Collections.Generic;
using System.Data;
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
using WSUI.Module.Strategy;
using System.Collections.ObjectModel;

namespace WSUI.Module.ViewModel
{
    [KindNameId("Email",2)]
    public class EmailViewModel : KindViewModelBase, IUView<EmailViewModel>, IScrollableView
    {
        private const string OrderTemplate = " ORDER BY System.Message.DateReceived DESC)";
        private const string FilterByFolder = " AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";
        private const int CountFirstProcess = 45;
        private const int CountSecondAndOtherProcess = 7;
        private readonly List<string> _listID = new List<string>();
        private int _lastId;
        private int _countAdded;
        private int _countProcess;

        public ICommand ScrollChangeCommand { get; protected set; }

        public EmailViewModel(IUnityContainer container, ISettingsView<EmailViewModel> settingsView, IDataView<EmailViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            QueryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email' AND System.Search.EntryID > {2} {0}AND CONTAINS(*,{1}) ";//¬ход€щие  //Inbox
            QueryAnd = " AND \"{0}\"";
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            DataSourceMail = new ObservableCollection<EmailSearchData>();
            Folder = OutlookHelper.AllFolders;
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
        }


        public ObservableCollection<EmailSearchData> DataSourceMail { get; private set; }


        protected override void ReadData(IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return;
            var item = groups.Items[0];

            if (_listID.Any(s => s == item.ConversationIndex))
                return;
            TypeSearchItem type = SearchItemHelper.GetTypeItem(item.Path);
            EmailSearchData si = new EmailSearchData()
            {
                Subject = item.Subject,
                Recepient = string.Format("{0}",
                item.Recepient),
                Count = groups.Items.Count.ToString(),
                Name = item.Name,
                Path = item.Path,
                Date = item.Date,
                Type = type,
                ID = Guid.NewGuid()
            };
            try
            {
                si.Attachments = OutlookHelper.Instance.GetAttachments(item);
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(e.Message);
            }
            
            int.TryParse(item.LastId, out _lastId);

            //TODO: paste item to datacontroller;
            _listID.Add(item.ConversationIndex);
            ListData.Add(si);
            _countAdded++;
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

            res = string.Format(QueryTemplate, folder != OutlookHelper.AllFolders ? string.Format(FilterByFolder, folder) : string.Empty, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause, _lastId) + OrderTemplate;

            return res;

        }

        protected override void OnInit()
        {
            base.OnInit();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
        }

        protected override void OnSearchStringChanged()
        {
            base.OnSearchStringChanged();
            ClearDataSource();
            ClearMainDataSource();
            _countProcess = CountFirstProcess;
            _listID.Clear();
            _lastId = 0;
            ShowMessageNoMatches = true;
        }

        protected override void OnFilterData()
        {
            _countProcess = CountFirstProcess;
            _listID.Clear();
            _lastId = 0;
            ShowMessageNoMatches = true;
            base.OnFilterData();
        }

        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            _countProcess = CountSecondAndOtherProcess;
        }

        protected override void OnStart()
        {
            //base.OnStart();
            //ClearDataSource();
            //ClearMainDataSource();
            DataSourceMail.Clear();
            OnPropertyChanged(() => DataSourceMail);
            ListData.Clear();

            FireStart();
            //Enabled = false;
            //OnPropertyChanged(() => Enabled);
        }

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            System.Diagnostics.Debug.WriteLine(scrollArgs.ToString());
            var result = scrollArgs.VerticalOffset * 100 / scrollArgs.ScrollableHeight;
            if (result > 75)
            {
                ShowMessageNoMatches = false;
                Search();
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

    }
}
