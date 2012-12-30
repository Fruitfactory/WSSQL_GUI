using System;
using System.Data;
using System.Data.OleDb;
using WSUI.Infrastructure.Models;

namespace WSUI.Infrastructure.Service.Helpers
{
    public class EmailGroupReaderHelpers
    {
        static public EmailData ReadGroups(IDataReader reader)
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

            string conversationIndex = reader[6] as string;

            return new EmailSearchData() { Subject = subject, Name = name, Path = url, Recepient = recep, Date = res,ConversationIndex = conversationIndex};

        }
    }
}
