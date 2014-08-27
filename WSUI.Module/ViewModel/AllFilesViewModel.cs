using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Input;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Practices.Prism;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Implements.Systems;
using WSUI.Infrastructure.Service;
using WSUI.Module.Core;
using WSUI.Module.Interface;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Service;
using WSUI.Module.Strategy;
using Action = System.Action;
using Application = System.Windows.Application;

namespace WSUI.Module.ViewModel
{
    [KindNameId(KindsConstName.Everything, 0, @"pack://application:,,,/WSUI.Module;Component/Images/View.png", @"M33.597977,10.759002C37.946865,10.759002 41.485962,14.285001 41.485962,18.649 41.485962,23 37.946865,26.535 33.597977,26.535 29.23909,26.535 25.709992,23 25.709992,18.649 25.709992,17.784 25.849955,16.953001 26.109888,16.174002 26.779719,16.881001 27.70948,17.327002 28.759213,17.327002 30.778696,17.327002 32.418278,15.691001 32.418278,13.668001 32.418278,12.663001 32.008381,11.748001 31.348551,11.087002 32.058369,10.876001 32.818176,10.759002 33.597977,10.759002z M33.606682,4.3679962C25.92741,4.3679957 19.698065,10.594956 19.698065,18.27293 19.698065,25.953894 25.92741,32.177862 33.606682,32.177862 41.295838,32.177862 47.515175,25.953894 47.515175,18.27293 47.515175,10.594956 41.295838,4.3679957 33.606682,4.3679962z M34.867642,1.546141E-09C36.890393,2.6508449E-05 58.705193,0.41938579 68.893006,18.299923 68.893006,18.299923 57.1442,36.139837 34.44656,34.768854 34.44656,34.768854 14.428583,36.59984 0,18.299923 0,18.299923 9.0791523,0.4590019 34.716553,0.0010111886 34.716553,0.0010114873 34.768162,-1.4442128E-06 34.867642,1.546141E-09z")]
    public class AllFilesViewModel : KindViewModelBase, IUView<AllFilesViewModel>, IScrollableView
    {
        private const int FirstPriority = 1;
        private const int CountForSkip = 5;
        private volatile bool _isFirstTime = true;

        public AllFilesViewModel(IUnityContainer container, IEventAggregator eventAggregator, ISettingsView<AllFilesViewModel> settingsView,
            IDataView<AllFilesViewModel> dataView)
            : base(container,eventAggregator)
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

        #region IScrollableView Members

        public ICommand ScrollChangeCommand { get; protected set; }

        #endregion IScrollableView Members

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
                IList<ISystemSearchResult> result = SearchSystem.GetResult();
                if (result.All(i => i.Result.Count == 0) && ShowMessageNoMatches)
                {
                    DataSource.Clear();
                    var message = new FileSearchObject
                    {
                        ItemName =
                            string.Format("Search for '{0}' returned no matches. Try different keywords.", SearchString),
                        TypeItem = TypeSearchItem.None
                    };
                    DataSource.Add(message);
                }
                else
                {
                    //var watch = new Stopwatch();
                    //watch.Start();
                    if (_isFirstTime)
                    {
                        ContactForFirstTime(result.Where(i => i.Priority <= FirstPriority).OrderBy(i => i.Priority));
                    }
                    foreach (
                        ISystemSearchResult col in
                            result.Where(i => i.Priority > FirstPriority).OrderBy(i => i.Priority))
                    {
                        CollectionExtensions.AddRange(DataSource, col.Result);
                    }
                    //watch.Stop();
                    //WSSqlLogger.Instance.LogInfo("ProcessMainResult (AllViewModel): {0}", watch.ElapsedMilliseconds);
                }
                ShowMessageNoMatches = true;
                _isFirstTime = false;
            }), null);
        }

        private void ContactForFirstTime(IEnumerable<ISystemSearchResult> result)
        {
            if (result.Sum(it => it.Result.Count) > CountForSkip)
            {
                foreach (ISearchObject systemSearchResult in result.SelectMany(i => i.Result).Take(5))
                {
                    DataSource.Add(systemSearchResult as BaseSearchObject);
                }
                var commandSearchData = new CommandSearchObject
                {
                    ItemName = "More",
                    TypeItem = TypeSearchItem.Command,
                };
                DataSource.Add(commandSearchData);
            }
            else
            {
                foreach (ISearchObject item in result.SelectMany(i => i.Result))
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
                adr = (string) obj;
            else if (obj is EmailContactSearchObject)
            {
                adr = (obj as EmailContactSearchObject).EMail;
            }
            else if (obj is ContactSearchObject)
            {
                var contact = (ContactSearchObject) obj;
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
            MailItem email = OutlookHelper.Instance.CreateNewEmail();
            email.To = adr;
            email.BodyFormat = OlBodyFormat.olFormatHTML;
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
            CommandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ICommandStrategy fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            CommandStrategies.Add(TypeSearchItem.File, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            CommandStrategies.Add(TypeSearchItem.Picture, fileAttach);
            CommandStrategies.Add(TypeSearchItem.FileAll, fileAttach);
            ScrollBehavior = new ScrollBehavior {CountFirstProcess = 300, CountSecondProcess = 100, LimitReaction = 99};
            ScrollBehavior.SearchGo += OnScrollNeedSearch;
            TopQueryResult = ScrollBehavior.CountFirstProcess;
        }

        #region IUIView

        public ISettingsView<AllFilesViewModel> SettingsView { get; set; }

        public IDataView<AllFilesViewModel> DataView { get; set; }

        #endregion IUIView
    }
}