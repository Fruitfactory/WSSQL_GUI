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
using WSUI.Core;
using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Utils.Dialog;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Service;
using WSUI.Module.Service.AdvancedSearch;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Email, 2, @"pack://application:,,,/WSUI.Module;Component/Images/Mail-1.png", "M0,4.0800388L0.030031017,4.0800388 12.610706,16.409995 26.621516,30.149985 40.642334,16.409995 53.223011,4.0800388 53.333001,4.0800388 53.333001,39.080039 0,39.080039z M3.1698808,0L26.660885,0 50.161892,0 38.411389,11.791528 26.660885,23.573054 14.920383,11.791528z")]
    public class EmailViewModel : KindViewModelBase, IUView<EmailViewModel>, IScrollableView
    {
        private readonly Dictionary<AdvancedSearchCriteriaType, string> _prefixes = new Dictionary<AdvancedSearchCriteriaType, string>();
        private bool _isAdvancedMode = false;

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
            Folder = OutlookHelper.AllFolders;
            TopQueryResult = 50;
            SearchSystem = new EmailSearchSystem();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            AddCriteria = new WSUIRelayCommand(AddExecute);
            RemoveCriteria = new WSUIRelayCommand(RemoveExecute);
            CriteriaSource = new ObservableCollection<IAdvancedSearchCriteria>();
            InitPrefixes();
        }

        

        public IEnumerable<MenuItem> EmailMenuItems { get { return Parent.EmailsMenuItems; } }

        public ObservableCollection<IAdvancedSearchCriteria> CriteriaSource { get; private set; }

        public bool IsAdvancedMode
        {
            get
            {
                return _isAdvancedMode;
            }
            set
            {
                _isAdvancedMode = value;
                OnAdvancedModeChanged();
            }
        }

        private void OnAdvancedModeChanged()
        {
            SearchSystem.Reset();
            SearchSystem.IsAdvancedMode = _isAdvancedMode;
            if (_isAdvancedMode)
            {
                AddCriteriaToList();
            }
            else
            {
                CriteriaSource.Clear();
            }
            SearchString = string.Empty;
            OnPropertyChanged(() => CriteriaSource);
            
        }

        private ICommand AddCriteria { get; set; }

        private ICommand RemoveCriteria { get; set; }
        
        #region IScrollableView Members

        public ICommand ScrollChangeCommand { get; protected set; }

        #endregion IScrollableView Members

        protected override void OnInit()
        {
            base.OnInit();
            ScrollBehavior = new ScrollBehavior {CountFirstProcess = 300, CountSecondProcess = 100, LimitReaction = 85};
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
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }

        private void RemoveExecute(object o)
        {
            var criteria = o as IAdvancedSearchCriteria;
            if (criteria.IsNull())
                return;
            (criteria as AdvancedSearchCriteria).PropertyChanged -= CriteriaOnPropertyChanged;
            CriteriaSource.Remove(criteria);
            if (CriteriaSource.Count == 1)
            {
                CriteriaSource.First().RemoveButtonVisibility = false;
            }
            OnPropertyChanged(() => CriteriaSource);
        }

        private void AddExecute(object o)
        {
            if (CriteriaSource.Count == 3)
                return;
            AddCriteriaToList();
            CriteriaSource.ForEach(c => c.RemoveButtonVisibility = true);
            OnPropertyChanged(() => CriteriaSource);
        }

        private void AddCriteriaToList(bool removeButtonVisibility = false)
        {
            var criteria = new AdvancedSearchCriteria(AddCriteria, RemoveCriteria){RemoveButtonVisibility = removeButtonVisibility};
            criteria.PropertyChanged += CriteriaOnPropertyChanged;
            CriteriaSource.Add(criteria);
        }

        private void InitPrefixes()
        {
            var type = typeof(AdvancedSearchCriteriaType);
            var values = Enum.GetValues(type);
            foreach (var value in values)
            {
                var mi = type.GetMember(value.ToString());
                if (mi.IsNull() || !mi.Any())
                    continue;
                var attr = mi.First().GetCustomAttributes(typeof(EnumPrefixAttribute), false).OfType<EnumPrefixAttribute>();
                if (attr.IsNull() || !attr.Any())
                    continue;
                _prefixes.Add((AdvancedSearchCriteriaType)value, attr.First().Prefix);
            }
        }

        private void CriteriaOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            var criteria = new StringBuilder();
            CriteriaSource.ForEach(c =>
            {
                if(c.CriteriaType == AdvancedSearchCriteriaType.None)
                    return;
                criteria.AppendFormat(GlobalConst.AdvancedSearchFormat, _prefixes[c.CriteriaType], c.Value);

            });

            SearchString = criteria.ToString();
            OnPropertyChanged(() => SearchString);
        }

        #region IUIView

        public ISettingsView<EmailViewModel> SettingsView { get; set; }

        public IDataView<EmailViewModel> DataView { get; set; }

        #endregion IUIView
    }
}