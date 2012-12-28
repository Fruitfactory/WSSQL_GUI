using System;
using System.Data;
using System.Linq;
using System.Text;
using C4F.DevKit.PreviewHandler.Service.Logger;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Module.Strategy;
using System.Collections.ObjectModel;

namespace WSUI.Module.ViewModel
{
    public class EmailViewModel : KindViewModelBase,IUView<EmailViewModel>
    {
        private const string OrderTemplate = " ORDER BY System.Message.DateReceived DESC)";
        private const string FilterByFolder = " AND CONTAINS(System.ItemPathDisplay,'{0}*',1033) ";


        public EmailViewModel(IUnityContainer container, ISettingsView<EmailViewModel> settingsView, IDataView<EmailViewModel> dataView)
            :base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            QueryTemplate = "GROUP ON System.Message.ConversationID OVER( SELECT System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID,System.Message.ConversationIndex FROM SystemIndex WHERE System.Kind = 'email' {0}AND CONTAINS(*,{1}) ";//¬ход€щие  //Inbox
            QueryAnd = " AND \"{0}\"";
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            DataSourceMail = new ObservableCollection<EmailSearchData>();
            Folder = OutlookHelper.AllFolders;
        }


        public ObservableCollection<EmailSearchData> DataSourceMail { get; private set; }


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
            var folder = Folder;
            var searchCriteria = SearchString.Trim();
            string res = string.Empty;
            string andClause = string.Empty;
            if (searchCriteria.IndexOf(' ') > -1)
            {
                StringBuilder temp = new StringBuilder();
                var list = searchCriteria.Split(' ').ToList();

                temp.Append(string.Format("'\"{0}\"", list[0]));
                for (int i = 1; i < list.Count; i++)
                {
                    temp.Append(string.Format(QueryAnd, list[i]));
                }
                andClause = temp.ToString() + "'";
            }
            res = string.Format(QueryTemplate, folder != OutlookHelper.AllFolders ? string.Format(FilterByFolder,folder) : string.Empty , string.IsNullOrEmpty(andClause) ? string.Format("'\"{0}\"'", searchCriteria) : andClause) + OrderTemplate;

            return res;

        }

        protected override void OnInit()
        {
            base.OnInit();
            _commandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
        }

        protected override void OnStart()
        {
            base.OnStart();
            ClearDataSource();
            ClearMainDataSource();
            DataSourceMail.Clear();
            OnPropertyChanged(() => DataSourceMail);
            _listData.Clear();

            FireStart();
            Enabled = false;
            OnPropertyChanged(() => Enabled);
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
