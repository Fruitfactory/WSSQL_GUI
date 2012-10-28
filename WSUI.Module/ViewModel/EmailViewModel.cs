using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Threading;
using C4F.DevKit.PreviewHandler.Service.Logger;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;

namespace WSUI.Module.ViewModel
{
    public class EmailViewModel : KindViewModelBase,IUView<EmailViewModel>
    {
        private const string OrderTemplate = " ORDER BY System.Message.DateReceived DESC)";

        public EmailViewModel(IUnityContainer container, ISettingsView<EmailViewModel> settingsView, IDataView<EmailViewModel> dataView )
            :base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            _queryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) AND CONTAINS(*,'{1}*') ";//¬ход€щие  //Inbox
            _queryAnd = " AND Contains(*,'{0}*')";
            ID = 1;
            _name = "E-Mail";
            UIName = _name;
            _prefix = "Email";
        }


        protected override void ReadData(IDataReader reader)
        {
            var groups = EmailGroupReaderHelpers.ReadGroups(reader);
            if (groups == null)
                return;
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
            _listData.Add(si);
        }

        protected override string CreateQuery()
        {
            var searchCriteria = SearchString;
            var folder = "¬ход€щие";//"Inbox";//
            SearchString = searchCriteria;
            string res = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();
                if (list == null || list.Count == 1)
                    return string.Format(_queryTemplate, searchCriteria);
                res = string.Format(_queryTemplate, folder, list[0]);
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

        #region IUIView

        public ISettingsView<EmailViewModel> SettingsView
        {
            get; set;
        }

        public IDataView<EmailViewModel> DataView
        {
            get; set;
        }

        #endregion

    }
}
