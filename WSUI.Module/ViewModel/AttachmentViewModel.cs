using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    public class AttachmentViewModel : KindViewModelBase, IUView<AttachmentViewModel>
    {
        public AttachmentViewModel(IUnityContainer container, ISettingsView<AttachmentViewModel> settingsView, IDataView<AttachmentViewModel> dataView ) : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            QueryTemplate =
                "SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateModified FROM SystemIndex WHERE Contains(System.ItemUrl,'at') AND (System.ItemName LIKE '%{0}%' OR  Contains(System.Search.Contents,{1}))"; //Contains(System.ItemName,'{0}*')  OR System.Search.Contents
            QueryAnd = " AND \"{0}\"";
            ID = 3;
            _name = "Attachments";
            UIName = _name;
            _prefix = "Attachment";
        }

        protected override void ReadData(System.Data.IDataReader reader)
        {
            string name = reader[0].ToString();
            string file = reader[1].ToString();
            var kind = reader[2] as object[];
            string id = reader[3].ToString();
            string display = reader[4].ToString();
            var date = reader[5].ToString();
            string tag = string.Empty;
            TypeSearchItem type = SearchItemHelper.GetTypeItem(file, kind != null && kind.Length > 0 ? kind[0].ToString() : string.Empty);
            if (type != TypeSearchItem.Attachment)
                return;
            var bs = new BaseSearchData()
            {
                Name = name,
                Path = file,
                Type = type,
                ID = Guid.NewGuid(),
                Display = display,
                DateModified = DateTime.Parse(date),
                Tag = tag
            };
            _listData.Add(bs);

        }

        protected override string CreateQuery()
        {
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
            res = string.Format(QueryTemplate, searchCriteria, string.IsNullOrEmpty(andClause) ? string.Format("'\"{0}\"'", searchCriteria) : andClause);

            return res;
        }

        protected override void OnInit()
        {
            base.OnInit();
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            _commandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
        }

        protected override void OnStart()
        {
            base.OnStart();
            FireStart();
            Enabled = false;
            OnPropertyChanged(() => Enabled);
        }


        public ISettingsView<AttachmentViewModel> SettingsView
        {
            get; set;
        }

        public IDataView<AttachmentViewModel> DataView
        {
            get; set;
        }
    }
}
