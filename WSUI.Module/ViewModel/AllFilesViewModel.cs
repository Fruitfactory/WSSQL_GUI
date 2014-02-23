using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Unity;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
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
    [KindNameId(KindsConstName.Everything, 0, @"pack://application:,,,/WSUI.Module;Component/Images/View.png")]
    public class AllFilesViewModel : KindViewModelBase, IUView<AllFilesViewModel>, IScrollableView
    {
        private const int FirstPriority = 1;
        private const int CountForSkip = 5;

        public ICommand ScrollChangeCommand { get; protected set; }

        public AllFilesViewModel(IUnityContainer container, ISettingsView<AllFilesViewModel> settingsView,
                                 IDataView<AllFilesViewModel> dataView)
            : base(container)
        {
            SettingsView = settingsView;
            SettingsView.Model = this;
            DataView = dataView;
            DataView.Model = this;
            // init
            ID = 0;
            _name = "Everything";
            UIName = _name;
            _prefix = "AllFiles";
            IsOpen = false;
            FlyCommand = new DelegateCommand<object>(o =>
                                                          {
                                                              IsOpen = !IsOpen;
                                                              OnPropertyChanged(() => IsOpen);
                                                          },
                                                          o => true);
            ScrollChangeCommand = new DelegateCommand<object>(OnScroll, o => true);
            EmailClickCommand = new DelegateCommand<object>(o => EmailClick(o), o => true);
            SearchSystem = new AllSearchSystem();
        }

        public bool IsOpen { get; set; }

        public ICommand FlyCommand { get; private set; }

        public ICommand EmailClickCommand { get; protected set; }

        protected override void OnSearchStringChanged()
        {
            ClearMainDataSource();
            _isFirstTime = true;
            base.OnSearchStringChanged();
        }

        private void OnScroll(object args)
        {
            var scrollArgs = args as ScrollData;
            if (scrollArgs != null && ScrollBehavior != null)
            {
                ScrollBehavior.NeedSearch(scrollArgs);
            }
        }

        protected override void ProcessMainResult()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var result = SearchSystem.GetResult();
                if (result.All(i => i.Result.Count == 0) && ShowMessageNoMatches)
                {
                    DataSource.Clear();
                    var message = new FileSearchObject()
                    {
                        ItemName = string.Format("Search for '{0}' returned no matches. Try different keywords.", SearchString),
                        TypeItem = TypeSearchItem.None
                    };
                    DataSource.Add(message);
                }
                else
                {
                    var watch = new Stopwatch();
                    watch.Start();
                    if (_isFirstTime)
                    {
                        ContactForFirstTime(result.Where(i => i.Priority <= FirstPriority).OrderBy(i => i.Priority));
                    }
                    foreach (var col in result.Where(i => i.Priority > FirstPriority).OrderBy(i => i.Priority))
                    {
                        foreach (var it in col.Result)
                        {
                            DataSource.Add(it as BaseSearchObject);
                        }
                    }
                    watch.Stop();
                    WSSqlLogger.Instance.LogInfo("ProcessMainResult (AllViewModel): {0}", watch.ElapsedMilliseconds);
                }
                _isFirstTime = false;
            }), null);
        }

        private void ContactForFirstTime(IEnumerable<ISystemSearchResult> result)
        {
            if (result.Sum(it => it.Result.Count) > CountForSkip)
            {
                foreach (var systemSearchResult in result.SelectMany(i => i.Result).Take(5))
                {
                    DataSource.Add(systemSearchResult as BaseSearchObject);
                }
                var commandSearchData = new CommandSearchObject()
                {
                    ItemName = "More",
                    TypeItem = TypeSearchItem.Command,
                };
                DataSource.Add(commandSearchData);
            }
            else
            {
                foreach (var item in result.SelectMany(i => i.Result))
                {
                    DataSource.Add(item as BaseSearchObject);
                }
            }
        }

        private void EmailClick(object obj)
        {
            if (CommandElementClick(obj))
                return;
            if (EmailElementClick(obj))
                return;
        }

        private bool EmailElementClick(object obj)
        {
            string adr = string.Empty;
            if (obj is string)
                adr = (string)obj;
            else if (obj is EmailContactSearchObject)
            {
                adr = (obj as EmailContactSearchObject).EMail;
            }
            else if (obj is ContactSearchObject)
            {
                var contact = (ContactSearchObject)obj;
                adr = !string.IsNullOrEmpty(contact.EmailAddress)
                    ? contact.EmailAddress
                    : !string.IsNullOrEmpty(contact.EmailAddress2)
                        ? contact.EmailAddress2
                        : !string.IsNullOrEmpty(contact.EmailAddress3)
                            ? contact.EmailAddress3
                            : string.Empty;
            }
            if (string.IsNullOrEmpty(adr))
                return false;
            var email = OutlookHelper.Instance.CreateNewEmail();
            email.To = adr;
            email.BodyFormat = Microsoft.Office.Interop.Outlook.OlBodyFormat.olFormatHTML;
            email.Display(false);

            return true;
        }

        private bool CommandElementClick(object obj)
        {
            if (!(obj is CommandSearchObject))
                return false;
            Parent.SelectKind(KindsConstName.People);
            return true;
        }

        protected override void OnInit()
        {
            base.OnInit();
            SearchSystem.Init();
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            var fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.File, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Picture, fileAttach);
            CommandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
            ScrollBehavior = new ScrollBehavior() { CountFirstProcess = 300, CountSecondProcess = 100, LimitReaction = 99 };
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        #region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        #endregion IUIView

        private volatile bool _isFirstTime = true;

    }
}