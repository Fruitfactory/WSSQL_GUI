using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Input;
using WSUI.Core;
using WSUI.Core.Core.AdvancedSearchCriteria;
using WSUI.Core.Core.Attributes;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service;
using WSUI.Infrastructure.AdvancedSearch;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.AdvancedSearch, -1, @"", "", false)]
    public class AdvancedSearchViewModel : KindViewModelBase, IUView<AdvancedSearchViewModel>, IAdvancedSearchViewModel, IScrollableView
    {
        private readonly Dictionary<AdvancedSearchCriteriaType, string> _prefixes = new Dictionary<AdvancedSearchCriteriaType, string>();
        private bool _isAdvancedMode = false;

        public AdvancedSearchViewModel(IUnityContainer container, IEventAggregator eventAggregator, ISettingsView<AdvancedSearchViewModel> settingsView, IDataView<AdvancedSearchViewModel> dataView)
            : base(container, eventAggregator)
        {
            SettingsView = settingsView;
            DataView = dataView;
            SettingsView.Model = this;
            DataView.Model = this;
            CriteriaSource = new ObservableCollection<IAdvancedSearchCriteria>();
            SearchSystem = new EmailSearchSystem();
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            InitPrefixes();
            InitCriterias();
        }

        public IEnumerable<MenuItem> EmailMenuItems { get { return Parent.EmailsMenuItems; } }

        public ISettingsView<AdvancedSearchViewModel> SettingsView { get; set; }

        public IDataView<AdvancedSearchViewModel> DataView { get; set; }

        public ObservableCollection<IAdvancedSearchCriteria> CriteriaSource { get; private set; }

        protected override string GetInternalSearchPattern()
        {
            var pattern = new  StringBuilder();
            CriteriaSource.ForEach(c =>
            {
                if (c.CriteriaType == AdvancedSearchCriteriaType.None || c.Value.IsNull() || c.Value.IsStringEmptyOrNull())
                    return;
                pattern.AppendFormat(" {0}", c.Value.ToString());
            });
            return pattern.ToString();
        }

        protected override void OnInit()
        {
            base.OnInit();
            SearchSystem.IsAdvancedMode = true;
            ScrollBehavior = new ScrollBehavior { CountFirstProcess = 500, CountSecondProcess = 100, LimitReaction = 85 };
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            SearchSystem.SetProcessingRecordCount(ScrollBehavior.CountFirstProcess,ScrollBehavior.CountSecondProcess);
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

        private void AddCriteriaToList(AdvancedSearchCriteriaType type)
        {
            var criteria = new StringAdvancedSearchCriteria(null, null) { CriteriaType = type };
            criteria.PropertyChanged += CriteriaOnPropertyChanged;
            CriteriaSource.Add(criteria);
        }

        private void InitCriterias()
        {
            AddCriteriaToList(AdvancedSearchCriteriaType.To);
            AddCriteriaToList(AdvancedSearchCriteriaType.Body);
            AddCriteriaToList(AdvancedSearchCriteriaType.Folder);
            var criteria = new SortByAdvancedSearchCriteria(null, null);
            criteria.Value = AdvancedSearchSortByType.Relevance;
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
                if (c.CriteriaType == AdvancedSearchCriteriaType.None || c.Value.IsNull() || c.Value.IsStringEmptyOrNull())
                    return;
                criteria.AppendFormat(GlobalConst.AdvancedSearchFormat, _prefixes[c.CriteriaType], c.Value.ToString());
            });

            SearchString = criteria.ToString();
            OnPropertyChanged(() => SearchString);
        }

        public ICommand ScrollChangeCommand { get; private set; }

        protected override void Search()
        {
            SearchSystem.SetAdvancedSearchCriterias(CriteriaSource.Select(c => c).ToList());
            base.Search();
        }

        protected override void AdvancedSearchButtonPress(object arg)
        {
            if(Parent.IsNull())
                return;
            Parent.SelectKind(KindsConstName.Everything);
        }
    }
}