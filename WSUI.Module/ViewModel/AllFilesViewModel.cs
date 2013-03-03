using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Attributes;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Service;
using WSUI.Module.Strategy;
using System.Diagnostics;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Everything,0)]
    public class AllFilesViewModel : KindViewModelBase, IUView<AllFilesViewModel>, IScrollableView
    {
        private const string KindGroup = "email";
        private DateTime _lastDate;
        private const string QueryForGroupEmails =
            "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email'  AND CONTAINS(System.Message.ConversationID,'{0}*')   ORDER BY System.Message.DateReceived DESC) ";//AND CONTAINS(System.ItemPathDisplay,'{0}*',1033)

       
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
                "SELECT TOP {3} System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateCreated,System.Subject,System.Message.ToAddress,System.Message.DateReceived,System.Size FROM SystemIndex WHERE System.Kind <> 'folder' AND System.DateCreated < '{1}' AND (Contains(System.Search.Contents,{0},1033) OR ( {2} ) ) ORDER BY System.DateCreated DESC";
            QueryAnd = " AND \"{0}\""; 
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
            TopQueryResult = 50;
            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
        }

        
        public bool IsOpen { get; set; }
        public ICommand FlyCommand { get; private set; }
        public ICommand EmailClickCommand { get; protected set; }

        protected override void ReadData(IDataReader reader)
        {
            GroupData data = new GroupData();
            ReadGroupData(reader, data);
            if (data.ItemName.Length > 0)
            {
                _listEverething.Add(data);                    
            }

        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            IsInterupt = false;

            var searchCriteria = SearchString.Trim();
            string res = string.Empty;
            
            ProcessSearchCriteria(searchCriteria);

            res = string.Format(QueryTemplate, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause, FormatDate(ref _lastDate), LikeCriteria(),TopQueryResult);//LikeCriteria()

            return res;
        }

        protected override void OnStart()
        {
            ListData.Clear();
            _listEverething.Clear();
            _listContacts.Clear();
            FireStart();
            if (_isFirstTime)
            {
                _resetEvent.Reset();
                RunContactQuery();
            }
            else
            {
                _resetEvent.Set();
            }
               
            //base.OnStart();
        }

        protected override void OnComplete(bool res)
        {
            _resetEvent.WaitOne();
            var watchGroup = new Stopwatch();
            watchGroup.Start();
            GetContactResult();
            var grouping = _listEverething.GroupBy(i => i.ConversationID);
            foreach (var item in grouping)
            {
                var ordered = item.OrderByDescending(i => i.DateCreated);
                if (item.Key != null)
                {
                    var itemE = ordered.ElementAt(0);
                    EmailSearchData newValue = new EmailSearchData()
                    {
                        Subject = itemE.Subject,
                        ConversationId = itemE.ConversationID,
                        Count = ordered.Count().ToString(),
                        Date = itemE.DateReceived,
                        DateModified = itemE.DateCreated,
                        Recepient = itemE.ToAddress.Length > 0 ? itemE.ToAddress[0] : string.Empty,
                        Display = itemE.ItemNameDisplay,
                        Path = itemE.ItemUrl,
                        ID = Guid.NewGuid(),
                        Name = itemE.ItemNameDisplay,
                        Size = itemE.Size
                    };

                    TypeSearchItem type = SearchItemHelper.GetTypeItem(itemE.ItemUrl);
                    newValue.Type = type;
                    ListData.Add(newValue);
                    _countAdded++;
                }
                else
                {
                    var groupByName = item.GroupBy(i => i.ItemName);
                    foreach (var itemByName in groupByName)
                    {
                        if(!itemByName.Any())
                            continue;
                        var fileItem = itemByName.ElementAt(0);

                        TypeSearchItem type = SearchItemHelper.GetTypeItem(fileItem.ItemUrl, fileItem.Kind != null && fileItem.Kind.Length > 0 ? fileItem.Kind[0].ToString() : string.Empty);
                        BaseSearchData bs = new BaseSearchData()
                        {
                            Name = fileItem.ItemName,
                            Path = fileItem.ItemUrl,
                            Type = type,
                            ID = Guid.NewGuid(),
                            Display = fileItem.ItemNameDisplay,
                            DateModified = fileItem.DateCreated,
                            Tag = "",
                            Count = itemByName.Count().ToString(),
                            Size = fileItem.Size
                        };
                        ListData.Add(bs);
                        _countAdded++;
                    }
                }
            }
            watchGroup.Stop();
            WSSqlLogger.Instance.LogInfo("Grouping (Everything) Elapsed: " + watchGroup.ElapsedMilliseconds.ToString());
            if(_listEverething.Count > 0)
                _lastDate = _listEverething[_listEverething.Count-1].DateCreated;
            base.OnComplete(res);
            _countProcess = CountSecondAndOtherProcess;
            TopQueryResult = ScrollBehavior.CountSecondProcess;
            _isFirstTime = false;
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
                TopQueryResult = ScrollBehavior.CountFirstProcess;
                _isFirstTime = true;
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

        private bool IsEmail(object value)
        {
            if (value.GetType().IsArray)
            {
                return (value as Array).Cast<string>().Any(i => i.IndexOf(KindGroup) > -1);
            }
            return false;
        }

        private void GetContactResult()
        {
            if(_listContacts == null || _listContacts.Count == 0)
                return;
            _listContacts.ForEach(c => ListData.Add(c));
        }

        private void EmailClick(object obj)
        {
            if(CommandElementClick(obj))
                return;
            if(EmailElementClick(obj))
                return;
        }

        private bool EmailElementClick(object obj )
        {
            string adr = string.Empty;
            if (obj is string)
                adr = (string)obj;
            else if (obj is EmailSearchData)
            {
                adr = (obj as EmailSearchData).From;
            }
            else if (obj is ContactSearchData)
            {
                var data = obj as ContactSearchData;
                var ci = OutlookHelper.Instance.GetContact(data.Name);
                if (ci == null || ci.Email1Address == null
                    || ci.Email1Address.Length == 0)
                    return false;
                adr = ci.Email1Address;
            }
            if (string.IsNullOrEmpty(adr))
                return false;
            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = adr;
            email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);

            return true;
        }

        private bool CommandElementClick(object obj)
        {
            if (!(obj is CommandSearchData))
                return false;
            Parent.SelectKind(KindsConstName.People);
            return true;
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
            ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 300, CountSecondProcess = 100,LimitReaction = 85};
            ScrollBehavior.SearchGo += () =>
                                           {
                                               ShowMessageNoMatches = false;
                                               Search();
                                           };
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }


        #region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        #endregion

        #region  for people query
        
        private ManualResetEvent _resetEvent = new ManualResetEvent(false);
        private readonly List<BaseSearchData> _listContacts = new List<BaseSearchData>();
        private volatile bool _isFirstTime = true;


        private void RunContactQuery()
        {
            var thread = new Thread(DoContactQuery);
            thread.Priority = ThreadPriority.Highest;
            thread.Start();
        }

        private void DoContactQuery()
        {
            string query = GetContactQuery(SearchString);
            OleDbDataReader dataReader = null;
            OleDbConnection connection = new OleDbConnection(ConnectionString);
            OleDbCommand cmd = new OleDbCommand(query, connection);
            cmd.CommandTimeout = 0;
            var watch = new Stopwatch();
            watch.Start();
            try
            {
                connection.Open();
                var watchOleDbCommand = new Stopwatch();
                watchOleDbCommand.Start();
                dataReader = cmd.ExecuteReader();
                watchOleDbCommand.Stop();
                WSSqlLogger.Instance.LogInfo("dataReaderContact = cmd.ExecuteReader(); Elapsed: " + watchOleDbCommand.ElapsedMilliseconds.ToString());
                while (dataReader.Read())
                {
                    try
                    {
                        ReadContactData(dataReader);
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError(String.Format("{0} - {1}", "DoContactQuery", ex.Message));
                    }
                }
                if (_listContacts.Count > 0)
                {
                    CommandSearchData commandSearchData = new CommandSearchData()
                                                {
                                                    Name = "more",
                                                    Type = TypeSearchItem.Command,
                                                    ID = Guid.NewGuid()
                                                };
                    _listContacts.Add(commandSearchData);
                }
            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                WSSqlLogger.Instance.LogError(String.Format("{0} - {1}", "DoContactQuery", oleDbException.Message));
            }
            finally
            {
                // Always call Close when done reading.
                if (dataReader != null)
                {
                    dataReader.Close();
                    dataReader.Dispose();
                }
                // Close the connection when done with it.
                if (connection.State == System.Data.ConnectionState.Open)
                {
                    connection.Close();
                }
                watch.Stop();
                WSSqlLogger.Instance.LogInfo("End query contact! Elapsed: " + watch.ElapsedMilliseconds.ToString());
                _resetEvent.Set();
            }


        }

        private void ReadContactData(OleDbDataReader dataReader)
        {
            var item = ReadRawContactData(dataReader);
            _listContacts.Add(item);
        }

        private string GetContactQuery(string searchCriteria)
        {
            return ContactHelpers.GetContactQuery(SearchString,FormatDate(ref _lastDate));
        }

        private BaseSearchData ReadRawContactData(IDataReader reader)
        {
            var data = new ContactItem();
            ReadGroupData(reader,data);
            switch (data.Kind[0])
            {
                case "contact":
                    ContactSearchData item = new ContactSearchData()
                    {
                        Name = data.ItemName,
                        Path = string.Empty,
                        FirstName = data.FirstName,
                        LastName = data.LastName,
                        ID = Guid.NewGuid(),
                        Type = TypeSearchItem.Contact
                    };
                    item.EmailList.Add(data.EmailAddress);
                    item.EmailList.Add(data.EmailAddress2);
                    item.EmailList.Add(data.EmailAddress3);
                    item.Foto = OutlookHelper.Instance.GetContactFotoTempFileName(item);
                    return item;
                case "email":
                    string fromAddress = ContactHelpers.GetEmailAddress(data.FromAddress,SearchString);
                    if (string.IsNullOrEmpty(fromAddress))
                        break;
                    EmailSearchData si = new EmailSearchData()
                    {
                        Subject = data.Subject,
                        Recepient = string.Format("{0}",
                        data.ToAddress != null && data.ToAddress.Length > 0 ? data.ToAddress[0] : string.Empty),
                        Name = data.ItemName,
                        Path = data.ItemUrl,
                        Date = data.DateReceived,
                        Count = string.Empty,
                        Type = TypeSearchItem.Email,
                        ID = Guid.NewGuid(),
                        From = fromAddress
                    };

                    return si;
            }
            return null;
        }


        #endregion
        
        class  GroupData : ISearchData
        {
            [FieldIndex(0)]
            public string ItemName { get; set; }
            [FieldIndex(1)]
            public string ItemUrl { get; set; }
            [FieldIndex(2)]
            public object[] Kind { get; set; }
            [FieldIndex(3)]
            public string ConversationID { get; set; }
            [FieldIndex(4)]
            public string ItemNameDisplay { get; set; }
            [FieldIndex(5)]
            public DateTime DateCreated { get; set; }
            [FieldIndex(6)]
            public string Subject { get; set; }
            [FieldIndex(7)]
            public string[] ToAddress { get; set; }
            [FieldIndex(8)]
            public DateTime DateReceived { get; set; }
            [FieldIndex(9)]
            public int Size { get; set; }

            public override string ToString()
            {
                return string.Format("{0}\t\t{1}:dd:MM:yyyy\t\t{2}", ItemName, DateCreated, ItemUrl);
            }
        }


    }

}
