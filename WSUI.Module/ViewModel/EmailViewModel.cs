using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core;
using OF.Core.Core.Attributes;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Utils.Dialog;
using OF.Infrastructure.Implements.Systems;
using OF.Infrastructure.Service;
using OF.Module.Core;
using OF.Module.Enums;
using OF.Module.Interface;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Service;
using OF.Module.Service.AdvancedSearch;
using OF.Module.Strategy;

namespace OF.Module.ViewModel
{
    [KindNameId(OFKindsConstName.Email, 2, @"pack://application:,,,/OF.Module;Component/Images/Mail-1.png", "M0,4.0800388L0.030031017,4.0800388 12.610706,16.409995 26.621516,30.149985 40.642334,16.409995 53.223011,4.0800388 53.333001,4.0800388 53.333001,39.080039 0,39.080039z M3.1698808,0L26.660885,0 50.161892,0 38.411389,11.791528 26.660885,23.573054 14.920383,11.791528z")]
    public class EmailViewModel : OFKindViewModelBase, IUView<EmailViewModel>, IScrollableView
    {
        

        public EmailViewModel(IUnityContainer container, IEventAggregator eventAggregator, ISettingsView<EmailViewModel> settingsView,
            IDataView<EmailViewModel> dataView)
            : base(container,eventAggregator)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            ID = 2;
            _name = "Email";
            UIName = _name;
            _prefix = "Email";
            Folder = OFOutlookHelper.AllFolders;
            TopQueryResult = 50;
            SearchSystem = new OFEmailSearchSystem();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
        }

        

        public IEnumerable<MenuItem> EmailMenuItems { get { return Parent.EmailsMenuItems; } }

        #region IScrollableView Members

        public ICommand ScrollChangeCommand { get; protected set; }

        #endregion IScrollableView Members

        protected override void OnInit()
        {
            base.OnInit();
            ScrollBehavior = new OFScrollBehavior {CountFirstProcess = 300, CountSecondProcess = 100, LimitReaction = 85};
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        protected override void OnSearchStringChanged()
        {
            base.OnSearchStringChanged();
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
            var scrollArgs = args as OFScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }

        

        #region IUIView

        public ISettingsView<EmailViewModel> SettingsView { get; set; }

        public IDataView<EmailViewModel> DataView { get; set; }

        #endregion IUIView
    }
}