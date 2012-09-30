using System;
using System.Collections.Generic;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.Views;
using WSSQLGUI.Services;
using System.Data;
using System.Data.OleDb;
using System.Threading;

namespace WSSQLGUI.Core
{
	internal abstract class BaseKindItemStrategy : IKindItem
	{
        protected readonly string _connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
		protected string _queryTemplate;
		protected string _queryAnd;
        protected ControllerBase _settingsController;
        protected ControllerBase _dataController;
        protected string _name = String.Empty;
        protected string _query = string.Empty;

		protected virtual void DoQuery()
		{
            if (string.IsNullOrEmpty(_query))
                return;
            bool result = true;
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(_connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(_query, myOleDbConnection);
            try
            {

                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();
                while (myDataReader.Read())
                {
                    ReadData(myDataReader);
                }

            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                //MessageBox.Show(oleDbException.Message);
                result = false;
            }
            finally
            {
                // Always call Close when done reading.
                if (myDataReader != null)
                {
                    myDataReader.Close();
                    myDataReader.Dispose();
                }
                // Close the connection when done with it.
                if (myOleDbConnection.State == System.Data.ConnectionState.Open)
                {
                    myOleDbConnection.Close();
                }
                OnComplete(result);
            }
		}

		protected virtual void ReadData(IDataReader reader)
		{
            
		}

		protected virtual string CreateSqlQuery()
		{
            return string.Empty;
		}

        protected virtual void OnStart()
        {
            EventHandler temp = Start;
            if (temp != null)
            {
                temp(null, null);
            }
        }


        protected virtual void OnComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Complete;
            if (temp != null)
            {
                temp(null, new EventArgs<bool>(res));
            }
        }

        protected virtual void OnError(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Error;
            if (temp != null)
            {
                temp(null, new EventArgs<bool>(res));
            }
        }

        protected virtual void OnCurrentItemChanged(BaseSearchData bd)
        {
            EventHandler<EventArgs<BaseSearchData>> temp = CurrentItemChanged;
            if (temp != null)
            {
                temp(null, new EventArgs<BaseSearchData>(bd));
            }
        }

		public void Search()
		{
			
		}

        public virtual void OnInit()
        {
        
        }

        public virtual void ConnectWithSettingsView(IView settingsView)
        {
            if (settingsView == null ||
                SettingsView != null) 
                return;
            SettingsView = settingsView;
            _settingsController = SettingsView.Controller as ControllerBase;
            (_settingsController as BaseSettingsController).Search += (o, e) =>
            {
                Thread thread = new Thread(() => DoQuery());
                OnStart();
                (DataView as IDataView).IsLoading = true;
                (DataView as IDataView).Clear();
                _query = CreateSqlQuery();
                thread.Start();
            };
            (_settingsController as BaseSettingsController).Error += (o, e) => { OnError(e.Value); };
            
        }

        public virtual void ConnectWithDataView(IView dataView)
        {
            if (dataView == null || 
                DataView != null) 
                return;
            DataView = dataView;
            _dataController = DataView.Controller as ControllerBase;

            (DataView as IDataView).SelectedItemChanged += (o, e) => { OnCurrentItemChanged(e.Value); };
        }


        public string Name
        {
            get { return _name; }
        }

        public IView SettingsView
        {
            get;
            protected set;
        }

        public IView DataView
        {
            get;
            protected set;
        }

        public event EventHandler Start;
        public event EventHandler<Services.EventArgs<bool>> Complete;
        public event EventHandler<Services.EventArgs<bool>> Error;
        public event EventHandler<EventArgs<BaseSearchData>> CurrentItemChanged;

        public int ID
        {
            get;
            protected set;
        }

        public Type SettingsTaskType
        {
            get;
            protected set;
        }

        public Type DataTaskType
        {
            get;
            protected set;
        }
    }
}
