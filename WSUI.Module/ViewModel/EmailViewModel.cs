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
        private const string OrderTemplate = " ORDER BY System.Message.DateReceived DESC";//, System.Search.EntryID DECS
        private const string FilterByFolder = " AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";
        private const int CountFirstProcess = 45;
        private const int CountSecondAndOtherProcess = 7;
        private readonly List<string> _listID = new List<string>();
        private readonly List<EmailSearchData> _listEmails = new List<EmailSearchData>(); 
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

            //QueryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email' AND System.Search.EntryID > {2} {0}AND CONTAINS(*,{1}) ";//¬ход€щие  //Inbox
            QueryTemplate = "SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) ";//¬ход€щие  //Inbox
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
            //var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            //if (groups == null)
            //    return;
            //var item = groups.Items[0];

            //if (_listID.Any(s => s == item.ConversationIndex))
            //    return;
            //TypeSearchItem type = SearchItemHelper.GetTypeItem(item.Path);
            //EmailSearchData si = new EmailSearchData()
            //{
            //    Subject = item.Subject,
            //    Recepient = string.Format("{0}",
            //    item.Recepient),
            //    Count = groups.Items.Count.ToString(),
            //    Name = item.Name,
            //    Path = item.Path,
            //    Date = item.Date,
            //    DateModified = item.Date,
            //    Type = type,
            //    ID = Guid.NewGuid()
            //};
            //try
            //{
            //    si.Attachments = OutlookHelper.Instance.GetAttachments(item);
            //}
            //catch (Exception e)
            //{
            //    WSSqlLogger.Instance.LogError(e.Message);
            //}
            
            //int.TryParse(item.LastId, out _lastId);


            var item = GetData(reader);
          

            //TODO: paste item to datacontroller;
            
            //ListData.Add(item);
            _listEmails.Add(item);
            _countAdded = _listEmails.GroupBy(e => e.ConversationIndex).Count();
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
        }

        protected override void OnSearchStringChanged()
        {
            base.OnSearchStringChanged();
            ClearDataSource();
            ClearMainDataSource();
            _countProcess = CountFirstProcess;
            lock (_lock)
                _listID.Clear();
            _lastDate = DateTime.Now;
            ShowMessageNoMatches = true;
        }

        protected override void OnFilterData()
        {
            _countProcess = CountFirstProcess;
            lock(_lock)
                _listID.Clear();
            _lastDate = DateTime.Now;
            ShowMessageNoMatches = true;
            base.OnFilterData();
        }

        protected override void OnComplete(bool res)
        {

            var groups = _listEmails.GroupBy(e => e.ConversationIndex);
            lock (_lock)
            {
                foreach (var group in groups)
                {
                    var email = group.OrderByDescending(e => e.Date).FirstOrDefault();

                    if (_listID.Any(s => s == email.ConversationIndex))
                        continue;
                    _listID.Add(email.ConversationIndex);
                    email.Type = SearchItemHelper.GetTypeItem(email.Path);
                    try
                    {
                        email.Attachments = OutlookHelper.Instance.GetAttachments(email);
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError(ex.Message);
                    }
                    email.Count = group.Count().ToString();
                    ListData.Add(email);
                }
            }
            base.OnComplete(res);
            _countProcess = CountSecondAndOtherProcess;
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
            System.Diagnostics.Debug.WriteLine(scrollArgs.ToString());
            var result = scrollArgs.VerticalOffset * 100 / scrollArgs.ScrollableHeight;
            if (result > 95)
            {
                ShowMessageNoMatches = false;
                Search();
            }
        }

        private string FormatDate(ref DateTime date)
        {
            return date.ToString("yyyy/MM/dd hh:mm:ss").Replace('.', '/');
        }

        private EmailSearchData GetData(IDataReader reader)
        {
            string subject = reader[0].ToString();
            string name = reader[1].ToString();
            var recArr = reader[3] as string[];
            string recep = string.Empty;
            if (recArr != null && recArr.Length > 0)
            {
                recep = recArr[0];
            }
            string url = reader[2].ToString();

            var datetime = reader[4];
            DateTime res;
            DateTime.TryParse(datetime.ToString(), out res);

            string conversationid = reader[5].ToString();
            string conversationIndex = reader[6].ToString();
            string entryId = string.Empty;
            if (reader.FieldCount > 7)
                entryId = reader[7].ToString();
            var item = new EmailSearchData()
                           {
                               Subject = subject,
                               Name = name,
                               Recepient = recep,
                               Path = url,
                               Date = res,
                               DateModified = res,
                               ID = Guid.NewGuid(),
                               ConversationId = conversationid,
                               ConversationIndex = OutlookHelper.Instance.EIDFromEncodeStringWDS30(conversationIndex)
                           };
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

    }
}
