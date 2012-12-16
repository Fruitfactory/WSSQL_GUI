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
using System.Windows.Threading;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Models;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    public class AllFilesViewModel : KindViewModelBase, IUView<AllFilesViewModel>
    {
        private const string KindGroup = "email";
        private const string InboxFolder = HelperConst.Inbox2;
        private int _lastID = 0;
        private const string QueryForGroupEmails =
            "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email'  AND CONTAINS(System.Message.ConversationID,'{1}*')   ORDER BY System.Message.DateReceived DESC) ";//AND CONTAINS(System.ItemPathDisplay,'{0}*',1033)

        private int _countAdded = 0;
        private List<string> _listID = new List<string>();
        private List<string> _listName = new List<string>();
        private string _folder = string.Empty;

        public AllFilesViewModel(IUnityContainer container, ISettingsView<AllFilesViewModel> settingsView,
                                 IDataView<AllFilesViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            // init
            _queryTemplate =
                "GROUP ON System.ItemName OVER (SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateModified,System.Search.EntryID FROM SystemIndex WHERE System.Kind <> 'folder' AND System.Search.EntryID > {1} AND Contains(*,{0}) ORDER BY System.Search.EntryID ASC)";//OR (System.Kind == 'email' AND Contains(*,'{0}*'))
            _queryAnd = " AND \"{0}\""; //" AND \"{0}\"";
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

            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
            MoveFirstCommand = new DelegateCommand<object>(o => MoveToFirstInternal(),o => CanMoveLeft());
            MovePreviousCommand = new DelegateCommand<object>( o => MoveToLeft(), o => CanMoveLeft());
            MoveLastCoommand = new DelegateCommand<object>(o => MoveToLastInternal(), o => CanModeRight());
            MoveNextCommand = new DelegateCommand<object>(o => MoveToRight(), o => CanModeRight());
            LinkCommand = new DelegateCommand<object>(o => LinkClicked(o), o => true);
        }

        
        public bool IsOpen { get; set; }
        public ICommand FlyCommand { get; private set; }
        public ICommand EmailClickCommand { get; protected set; }

        protected override void ReadData(IDataReader reader)
        {
            var group = new List<GroupData>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).ToString() != "System.Data.IDataReader")
                    continue;
                OleDbDataReader itemsReader = reader.GetValue(i) as OleDbDataReader;

                while (itemsReader.Read())
                {
                    group.Add(ReadGroupData(itemsReader));
                }
            }

            if (group == null || group.Count == 0)
                return;
            var groupItem = group[0];
            string tag = string.Empty;
            if (groupItem.Kind != null && IsEmail(groupItem.Kind) && !_listID.Any(it => it == groupItem.ID))
            {
                var newValue = GroupEmail(groupItem.Name, groupItem.ID);
                if (newValue == null)
                    return;
                _listID.Add(groupItem.ID);
                TypeSearchItem type = SearchItemHelper.GetTypeItem(groupItem.File);
                newValue.Type = type;
                _listData.Add(newValue);
                _countAdded++;
            }
            else if (groupItem.Kind != null && IsEmail(value: groupItem.Kind) && _listID.Any(it => it == groupItem.ID))
                return;
            else if (!_listName.Any(it => it == groupItem.Name))
            {
                _listName.Add(groupItem.Name);
                TypeSearchItem type = SearchItemHelper.GetTypeItem(groupItem.File, groupItem.Kind != null && groupItem.Kind.Length > 0 ? groupItem.Kind[0].ToString() : string.Empty);
                BaseSearchData bs = new BaseSearchData()
                {
                    Name = groupItem.Name,
                    Path = groupItem.File,
                    Type = type,
                    ID = Guid.NewGuid(),
                    Display = groupItem.Display,
                    DateModified = DateTime.Parse(groupItem.Date),
                    Tag = tag,
                    Count = group.Count.ToString()
                };
                _listData.Add(bs);
                _countAdded++;
            }
            if (_countAdded == CountItemsInPage)
                _isInterupt = true;

        }


        private GroupData ReadGroupData(IDataReader reader)
        {
            string name = reader[0].ToString();
            string file = reader[1].ToString();
            var kind = reader[2] as object[];
            string id = reader[3].ToString();
            string display = reader[4].ToString();
            var date = reader[5].ToString();
            int.TryParse(reader[6].ToString(), out _lastID);
            
            return new GroupData()
            {
                Name = name,
                File = file,
                Kind = kind,
                ID = id,
                Display = display,
                Date = date
            };

        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            _isInterupt = false;
            var searchCriteria = SearchString.Trim();
            string res = string.Empty;
            string andClause = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();

                temp.Append(string.Format("'\"{0}\"", list[0]));
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(_queryAnd, list[i]));
                }
                andClause = temp.ToString() + "'";
            }
            res = string.Format(_queryTemplate,  string.IsNullOrEmpty(andClause) ? string.Format("'\"{0}\"'",searchCriteria) : andClause,  _lastID);

            return res;
        }

        protected override void OnComplete(bool res)
        {
            
            base.OnComplete(res);

            Application.Current.Dispatcher.BeginInvoke(new Action(() => UpdateData()), null);
        }

        protected override void OnSearchStringChanged()
        {
            _lastID = 0;
            _currentPageNumber = 0;
            if(DataPage != null)
                DataPage.Clear();
            if(DataSourceOfPage  != null)
                DataSourceOfPage.Clear();
            DataSource.Clear();
            _listID.Clear();
            _listName.Clear();
            OnPropertyChanged(() => DataSource);
            OnPropertyChanged(() => DataPage);
            OnPropertyChanged(() => DataSourceOfPage);
            if (_parentViewModel != null && _parentViewModel.MainDataSource != null)
                _parentViewModel.MainDataSource.Clear();
            ClearDaraSource();
        }

        private EmailSearchData GroupEmail(string name, string id)
        {
            var query = string.Format(QueryForGroupEmails, _folder, id); //,InboxFolder
            EmailSearchData data = null;
            OleDbDataReader reader = null;
            OleDbConnection con = new OleDbConnection(_connectionString);
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
            _commandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            _commandStrategies.Add(TypeSearchItem.File, fileAttach);
            _commandStrategies.Add(TypeSearchItem.Attachment,fileAttach);
            _commandStrategies.Add(TypeSearchItem.Picture, fileAttach);
            _commandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
            var list = OutlookHelper.Instance.GetFolderList();
            if (list.IndexOf(HelperConst.Inbox1) > -1)
                _folder = HelperConst.Inbox1;
            else if (list.IndexOf(HelperConst.Inbox2) > -1)
                _folder = HelperConst.Inbox2;
        }


        #region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        #endregion

        #region paging

        public class PageEntity : INotifyPropertyChanged
        {
            private int _number = 0;
            private bool _isVisited = false;
            private bool _isVisible = false;


            public int Number
            {
                get { return _number; }
                set { _number = value; OnPropertyChanged("Number"); }
            }

            public string Name
            {
                get { return (_number + 1).ToString(); }
            }

            public bool IsVisited
            {
                get { return _isVisited; }
                set { _isVisited = value; OnPropertyChanged("IsVisited"); }
            }

            public bool IsVisible
            {
                get { return _isVisible; }
                set { _isVisible = value; OnPropertyChanged("IsVisible"); }
            }


            public event PropertyChangedEventHandler PropertyChanged;

            private void OnPropertyChanged(string property)
            {
                PropertyChangedEventHandler temp = PropertyChanged;
                if (temp != null)
                {
                    temp(this,new PropertyChangedEventArgs(property));
                }
            }
        }

        private const int CountItemsInPage = 25;
        private const int MaxLinks = 10;

        private int _currentPageNumber = -1;
        private int _pageCount;
        private List<PageEntity> _pages = new List<PageEntity>();

        public ObservableCollection<PageEntity> DataPage { get; set; }
        public ObservableCollection<BaseSearchData> DataSourceOfPage { get; set; }
        public string PageCount { get; set; }
        public string CurrentPage { get; set; }

        public ICommand MoveFirstCommand { get; private set; }
        public ICommand MovePreviousCommand { get; private set; }
        public ICommand MoveLastCoommand { get; private set; }
        public ICommand MoveNextCommand { get; private set; }
        public ICommand LinkCommand { get; private set; }


        private void UpdateData()
        {
            if (DataSource == null)
            {
                _pageCount = 0;
                _currentPageNumber = -1;
            }
            else
            {
                _pageCount = DataSource.Count / CountItemsInPage;
                _pageCount += DataSource.Count % CountItemsInPage > 0 ? 1 : 0;
                _pages.Clear();
                for (int i = 0; i < _pageCount; i++)
                {
                    _pages.Add(new PageEntity()
                    {
                        Number = i,
                        IsVisible = true,
                        IsVisited = false
                    });
                }
                _currentPageNumber = _pageCount - 1;
                SetCurrentPageSource();
                UpdateLinks();
                UpdateLink();
            }
            UpdatedStatics();
        }

        private void SetCurrentPageSource()
        {
            if (_currentPageNumber < 0 ||  _currentPageNumber >= _pageCount)
                return;
            int begin = _currentPageNumber * CountItemsInPage;
            int count = (begin + CountItemsInPage) < DataSource.Count ? CountItemsInPage : DataSource.Count - begin;
            var list = new ObservableCollection<BaseSearchData>();
            for (int i = begin; i < (begin + count);i++ )
                list.Add(DataSource[i]);
            if(DataSourceOfPage != null && DataSourceOfPage.Count > 0)
                DataSourceOfPage.Clear();
            DataSourceOfPage = new ObservableCollection<BaseSearchData>(list.OrderBy(i => i.Type));
            UpdatedStatics();
            OnPropertyChanged(() => DataSourceOfPage);
        }

        private void UpdateLinks()
        {
            if (_currentPageNumber >= _pageCount)
                return;
            // TODO add updating links
            if(DataPage == null)
                DataPage = new ObservableCollection<PageEntity>();
            DataPage.Clear();
            for (int i = _pageCount > MaxLinks ? _pageCount - MaxLinks : 0; i < (_pageCount > MaxLinks ? _pageCount : MaxLinks) ; i++)
            {
                if (i >= _pageCount)
                    break;
                DataPage.Add(_pages[i]);
            }
            
            OnPropertyChanged(() => DataPage);
        }

        private void UpdateLink()
        {
            var page = _pages.Find(p => p.Number == _currentPageNumber);
            if (page != null)
            {
                page.IsVisited = true;
            }
        }

        private void UpdatedStatics()
        {
            PageCount = _pageCount.ToString();
            CurrentPage = (_currentPageNumber + 1).ToString();
            OnPropertyChanged(() => PageCount);
            OnPropertyChanged(() => CurrentPage);
        }

        private void MoveToFirstInternal()
        {
            if (_pages.Count == 0)
                return;
            int start = 0;
            
            for (int i = 0; i < MaxLinks; i++)
            {
                DataPage[i] = _pages[start];
                start++;
            }
            UpdatedStatics();
            OnPropertyChanged(() => DataPage);
        }

        private void MoveToLastInternal()
        {
            if (_pages.Count == 0 || _pages.Count < MaxLinks)
                return;

            int start = _pages[_pages.Count - 1].Number;
            int max = _pages.Count > MaxLinks ? MaxLinks : _pages.Count;

            for (int i = max - 1; i >= 0; i--)
            {
                DataPage[i] = _pages[start];
                start--;
            }
            UpdatedStatics();
            OnPropertyChanged(() => DataPage);
        }

        private void MoveToRight()
        {
            if (_pages.Count == 0)
                return;
            var curRight = DataPage[DataPage.Count - 1];

            if (curRight.Number == _pageCount - 1 )
            {
                Search();
            }
            else 
            {
                int start = curRight.Number + 1;
                for (int i = MaxLinks - 1; i >= 0; i--)
                {
                    DataPage[i] = _pages[start];
                    start--;
                }
                UpdatedStatics();
                OnPropertyChanged(() => DataPage);
            }
        }

        private void MoveRightStep(int number)
        {
            if (number == _pages.Count - 1)
                return;
            int start = number + 1;
            for (int i = MaxLinks - 1; i >= 0; i--)
            {
                DataPage[i] = _pages[start];
                start--;
            }
            UpdatedStatics();
            OnPropertyChanged(() => DataPage);
        }

        private void MoveToLeft()
        {
            if (_pages.Count == 0)
                return;

            var curLeft = DataPage[0];
            if (curLeft.Number == 0)
                return;
            int start = curLeft.Number - 1;
            for (int i = 0; i < MaxLinks; i++)
            {
                DataPage[i] = _pages[start];
                start++;
            }
            UpdatedStatics();
            OnPropertyChanged(() => DataPage);
        }


        private bool CanMoveLeft()
        {
            return true;//DataPage != null && !(DataPage[0].Number == 0);
        }

        private bool CanModeRight()
        {
            return true;// DataPage != null && !(DataPage[DataPage.Count - 1].Number == _pageCount - 1);
        }


        private void LinkClicked(object id)
        {
            if (id == null)
                return;
            var idPage = (int)id;
            _currentPageNumber = idPage;
            var page = DataPage.ToList().Find(p => p.Number == idPage);
            if (page != null)
                page.IsVisited = true;
            SetCurrentPageSource();
            OnPropertyChanged(() => DataPage);
        }


        #endregion


        class  GroupData
        {
            public string Name;
            public string File;
            public object[] Kind;
            public string ID;
            public string Display;
            public string Date;

        }


    }

}
