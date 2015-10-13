using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Infrastructure.Implements.Systems;
using OF.Infrastructure.Service;
using OF.Module.Core;
using OF.Module.Interface;
using OF.Module.Interface.View;
using OF.Module.Service;
using OF.Module.Strategy;
using Action = System.Action;
using Application = System.Windows.Application;

namespace OF.Module.ViewModel
{
    [KindNameId(OFKindsConstName.People, 1, @"pack://application:,,,/OF.Module;Component/Images/People.png", "M30.665044,29.43524C32.627258,29.510739,37.909897,33.510706,38.363001,46.038002L0.022914886,46.339998C0.022914886,46.339998 -1.0330925,30.340132 11.192898,29.585039 16.023035,28.831045 20.851871,33.660504 23.569891,33.207309 26.288011,32.755512 28.702229,29.35974 30.665044,29.43524z M38.318982,23.924138C39.961614,23.855138 41.984453,26.947552 44.259894,27.359052 46.536636,27.771854 50.580315,23.373336 54.62529,24.060839 64.864386,24.748342 63.980869,39.320003 63.980869,39.320003L40.691429,39.119501C39.285801,31.718471 35.915338,28.750959 34.093004,28.239255 35.56763,25.150744 37.388667,23.96184 38.318982,23.924138z M21.908853,4.0760002C27.411691,4.0760002 31.872003,9.144783 31.872004,15.395785 31.872003,21.649417 27.411691,26.717 21.908853,26.717 16.407515,26.717 11.947002,21.649417 11.947002,15.395785 11.947002,9.144783 16.407515,4.0760002 21.908853,4.0760002z M45.455803,0C50.541592,0 54.664001,4.5950603 54.664001,10.2643 54.664001,15.9336 50.541592,20.53 45.455803,20.53 40.371314,20.53 36.249004,15.9336 36.249004,10.2643 36.249004,4.5950603 40.371314,0 45.455803,0z")]
    public class ContactViewModel : OFKindViewModelBase, IUView<ContactViewModel>, IScrollableView
    {
        private OFContactSuggestingService _contactSuggesting;

        public ContactViewModel(IUnityContainer container, IEventAggregator eventAggregator, ISettingsView<ContactViewModel> settingsView,
            IDataView<ContactViewModel> dataView)
            : base(container,eventAggregator)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            ID = 1;
            _name = "People";
            UIName = _name;
            _prefix = "Contact";

            Folder = OFOutlookHelper.AllFolders;

            EmailClickCommand = new DelegateCommand<object>(o => { }, o => true);
            _contactSuggesting = new OFContactSuggestingService();
            _contactSuggesting.Suggest += (o, e) =>
            {
                DataSourceSuggest = new ObservableCollection<string>();
                if (e.Value != null)
                {
                    Application.Current.Dispatcher.BeginInvoke(
                        new Action(
                            () =>
                            {
                                e.Value.ForEach(s => DataSourceSuggest.Add(s));
                                OnPropertyChanged(() => DataSourceSuggest);
                            }),
                        null);
                }
            };
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            SearchSystem = new OFContactSearchSystem();
        }

        public ICommand EmailClickCommand { get; protected set; }

        public ObservableCollection<string> DataSourceSuggest { get; set; }

        protected override void OnSearchStringChanged()
        {
            TopQueryResult = ScrollBehavior.CountFirstProcess;
            base.OnSearchStringChanged();
        }

        protected override void OnFilterData()
        {
            base.OnFilterData();
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        
        protected override void OnInit()
        {
            base.OnInit();
            ScrollBehavior = new OFScrollBehavior {CountFirstProcess = 400, CountSecondProcess = 100, LimitReaction = 85};
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
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

        public ISettingsView<ContactViewModel> SettingsView { get; set; }

        public IDataView<ContactViewModel> DataView { get; set; }

        #endregion IUIView

        #region Implementation of IScrollableView

        public ICommand ScrollChangeCommand { get; private set; }

        #endregion Implementation of IScrollableView
    }
}