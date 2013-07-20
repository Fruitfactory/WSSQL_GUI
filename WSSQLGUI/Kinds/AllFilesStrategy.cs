using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using WSSQLGUI.Models;
using WSSQLGUI.Views;
using WSSQLGUI.Controllers;
using System.Threading;
using System.Data.OleDb;
using System.Data;
using WSSQLGUI.Services.Enums;
using WSSQLGUI.Services.Helpers;
using MVCSharp.Core.Views;
using WSPreview.PreviewHandler.Service.Logger;

namespace WSSQLGUI.Kinds
{
    internal class AllFilesStrategy : BaseKindItemStrategy
    {
        private const string KindGroup = "email";
        private const string InboxFolder = "¬ход€щие";//"Inbox";
        private const string QueryForGroupEmails = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(System.Message.ConversationID,'{0}*')   ORDER BY System.Message.DateReceived DESC) ";//AND CONTAINS(System.ItemPathDisplay,'{1}*',1033)


        private  List<string> _listID = new List<string>(); 


        public AllFilesStrategy()
        {
            // init
            _queryTemplate = "SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID FROM SystemIndex WHERE Contains(*,'{0}*')";
            _queryAnd = " AND Contains(*,'{0}*')";
            ID = 0;
            _name = "All Files";
            _prefix = "AllFiles";
        }


        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            (DataView as IDataView).IsLoading = false;
            _listID.Clear();
        }

        protected override void ReadData(IDataReader reader)
        {
            string name = reader[0].ToString();
            string file = reader[1].ToString();
            var kind = reader[2];
            string id = reader[3].ToString();

            if (kind != null && IsEmail(kind) && !_listID.Any(i => i == id))
            {
                var newValue = GroupEmail(name,id);
                _listID.Add(id);
                name = newValue.Item1;
                file = newValue.Item2;
            }
            else if (kind != null && IsEmail(kind) && _listID.Any(i => i == id))
                return;
            TypeSearchItem type = SearchItemHelper.GetTypeItem(file);
            (_dataController as AllFilesDataController).SetData( new BaseSearchData() { Name = name, Path = file, Type = type, ID = Guid.NewGuid() });
        }

        protected override string CreateSqlQuery()
        {
            var searchCriteria = (SettingsView as IAllFilesSettingsView).SearchCriteria;
            SearchString = searchCriteria;
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


        #region private

        private Tuple<string,string>  GroupEmail(string name,string id)
        {
            var query = string.Format(QueryForGroupEmails, id);//,InboxFolder
            int count = 0;
            string path = string.Empty;
            OleDbDataReader reader = null;
            OleDbConnection con = new OleDbConnection(_connectionString);
            OleDbCommand cmd = new OleDbCommand(query, con);
            try
            {

                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    var resg = ReadGroup(reader);
                    count = resg.Item1;
                    path = resg.Item2;
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


            return new Tuple<string, string>(string.Format("{0}, ({1})",name,count),path);
        }


        private Tuple<int,string> ReadGroup(IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return default(Tuple<int,string>);
            var item = groups.Items[0];

            var res = new Tuple<int, string>(groups.Items.Distinct(new EmailSearchDataComparer()).Count(), item.Path);
            return res;
        }

        private bool IsEmail(object value)
        {
            if (value.GetType().IsArray)
            {
                return (value as Array).Cast<string>().Any(i => i.IndexOf(KindGroup) > -1);
            }
            return false;
        }


        #endregion

    }
}
