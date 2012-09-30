using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using WSSQLGUI.Core;
using System.Data;
using WSSQLGUI.Models;
using System.Data.OleDb;
using WSSQLGUI.Services.Helpers;
using WSSQLGUI.Services.Enums;
using WSSQLGUI.Views;
using WSSQLGUI.Controllers;
using System.Threading;

namespace WSSQLGUI.Kinds
{
	internal class EmailStrategy : BaseKindItemStrategy
	{


        public EmailStrategy()
        {
            _queryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(*,'{0}*') AND CONTAINS(System.ItemPathDisplay,'¬ход€щие*',1033) ORDER BY System.Message.DateReceived DESC) ";//¬ход€щие  //Inbox
            _queryAnd = " AND Contains(*,'{0}*')";
            ID = 1;
            _name = "E-Mail";
            SettingsTaskType = typeof(EmailSettingsTask);
            DataTaskType = typeof(EmailDataTask);
        }

        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            (DataView as IDataView).IsLoading = false;
        }

        protected override void ReadData(System.Data.IDataReader reader)
        {
            var groups = ReadGroups(reader);
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
            (DataView as EmailDataController).SetData(si);
        }

        protected override string CreateSqlQuery()
        {
            var searchCriteria = string.Empty;
            string res = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();
                if (list == null || list.Count == 1)
                    return string.Format(_queryTemplate, searchCriteria);
                res = string.Format(_queryTemplate, list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(_queryAnd, list[i]));
                }
                res += temp.ToString();// +groupOn;
            }
            else
                res = string.Format(_queryTemplate, searchCriteria);// +groupOn;

            return res;
        }


        #region private 

        private EmailData ReadGroups(IDataReader reader)
        {
            EmailData d = new EmailData();
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

        private EmailSearchData ReadItem(IDataReader reader)
        {
            string subject = reader[0].ToString();
            string name = reader[1].ToString();
            var recArr = reader[3] as string[];
            string recep = string.Empty;
            if (recArr.Length > 0)
            {
                recep = recArr[0];
            }
            string url = reader[2].ToString();

            DateTime res;
            string date = reader[4].ToString();
            DateTime.TryParse(date, out res);

            return new EmailSearchData() { Subject = subject, Name = name, Path = url, Recepient = recep, Date = res };
        }

        #endregion


    }
}
