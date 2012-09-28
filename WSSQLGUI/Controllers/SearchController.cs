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
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Text.RegularExpressions;
using WSSQLGUI.Services.Helpers;
using WSSQLGUI.Services.Enums;
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace WSSQLGUI.Controllers
{
    class SearchController : ControllerBase
    {
        #region const

        private const string connectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private const string qyeryTemplate = "SELECT System.ItemName, System.ItemUrl, System.IsAttachment, System.Message.ConversationID, System.Message.DateReceived  FROM SystemIndex WHERE Contains(*,'{0}*')";
        private const string emailQueryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(*,'{0}*') AND CONTAINS(System.ItemPathDisplay,'Входящие*',1033) ORDER BY System.Message.DateReceived DESC) ";//Входящие  //Inbox
        private const string qyeryAnd = " AND Contains(*,'{0}*')";
        
        

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
        public string FileName { get; private set;}

        #endregion

        #region public methods
       
        public void CurrentSearchItemChanged(SearchItem item)
        {
            CurrenItem = item;
            FileName = SearchItemHelper.GetFileName(CurrenItem);
        }

        #endregion

        #region private

        private void OpenCurrentFile()
        {

            if (CurrenItem == null)
                return;

            if (string.IsNullOrEmpty(FileName) ||
                FileService.IsDirectory(FileName))
                return;
            try
            {
                Process.Start(FileName);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "Open preview", ex.Message));
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
            string query = string.Format(emailQueryTemplate, queryString); //CreateSqlQyery((string)queryString);

            WSSqlLogger.Instance.LogInfo(string.Format("{0} - {1}", "User query", query));

            bool result = true;
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(query, myOleDbConnection);
            OnStart();
            try
            {

                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();


                List<EmailData> tempList = new List<EmailData>();
                while (myDataReader.Read())
                {
                    //OnAddItem(ReadResult(myDataReader));
                    //tempList.Add(ReadResult(myDataReader));
                    tempList.Add(ReadGroups(myDataReader));
                }

                foreach (var g in tempList)
                {
                    var item = g.Items[0];
                    TypeSearchItem type = SearchItemHelper.GetTypeItem(item.ItemUrl);
                    SearchItem si = new SearchItem()
                    {
                        Subject = item.Subject,
                        Recepient = string.Format("{0} ({1})",
                        item.Recepient, g.Items.Count),
                        Name = item.ItemName,
                        FileName = item.ItemUrl,
                        Date = item.Date,
                        Type = type
                    };

                    OnAddItem(si);
                }


            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1}", "Do Query", oleDbException.Message));
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

        private string CreateSqlQyery(string searchCriteria)
        {
            string res = string.Empty;
            if(searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();
                if (list == null || list.Count == 1)
                    return string.Format(qyeryTemplate, searchCriteria);
                res = string.Format(qyeryTemplate, list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(qyeryAnd,list[i]));
                }
                res += temp.ToString();// +groupOn;
            }
            else
                res = string.Format(qyeryTemplate, searchCriteria);// +groupOn;

            return res;
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

        private SearchData ReadResult(IDataReader reader)
        {
            string name = reader[0] as string;
            string file = reader[1] as string;
            bool att = (bool) reader[2];
            string convID = reader[3].ToString();
            string date = reader[4].ToString();

            //TypeSearchItem type = SearchItemHelper.GetTypeItem(file);
            
            //return new SearchItem() { Name = name, FileName = file,IsAttachment = att,ID = Guid.NewGuid(),Type = type };
            return new SearchData(){ItemName = name, ItemUrl = file, IsAttachment = att, ConversationID = convID,Date = date};
        }

        
        private EmailData ReadGroups(IDataReader reader)
        {
            EmailData d = new EmailData() { Type = TypeRecord.Group};
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).ToString() != "System.Data.IDataReader")
                    continue;
                //WSSqlLogger.Instance.LogInfo(reader.GetName(i));
                OleDbDataReader itemsReader = reader.GetValue(i) as OleDbDataReader;
                while (itemsReader.Read())
                {
                    d.Items.Add(ReadItem(itemsReader));
                }
            }

            return d;
        }

        private EmailItem  ReadItem(IDataReader reader)
        {
            string subject = reader[0].ToString();
            string name = reader[1].ToString();
            var recArr = reader[3] as string[];
            string recep = string.Empty;
            if (recArr.Length > 0)
            {
                recep = recArr[0];
            }
            string url  = reader[2].ToString();
            
            DateTime res;
            string date = reader[4].ToString();
            DateTime.TryParse(date,out res);
            //res = TimeZoneInfo.ConvertTimeToUtc(res);
            
            //WSSqlLogger.Instance.LogInfo(string.Format("{0} {1} {2} {3}", name, url, recep, res));

            return new EmailItem() {Subject = subject, ItemName = name, ItemUrl = url, Recepient = recep, Date = res, Type = TypeRecord.Item};
        }

        #endregion


    }
}
