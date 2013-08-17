using System;
using System.Data;
using System.Data.OleDb;
using WSPreview.PreviewHandler.Service.Logger;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Core;
using Outllok = Microsoft.Office.Interop.Outlook;

namespace WSUI.Infrastructure.Service.Helpers
{
    public class EmailGroupReaderHelpers
    {
        private const string ConnectionString = "Provider=Search.CollatorDSO;Extended Properties=\"Application=Windows\"";
        private const string QueryForGroupEmails =
    "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email'  AND CONTAINS(System.Message.ConversationID,'{0}*')   ORDER BY System.Message.DateReceived DESC) ";//AND CONTAINS(System.ItemPathDisplay,'{0}*',1033)

        private const string QueryForEmailDetails =
            "SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex,System.Search.EntryID FROM SystemIndex WHERE System.Kind = 'email'  AND CONTAINS(System.ItemUrl,'{0}*')   ORDER BY System.Message.DateReceived DESC";


        public static EmailSearchData FindEmailDetails(string path)
        {
            EmailSearchData data = null;

            var mail = OutlookHelper.Instance.GetEmailItem(new BaseSearchData() { Path = path }) as Outllok.MailItem;
            if (mail != null)
            {
                data = new EmailSearchData()
                {
                    Subject = mail.Subject,
                    Recepient = mail.To,
                    Date = mail.ReceivedTime,
                    Path = path
                };
                data.Attachments = OutlookHelper.Instance.GetAttachments((BaseSearchData)data);
            }

            return data;
        }


        public static EmailSearchData GroupEmail(string id)
        {
            var query = string.Format(QueryForGroupEmails, id);
            EmailSearchData data = null;
            OleDbDataReader reader = null;
            OleDbConnection con = new OleDbConnection(ConnectionString);
            OleDbCommand cmd = new OleDbCommand(query, con);
            try
            {

                con.Open();
                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    data = ReadGroup(reader);
                    break;
                }
            }
            catch (System.Data.OleDb.OleDbException oleDbException)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {2}", "GroupEmail", oleDbException.Message));
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }
                if (con.State == System.Data.ConnectionState.Open)
                {
                    con.Close();
                }
            }
            return data;
        }

        public static EmailSearchData ReadGroup(IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return null;
            var item = groups.Items[0];
            TypeSearchItem type = SearchItemHelper.GetTypeItem(item.Path);
            EmailSearchData si = new EmailSearchData()
            {
                Subject = item.Subject,
                Recepient = string.Format("{0}",
                item.Recepient),
                Count = groups.Items.Count.ToString(),
                Name = item.Name,
                Path = item.Path,
                Date = item.Date,
                Type = type,
                ConversationId = item.ConversationId,
                ID = Guid.NewGuid()
            };
            try
            {
                si.Attachments = OutlookHelper.Instance.GetAttachments(item);
            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {2}", "ReadGroup - AllFiles", e.Message));
            }
            return si;
        }


        public static EmailData ReadGroups(IDataReader reader)
        {
            EmailData d = new EmailData();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).ToString() != "System.Data.IDataReader")
                    continue;
                OleDbDataReader itemsReader = reader.GetValue(i) as OleDbDataReader;
                while (itemsReader.Read())
                {
                    d.Items.Add(ReadItem(itemsReader));
                }
                d.Items.Sort((e1,e2) =>
                                 {
                                     return e1.Date > e2.Date
                                                ? -1
                                                : e1.Date < e1.Date
                                                      ? 1
                                                      : 0;
                                 });
            }

            return d;
        }





        private static EmailSearchData ReadItem(IDataReader reader)
        {
            string subject = reader[0].ToString();
            string name = reader[1].ToString();
            var recArr = reader[3] as string[];
            string recep = string.Empty;
            if (recArr != null && recArr.Length > 0)
            {
                recep = recArr[0];
            }
            string url = reader[2].ToString();

            var datetime = reader[4]; 
            DateTime res;
            DateTime.TryParse(datetime.ToString(), out res);

            string conversationID = reader[5] as string;
            string entryId = string.Empty;
            if(reader.FieldCount > 7)
                entryId = reader[7].ToString();

            return new EmailSearchData() { Subject = subject, Name = name, Path = url, Recepient = recep, Date = res, ConversationId = conversationID, LastId = entryId };

        }
    }
}
