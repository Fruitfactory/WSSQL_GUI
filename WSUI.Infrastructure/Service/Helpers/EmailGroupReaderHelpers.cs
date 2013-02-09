﻿using System;
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
