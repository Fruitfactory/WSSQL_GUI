using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using OF.Core.Core.MVVM;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Infrastructure.Events;
using OF.Infrastructure.Payloads;
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Services;
using OF.Module.Interface.Service;
using OF.Module.Interface.ViewModel;
using OF.Module.Service.Dialogs.Message;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace OF.Module.Core
{
    public abstract class OFKindViewModelBase : OFViewModelBase, IKindItem
    {
        protected string _name = string.Empty;

        protected string _prefix = string.Empty;
        protected bool _toggle = false;
        protected readonly List<OFBaseSearchObject> ListData = new List<OFBaseSearchObject>();
        protected IMainViewModel ParentViewModel;

        protected volatile bool ShowMessageNoMatches = true;
        protected readonly IUnityContainer Container;
        protected readonly IEventAggregator EventAggregator;

        protected readonly object Lock = new object();

        protected IScrollBehavior ScrollBehavior = null;
        protected int TopQueryResult = 100;

        private volatile bool _isQueryRun = false;

        private OFBaseSearchObject _current = null;
        private string _searchString = string.Empty;
        private bool _canSearch = true;

        // new
        protected ISearchSystem SearchSystem { get; set; }

        protected OFKindViewModelBase(IUnityContainer container, IEventAggregator eventAggregator)
        {
            Container = container;
            EventAggregator = eventAggregator;
            ChooseCommand = new DelegateCommand<object>(o => OnChoose(), o => true);
            SearchCommand = new DelegateCommand<object>(o => Search(), o => CanSearch());
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(KeyDown, o => true);
            DoubleClickCommand = new DelegateCommand<MouseButtonEventArgs>(DoubleClick, o => true);
            ClearCriteriaCommand = new DelegateCommand<object>(ClearCriteriaClicked, o => true);
            ShowAdvancedSearchCommand = new DelegateCommand<object>(ShowAdvancedSearchCommandExecute, o => true);
            Enabled = true;
            DataSource = new ObservableCollection<ISearchObject>();
            Host = ReferenceEquals(Application.Current.MainWindow, null) ? OFHostType.Plugin : OFHostType.Application;
        }

        protected virtual DateTime GetCurrentDate()
        {
            return DateTime.Now.AddDays(1);
        }

        protected virtual void OnStart()
        {
            Enabled = false;
            ListData.Clear();
            OnPropertyChanged(() => Enabled);
            FireStart();
        }

        protected virtual void OnComplete(bool res)
        {
            Enabled = true;
            ProcessMainResult();
            OnPropertyChanged(() => DataSource);
            OnPropertyChanged(() => Enabled);
            FireComplete(res);
        }

        protected virtual void ProcessMainResult()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                var result = SearchSystem.GetResult();

                ProcessError(result);

                if (result.All(i => i.Result.OperationResult.Count == 0) && ShowMessageNoMatches)
                {
                    DataSource.Clear();
                    var message = new OFFileSearchObject()
                    {
                        ItemName = string.Format("Search for '{0}' returned no matches. Try different keywords.", SearchString),
                        TypeItem = OFTypeSearchItem.None
                    };
                    DataSource.Add(message);
                }
                else
                {
                    result.OrderBy(i => i.Priority).ForEach(it =>
                    {
                        foreach (var systemSearchResult in it.Result.OperationResult)
                        {
                            DataSource.Add(systemSearchResult as OFBaseSearchObject);
                        }
                    });
                }
                ShowMessageNoMatches = false;
            }), null);
        }

        protected void ProcessError(IList<ISystemSearchResult> result)
        {
            if (result.All(r => r.Result.Type != OFTypeResult.Error))
            {
                return;
            }

            var index = 1;
            var message = new StringBuilder();

            foreach (var systemSearchResult in result.Where(r => r.Result.Type == OFTypeResult.Error))
            {
                message.AppendFormat("{0}. {1}\n", index,
                    string.Join(";\n", systemSearchResult.Result.Messages.Select(m => m.Message)));
                index++;
            }
            if (message.Length == 0)
            {
                return;
            }
            OFMessageBoxService.Instance.Show("Messages", message.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
        }

        protected virtual void OnError(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Error;
            if (temp != null)
                temp(this, new EventArgs<bool>(res));
        }

        protected virtual void OnCurrentItemChanged(OFBaseSearchObject data)
        {
            if (EventAggregator == null || data == null)
                return;
            EventAggregator.GetEvent<OFSelectedChangedPayloadEvent>().Publish(new OFSearchObjectPayload(Current));
        }

        protected virtual void OnChoose()
        {
            EventHandler temp = Choose;
            if (temp != null)
                temp(this, null);
        }

        protected virtual void Search()
        {
            _canSearch = false;
            if (!IsShouldSearch())
            {
                _canSearch = true;
                OFLogger.Instance.LogWarning("Please, activate the 'OutlookFinder'");
                OFMessageBoxService.Instance.Show("Warning", "Please, activate the 'OutlookFinder'", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                RunBuyProcess();
                return;
            }

            if (_isQueryRun)
            {
                OFLogger.Instance.LogWarning("Query have already started");
                return;
            }
            if (string.IsNullOrEmpty(SearchString))
            {
                OFMessageBoxService.Instance.Show("Warning", "Search criteria is empty");
                OFLogger.Instance.LogWarning("Search criteria is empty");
                return;
            }
            SearchSystem.SetSearchCriteria(SearchString);
            SearchSystem.Search();
            OnStart();
        }

        private void RunBuyProcess()
        {
            if (ParentViewModel != null && ParentViewModel.BuyCommand != null)
            {
                ParentViewModel.BuyCommand.Execute(null);
            }
        }

        protected virtual bool CanSearch()
        {
            return _canSearch;
        }

        protected virtual void OnSearchStringChanged()
        {
            ResetSearchSystem();
            ClearDataSource();
            if (SearchString.IsStringEmptyOrNull())
            {
                OnSearchStringClear();
            }
            if (Parent != null)
                Parent.ForceClosePreview();
            _canSearch = true;
            ShowMessageNoMatches = true;
        }

        private void ResetSearchSystem()
        {
            if (SearchSystem.IsNotNull())
            {
                SearchSystem.Reset();
            }
        }

        protected virtual void OnInit()
        {
            if (SearchSystem != null)
            {
                SearchSystem.Init(Container);
                SearchSystem.SearchStarted += SearchSystemOnSearchStarted;
                SearchSystem.SearchFinished += SearchSystemOnSearchFinished;
                SearchSystem.SearchStoped += SearchSystemOnSearchStoped;
            }
        }

        private void SearchSystemOnSearchStoped(object o)
        {
            OnError(false);
        }

        private void SearchSystemOnSearchFinished(object o)
        {
            OnComplete(true);
        }

        private void SearchSystemOnSearchStarted(object o)
        {
        }

        private void OnSearchStringClear()
        {
            var temp = SearchStringCleared;
            if (temp != null)
            {
                temp(this,EventArgs.Empty);
            }
        }

        protected virtual void OnFilterData()
        {
            if (string.IsNullOrEmpty(SearchString))
                return;
            ResetSearchSystem();
            ClearDataSource();
            Search();
        }

        protected virtual void ClearDataSource()
        {
            DataSource.Clear();
            OnPropertyChanged(() => DataSource);
        }

        protected void FireStart()
        {
            EventHandler temp = Start;
            if (temp != null)
                temp(this, new EventArgs());
        }

        protected void FireComplete(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Complete;
            if (temp != null)
                temp(this, new EventArgs<bool>(res));
        }

        protected void ClearMainDataSource()
        {
            if (ParentViewModel.MainDataSource == null
              || ParentViewModel.MainDataSource.Count == 0)
                return;
            ParentViewModel.MainDataSource.Clear();
        }

        protected bool IsShouldSearch()
        {
            switch (Parent.ActivateStatus)
            {
                case OFActivationState.Activated:
                case OFActivationState.Trial:
                    return true;

                default:
                    return false;
            }
        }

        protected bool IsSearchCriteriaEmpty
        {
            get { return string.IsNullOrEmpty(SearchString); }
        }

        #region IKindItem

        public string Name
        {
            get { return _name; }
        }

        public string SearchString
        {
            get { return _searchString; }
            set
            {
                _searchString = value;
                OnSearchStringChanged();
                OnPropertyChanged(() => SearchString);
            }
        }

        public IMainViewModel Parent
        {
            get { return ParentViewModel; }
            set { ParentViewModel = value; }
        }

        public string Prefix { get { return _prefix; } }

        public int ID { get; protected set; }

        public string UIName { get; protected set; }

        public bool Toggle { get { return _toggle; } set { _toggle = value; OnPropertyChanged(() => Toggle); } }

        public OFBaseSearchObject CurrentTrackedObject { get; set; }

        public ObservableCollection<ISearchObject> DataSource { get; protected set; }

        public ICommand ChooseCommand { get; protected set; }

        public ICommand SearchCommand { get; protected set; }

        public ICommand KeyDownCommand { get; protected set; }

        public ICommand ClearCriteriaCommand { get; private set; }

        public ICommand DoubleClickCommand { get; protected set; }

        public ICommand ShowAdvancedSearchCommand { get; protected set; }

        public event EventHandler Start;

        public event EventHandler<EventArgs<bool>> Complete;

        public event EventHandler<EventArgs<bool>> Error;
#pragma warning disable 0067
        public event EventHandler<EventArgs<OFBaseSearchObject>> CurrentItemChanged;
#pragma warning restore 0067

        public event EventHandler Choose;


        public event EventHandler SearchStringCleared;


        public OFBaseSearchObject Current
        {
            get { return _current; }
            set
            {
                _current = value;
                OnCurrentItemChanged(_current);
            }
        }

        public void Init()
        {
            OnInit();
        }

        public void FilterData()
        {
            OnFilterData();
        }

        #endregion IKindItem

        public List<string> FolderList
        {
            get
            {
                var list = OFOutlookHelper.Instance.GetFolderNameList();
                if (list.Count > 0)
                {
                    OnPropertyChanged(() => FolderList);
                }
                return list;
            }
            set { }
        }

        public string Folder
        {
            get;
            set;
        }

        public bool Enabled
        {
            get;
            set;
        }


        public string HighlightPattern
        {
            get { return GetSearchPattern(); }
        }

        private void ShowPath()
        {
            var filename = OFSearchItemHelper.GetFileName(Current);
            OFMessageBoxService.Instance.Show("Filename", filename, MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OpenItemFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return;
            try
            {
                Process.Start(fileName);
            }
            catch (System.Exception ex)
            {
                OFLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "OpenItemFile", fileName, ex.Message));
            }
        }

        private void OpenFolder()
        {
            var filename = OFSearchItemHelper.GetFileName(Current);
            filename = Path.GetDirectoryName(filename);
            OpenItemFile(filename);
        }

        private void OpenFile()
        {
            var fileName = OFSearchItemHelper.GetFileName(Current);
            if (FileService.IsDirectory(fileName))
                return;

            OpenItemFile(fileName);
        }

        protected virtual void KeyDown(object args)
        {
            if (args == null || !(args is KeyEventArgs))
                return;
            var keys = args as KeyEventArgs;
            switch (keys.Key)
            {
                case Key.Enter:
                    if (SearchCommand.CanExecute(null))
                        SearchCommand.Execute(null);
                    break;
            }
        }

        private void DoubleClick(MouseButtonEventArgs obj)
        {
            if (Current == null)
                return;
            OpenFile();
        }

        private void ClearCriteriaClicked(object arg)
        {
            if (Parent == null)
                return;
            Parent.PassAction(new OFAction(OFActionType.ClearText, arg));
        }

        protected void OnScrollNeedSearch()
        {
            if (SearchSystem.IsSearching)
                return;
            ShowMessageNoMatches = false;
            Search();
        }

        private void ShowAdvancedSearchCommandExecute(object arg)
        {
            AdvancedSearchButtonPress(arg);
        }

        protected virtual void AdvancedSearchButtonPress(object arg)
        {
            if (Parent.IsNull())
                return;
            Parent.ShowAdvancedSearch(null);
        }

        public string GetSearchPattern()
        {
            return GetInternalSearchPattern();
        }

        public void SetSearchString(string searchCriteria)
        {
            SetInternalSearchCriteria(searchCriteria);    
        }

        protected virtual string GetInternalSearchPattern()
        {
            return SearchString;
        }

        protected virtual void SetInternalSearchCriteria(string searchCriteria)
        {
            SearchString = searchCriteria;
        }


    }
}