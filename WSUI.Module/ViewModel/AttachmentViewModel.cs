using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Infrastructure.Core;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Enums;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId("Attachments", 3)]
    public class AttachmentViewModel : KindViewModelBase, IUView<AttachmentViewModel>, IScrollableView
    {
        private int _countAdded = 0;
        private int _countProcess;
        private DateTime _lastDate;

        public AttachmentViewModel(IUnityContainer container, ISettingsView<AttachmentViewModel> settingsView, IDataView<AttachmentViewModel> dataView ) : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            QueryTemplate =
                "GROUP ON System.Message.DateReceived  ORDER BY System.Message.DateReceived DESC  OVER (SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.Message.DateReceived FROM SystemIndex WHERE Contains(System.ItemUrl,'at') AND System.Message.DateReceived < '{2}'  AND ( ( {0} ) OR  Contains(System.Search.Contents,{1})))"; //Contains(System.ItemName,'{0}*')  OR System.Search.Contents  AND System.DateCreated < '{2}'
            QueryAnd = " AND \"{0}\"";
            ID = 3;
            _name = "Attachments";
            UIName = _name;
            _prefix = "Attachment";
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            _lastDate = DateTime.Now;
        }

        protected override void ReadData(System.Data.IDataReader reader)
        {
            var group = new List<BaseSearchData>();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).ToString() != "System.Data.IDataReader")
                    continue;
                OleDbDataReader itemsReader = reader.GetValue(i) as OleDbDataReader;

                while (itemsReader.Read())
                {
                    var item = ReadGroupData(itemsReader);
                    if(item != null)
                        group.Add(item);
                }
            }

            if (group.Count > 0)
            {
                var list = GroupDataByName(group);
                list.ForEach(i =>
                                 {
                                     ListData.Add(i);
                                     _lastDate = i.DateModified;
                                     _countAdded++;
                                 });
            }
             if (_countAdded == _countProcess)
                IsInterupt = true;
        }

        protected override string CreateQuery()
        {
            _countAdded = 0;
            var searchCriteria = SearchString.Trim();
            string res = string.Empty;
           
            ProcessSearchCriteria(searchCriteria);

            res = string.Format(QueryTemplate, LikeCriteria(), string.IsNullOrEmpty(_andClause) ? string.Format("'\"{0}\"'", _listW[0]) : _andClause,FormatDate(ref _lastDate));//,FormatDate(ref _lastDate)

            return res;
        }

        protected override void OnInit()
        {
            base.OnInit();
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            ScrollBehavior = new ScrollBehavior(){CountFirstProcess = 45, CountSecondProcess = 5,LimitReaction = 75};
            ScrollBehavior.SearchGo += () =>
                                           {
                                               ShowMessageNoMatches = false;
                                               Search();
                                           };
        }

        protected override void OnStart()
        {
            base.OnStart();
            FireStart();
            Enabled = false;
            OnPropertyChanged(() => Enabled);
        }

        protected override void OnSearchStringChanged()
        {
            _countProcess = ScrollBehavior.CountFirstProcess;
            _lastDate = DateTime.Now;
            ClearDataSource();
            base.OnSearchStringChanged();
            
        }

        protected override void OnFilterData()
        {
            base.OnFilterData();
            _countProcess = ScrollBehavior.CountFirstProcess;
            _lastDate = DateTime.Now;
        }

        protected override void OnComplete(bool res)
        {
            base.OnComplete(res);
            _countProcess = ScrollBehavior.CountSecondProcess;
        }


        public ISettingsView<AttachmentViewModel> SettingsView
        {
            get; set;
        }

        public IDataView<AttachmentViewModel> DataView
        {
            get; set;
        }

        #region Implementation of IScrollableView

        public ICommand ScrollChangeCommand { get; private set; }

        #endregion

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }


        private BaseSearchData ReadGroupData(IDataReader reader)
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
                return null;
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
            return bs;
        }

        private List<BaseSearchData> GroupDataByName(IEnumerable<BaseSearchData> listData)
        {
            var result = new List<BaseSearchData>();
            var groups = listData.GroupBy(item => item.Name);
            foreach (var group in groups)
            {
                int count = 0;
                if ( (count =  group.Count()) > 0)
                {
                    var item = group.First();
                    item.Count = count.ToString();
                    result.Add(item);
                }
            }
            return result;
        }

    }
}
