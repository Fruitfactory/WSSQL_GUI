using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Core.Win32;
using WSUI.Infrastructure.Controls.ProgressManager;
using WSUI.Infrastructure.Events;
using WSUI.Infrastructure.Payloads;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Service.Rules;
using WSUI.Infrastructure.Services;
using WSUI.Module.Interface;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service;
using WSUI.Module.Service.Dialogs.Message;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;

namespace WSUI.Module.Core
{
    public abstract class KindViewModelBase : ViewModelBase, IKindItem
    {

        protected string _name = string.Empty;
        
        protected string _prefix = string.Empty;
        protected bool _toggle = false;
        protected readonly List<BaseSearchObject> ListData = new List<BaseSearchObject>();
        protected IMainViewModel ParentViewModel;
        
        protected volatile bool ShowMessageNoMatches = true;
        protected readonly IUnityContainer Container;
        protected readonly IEventAggregator EventAggregator;
        
        protected readonly object Lock = new object();
        
        protected IScrollBehavior ScrollBehavior = null;
        protected int TopQueryResult = 100;
        

        private volatile bool _isQueryRun = false;
        
        private BaseSearchObject _current = null;
        private string _searchString = string.Empty;
        private bool _canSearch = true;

        // new
        protected ISearchSystem SearchSystem { get; set; }

        protected KindViewModelBase(IUnityContainer container, IEventAggregator eventAggregator)
        {
            Container = container;
            EventAggregator = eventAggregator;
            ChooseCommand = new DelegateCommand<object>(o => OnChoose(), o => true);
            SearchCommand = new DelegateCommand<object>(o => Search(), o => CanSearch());
            OpenCommand = new DelegateCommand<object>(o => OpenFile(), o => CanOpenFile());
            OpenFolderCommand = new DelegateCommand<object>(o => OpenFolder(), o => CanOpenFile());
            ShowPathCommand = new DelegateCommand(ShowPath, CanOpenFile);
            KeyDownCommand = new DelegateCommand<KeyEventArgs>(KeyDown, o => true);
            DoubleClickCommand = new DelegateCommand<MouseButtonEventArgs>(DoubleClick, o => true);
            ClearCriteriaCommand = new DelegateCommand<object>(ClearCriteriaClicked, o => true);
            Enabled = true;
            DataSource = new ObservableCollection<ISearchObject>();
            Host = ReferenceEquals(Application.Current.MainWindow, null) ? HostType.Plugin : HostType.Application;
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
            //Application.Current.Dispatcher.BeginInvoke(new Action(() => BusyPopupAdorner.Instance.IsBusy = false), null);
            //if (ProgressManager.Instance.InProgress)
            //{
            //    ProgressManager.Instance.StopOperation();
            //}
        }

        protected virtual void ProcessMainResult()
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
                    result.OrderBy(i => i.Priority).ForEach(it =>
                    {
                        foreach (var systemSearchResult in it.Result)
                        {
                            DataSource.Add(systemSearchResult as BaseSearchObject);
                        }
                    });
                }
                ShowMessageNoMatches = true;
            }), null);
        }

        protected virtual void OnError(bool res)
        {
            EventHandler<EventArgs<bool>> temp = Error;
            if (temp != null)
                temp(this, new EventArgs<bool>(res));
        }

        protected virtual void OnCurrentItemChanged(BaseSearchObject data)
        {
            if (EventAggregator == null)
                return;
            EventAggregator.GetEvent<SelectedChangedPayloadEvent>().Publish(new SearchObjectPayload(Current));
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
                WSSqlLogger.Instance.LogWarning("Please, activate the 'OutlookFinder'");
                MessageBoxService.Instance.Show("Warning", "Please, activate the 'OutlookFinder'", MessageBoxButton.OK,
                    MessageBoxImage.Asterisk);
                RunBuyProcess();
                return;
            }

            if (_isQueryRun)
            {
                WSSqlLogger.Instance.LogWarning("Query have already started");
                return;
            }
            if (string.IsNullOrEmpty(SearchString))
            {
                MessageBoxService.Instance.Show("Warning", "Search criteria is empty");
                WSSqlLogger.Instance.LogWarning("Search criteria is empty");
                return;
            }
            SearchSystem.SetSearchCriteria(SearchString);
            SearchSystem.Search();
            OnStart();

            //if (!ProgressManager.Instance.InProgress)
            //{
            //    ProgressManager.Instance.StartOperation(new ProgressOperation()
            //    {
            //        Caption = "Searching...",
            //        DelayTime = 2500,
            //        Canceled = false,
            //        Location = new Point(mwi.MainWindowRect.Left, mwi.MainWindowRect.Top),
            //        Size = new Size(mwi.MainWindowRect.Width, mwi.MainWindowRect.Height),
            //        MainHandle = mwi.MainWindowHandle
            //    });
            //}
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
            SearchSystem.Reset();
            ClearDataSource();
            if (Parent != null)
                Parent.ForceClosePreview();
            _canSearch = true;
        }

        protected virtual void OnInit()
        {
            if (SearchSystem != null)
            {
                SearchSystem.Init();
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

        protected virtual void OnFilterData()
        {
            if (string.IsNullOrEmpty(SearchString))
                return;
            SearchSystem.Reset();
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
                case ActivationState.Activated:
                case ActivationState.Trial:
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

        public ObservableCollection<ISearchObject> DataSource { get; protected set; }

        public ICommand ChooseCommand { get; protected set; }

        public ICommand SearchCommand { get; protected set; }

        public ICommand KeyDownCommand { get; protected set; }

        public ICommand ClearCriteriaCommand { get; private set; }

        public ICommand DoubleClickCommand { get; protected set; }

        public event EventHandler Start;

        public event EventHandler<EventArgs<bool>> Complete;

        public event EventHandler<EventArgs<bool>> Error;

        public event EventHandler<EventArgs<BaseSearchObject>> CurrentItemChanged;

        public event EventHandler Choose;

        public BaseSearchObject Current
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
                var list = OutlookHelper.Instance.GetFolderList();
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

        public ICommand OpenCommand { get; protected set; }

        public ICommand OpenFolderCommand { get; protected set; }

        public ICommand ShowPathCommand { get; protected set; }

        private void ShowPath()
        {
            var filename = SearchItemHelper.GetFileName(Current);
            MessageBoxService.Instance.Show("Filename", filename, MessageBoxButton.OK, MessageBoxImage.Information);
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
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "OpenItemFile", fileName, ex.Message));
            }
        }

        private bool CanOpenFile()
        {
            return Current != null;
        }

        private void OpenFolder()
        {
            var filename = SearchItemHelper.GetFileName(Current);
            filename = Path.GetDirectoryName(filename);
            OpenItemFile(filename);
        }

        private void OpenFile()
        {
            var fileName = SearchItemHelper.GetFileName(Current);
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

        private MainWindowInfo GetWindowInfo()
        {
            MainWindowInfo mwi = new MainWindowInfo();
            Rect rect = new Rect();
            switch (Host)
            {
                case HostType.Application:
                    rect.Location = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
                    rect.Size = new Size(Application.Current.MainWindow.ActualWidth, Application.Current.MainWindow.ActualHeight);
                    mwi.MainWindowRect = rect;
                    mwi.MainWindowHandle = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                    break;

                case HostType.Plugin:
                    FormCollection formsCollection = System.Windows.Forms.Application.OpenForms;
                    if (formsCollection.Count > 0)
                    {
                        mwi.MainWindowRect = new Rect(formsCollection[0].DesktopLocation.X, formsCollection[0].DesktopLocation.Y,
                            formsCollection[0].DesktopBounds.Size.Width, formsCollection[0].DesktopBounds.Size.Height);
                        mwi.MainWindowHandle = formsCollection[0].Handle;
                    }
                    else
                    {
                        ApplyMainWindowInfo(mwi);
                    }
                    break;

                default:
                    ApplyMainWindowInfo(mwi);
                    break;
            }

            return mwi;
        }

        private Tuple<IntPtr, Rect> GetForegroundWindowInfo()
        {
            try
            {
                var hwnd = WindowsFunction.GetForegroundWindow();
                WindowsFunction.RECT rect;
                WindowsFunction.GetWindowRect(hwnd, out rect);
                Point pt = new Point(rect.Left, rect.Top);
                Size sz = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                return new Tuple<IntPtr, Rect>(hwnd, new Rect(pt, sz));
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return null;
        }

        private void ApplyMainWindowInfo(MainWindowInfo info)
        {
            var result = GetForegroundWindowInfo();
            if (result == null)
                return;
            info.MainWindowHandle = result.Item1;
            info.MainWindowRect = result.Item2;
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
            Parent.PassAction(new WSAction(WSActionType.ClearText, arg));
        }

        protected void OnScrollNeedSearch()
        {
            if (SearchSystem.IsSearching)
                return;
            ShowMessageNoMatches = false;
            Search();
        }
    }
}