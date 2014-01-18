using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Core.Core;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Attachments, 3, @"pack://application:,,,/WSUI.Module;Component/Images/Mail-Attachment.png")]
    public class AttachmentViewModel : KindViewModelBase, IUView<AttachmentViewModel>, IScrollableView
    {
        private int _countAdded = 0;
        private int _countProcess;
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
                "SELECT System.ItemName, System.ItemUrl,System.Kind,System.Message.ConversationID,System.ItemNameDisplay, System.DateModified,System.Size FROM SystemIndex WHERE Contains(System.ItemUrl,'at=') AND System.DateModified < '{2}'  AND ( ( {0} ) OR  Contains(System.Search.Contents,{1})) ORDER BY System.DateModified DESC"; //Contains(System.ItemName,'{0}*')  OR System.Search.Contents  AND System.DateCreated < '{2}'
            QueryAnd = " AND \"{0}\"";
            ID = 3;
            _name = "Attachments";
            UIName = _name;
            _prefix = "Attachment";
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);

            SearchSystem = new AttachmentSearchSystem();
        }

        

        protected override void OnInit()
        {
            base.OnInit();
            SearchSystem.Init();
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            ScrollBehavior = new ScrollBehavior() { CountFirstProcess = 100, CountSecondProcess = 50, LimitReaction = 75 };
            ScrollBehavior.SearchGo += OnScroolNeedSearch;
        }

        protected override void OnStart()
        {
            _list.Clear();
            ListData.Clear();
            FireStart();
            Enabled = false;
        }

        protected override void OnSearchStringChanged()
        {
            _listId.Clear();
            _countProcess = ScrollBehavior.CountFirstProcess;
            ClearDataSource();
            base.OnSearchStringChanged();

        }

        protected override void OnFilterData()
        {
            _listId.Clear();
            base.OnFilterData();
            _countProcess = ScrollBehavior.CountFirstProcess;
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


        

    }
}
