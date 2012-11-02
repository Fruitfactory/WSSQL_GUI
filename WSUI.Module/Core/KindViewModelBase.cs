using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using C4F.DevKit.PreviewHandler.Service.Logger;
using Microsoft.Practices.Prism.Commands;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Services;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using System.Windows;
using System.Windows.Data;

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

        protected readonly IUnityContainer _container;

        protected KindViewModelBase(IUnityContainer container)
        {
            _container = container;
            ChooseCommand = new DelegateCommand<object>(o => OnChoose(), o => true);
            SearchCommand = new DelegateCommand<object>(o => Search(),o => CanSearch());
        }


        protected virtual void DoQuery()
        {
            if (string.IsNullOrEmpty(_query))
                return;

            OleDbDataReader dataReader = null;
            OleDbConnection connection = new OleDbConnection(_connectionString);
            OleDbCommand cmd = new OleDbCommand(_query, connection);
           
            try
            {

                connection.Open();
                dataReader = cmd.ExecuteReader();
                while (dataReader.Read())
                {
                    ReadData(dataReader);
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
            return string.Empty;
        }

        protected virtual void OnStart()
        {
            if(DataSource == null)
                DataSource = new ObservableCollection<BaseSearchData>();
            DataSource.Clear();
            _listData.Clear();

            EventHandler temp = Start;
            if (temp != null)
                temp(this,new EventArgs());
        }

        protected virtual void OnComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Complete;
            if(temp != null)
                temp(this,new EventArgs<bool>(res));

            Application.Current.Dispatcher.BeginInvoke(new Action(() => _listData.ForEach(s => DataSource.Add(s))), null);

            OnPropertyChanged(() => DataSource);
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
            }
        }

        public string Prefix { get { return _prefix; } }
        public int ID { get; protected set; }
        public string UIName { get; protected set; }
        public bool Toggle { get { return _toggle; } set { _toggle = value; OnPropertyChanged(() => Toggle); } }
        public ObservableCollection<BaseSearchData> DataSource { get; protected set; }
        public ICommand ChooseCommand { get; protected set; }
        public ICommand SearchCommand { get; protected set; }
        public event EventHandler Start;
        public event EventHandler<EventArgs<bool>> Complete;
        public event EventHandler<EventArgs<bool>> Error;
        public event EventHandler<EventArgs<BaseSearchData>> CurrentItemChanged;
        public event EventHandler Choose;

        public BaseSearchData Current
        {
            get { return _current; }
            set { _current = value; OnCurrentItemChanged(_current);}
        }

        #endregion

    }
}
