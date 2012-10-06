using System;
using System.Data.OleDb;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using C4F.DevKit.PreviewHandler.Service.Logger;
using WSSQLGUI.Controllers;
using WSSQLGUI.Core;
using System.Data;
using WSSQLGUI.Services.Enums;
using WSSQLGUI.Views;
using WSSQLGUI.Models;
using WSSQLGUI.Services.Helpers;

namespace WSSQLGUI.Kinds
{
	internal class ContactStrategy : BaseKindItemStrategy
	{
        private string _queryContectEmail = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) AND CONTAINS(System.Message.FromAddress,'{1}*')  ORDER BY System.Message.DateReceived DESC) ";
	    private volatile string _currentEmail = String.Empty;
	    private volatile string _folder = String.Empty;

        public ContactStrategy()
        {
            _queryTemplate = "SELECT TOP 1 System.ItemName, System.Contact.FirstName, System.Contact.LastName,System.Contact.EmailAddress,System.Contact.EmailAddress2,System.Contact.EmailAddress3 FROM SystemIndex WHERE System.Kind = 'contact' AND ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') )";
            _queryAnd = " AND ( CONTAINS(System.Contact.FirstName,'\"{0}*\"') OR CONTAINS(System.Contact.LastName,'\"{0}*\"') ) ";
            ID = 2;
            _name = "Contact";
            SettingsTaskType = typeof (ContactSettingsTask);
            DataTaskType = typeof (ContactDataTask);
        }


        protected override void ReadData(IDataReader reader)
        {
            if (reader == null)
                return;
            string itemname = reader[0].ToString();
            string first = reader[1].ToString();
            string last = reader[2].ToString();
            string em1 = reader[3].ToString();
            string em2 = reader[4].ToString();
            string em3 = reader[5].ToString();

            ContactSearchData data = new ContactSearchData()
                                         {
                                             Name = itemname,
                                             Path = string.Empty,
                                             FirstName = first,
                                             LastName = last,
                                             EmailAddress = em1,
                                             EmailAddress2 = em2,
                                             EmailAddress3 = em3,
                                             ID = Guid.NewGuid(),
                                             Type = TypeSearchItem.Contact
                                         };
            _currentEmail = em1;
            data.Foto = OutlookHelper.Instance.GetContactFotoTempFileName(data);
            //TODO: add SetData;
            (_dataController as ContactDataController).SetContactData(data);
        }

        protected override string CreateSqlQuery()
        {
            var searchCriteria = (SettingsView as IContactSettingsView).SearchCriteria;
            string res = String.Empty;

            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder strBuilder = new StringBuilder();
                var arr = searchCriteria.Split(' ').ToList();
                if (arr == null || arr.Count == 1)
                    return string.Format(_queryTemplate, searchCriteria);
                res = string.Format(_queryTemplate, arr[0]);
                for (int i = 1; i < arr.Count;i++)
                {
                    strBuilder.Append(string.Format(_queryAnd, arr[i]));
                }
                res += strBuilder.ToString();
            }
            else
                res = string.Format(_queryTemplate, searchCriteria);

            return res;
        }

        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            (DataView as IDataView).IsLoading = false;
        }

        protected override void OnStart()
        {
            base.OnStart();
            _folder = (SettingsView as IContactSettingsView).FolderContact;
        }

        // read contacts email
        protected override void DoAddidionalQuery()
        {
            if (string.IsNullOrEmpty(_currentEmail))
                return;
            var query = string.Format(_queryContectEmail, _folder, _currentEmail);
            OleDbDataReader myDataReader = null;
            OleDbConnection myOleDbConnection = new OleDbConnection(_connectionString);
            OleDbCommand myOleDbCommand = new OleDbCommand(query, myOleDbConnection);
            try
            {
                myOleDbConnection.Open();
                myDataReader = myOleDbCommand.ExecuteReader();
                while (myDataReader.Read())
                {
                    ReadContactEmail(myDataReader);
                }

            }
            finally
            {
                if (myDataReader != null)
                {
                    myDataReader.Close();
                    myDataReader.Dispose();
                }
                if (myOleDbConnection.State == System.Data.ConnectionState.Open)
                {
                    myOleDbConnection.Close();
                }

            }
            base.DoAddidionalQuery();
        }

        private  void ReadContactEmail(System.Data.IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return;
            var item = groups.Items[0];
            TypeSearchItem type = SearchItemHelper.GetTypeItem(item.Path);
            EmailSearchData si = new EmailSearchData()
            {
                Subject = item.Subject,
                Recepient = string.Format("{0} ({1})",
                item.Recepient, groups.Items.Count),
                Name = item.Name,
                Path = item.Path,
                Date = item.Date,
                Type = type,
                ID = Guid.NewGuid()
            };
            
            //TODO: paste item to datacontroller;
            (_dataController as ContactDataController).SetData(si);
        }

	}
}
