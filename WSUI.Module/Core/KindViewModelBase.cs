using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Controls.ProgressManager;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Service.Rules;
using WSUI.Infrastructure.Services;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Module.Service;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WSUI.Module.Core
{
    public abstract class KindViewModelBase : ViewModelBase, IKindItem
    {
        protected readonly string ConnectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        protected const string OrLikeTemplate = " AND System.ItemName LIKE '%{0}%'";


        protected string QueryTemplate;
        protected string QueryAnd;
        protected string _name = string.Empty;
        protected string _query = string.Empty;
        protected string _prefix = string.Empty;
        protected bool _toggle = false;
        protected readonly List<BaseSearchData> ListData = new List<BaseSearchData>();
        protected Dictionary<TypeSearchItem, ICommandStrategy> CommandStrategies;
        protected IMainViewModel ParentViewModel;
        protected volatile bool IsInterupt = false;
        protected volatile bool ShowMessageNoMatches = true;
        protected readonly IUnityContainer Container;
        protected List<IRule> RuleCollection;
        protected readonly object Lock = new object();
        protected string _andClause;
        protected List<string> _listW;
        protected IScrollBehavior ScrollBehavior =  null;


        private volatile bool _isQueryRun = false;
        private object _lock = new object();
        private BaseSearchData _current = null;
        private string _searchString = string.Empty;
        private ICommandStrategy _currentStrategy;
        private ManualResetEvent _eventForContinue;


        protected KindViewModelBase(IUnityContainer container)
        {
            Container = container;
            ChooseCommand = new DelegateCommand<object>(o => OnChoose(), o => true);
            SearchCommand = new DelegateCommand<object>(o => Search(),o => CanSearch());
            OpenCommand = new DelegateCommand<object>(o => OpenFile(), o => CanOpenFile());
            OpenFolderCommand = new DelegateCommand<object>(o => OpenFolder(), o => CanOpenFile());
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(KeyDown, o => true);
            Enabled = true;
            DataSource = new ObservableCollection<BaseSearchData>();
        }

        protected virtual void DoQuery(object mwi)
        {
            _isQueryRun = true;
            WSSqlLogger.Instance.LogInfo("Begin query!");
            if (string.IsNullOrEmpty(_query))
            {
                _eventForContinue.Set();
                return;
            }
            WSSqlLogger.Instance.LogInfo(string.Format("QUERY: {0}",_query));
            var mainWnd = mwi as MainWindowInfo;
            if(mainWnd == null)
            {
                WSSqlLogger.Instance.LogInfo("Information about Main Window is empty.");
                return;
            }
            OleDbDataReader dataReader = null;
            OleDbConnection connection = new OleDbConnection(ConnectionString);
            OleDbCommand cmd = new OleDbCommand(_query, connection);
            cmd.CommandTimeout = 0;
            ProgressManager.Instance.StartOperation(new ProgressOperation()
            {
                Caption = "Searching...",
                DelayTime = 2500,
                Canceled = false,
                Location = new Point(mainWnd.MainWindowRect.Left,mainWnd.MainWindowRect.Top),
                Size = new Size(mainWnd.MainWindowRect.Width,mainWnd.MainWindowRect.Height),
                MainHandle = mainWnd.MainWindowHandle
            });
            var watch = new Stopwatch();
            watch.Start();
            try
            {

                connection.Open();

                var watchOleDbCommand = new Stopwatch();
                watchOleDbCommand.Start();
                dataReader = cmd.ExecuteReader();
                watchOleDbCommand.Stop();
                WSSqlLogger.Instance.LogInfo("dataReader = cmd.ExecuteReader(); Elapsed: " + watchOleDbCommand.ElapsedMilliseconds.ToString());

                while (dataReader.Read())
                {
                    try
                    {
                        ReadData(dataReader);
                        if (IsInterupt)
                            break;
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError(String.Format("{0} - {1}","DoQuery _ main cycle", ex.Message));
                    }
                }

            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                WSSqlLogger.Instance.LogError(String.Format("{0} - {1}","DoQuery",oleDbException.Message));
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
                ProgressManager.Instance.StopOperation();
                watch.Stop();
                System.Diagnostics.Debug.WriteLine("DoQuery {0}",watch.ElapsedMilliseconds);
                WSSqlLogger.Instance.LogInfo("End query! Elapsed: " + watch.ElapsedMilliseconds.ToString());
                _eventForContinue.Set();
            }
        }

        protected virtual void DoAdditionalQuery()
        {
            _eventForContinue.WaitOne();
            var watch = new Stopwatch();
            watch.Start();
            OnComplete(true);
            watch.Stop();
            System.Diagnostics.Debug.WriteLine("DoAdditionalQuery {0}", watch.ElapsedMilliseconds);
            WSSqlLogger.Instance.LogInfo("End additionl query! Elapsed: " + watch.ElapsedMilliseconds.ToString());
            _isQueryRun = false;
        }

        protected virtual void ReadData(IDataReader reader)
        {
            
        }

        protected virtual string CreateQuery()
        {
            var searchCriteria = SearchString.Trim();
            string res = string.Empty;
            
            ProcessSearchCriteria(searchCriteria);

            res = string.Format(QueryTemplate, string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause);

            return res;
        }

        protected void ProcessSearchCriteria(string searchCriteria)
        {
            _andClause = string.Empty;
            _listW = new List<string>();

            foreach (var rule in RuleCollection.OrderBy(i => i.Priority))
            {
                _listW.AddRange(rule.ApplyRule(searchCriteria));
                searchCriteria = rule.ClearCriteriaAccordingRule(searchCriteria);
            }

            if (_listW.Count > 1)
            {
                StringBuilder temp = new StringBuilder();
                temp.Append(string.Format("'\"{0}\"", _listW[0]));
                for (int i = 1; i < _listW.Count; i++)
                {
                    temp.Append(string.Format(QueryAnd, _listW[i]));
                }
                _andClause = temp.ToString() + "'";
            }
        }

        protected string LikeCriteria()
        {
            if (_listW.Count == 0)
                return string.Empty;
            var temp = new StringBuilder();

            temp.Append(string.Format("\"System.ItemName\" LIKE '%{0}%'", _listW[0]));

            if (_listW.Count > 1)
                for (int i = 1; i < _listW.Count; i++)
                    temp.Append(string.Format(OrLikeTemplate, _listW.ElementAt(i)));

            return temp.ToString();
        }

        protected string FormatDate(ref DateTime date)
        {
            return date.ToString("yyyy/MM/dd hh:mm:ss").Replace('.', '/');
        }


        protected virtual void OnStart()
        {
            ListData.Clear();
            Enabled = false;
            OnPropertyChanged(() => Enabled);
            FireStart();
        }

        protected virtual void OnComplete(bool res)
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() => 
            {
                if (ListData.Count == 0 && ShowMessageNoMatches)
                {
                    DataSource.Clear();
                    var message = new BaseSearchData() { Name = string.Format("Search for '{0}' returned no matches. Try different keywords.", SearchString), Type = TypeSearchItem.None };
                    ListData.Add(message);
                    ListData.ForEach(s => DataSource.Add(s));
                }
                else
                {
                    
                    ListData.ForEach(s => DataSource.Add(s));
                }
            }), null);
            OnPropertyChanged(() => DataSource);
            Enabled = true;
            OnPropertyChanged(() => Enabled);
            FireComplete(res);
        }

        protected virtual void OnError(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Error;
            if(temp != null)
                temp(this,new EventArgs<bool>(res));
        }

        protected virtual void OnCurrentItemChanged(BaseSearchData data)
        {
            EventHandler<EventArgs<BaseSearchData>> temp = CurrentItemChanged;
            if (temp != null)
            {
                temp(this,new EventArgs<BaseSearchData>(data));
            }
        }

        protected virtual void OnChoose()
        {
            EventHandler temp = Choose;
            if (temp != null)
                temp(this, null);
        }

        protected virtual void Search()
        {
            if (_isQueryRun)
            {
                WSSqlLogger.Instance.LogWarning("Query have already started");
                return;
            }
                
            OnStart();
            MainWindowInfo mwi = new MainWindowInfo();
            Rect rect = new Rect();
            rect.Location = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
            rect.Size = new Size(Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);
            mwi.MainWindowRect = rect;
            mwi.MainWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
            //Task thread = new Task(() => DoQuery(mwi),TaskCreationOptions.None);
            //Task thread2 = thread.ContinueWith((t) => DoAdditionalQuery());
            _query = CreateQuery();
            //thread.Start();
            if(_eventForContinue == null)
                _eventForContinue = new ManualResetEvent(false);
            var thread = new Thread(() => DoQuery(mwi));
            thread.Priority = ThreadPriority.Highest;
            var thread2 = new Thread(DoAdditionalQuery);
            thread2.Priority = ThreadPriority.Highest;
            thread.Start();
            _eventForContinue.Reset();
            thread2.Start();
        }

        protected virtual bool CanSearch()
        {
            return true;
        }
        
        protected virtual void OnSearchStringChanged()
        {}

        protected virtual void OnInit()
        {
            CommandStrategies  = new Dictionary<TypeSearchItem, ICommandStrategy>();
            RuleCollection = new List<IRule>();
            RuleCollection.Add(new QuoteRule());
            RuleCollection.Add(new WordRule());
            RuleCollection.ForEach(rule => rule.InitRule());
        }

        protected virtual void OnFilterData()
        {
            if (string.IsNullOrEmpty(SearchString))
                return;
            ClearDataSource();
            Search();
        }

        protected  void ClearDataSource()
        {
            DataSource.Clear();
            OnPropertyChanged(() => DataSource);
        }

        protected void FireStart()
        {
            EventHandler temp = Start;
            if (temp != null)
                temp(this, new EventArgs());
        }

        protected void FireComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Complete;
            if (temp != null)
                temp(this, new EventArgs<bool>(res));
        }

        protected void ClearMainDataSource()
        {
            if(ParentViewModel.MainDataSource == null 
              || ParentViewModel.MainDataSource.Count == 0)
                return;
            ParentViewModel.MainDataSource.Clear();
        }

        #region IKindItem

        public string Name
        {
            get { return _name; }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                OnSearchStringChanged();
                OnPropertyChanged(() => SearchString);
            }
        }
        public IMainViewModel Parent
        {
            get { return ParentViewModel; }
            set { ParentViewModel = value; }
        }
        public string Prefix { get { return _prefix; } }
        public int ID { get; protected set; }
        public string UIName { get; protected set; }
        public bool Toggle { get { return _toggle; } set { _toggle = value; OnPropertyChanged(() => Toggle); } }
        public ObservableCollection<BaseSearchData> DataSource { get; protected set; }
        public ICommand ChooseCommand { get; protected set; }
        public ICommand SearchCommand { get; protected set; }
        public ICommand KeyDownCommand { get; protected set; }
        public event EventHandler Start;
        public event EventHandler<EventArgs<bool>> Complete;
        public event EventHandler<EventArgs<bool>> Error;
        public event EventHandler<EventArgs<BaseSearchData>> CurrentItemChanged;
        public event EventHandler Choose;

        public BaseSearchData Current
        {
            get { return _current; }
            set
            {
                _current = value; 
                OnCurrentItemChanged(_current);
                ChooseStrategy();

            }
        }

        public void Init()
        {
            OnInit();
        }

        public void FilterData()
        {
            OnFilterData();
        }

        public ObservableCollection<IWSCommand> Commands
        {
            get
            {
                return _currentStrategy != null ? _currentStrategy.Commands : null;
            }
        }

        #endregion



        public List<string> FolderList
        {
            get
            {
                var list = OutlookHelper.Instance.GetFolderList();
                if (list.Count > 0)
                {
                    OnPropertyChanged(() => FolderList);
                }
                return list;
            }
            set { }
        }

        public string Folder
        {
            get; set; 
        }


        public bool Enabled
        {
            get; set;
        }


        public ICommand OpenCommand { get; protected set; }

        public ICommand OpenFolderCommand { get; protected set; }


        private void OpenItemFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            try
            {
                Process.Start(fileName);
            }
            catch (System.Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "OpenItemFile", fileName, ex.Message));
            }
        }

        private  bool CanOpenFile()
        {
            return Current != null;
        }

        private void OpenFolder()
        {
            var filename = SearchItemHelper.GetFileName(Current);
            filename = Path.GetDirectoryName(filename);
            OpenItemFile(filename);
        }


        private void OpenFile()
        {
            var fileName = SearchItemHelper.GetFileName(Current);
            if(FileService.IsDirectory(fileName))
                return;
            
            OpenItemFile(fileName);
        }

        private void ChooseStrategy()
        {
            if(Current == null)
                return;
            if (!CommandStrategies.ContainsKey(Current.Type))
            {
                _currentStrategy = null;
            }
            else
                _currentStrategy = CommandStrategies[Current.Type];
            
            OnPropertyChanged(() => Commands);
        }

        protected virtual void KeyDown(object args)
        {
            if (args == null || !(args is KeyEventArgs))
                return;
            var keys = args as KeyEventArgs;
            switch (keys.Key)
            {
                case Key.Enter:
                    SearchCommand.Execute(null);
                    break;
            }
        }


    }
}
