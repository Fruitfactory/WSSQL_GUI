using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.Views;
using MVCSharp.Core.CommandManager;
using WSSQLGUI.Models;
using WSSQLGUI.Services;
using System.Data.OleDb;
using System.Data;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;

namespace WSSQLGUI.Controllers
{
    class SearchController : ControllerBase
    {
        #region const

        private const string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private const string qyeryTemplate = "SELECT System.ItemName, System.ItemUrl  FROM SystemIndex WHERE Contains(*,'{0}*')";
        private const string FILEPREFIX = "file:";
        private const string FILEPREFIX1 = "file:///";
        

        #endregion

        #region fields

        private DelegateCommand _openFileCommand;
        private DelegateCommand _searchCommand;

        #endregion

        #region events

        public event EventHandler OnStartSearch;
        public event EventHandler<EventArgs<bool>> OnCompleteSearch;
        public event EventHandler<EventArgs<SearchItem>> OnAddSearchItem;

        #endregion

        #region commands

        public ICommand OpenFileCommand
        {
            get
            {
                if (_openFileCommand == null)
                    _openFileCommand = new DelegateCommand("Preview", CanOpenFile, OpenCurrentFile);
                return _openFileCommand;
            }
        }

        public ICommand SearchCommand
        {
            get 
            {
                if (_searchCommand == null)
                    _searchCommand = new DelegateCommand("Search", CanSearch, Search);
                return _searchCommand;
            }
        }

        #endregion


        #region properties

        public override IView View
        {
            get
            {
                return base.View;
            }
            set
            {
                base.View = value;
            }
        }

        public SearchItem CurrenItem { get; private set; }
        public string SearchCriteria { get; set;}

        #endregion

        #region public methods
       
        public void CurrentSearchItemChanged(SearchItem item)
        {
            CurrenItem = item;
        }

        #endregion

        #region private

        private void OpenCurrentFile()
        {
            if (CurrenItem == null ||
                string.IsNullOrEmpty(CurrenItem.FileName) ||
                FileService.IsDirectory(CurrenItem.FileName))
                return;
            try
            {
                Process.Start(CurrenItem.FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool CanOpenFile()
        {
            return CurrenItem != null;
        }

        private void Search()
        {
            Thread thread = new Thread(() => DoQuery(SearchCriteria));
            thread.Start();
        }

        private bool CanSearch()
        {
            return !string.IsNullOrEmpty(SearchCriteria);
        }


        private void DoQuery(object queryString)
        {
            string query = String.Format(qyeryTemplate, queryString);
            bool result = true;
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(query, myOleDbConnection);
            OnStart();
            try
            {

                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();
                while (myDataReader.Read())
                {
                    OnAddItem(ReadResult(myDataReader));                    
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

        private void OnStart()
        {
            EventHandler temp = OnStartSearch;
            if (temp != null)
                temp(this, new EventArgs());
        }

        private void OnComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = OnCompleteSearch;
            if (temp != null)
                temp(this, new EventArgs<bool>(res));
        }

        private void OnAddItem(SearchItem item)
        {
            EventHandler<EventArgs<SearchItem>> temp = OnAddSearchItem;
            if (temp != null)
                temp(this, new EventArgs<SearchItem>(item));
        }

        private SearchItem ReadResult(IDataReader reader)
        {
            string name = reader[0] as string;
            string file = reader[1] as string;
            int index = -1;
            if ( (index =  file.IndexOf(FILEPREFIX1)) > -1 )
            {
                file = file.Substring(index + FILEPREFIX1.Length);
            }
            else if ((index = file.IndexOf(FILEPREFIX)) > -1)
            {
                file = file.Substring(index + FILEPREFIX.Length);
            }
            

            return new SearchItem() { Name = name, FileName = file };
        }

        #endregion


    }
}
