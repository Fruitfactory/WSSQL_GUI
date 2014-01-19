using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using WSUI.Core.Enums;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using Microsoft.Practices.Unity;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Email, 2, @"pack://application:,,,/WSUI.Module;Component/Images/Mail-1.png")]
    public class EmailViewModel : KindViewModelBase, IUView<EmailViewModel>, IScrollableView
    {
        public ICommand ScrollChangeCommand { get; protected set; }

        public EmailViewModel(IUnityContainer container, ISettingsView<EmailViewModel> settingsView, IDataView<EmailViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            QueryTemplate = " SELECT TOP {3}  System.Subject,System.ItemName,System.ItemUrl,System.Message.ToAddress,System.Message.DateReceived, System.Message.ConversationID FROM SystemIndex WHERE System.Kind = 'email' AND System.Message.DateReceived < '{2}' {0}AND CONTAINS(*,{1}) {4}";
            QueryAnd = " AND \"{0}\"";
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            Folder = OutlookHelper.AllFolders;
            TopQueryResult = 50;
            SearchSystem = new EmailSearchSystem();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
        }

       

        protected override void OnInit()
        {
            base.OnInit();
            SearchSystem.Init();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ScrollBehavior = new ScrollBehavior() {CountFirstProcess = 300,CountSecondProcess = 100 ,LimitReaction = 85};
            ScrollBehavior.SearchGo += OnScroolNeedSearch;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        protected override void OnSearchStringChanged()
        {
            base.OnSearchStringChanged();
            ClearDataSource();
            ClearMainDataSource();
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            ShowMessageNoMatches = true;
        }

        protected override void OnFilterData()
        {
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            ShowMessageNoMatches = true;
            base.OnFilterData();
        }

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }


        #region IUIView

        public ISettingsView<EmailViewModel> SettingsView
        {
            get;
            set;
        }

        public IDataView<EmailViewModel> DataView
        {
            get;
            set;
        }

        #endregion

    }
}
