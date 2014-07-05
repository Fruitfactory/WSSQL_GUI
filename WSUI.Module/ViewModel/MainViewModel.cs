using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Regions;
using Microsoft.Practices.Unity;
using WSPreview.PreviewHandler.Service.OutlookPreview;
using WSUI.Core.Core.LimeLM;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Core.Utils.Dialog;
using WSUI.Core.Win32;
using WSUI.Infrastructure;
using WSUI.Infrastructure.Controls.ProgressManager;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Services;
using WSUI.Module.Core;
using WSUI.Module.Enums;
using WSUI.Module.Interface;
using WSUI.Module.Service;
using WSUI.Module.Service.Dialogs.Message;
using Application = System.Windows.Application;

namespace WSUI.Module.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region [urls]

        private const string BuyUrl = "https://outlookfinder.com/buy/";

        #endregion [urls]

        private const string Interface = "WSUI.Module.Interface.IKindItem";

        private readonly IUnityContainer _container;
        private readonly IRegionManager _regionManager;
        private BaseSearchObject _currentData;
        private IKindItem _currentItem;
        private bool _enabled = true;
        private List<LazyKind> _listItems;
        private ILazyKind _selectedLazyKind;
        private Visibility _dataVisibility;
        private Visibility _previewVisibility;
        private bool _isBusy;

        public MainViewModel(IUnityContainer container, IRegionManager region, IKindsView kindView,
            IPreviewView previewView)
        {
            _container = container;
            _regionManager = region;
            KindsView = kindView;
            kindView.Model = this;
            PreviewView = previewView;
            previewView.Model = this;
            MainDataSource = new List<BaseSearchObject>();
            Host = ReferenceEquals(Application.Current.MainWindow, null) ? HostType.Plugin : HostType.Application;
            DataVisibility = Visibility.Visible;
            PreviewVisibility = Visibility.Collapsed;
        }

        public IKindsView KindsView { get; protected set; }

        public IPreviewView PreviewView { get; protected set; }

        public ObservableCollection<LazyKind> KindsCollection { get; protected set; }

        public ILazyKind SelectedLazyKind
        {
            get { return _selectedLazyKind; }
            set
            {
                _selectedLazyKind = value;
                OnChoose(value.Kind);
            }
        }

        public ObservableCollection<IWSCommand> Commands
        {
            get { return _currentItem != null ? _currentItem.Commands : null; }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                _enabled = value;
                OnPropertyChanged(() => Enabled);
            }
        }

        #region private

        private void InitializeInThread()
        {
            _listItems = new List<LazyKind>();
            KindsCollection = new ObservableCollection<LazyKind>();
            GetAllKinds();
            if (_listItems.Count > 0)
            {
                _listItems.ForEach(k => KindsCollection.Add(k));
                SelectedLazyKind = _listItems[0];
            }
            OnPropertyChanged(() => KindsCollection);
            UpdatedActivatedStatus();
        }

        private void UpdatedActivatedStatus()
        {
            ActivateStatus = TurboLimeActivate.Instance.State;

            WSSqlLogger.Instance.LogInfo("Activated Status: {0}", ActivateStatus.ToString());
            OnPropertyChanged(() => ActivateStatus);
            OnPropertyChanged(() => VisibleTrialLabel);
            OnPropertyChanged(() => DaysLeft);
        }

        private void CheckStateAndShowActivatedForm()
        {
            TurboLimeActivate.Instance.TryCheckAgain();
            ActivateStatus = TurboLimeActivate.Instance.State;
            switch (ActivateStatus)
            {
#if !TRIAL
                //case ActivationState.Trial:
#endif
                case ActivationState.TrialEnded:
                case ActivationState.NonActivated:
                    RunInternalActivate();
                    break;
                case ActivationState.Trial:
                case ActivationState.Activated:
                    
                    break;
            }
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => TurboLimeActivate.Instance.IncreaseTimeUsedFlag()));
        }

        private void RunInternalActivate()
        {
            TurboLimeActivate.Instance.Activate(UpdatedActivatedStatus);
        }

        private void GetAllKinds()
        {
            IEnumerable<Type> types = this.GetType().Assembly.GetTypes().Where(t => !t.IsAbstract && t.IsClass && t.GetInterface(Interface, true) != null);
            foreach (Type type in types)
            {
                var kind = new LazyKind(_container, type, this, null, OnPropertyChanged);
                kind.Initialize();
                _listItems.Add(kind);
            }
            _listItems.Sort((x, y) =>
            {
                if (x.ID < y.ID)
                    return -1;
                if (x.ID > y.ID)
                    return 1;
                return 0;
            });
        }

        private void CurrentKindChanged(object kindItem)
        {
            IRegion regionSidebarSearch = _regionManager.Regions[RegionNames.SidebarSearchRegion];
            if (regionSidebarSearch != null)
            {
                object viewSettings = GetView(kindItem, "SettingsView");
                if (viewSettings != null && !regionSidebarSearch.Views.Contains(viewSettings))
                {
                    regionSidebarSearch.Add(viewSettings);
                    regionSidebarSearch.Activate(viewSettings);
                }
                else if (viewSettings != null)
                {
                    regionSidebarSearch.Activate(viewSettings);
                }
            }
            IRegion regionSidebarData = _regionManager.Regions[RegionNames.SidebarDataRegion];
            if (regionSidebarData != null)
            {
                object viewData = GetView(kindItem, "DataView");
                if (viewData != null   && !regionSidebarData.Views.Contains(viewData))
                {
                    regionSidebarData.Add(viewData);
                    regionSidebarData.Activate(viewData);
                }
                else if (viewData != null)
                {
                    regionSidebarData.Activate(viewData);
                }
            }

            if (PreviewView != null)
                PreviewView.ClearPreview();
            OnPropertyChanged(() => Commands);
        }

        private object GetView(object viewModel, string propertyName)
        {
            PropertyInfo prop = viewModel.GetType().GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.NonPublic |
                BindingFlags.Instance);
            if (prop != null)
            {
                return prop.GetValue(viewModel, null);
            }
            return null;
        }

        private void Disconnect()
        {
            if (_currentItem == null)
                return;

            _currentItem.Start -= OnStart;
            _currentItem.Complete -= OnComplete;
            _currentItem.Error -= OnError;
            _currentItem.CurrentItemChanged -= OnCurrentItemChanged;
        }

        private void Connect()
        {
            if (_currentItem == null)
                return;

            _currentItem.Start += OnStart;
            _currentItem.Complete += OnComplete;
            _currentItem.Error += OnError;
            _currentItem.CurrentItemChanged += OnCurrentItemChanged;
        }

        private void OnCurrentItemChanged(object sender, EventArgs<BaseSearchObject> args)
        {
            if (args.Value == null || args.Value.TypeItem == TypeSearchItem.Contact)
                return;

            _currentData = args.Value;
            
            try
            {
                Tuple<Point, Size> mwi = GetMainWindowInfo();
                Enabled = false;
                Application.Current.Dispatcher.BeginInvoke(new Action(ShowPreviewForCurrentItem), null);

                IsBusy = true;
                DataVisibility = Visibility.Collapsed;
                PreviewVisibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "OnCurrentImageChanged", ex.Message));
            }
        }

        private void ShowPreviewForCurrentItem()
        {
            try
            {
                string filename = SearchItemHelper.GetFileName(_currentData);
                if (PreviewView != null)
                {
                    if (!string.IsNullOrEmpty(filename))
                    {
                        PreviewView.SetSearchPattern(_currentItem != null
                            ? _currentItem.SearchString
                            : string.Empty);
                        PreviewView.SetPreviewFile(filename);
                        OnSlide(UiSlideDirection.DataToPreview);
                    }
                    else
                        PreviewView.ClearPreview();
                }
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "ShowPreviewForCurrentItem", ex.Message));
            }
            finally
            {
                Enabled = true;
                IsBusy = false;
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            EventHandler temp = Start;
            if (temp != null)
                temp(this, null);
            Enabled = _currentItem.Enabled;
            IsBusy = true;
        }

        private void OnComplete(object sender, EventArgs<bool> e)
        {
            EventHandler temp = Complete;
            if (temp != null)
                temp(this, null);
            Enabled = _currentItem.Enabled;
            IsBusy = false;
        }

        private void OnError(object sender, EventArgs<bool> e)
        {
        }

        private void OnChoose(object sender)
        {
            var sendItem = sender as IKindItem;
            if (sendItem == null)
                return;
            Disconnect();
            string searchString = string.Empty;
            if (_currentItem != null)
                searchString = _currentItem.SearchString;
            _currentItem = sendItem;
            Connect();
            CurrentKindChanged(_currentItem);
            if (!string.IsNullOrEmpty(searchString) && searchString != _currentItem.SearchString)
            {
                _currentItem.SearchString = searchString;
                _currentItem.FilterData();
            }
        }

        private Tuple<Point, Size> GetMainWindowInfo()
        {
            var pt = new Point();
            var sz = new Size();
            switch (Host)
            {
                case HostType.Application:
                    pt = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
                    sz = new Size(Application.Current.MainWindow.ActualWidth,
                        Application.Current.MainWindow.ActualHeight);

                    break;

                case HostType.Plugin:
                    FormCollection formsCollection = System.Windows.Forms.Application.OpenForms;
                    if (formsCollection.Count > 0)
                    {
                        pt = new Point(formsCollection[0].DesktopLocation.X, formsCollection[0].DesktopLocation.Y);
                        sz = new Size(formsCollection[0].DesktopBounds.Width, formsCollection[0].DesktopBounds.Height);
                    }
                    else
                    {
                        return GetForegroundWindowInfo();
                    }
                    break;

                default:
                    return GetForegroundWindowInfo();
            }

            return new Tuple<Point, Size>(pt, sz);
        }

        private Tuple<Point, Size> GetForegroundWindowInfo()
        {
            try
            {
                IntPtr hwnd = WindowsFunction.GetForegroundWindow();
                WindowsFunction.RECT rect;
                WindowsFunction.GetWindowRect(hwnd, out rect);
                var pt = new Point(rect.Left, rect.Top);
                var sz = new Size(rect.Right - rect.Left, rect.Bottom - rect.Top);
                return new Tuple<Point, Size>(pt, sz);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return null;
        }

        private void InternalBuy()
        {
            try
            {
                Process.Start(BuyUrl);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Buy: {0}", ex.Message);
            }
        }

        private void InternalTryAgain()
        {
            TurboLimeActivate.Instance.TryCheckAgain();
            UpdatedActivatedStatus();
        }

        private void InternalDeactivate()
        {
            if (TurboLimeActivate.Instance.Deactivate(true))
            {
                UpdatedActivatedStatus();
            }
            else
            {
                MessageBoxService.Instance.Show("Warning", "Something wrong during Deactivate", MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
        }

        private void ClearTextCriteriaForAllKinds()
        {
            _listItems.ForEach(k =>
            {
                if (k.Kind != null)
                    k.Kind.SearchString = string.Empty;
            });
        }

        #endregion private

        #region Implementation of IMainViewModel

        public string DaysLeft
        {
            get
            {
#if !TRIAL
                return TurboLimeActivate.Instance.DaysRemain.ToString(); //return string.Empty;
#else
      return TurboLimeActivate.Instance.DaysRemain.ToString();
#endif
            }
        }

        public event EventHandler Start;

        public event EventHandler Complete;
        public event EventHandler<SlideDirectionEventArgs> Slide;

        public List<BaseSearchObject> MainDataSource { get; protected set; }

        public void Clear()
        {
            TempFileManager.Instance.ClearTempFolder();
        }

        public void SelectKind(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            LazyKind lazyKind = _listItems.Find(lk => lk.UIName == name);
            if (lazyKind != null)
            {
                SelectedLazyKind = lazyKind;
                OnPropertyChanged(() => SelectedLazyKind);
            }
        }

        public void PassAction(IWSAction action)
        {
            switch (action.Action)
            {
                case WSActionType.Copy:
                case WSActionType.Cut:
                case WSActionType.Paste:
                case WSActionType.ShowContextMenu:
                    PreviewView.PassActionForPreview(action);
                    break;

                case WSActionType.Search:
                    var searchCriteria = action.Data as string;
                    if (string.IsNullOrEmpty(searchCriteria))
                        break;
                    _currentItem.SearchString = searchCriteria;
                    _currentItem.SearchCommand.Execute(null);
                    break;

                case WSActionType.Show:
                    CheckStateAndShowActivatedForm();
                    break;

                case WSActionType.Hide:
                    break;

                case WSActionType.Quit:
                    Clear();
                    break;

                case WSActionType.ClearText:
                    ClearTextCriteriaForAllKinds();
                    break;
            }
        }

        public void ForceClosePreview()
        {
            if (PreviewView != null)
            {
                PreviewView.ClearPreview();
            }
        }

        public ActivationState ActivateStatus { get; private set; }

        public ICommand BuyCommand { get; private set; }
        public ICommand ActivateCommand { get; private set; }
        public ICommand BackCommand { get; private set; }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            private set
            {
                _isBusy = value;
                OnPropertyChanged(() =>  IsBusy);
            }
        }

        public Visibility VisibleTrialLabel
        {
            get
            {
#if !TRIAL
                return ActivateStatus == ActivationState.Activated ? Visibility.Collapsed : Visibility.Visible; //return Visibility.Collapsed;
#else
      return ActivateStatus == ActivationState.Activated ? Visibility.Collapsed : Visibility.Visible;
#endif
            }
        }

        public Visibility DataVisibility 
        {
            get { return _dataVisibility; }
            private set
            {
                _dataVisibility = value;
                OnPropertyChanged(() => DataVisibility);
            }
        }

        public Visibility PreviewVisibility
        {
            get { return _previewVisibility; }
            private set
            {
                _previewVisibility = value;
                OnPropertyChanged(() => PreviewVisibility);
            }
        }

        #endregion Implementation of IMainViewModel

        public virtual void Init()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(InitializeInThread), null);
            OutlookPreviewHelper.Instance.PreviewHostType = HostType.Application;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            BuyCommand = new DelegateCommand(InternalBuy);
            ActivateCommand= new DelegateCommand(RunInternalActivate);
            BackCommand = new WSUIRelayCommand(InternalBack,CanInternalBack);
        }

        private void InternalBack(object arg)
        {
            switch (PreviewVisibility)
            {
                case Visibility.Visible:
                    DataVisibility = Visibility.Visible;
                    PreviewVisibility = Visibility.Collapsed;
                    OnSlide(UiSlideDirection.PreviewToData);
                    break;
            }
        }

        private bool CanInternalBack(object arg)
        {
            return PreviewVisibility == Visibility.Visible;
        }

        private void OnSlide(UiSlideDirection dir)
        {
            var temp = Slide;
            if (temp != null)
            {
                temp(this,new SlideDirectionEventArgs(dir));
            }
        }

    }
}