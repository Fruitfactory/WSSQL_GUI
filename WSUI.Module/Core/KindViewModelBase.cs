using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Threading;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Controls.ProgressManager;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Services;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using System.Windows.Data;
using WSUI.Module.Service;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WSUI.Module.Core
{
    public abstract class KindViewModelBase : ViewModelBase, IKindItem
    {
        protected readonly string _connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        protected string _queryTemplate;
        protected string _queryAnd;
        protected string _name = string.Empty;
        protected string _query = string.Empty;
        protected string _prefix = string.Empty;
        protected bool _toggle = false;
        protected readonly List<BaseSearchData> _listData = new List<BaseSearchData>();
        private object _lock = new object();
        private BaseSearchData _current = null;
        private string _searchString = string.Empty;
        protected Dictionary<TypeSearchItem, ICommandStrategy> _commandStrategies;
        private ICommandStrategy _currentStrategy;
        protected IMainViewModel _parentViewModel;
        protected volatile bool _isInterupt = false;

        protected readonly IUnityContainer _container;

        protected KindViewModelBase(IUnityContainer container)
        {
            _container = container;
            ChooseCommand = new DelegateCommand<object>(o => OnChoose(), o => true);
            SearchCommand = new DelegateCommand<object>(o => Search(),o => CanSearch());
            OpenCommand = new DelegateCommand<object>(o => OpenFile(), o => CanOpenFile());
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(KeyDown, o => true);
            Enabled = true;
            DataSource = new ObservableCollection<BaseSearchData>();
        }


        protected virtual void DoQuery()
        {
            if (string.IsNullOrEmpty(_query))
                return;

            OleDbDataReader dataReader = null;
            OleDbConnection connection = new OleDbConnection(_connectionString);
            OleDbCommand cmd = new OleDbCommand(_query, connection);

            ProgressManager.Instance.StartOperation(new ProgressOperation()
            {
                Caption = "Searching...",
                DelayTime = 2500,
                Canceled = false
            });

            try
            {

                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    ReadData(dataReader);
                    if (_isInterupt)
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
            }
        }

        protected virtual void DoAdditionalQuery()
        {
            OnComplete(true);
        }

        protected virtual void ReadData(IDataReader reader)
        {
            
        }

        protected virtual string CreateQuery()
        {
            var searchCriteria = SearchString;
            string res = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();
                if (list == null || list.Count == 1)
                    return searchCriteria;
                res = string.Format(_queryTemplate, list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(_queryAnd, list[i]));
                }
                res += temp.ToString();
            }
            else
                res = string.Format(_queryTemplate, searchCriteria);

            return res;
        }

        protected virtual void OnStart()
        {
            _listData.Clear();

           FireStart();
            Enabled = false;
            OnPropertyChanged(() => Enabled);
            
        }

        protected virtual void OnComplete(bool res)
        {
            FireComplete(res);
            Application.Current.Dispatcher.BeginInvoke(new Action(() => _listData.ForEach(s => { DataSource.Add(s); _parentViewModel.MainDataSource.Add(s);})), null);
            OnPropertyChanged(() => DataSource);
            Enabled = true;
            OnPropertyChanged(() => Enabled);
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
            OnStart();

            Task thread = new Task(() => DoQuery());
            Task thread2 = thread.ContinueWith((t) => DoAdditionalQuery());
            _query = CreateQuery();
            thread.Start();
        }

        protected  virtual  bool CanSearch()
        {
            return true;
        }
        
        protected  virtual void OnSearchStringChanged()
        {}

        protected virtual void OnInit()
        {
            _commandStrategies  = new Dictionary<TypeSearchItem, ICommandStrategy>();
        }

        protected virtual void OnFilterData()
        {
           
        }

        protected  void ClearDaraSource()
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

        protected  void FireComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Complete;
            if (temp != null)
                temp(this, new EventArgs<bool>(res));
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
            get { return _parentViewModel; }
            set { _parentViewModel = value; }
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
                #region ti4ka
                int index = -1;
                if ((index = list.IndexOf(HelperConst.Inbox1)) > -1)
                {
                    Folder = list[index];
                }
                else if ((index = list.IndexOf(HelperConst.Inbox2)) > -1)
                {
                    Folder = list[index];
                }
                OnPropertyChanged(() => Folder);

                #endregion

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


        private void OpenFile()
        {
            var fileName = SearchItemHelper.GetFileName(Current);
            if (string.IsNullOrEmpty(fileName) ||
                FileService.IsDirectory(fileName))
                return;
            try
            {
                Process.Start(fileName);
            }
            catch (System.Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}","Start error",fileName,ex.Message));
            }
        }

        private  bool CanOpenFile()
        {
            return Current != null;
        }


        private void ChooseStrategy()
        {
            if(Current == null)
                return;
            if (!_commandStrategies.ContainsKey(Current.Type))
            {
                _currentStrategy = null;
            }
            else
                _currentStrategy = _commandStrategies[Current.Type];
            
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
