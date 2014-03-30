using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Models;
using WSUI.Infrastructure.Service;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Service;
using WSUI.Module.Strategy;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.People, 1, @"pack://application:,,,/WSUI.Module;Component/Images/People.png")]
    public class ContactViewModel : KindViewModelBase, IUView<ContactViewModel>, IScrollableView
    {
        private ContactSearchData _contactData = null;
        private ContactSuggestingService _contactSuggesting;

        public ContactViewModel(IUnityContainer container, ISettingsView<ContactViewModel> settingsView, IDataView<ContactViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;

            ID = 1;
            _name = "People";
            UIName = _name;
            _prefix = "Contact";

            Folder = OutlookHelper.AllFolders;

            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
            _contactSuggesting = new ContactSuggestingService();
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
            SearchSystem = new ContactSearchSystem();
        }

        public ICommand EmailClickCommand { get; protected set; }

        public ContactSearchData Contact
        {
            get { return _contactData; }
        }

        public ObservableCollection<string> DataSourceSuggest { get; set; }

        public Visibility Visible
        {
            get { return _contactData != null ? Visibility.Visible : Visibility.Collapsed; }
        }

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

        private void EmailClick(object address)
        {
            string adr = string.Empty;
            if (address is string)
                adr = (string)address;
            else if (address is EmailContactSearchObject)
            {
                adr = (address as EmailContactSearchObject).EMail;
            }
            else if (address is ContactSearchObject)
            {
                var contact = (ContactSearchObject)address;
                adr = !string.IsNullOrEmpty(contact.EmailAddress)
                    ? contact.EmailAddress
                    : !string.IsNullOrEmpty(contact.EmailAddress2)
                        ? contact.EmailAddress2
                        : !string.IsNullOrEmpty(contact.EmailAddress3)
                            ? contact.EmailAddress3
                            : string.Empty;
            }

            if (string.IsNullOrEmpty(adr))
                return;
            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = adr;
            email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);
        }

        protected override void OnInit()
        {
            base.OnInit();
            SearchSystem.Init();

            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ScrollBehavior = new ScrollBehavior() { CountFirstProcess = 400, CountSecondProcess = 100, LimitReaction = 99 };
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
        }

        #region IUIView

        public ISettingsView<ContactViewModel> SettingsView
        {
            get;
            set;
        }

        public IDataView<ContactViewModel> DataView
        {
            get;
            set;
        }

        #endregion IUIView

        #region Implementation of IScrollableView

        public ICommand ScrollChangeCommand { get; private set; }

        #endregion Implementation of IScrollableView

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