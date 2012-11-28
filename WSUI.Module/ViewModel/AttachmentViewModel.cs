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

            _queryTemplate =
                "SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateModified FROM SystemIndex WHERE Contains(*,'{0}*')";
            _queryAnd = " AND Contains(*,'{0}*')";
            ID = 3;
            _name = "Attachments";
            UIName = _name;
            _prefix = "Attachment";
            DataSourceAttachment = new ObservableCollection<BaseSearchData>();
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

        protected override void OnInit()
        {
            base.OnInit();
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            _commandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
        }

        protected override void OnFilterData()
        {
            if (_parentViewModel == null || _parentViewModel.MainDataSource.Count == 0)
                return;
            DataSourceAttachment.Clear();
            _parentViewModel.MainDataSource.ForEach(item =>
            {
                if (item.Type == TypeSearchItem.Attachment && item is BaseSearchData)
                {
                    DataSourceAttachment.Add(item as BaseSearchData);
                }
            });
            OnPropertyChanged(() => DataSourceAttachment);
        }

        protected override void OnStart()
        {
            ClearDaraSource();
            DataSourceAttachment.Clear();
            OnPropertyChanged(() => DataSourceAttachment);
            _listData.Clear();

            FireStart();
            Enabled = false;
            OnPropertyChanged(() => Enabled);
        }

        protected override void OnComplete(bool res)
        {
            FireComplete(res);
            Application.Current.Dispatcher.BeginInvoke(new Action(() => _listData.ForEach(s => DataSourceAttachment.Add(s))), null);
            OnPropertyChanged(() => DataSourceAttachment);
            Enabled = true;
            OnPropertyChanged(() => Enabled);
        }

        public ObservableCollection<BaseSearchData> DataSourceAttachment { get; private set; }

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
