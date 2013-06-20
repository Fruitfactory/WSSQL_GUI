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
    [KindNameId(KindsConstName.Attachments, 3)]
    public class AttachmentViewModel : KindViewModelBase, IUView<AttachmentViewModel>, IScrollableView
    {
        private int _countAdded = 0;
        private int _countProcess;
        private DateTime _lastDate;
        private List<BaseSearchData> _list = new List<BaseSearchData>();
        private List<string> _listId = new List<string>();
        private object _lock = new object();

        public AttachmentViewModel(IUnityContainer container, ISettingsView<AttachmentViewModel> settingsView, IDataView<AttachmentViewModel> dataView ) : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            QueryTemplate =
                "SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateCreated,System.Size FROM SystemIndex WHERE Contains(System.ItemUrl,'at=') AND System.DateCreated < '{2}'  AND ( ( {0} ) OR  Contains(System.Search.Contents,{1})) ORDER BY System.DateCreated DESC"; //Contains(System.ItemName,'{0}*')  OR System.Search.Contents  AND System.DateCreated < '{2}'
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

            var item = ReadGroupData(reader);
            if(item != null)
                _list.Add(item);

            _countAdded = _list.GroupBy(i => new {Name = i.Name, Size = i.Size}).Count(); 

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

        protected override string LikeCriteria()
        {
            if (_listW.Count == 0)
                return string.Empty;
            var temp = new StringBuilder();

            temp.Append(string.Format("Contains(*,'\"{0}\"') ", _listW[0]));

            if (_listW.Count > 1)
                for (int i = 1; i < _listW.Count; i++)
                    temp.Append(string.Format(" Contains(*,'\"{0}\"') ", _listW.ElementAt(i)));

            return temp.ToString();

        }

        protected override void OnInit()
        {
            base.OnInit();
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            ScrollBehavior = new ScrollBehavior(){CountFirstProcess = 100, CountSecondProcess = 50,LimitReaction = 75};
            ScrollBehavior.SearchGo += () =>
                                           {
                                               ShowMessageNoMatches = false;
                                               Search();
                                           };
        }

        protected override void OnStart()
        {
            _list.Clear();
            ListData.Clear();
            FireStart();
        }

        protected override void OnSearchStringChanged()
        {
            _listId.Clear();
            _countProcess = ScrollBehavior.CountFirstProcess;
            _lastDate = DateTime.Now;
            ClearDataSource();
            base.OnSearchStringChanged();
            
        }

        protected override void OnFilterData()
        {
            _listId.Clear();
            base.OnFilterData();
            _countProcess = ScrollBehavior.CountFirstProcess;
            _lastDate = DateTime.Now;
        }

        protected override void OnComplete(bool res)
        {
            var groups = _list.GroupBy(i => new {Name = i.Name, Size = i.Size});
            lock (_lock)
            {
                foreach (var group in groups)
                {
                    var item = group.FirstOrDefault();
                    if(_listId.Any(i => i == item.Tag.ToString())) // TODO temporary solution doesn't react on last DateTime
                        continue;
                    item.Count = group.Count().ToString();
                    _listId.Add(item.Tag.ToString());
                    ListData.Add(item);
                }                
            }
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
            string tag = reader[3].ToString();
            string display = reader[4].ToString();
            var date = reader[5].ToString();
            var strsize = reader[6].ToString();
            int size;
            int.TryParse(strsize, out size);
            DateTime last;
            DateTime.TryParse(date, out last);
            _lastDate = last;

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
                DateModified = last,
                Tag = tag,
                Size = size
            };
            return bs;
        }

    }
}
