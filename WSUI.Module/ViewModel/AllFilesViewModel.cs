using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Threading;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using WSUI.Core.Core;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
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
        private const string QueryForGroupEmails =
            "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email'  AND CONTAINS(System.Message.ConversationID,'{0}*')   ORDER BY System.Message.DateReceived DESC) ";//AND CONTAINS(System.ItemPathDisplay,'{0}*',1033)

       
        private const int CountFirstProcess = 35;
        private const int CountSecondAndOtherProcess = 7;
        private int _countAdded = 0;
        private List<string> _listID = new List<string>();
        private List<string> _listName = new List<string>();
        private string _folder = string.Empty;
        //private readonly List<GroupData> _listEverething = new List<GroupData>();
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
                "SELECT TOP {3} System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateCreated,System.Subject,System.Message.ToAddress,System.Message.DateReceived,System.Size FROM SystemIndex WHERE System.Kind <> 'folder' AND System.Kind <> 'contact' AND System.DateCreated < '{1}' AND (Contains(*,{0},1033) OR ( {2} ) OR (Contains(System.Search.Contents,{0},1033) ) ) ORDER BY System.DateCreated DESC";//System.Search.Contents
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
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            TopQueryResult = 50;
            //EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
        }

        
        public bool IsOpen { get; set; }
        public ICommand FlyCommand { get; private set; }
        public ICommand EmailClickCommand { get; protected set; }

        //protected override void ReadData(IDataReader reader)
        //{
        //    GroupData data = new GroupData();
        //    ReadGroupData(reader, data);
        //    if (data.ItemName.Length > 0)
        //    {
        //        _listEverething.Add(data);                    
        //    }

        //}

        //protected override string CreateQuery()
        //{
        //    _countAdded = 0;
        //    IsInterupt = false;

        //    var searchCriteria = SearchString.Trim();
        //    string res = string.Empty;
            
        //    ProcessSearchCriteria(searchCriteria);

        //    res = string.Format(QueryTemplate, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause, FormatDate(ref _lastDate), LikeCriteria(),TopQueryResult);//LikeCriteria()

        //    return res;
        //}

        //protected override void OnStart()
        //{
        //    ListData.Clear();
        //    _listEverething.Clear();
        //    _listContacts.Clear();
        //    FireStart();
        //    if (_isFirstTime)
        //    {
        //        _resetEvent.Reset();
        //        RunContactQuery();
        //    }
        //    else
        //    {
        //        _resetEvent.Set();
        //    }
               
        //    //base.OnStart();
        //}

        //protected override void OnComplete(bool res)
        //{
        //    _resetEvent.WaitOne();
        //    var watchGroup = new Stopwatch();
        //    watchGroup.Start();
        //    GetContactResult();
        //    var grouping = _listEverething.GroupBy(i => i.ConversationID).OrderByDescending(i => i.First().DateCreated);
        //    foreach (var item in grouping)
        //    {
        //        var ordered = item.OrderByDescending(i => i.DateCreated);
        //        if (item.Key != null)
        //        {
        //            //only emails and attachments
        //            var itemE = ordered.ElementAt(0);
        //            EmailSearchData newValue = new EmailSearchData()
        //            {
        //                Subject = itemE.Subject,
        //                ConversationId = itemE.ConversationID,
        //                Count = ordered.Count().ToString(),
        //                Date = itemE.DateReceived,
        //                DateModified = itemE.DateCreated,
        //                Recepient = itemE.ToAddress != null && itemE.ToAddress.Length > 0 ? itemE.ToAddress[0] : string.Empty,
        //                Display = itemE.ItemNameDisplay,
        //                Path = itemE.ItemUrl,
        //                ID = Guid.NewGuid(),
        //                Name = itemE.Subject,
        //                Size = itemE.Size
        //            };
        //            TypeSearchItem type = SearchItemHelper.GetTypeItem(itemE.ItemUrl);
        //            newValue.Type = type;
        //            ListData.Add(newValue);
        //            _countAdded++;

        //            #region [attacment could be hidden if they have the same ConversationID, so we should pass through list and add them by hand]
        //            if (ordered.Any(a => a.ItemUrl.Contains("at=")))
        //            {
        //                var listAttachment = ordered.Where(o => o.ItemUrl.Contains("at=")).GroupBy(g => g.ConversationID);
        //                foreach (var group in listAttachment)
        //                {
        //                    var b = CreateBaseEntity(group.First(),null);
        //                    b.DateModified = newValue.DateModified;
        //                    b.Count = group.Count().ToString();
        //                    ListData.Add(b);
        //                    _countAdded++;
                            
        //                }
        //            }
        //            #endregion

        //        }
        //        else
        //        {
        //            // without emails and attacments
        //            var groupByName = item.GroupBy(i => i.ItemName);
        //            foreach (var itemByName in groupByName)
        //            {
        //                if(!itemByName.Any())
        //                    continue;
        //                var fileItem = itemByName.ElementAt(0);
        //                BaseSearchData bs = CreateBaseEntity(fileItem,itemByName.Count());
        //                if (bs.Type == TypeSearchItem.Email || bs.Type == TypeSearchItem.Attachment)
        //                    continue;
        //                ListData.Add(bs);
        //                _countAdded++;
        //            }
        //        }
        //    }
        //    watchGroup.Stop();
        //    WSSqlLogger.Instance.LogInfo("Grouping (Everything) Elapsed: " + watchGroup.ElapsedMilliseconds.ToString());
        //    if (grouping.Count() > 0)
        //        _lastDate = grouping.ElementAt(grouping.Count() - 1).First().DateCreated;
        //    base.OnComplete(res);
        //    _countProcess = CountSecondAndOtherProcess;
        //    TopQueryResult = ScrollBehavior.CountSecondProcess;
        //    _isFirstTime = false;
        //}

        //private BaseSearchData CreateBaseEntity(GroupData data, int? count)
        //{
        //    TypeSearchItem type = SearchItemHelper.GetTypeItem(data.ItemUrl, data.Kind != null && data.Kind.Length > 0 ? data.Kind[0].ToString() : string.Empty);
        //    BaseSearchData bs = new BaseSearchData()
        //    {
        //        Name = data.ItemName,
        //        Path = data.ItemUrl,
        //        Type = type,
        //        ID = Guid.NewGuid(),
        //        Display = data.ItemNameDisplay,
        //        DateModified = data.DateCreated,
        //        Tag = "",
        //        Count = count.HasValue ? count.Value.ToString() : string.Empty,
        //        Size = data.Size
        //    };
        //    return bs;
        //}

        //protected override void OnSearchStringChanged()
        //{
        //    ClearSearchingInfo();
        //    ClearDataSource();
        //    OnPropertyChanged(() => DataSource);
        //    if (ParentViewModel != null && ParentViewModel.MainDataSource != null)
        //        ParentViewModel.MainDataSource.Clear();
            
        //}

        //protected override void OnFilterData()
        //{
        //    ClearSearchingInfo();
        //    base.OnFilterData();
        //}

        //private void ClearSearchingInfo()
        //{
        //    lock (_lockObjcet)
        //    {
        //        _lastDate = GetCurrentDate();
        //        _countProcess = CountFirstProcess;
        //        TopQueryResult = ScrollBehavior.CountFirstProcess;
        //        _isFirstTime = true;
        //        _listID.Clear();
        //        _listName.Clear();
        //    }
        //}

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }

        //private bool IsEmail(object value)
        //{
        //    if (value.GetType().IsArray)
        //    {
        //        return (value as Array).Cast<string>().Any(i => i.IndexOf(KindGroup) > -1);
        //    }
        //    return false;
        //}

        //private void GetContactResult()
        //{
        //    if(_listContacts == null || _listContacts.Count == 0)
        //        return;
        //    _listContacts.ForEach(c => ListData.Add(c));
        //}

        //private void EmailClick(object obj)
        //{
        //    if(CommandElementClick(obj))
        //        return;
        //    if(EmailElementClick(obj))
        //        return;
        //}

        //private bool EmailElementClick(object obj )
        //{
        //    string adr = string.Empty;
        //    if (obj is string)
        //        adr = (string)obj;
        //    else if (obj is EmailSearchData)
        //    {
        //        adr = (obj as EmailSearchData).From;
        //    }
        //    else if (obj is ContactSearchData)
        //    {
        //        var data = obj as ContactSearchData;
        //        adr = data.EmailList != null && data.EmailList.Count > 0 ? data.EmailList[0] : string.Empty;
        //    }
        //    if (string.IsNullOrEmpty(adr))
        //        return false;
        //    var email = OutlookHelper.Instance.CreateNewEmail();
        //    email.To = adr;
        //    email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
        //    email.Display(false);

        //    return true;
        //}

        //private bool CommandElementClick(object obj)
        //{
        //    if (!(obj is CommandSearchData))
        //        return false;
        //    Parent.SelectKind(KindsConstName.People);
        //    return true;
        //}

        //protected override void OnInit()
        //{
        //    base.OnInit();
        //    CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
        //    var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
        //    CommandStrategies.Add(TypeSearchItem.File, fileAttach);
        //    CommandStrategies.Add(TypeSearchItem.Attachment,fileAttach);
        //    CommandStrategies.Add(TypeSearchItem.Picture, fileAttach);
        //    CommandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
        //    ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 300, CountSecondProcess = 100,LimitReaction = 99};
        //    ScrollBehavior.SearchGo += () =>
        //                                   {
        //                                       ShowMessageNoMatches = false;
        //                                       Search();
        //                                   };
        //    TopQueryResult = ScrollBehavior.CountFirstProcess;
        //}


        //#region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        //#endregion

        //#region  for people query
        
        //private ManualResetEvent _resetEvent = new ManualResetEvent(false);
        //private readonly List<BaseSearchData> _listContacts = new List<BaseSearchData>();
        //private volatile bool _isFirstTime = true;


        //private void RunContactQuery()
        //{
        //    var thread = new Thread(DoContactQuery);
        //    thread.Start();
        //}

        //private void DoContactQuery()
        //{
        //    string query = GetContactQuery(SearchString);
        //    OleDbDataReader dataReader = null;
        //    OleDbConnection connection = new OleDbConnection(ConnectionString);
        //    WSSqlLogger.Instance.LogInfo("SQL Query for Contact: " + query);
        //    OleDbCommand cmd = new OleDbCommand(query, connection);
        //    cmd.CommandTimeout = 0;
        //    var watch = new Stopwatch();
        //    watch.Start();
        //    try
        //    {
        //        connection.Open();
        //        var watchOleDbCommand = new Stopwatch();
        //        watchOleDbCommand.Start();
        //        dataReader = cmd.ExecuteReader();
        //        watchOleDbCommand.Stop();
        //        WSSqlLogger.Instance.LogInfo("dataReaderContact = cmd.ExecuteReader(); Elapsed: " + watchOleDbCommand.ElapsedMilliseconds.ToString());
        //        while (dataReader.Read())
        //        {
        //            try
        //            {
        //                ReadContactData(dataReader);
        //            }
        //            catch (Exception ex)
        //            {
        //                WSSqlLogger.Instance.LogError(String.Format("{0} - {1}", "DoContactQuery", ex.Message));
        //            }
        //        }
        //        if (_listContacts.Count > 0)
        //        {
        //            bool isMore = false;
        //            var contacts = _listContacts.OfType<BaseSearchData>().GroupBy(em => em.Name.ToLower()).ToList();
        //            _listContacts.Clear();
        //            foreach (var contact in contacts)
        //            {
        //                if (!contact.Any())
        //                    continue;
        //                if (_listContacts.Count == 5)
        //                {
        //                    isMore = true;
        //                    break;
        //                }
        //                _listContacts.Add(contact.ElementAt(0));
        //            }

        //            if (isMore)
        //            {
        //                CommandSearchData commandSearchData = new CommandSearchData()
        //                {
        //                    Name = "more",
        //                    Type = TypeSearchItem.Command,
        //                    ID = Guid.NewGuid()
        //                };
        //                _listContacts.Add(commandSearchData);
        //            }
                        
        //        }
        //    }
        //    catch (System.Data.OleDb.OleDbException oleDbException)
        //    {
        //        WSSqlLogger.Instance.LogError(String.Format("{0} - {1}", "DoContactQuery", oleDbException.Message));
        //    }
        //    finally
        //    {
        //        // Always call Close when done reading.
        //        if (dataReader != null)
        //        {
        //            dataReader.Close();
        //            dataReader.Dispose();
        //        }
        //        // Close the connection when done with it.
        //        if (connection.State == System.Data.ConnectionState.Open)
        //        {
        //            connection.Close();
        //        }
        //        watch.Stop();
        //        WSSqlLogger.Instance.LogInfo("End query contact! Elapsed: " + watch.ElapsedMilliseconds.ToString());
        //        _resetEvent.Set();
        //    }


        //}

        //private void ReadContactData(OleDbDataReader dataReader)
        //{
        //    var item = ReadRawContactData(dataReader);
        //    if(item != null && !string.IsNullOrEmpty(item.Name))
        //        _listContacts.Add(item);
        //}

        //private string GetContactQuery(string searchCriteria)
        //{
        //    return ContactHelpers.GetContactQuery(SearchString,FormatDate(ref _lastDate),250);
        //}

        //private BaseSearchData ReadRawContactData(IDataReader reader)
        //{
        //    var data = new ContactItem();
        //    ReadGroupData(reader,data);
        //    switch (data.Kind[0])
        //    {
        //        case "contact":
        //            ContactSearchData item = new ContactSearchData()
        //            {
        //                Name = data.EmailAddress,
        //                Path = string.Empty,
        //                FirstName = data.FirstName,
        //                LastName = data.LastName,
        //                ID = Guid.NewGuid(),
        //                Type = TypeSearchItem.Contact
        //            };
        //            if(!string.IsNullOrEmpty(data.EmailAddress))
        //                item.EmailList.Add(data.EmailAddress);
        //            if(!string.IsNullOrEmpty(data.EmailAddress2))
        //                item.EmailList.Add(data.EmailAddress2);
        //            if(!string.IsNullOrEmpty(data.EmailAddress3))
        //                item.EmailList.Add(data.EmailAddress3);
        //            //item.Foto = OutlookHelper.Instance.GetContactFotoTempFileName(item);
        //            return item.EmailList.Count > 0 && !string.IsNullOrEmpty(item.Name) ? item : null;
        //        case "email":
        //            string fromAddress = ContactHelpers.GetEmailAddress(data.FromAddress,SearchString) ?? ContactHelpers.GetEmailAddress(data.CcAddress,SearchString) ?? ContactHelpers.GetEmailAddress(data.ToAddress,SearchString);
        //            if (string.IsNullOrEmpty(fromAddress))
        //                break;
        //            EmailSearchData si = new EmailSearchData()
        //            {
        //                Subject = data.Subject,
        //                Recepient = string.Format("{0}",
        //                data.ToAddress != null && data.ToAddress.Length > 0 ? data.ToAddress[0] : string.Empty),
        //                Name = fromAddress,
        //                Path = data.ItemUrl,
        //                Date = data.DateReceived,
        //                Count = string.Empty,
        //                Type = TypeSearchItem.Contact,
        //                ID = Guid.NewGuid(),
        //                From = fromAddress,
        //                Tag = "Click to email recipient"
        //            };

        //            return si;
        //    }
        //    return null;
        //}


        //#endregion
        
        //class  GroupData : ISearchData
        //{
        //    [FieldIndex(0)]
        //    public string ItemName { get; set; }
        //    [FieldIndex(1)]
        //    public string ItemUrl { get; set; }
        //    [FieldIndex(2)]
        //    public object[] Kind { get; set; }
        //    [FieldIndex(3)]
        //    public string ConversationID { get; set; }
        //    [FieldIndex(4)]
        //    public string ItemNameDisplay { get; set; }
        //    [FieldIndex(5)]
        //    public DateTime DateCreated { get; set; }
        //    [FieldIndex(6)]
        //    public string Subject { get; set; }
        //    [FieldIndex(7)]
        //    public string[] ToAddress { get; set; }
        //    [FieldIndex(8)]
        //    public DateTime DateReceived { get; set; }
        //    [FieldIndex(9)]
        //    public int Size { get; set; }

        //    public override string ToString()
        //    {
        //        return string.Format("{0}\t\t{1}:dd:MM:yyyy\t\t{2}", ItemName, DateCreated, ItemUrl);
        //    }
        //}


    }

}
