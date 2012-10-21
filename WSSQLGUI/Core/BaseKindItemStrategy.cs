using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MVCSharp.Core;
using MVCSharp.Core.Views;
using WSSQLGUI.Controls.ProgressControl;
using WSSQLGUI.Services;
using System.Data;
using System.Data.OleDb;
using System.Threading;
using WSSQLGUI.Services.Helpers;
using C4F.DevKit.PreviewHandler.Service.Logger;

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
	    protected string _prefix = string.Empty;


		protected virtual void DoQuery()
		{
            if (string.IsNullOrEmpty(_query))
                return;
            
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(_connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(_query, myOleDbConnection);
            ProgressManager.Instance.StartOperation(new ProgressOperation()
                                                        {
                                                            Caption = "Searching...",
                                                            DelayTime = 2500,
                                                            Style = ProgressBarStyle.Marquee,
                                                            Canceled = false
                                                        });
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
                WSSqlLogger.Instance.LogError(oleDbException.Message);
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
                ProgressManager.Instance.StopOperation();
            }
		}

        protected  virtual void DoAddidionalQuery()
        {
            OnComplete(true);
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
            if (DataView != null)
                (DataView as IDataView).StartLoading();
            if (SettingsView.Controller != null)
                (SettingsView.Controller as BaseSettingsController).IsLoading = true;
            EventHandler temp = Start;
            if (temp != null)
            {
                temp(null, null);
            }
        }


        protected virtual void OnComplete(bool res)
        {
            if (DataView != null)
                (DataView as IDataView).FinishLoading();
            if (SettingsView.Controller != null)
                (SettingsView.Controller as BaseSettingsController).IsLoading = false;

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

		public virtual void OnInit()
        {
            SettingsTaskType = TaskFinderHelper.Instance.GetSettingsTask(Prefix);
            DataTaskType = TaskFinderHelper.Instance.GetDataTask(Prefix);
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
                OnStart();
                (DataView as IDataView).IsLoading = true;
                (DataView as IDataView).Clear();

                Task thread = new Task(() => DoQuery());
                Task thread2 = thread.ContinueWith((t) => DoAddidionalQuery());
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

        public string SearchString { get; set; }

	    public string Prefix
	    {
            get { return _prefix; }
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
