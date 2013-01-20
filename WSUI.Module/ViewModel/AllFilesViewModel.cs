using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Rules;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId("Everything",0)]
    public class AllFilesViewModel : KindViewModelBase, IUView<AllFilesViewModel>, IScrollableView
    {
        private const string KindGroup = "email";
        private DateTime _lastDate;
        private const string QueryForGroupEmails =
            "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email'  AND CONTAINS(System.Message.ConversationID,'{0}*')   ORDER BY System.Message.DateReceived DESC) ";//AND CONTAINS(System.ItemPathDisplay,'{0}*',1033)

        private const string OrLikeTemplate = " OR System.ItemName LIKE '%{0}%'";
        private const int CountFirstProcess = 35;
        private const int CountSecondAndOtherProcess = 7;
        private int _countAdded = 0;
        private List<string> _listID = new List<string>();
        private List<string> _listName = new List<string>();
        private string _folder = string.Empty;
        private readonly List<GroupData> _listEverething = new List<GroupData>();
        private int _countProcess;
        private string _lastName = string.Empty;
        private readonly object _lockObjcet = new object();

        public ICommand ScrollChangeCommand { get; protected set; }

        public AllFilesViewModel(IUnityContainer container, ISettingsView<AllFilesViewModel> settingsView,
                                 IDataView<AllFilesViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            // init
            QueryTemplate =
                "SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateCreated,System.Search.EntryID FROM SystemIndex WHERE System.Kind <> 'folder' AND System.DateCreated < '{1}' AND (Contains(System.Search.Contents,{0}) {2} ) ORDER BY System.DateCreated DESC";//OR (System.Kind == 'email' AND Contains(*,'{0}*'))  OR Contains(*,{0})   , System.Search.EntryID DESC
            QueryAnd = " AND \"{0}\""; //" AND \"{0}\"";
            ID = 0;
            _name = "Everything";
            UIName = _name;
            _prefix = "AllFiles";
            IsOpen = false;
            FlyCommand = new DelegateCommand<object>( o =>
                                                          {
                                                              IsOpen = !IsOpen;
                                                              OnPropertyChanged(() => IsOpen);
                                                          },
                                                          o => true);
            _lastDate = DateTime.Now;
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);

            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
        }

        
        public bool IsOpen { get; set; }
        public ICommand FlyCommand { get; private set; }
        public ICommand EmailClickCommand { get; protected set; }

        protected override void ReadData(IDataReader reader)
        {
           
            var item = ReadGroupData(reader);

            if(!string.IsNullOrEmpty(item.ID) && _listID.Any(id => id == item.ID))
                return;
            
            if(!string.IsNullOrEmpty(item.ID))
                _listID.Add(item.ID);

            _listEverething.Add(item);
            if (_lastName != item.Name)
            {
                _countAdded++;
                _lastName = item.Name;
            }

            if (_countAdded == _countProcess)
                IsInterupt = true;
        }


        private GroupData ReadGroupData(IDataReader reader)
        {
            string name = reader[0].ToString();
            string file = reader[1].ToString();
            var kind = reader[2] as object[];
            string id = reader[3].ToString();
            string display = reader[4].ToString();
            var date = reader[5].ToString();
            DateTime temp;
            DateTime.TryParse(date, out temp);
            _lastDate = temp;
            return new GroupData()
            {
                Name = name,
                File = file,
                Kind = kind,
                ID = id,
                Display = display,
                Date = temp
            };

        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            IsInterupt = false;

            var searchCriteria = SearchString.Trim();
            string res = string.Empty;
            
            ProcessSearchCriteria(searchCriteria);

            res = string.Format(QueryTemplate, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause, FormatDate(ref _lastDate), LikeCriteria());//LikeCriteria()

            return res;
        }

        private string LikeCriteria()
        {
            if (_listW.Count == 0)
                return string.Empty;
            var temp = new StringBuilder();
            _listW.ForEach(str => temp.Append(string.Format(OrLikeTemplate,str)));
            return temp.ToString();
        }

        protected override void OnStart()
        {
            ListData.Clear();
            _listEverething.Clear();
            FireStart();
            //base.OnStart();
        }

        protected override void OnComplete(bool res)
        {
            var groups = _listEverething.GroupBy(i => i.Name);
            lock (_lockObjcet)
            {
                foreach (var group in groups)
                {
                    var item = group.OrderByDescending(i => i.Date).FirstOrDefault();
                    int count = group.Count();
                    var groupItem = item;
                    string tag = string.Empty;
                    if (groupItem.Kind != null && IsEmail(groupItem.Kind)) // && !_listID.Any(it => it == groupItem.ID)
                    {
                        var newValue = GroupEmail(groupItem.Name, groupItem.ID);
                        if (newValue == null)
                            return;
                        //_listID.Add(groupItem.ID);
                        TypeSearchItem type = SearchItemHelper.GetTypeItem(groupItem.File);
                        newValue.Type = type;
                        ListData.Add(newValue);
                    }
                    else if (groupItem.Kind != null && IsEmail(value: groupItem.Kind) &&
                             _listID.Any(it => it == groupItem.ID))
                        return;
                    else if (!_listName.Any(it => it == groupItem.Name))
                    {
                        _listName.Add(groupItem.Name);
                        TypeSearchItem type = SearchItemHelper.GetTypeItem(groupItem.File,
                                                                           groupItem.Kind != null &&
                                                                           groupItem.Kind.Length > 0
                                                                               ? groupItem.Kind[0].ToString()
                                                                               : string.Empty);
                        BaseSearchData bs = new BaseSearchData()
                                                {
                                                    Name = groupItem.Name,
                                                    Path = groupItem.File,
                                                    Type = type,
                                                    ID = Guid.NewGuid(),
                                                    Display = groupItem.Display,
                                                    DateModified = groupItem.Date,
                                                    Tag = tag,
                                                    Count = count.ToString()
                                                };
                        ListData.Add(bs);
                    }
                }
            }
            base.OnComplete(res);
            _countProcess = CountSecondAndOtherProcess;
        }

        protected override void OnSearchStringChanged()
        {
            ClearSearchingInfo();
            ClearDataSource();
            OnPropertyChanged(() => DataSource);
            if (ParentViewModel != null && ParentViewModel.MainDataSource != null)
                ParentViewModel.MainDataSource.Clear();
            
        }

        protected override void OnFilterData()
        {
            ClearSearchingInfo();
            base.OnFilterData();
        }

        private void ClearSearchingInfo()
        {
            lock (_lockObjcet)
            {
                _lastDate = DateTime.Now;
                _countProcess = CountFirstProcess;
                _listID.Clear();
                _listName.Clear();
            }
        }

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }

        private EmailSearchData GroupEmail(string name, string id)
        {
            var query = string.Format(QueryForGroupEmails, id);
            EmailSearchData data = null;
            OleDbDataReader reader = null;
            OleDbConnection con = new OleDbConnection(ConnectionString);
            OleDbCommand cmd = new OleDbCommand(query, con);
            try
            {

                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data = ReadGroup(reader);
                    break;
                }
            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                WSSqlLogger.Instance.LogError(oleDbException.Message);
            }
            finally
            {
                // Always call Close when done reading.
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                // Close the connection when done with it.
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }

            }


            return data;
        }

        private EmailSearchData ReadGroup(IDataReader reader)
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
            return si;
        }

        private bool IsEmail(object value)
        {
            if (value.GetType().IsArray)
            {
                return (value as Array).Cast<string>().Any(i => i.IndexOf(KindGroup) > -1);
            }
            return false;
        }

        private void EmailClick(object obj)
        {
            var data = obj as BaseSearchData;
            var ci = OutlookHelper.Instance.GetContact(data.Name);
            if (ci == null || ci.Email1Address == null 
                || ci.Email1Address.Length == 0)
                return;
            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = (string)ci.Email1Address;
            email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);
        }


        protected override void OnInit()
        {
            base.OnInit();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.File, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Attachment,fileAttach);
            CommandStrategies.Add(TypeSearchItem.Picture, fileAttach);
            CommandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
            ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 35, CountSecondProcess = 7,LimitReaction = 85};
            ScrollBehavior.SearchGo += () =>
                                           {
                                               ShowMessageNoMatches = false;
                                               Search();
                                           };
        }


        #region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        #endregion

        class  GroupData
        {
            public string Name;
            public string File;
            public object[] Kind;
            public string ID;
            public string Display;
            public DateTime Date;

        }


    }

}
