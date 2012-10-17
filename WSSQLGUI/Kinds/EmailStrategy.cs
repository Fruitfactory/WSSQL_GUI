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
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace WSSQLGUI.Kinds
{
	internal class EmailStrategy : BaseKindItemStrategy
	{
        private readonly string OrderTemplate = " ORDER BY System.Message.DateReceived DESC)";

        public EmailStrategy()
        {
            _queryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) AND CONTAINS(*,'{1}*') ";//¬ход€щие  //Inbox
            _queryAnd = " AND Contains(*,'{0}*')";
            ID = 1;
            _name = "E-Mail";
            _prefix = "Email";
        }

        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            (DataView as IDataView).IsLoading = false;
        }

        protected override void ReadData(System.Data.IDataReader reader)
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
            try
            {

                si.Attachments = OutlookHelper.Instance.GetAttachments(item);

            }
            catch (Exception e)
            {
                WSSqlLogger.Instance.LogError(e.Message);
            }

            //TODO: paste item to datacontroller;
            (_dataController as EmailDataController).SetData(si);
        }

        protected override string CreateSqlQuery()
        {
            var searchCriteria = (SettingsView as IEmailSettingsView).SearchCriteria;
            var folder = (SettingsView as IEmailSettingsView).Folder;
            SearchString = searchCriteria;
            string res = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();
                if (list == null || list.Count == 1)
                    return string.Format(_queryTemplate, searchCriteria);
                res = string.Format(_queryTemplate,folder,list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(_queryAnd, list[i]));
                }
                res += temp.ToString() + OrderTemplate;
            }
            else
                res = string.Format(_queryTemplate, folder, searchCriteria) + OrderTemplate;

            return res;
        }


        

    }
}
