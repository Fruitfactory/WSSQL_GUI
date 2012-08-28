using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MVCSharp.Core;
using MVCSharp.Core.Views;
using WSSQLGUI.Models;
using WSSQLGUI.Services;
using System.Data.OleDb;
using System.Data;
using System.Threading;

namespace WSSQLGUI.Controllers
{
    class SearchController : ControllerBase
    {
        #region const

        private const string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private const string qyeryTemplate = "SELECT System.ItemName, System.ItemUrl  FROM SystemIndex WHERE Contains(*,'{0}*')";

        #endregion

        #region events

        public event EventHandler OnStartSearch;
        public event EventHandler<EventArgs<bool>> OnCompleteSearch;
        public event EventHandler<EventArgs<SearchItem>> OnAddSearchItem;

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

        #endregion

        #region public methods
        
        public void StartSearch(string searchString)
        {
            Thread thread = new Thread(() => DoQuery(searchString));
            thread.Start();
        }

        public void CurrentSearchItemChanged(SearchItem item)
        {
            CurrenItem = item;
        }

        #endregion

        #region private


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
                temp.Invoke(this, new EventArgs());
        }

        private void OnComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = OnCompleteSearch;
            if (temp != null)
                temp.Invoke(this, new EventArgs<bool>(res));
        }

        private void OnAddItem(SearchItem item)
        {
            EventHandler<EventArgs<SearchItem>> temp = OnAddSearchItem;
            if (temp != null)
                temp.Invoke(this, new EventArgs<SearchItem>(item));
        }

        private SearchItem ReadResult(IDataReader reader)
        {
            string name = reader[0] as string;
            string file = reader[1] as string;

            return new SearchItem() { Name = name, FileName = file };
        }

        #endregion


    }
}
