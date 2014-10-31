using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using Transitionals;
using WSPreview.PreviewHandler.Service.OutlookPreview;
using WSUI.Core.Core.LimeLM;
using WSUI.Core.Data;
using WSUI.Core.Data.UI;
using WSUI.Core.Enums;
using WSUI.Core.Events;
using WSUI.Core.Extensions;
using WSUI.Core.Helpers;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;
using WSUI.Core.Utils.Dialog;
using WSUI.Infrastructure.Events;
using WSUI.Infrastructure.Payloads;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Services;
using WSUI.Module.Core;
using WSUI.Module.Interface.Service;
using WSUI.Module.Interface.View;
using WSUI.Module.Interface.ViewModel;
using WSUI.Module.Service;
using WSUI.Module.Service.Dialogs.Message;
using WSUI.Module.Strategy;
using Application = System.Windows.Application;

namespace WSUI.Module.ViewModel
{
    public class MainViewModel : ViewModelBase, IMainViewModel
    {
        #region [urls]

        private const string BuyUrl = "https://outlookfinder.com/buy/";

        #endregion [urls]

        private const string Interface = "WSUI.Module.Interface.ViewModel.IKindItem";

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;

        private BaseSearchObject _currentData;
        private IKindItem _currentItem;
        private bool _enabled = true;
        private List<LazyKind> _listItems;
        private ILazyKind _selectedLazyKind;
        private Visibility _dataVisibility;
        private Visibility _backButtonVisibility;
        private bool _isBusy;
        private object _oldView = null;
        private INavigationService _navigationService;
        private SubscriptionToken _token;

        private Dictionary<TypeSearchItem, ICommandStrategy> _commandStrategies;
        private Dictionary<TypeSearchItem, IEnumerable<MenuItem>> _menuItems;
        private ICommandStrategy _currentStrategy;
        private int _selectedUIItemIndex;
        private IContactDetailsViewModel _contactDetails;

        public MainViewModel(IUnityContainer container, IKindsView kindView, IEventAggregator eventAggregator)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            KindsView = kindView;
            kindView.Model = this;
            MainDataSource = new List<BaseSearchObject>();
            Host = ReferenceEquals(Application.Current.MainWindow, null) ? HostType.Plugin : HostType.Application;
            DataVisibility = Visibility.Visible;
            _navigationService = _container.Resolve<INavigationService>();
            if (_navigationService != null)
            {
                _navigationService.SetMainViewModel(this);
            }
            _token = _eventAggregator.GetEvent<SelectedChangedPayloadEvent>().Subscribe(OnSelectedItemChanged);
        }

        public IKindsView KindsView { get; protected set; }

        public ObservableCollection<LazyKind> KindsCollection { get; protected set; }

        public ObservableCollection<UIItem> ContactUIItems { get; private set; }

        public int SelectedUIItemIndex
        {
            get { return _selectedUIItemIndex; }
            set
            {
                _selectedUIItemIndex = value;
                if (_contactDetails != null)
                {
                    _contactDetails.ApplyIndexForShowing(value);
                }
            }
        }

        public ILazyKind SelectedLazyKind
        {
            get { return _selectedLazyKind; }
            set
            {
                _selectedLazyKind = value;
                OnChoose(value.Kind);
                OnPropertyChanged(() => SelectedLazyKind);
            }
        }

        public ObservableCollection<IWSCommand> Commands
        {
            get { return _currentStrategy != null ? _currentStrategy.Commands : null; }
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

        public bool IsKindsVisible
        {
            get { return _navigationService != null && !(_navigationService.CurrentView is IContactDetailsView); }
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
            InitCommandStrategies();

        }

        private void InitCommandStrategies()
        {
            _commandStrategies = new Dictionary<TypeSearchItem, ICommandStrategy>();
            _commandStrategies.Add(TypeSearchItem.Email, CommadStrategyFactory.CreateStrategy(TypeSearchItem.Email, this));
            ICommandStrategy fileAttach = CommadStrategyFactory.CreateStrategy(TypeSearchItem.FileAll, this);
            _commandStrategies.Add(TypeSearchItem.File, fileAttach);
            _commandStrategies.Add(TypeSearchItem.Attachment, fileAttach);
            _commandStrategies.Add(TypeSearchItem.Picture, fileAttach);
            _commandStrategies.Add(TypeSearchItem.FileAll, fileAttach);

            InitMenuItems(_commandStrategies);

        }

        private void InitMenuItems(Dictionary<TypeSearchItem, ICommandStrategy> commandStrategies)
        {
            if (commandStrategies == null || !commandStrategies.Any())
                return;
            _menuItems = new Dictionary<TypeSearchItem, IEnumerable<MenuItem>>();
            if (commandStrategies.ContainsKey(TypeSearchItem.Email))
            {
                var list = commandStrategies[TypeSearchItem.Email].Commands.Select(wsCommand => new MenuItem()
                {
                    Command = wsCommand,
                    Header = wsCommand.Caption,
                    Icon = new Image() { Source = new BitmapImage(new Uri(wsCommand.Icon)) }
                }).ToList();
                _menuItems.Add(TypeSearchItem.Email, list);
            }
            if (commandStrategies.ContainsKey(TypeSearchItem.File))
            {
                var list = commandStrategies[TypeSearchItem.File].Commands.Select(wsCommand => new MenuItem()
                {
                    Command = wsCommand,
                    Header = wsCommand.Caption,
                    Icon = new Image() { Source = new BitmapImage(new Uri(wsCommand.Icon)) }
                }).ToList();
                _menuItems.Add(TypeSearchItem.File, list);
                _menuItems.Add(TypeSearchItem.Attachment, list);
                _menuItems.Add(TypeSearchItem.Picture, list);
                _menuItems.Add(TypeSearchItem.FileAll, list);

            }
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
            if (_navigationService != null)
            {
                ResetContactDetails();
                _navigationService.ShowSelectedKind(kindItem);
            }

            if (_navigationService.IsPreviewVisible)
            {
                _navigationService.PreviewView.ClearPreview();
            }
            OnPropertyChanged(() => Commands);
            OnPropertyChanged(() => BackButtonVisibility);
            OnPropertyChanged(() => IsKindsVisible);
        }

        private void Disconnect()
        {
            if (_currentItem == null)
                return;

            _currentItem.Start -= OnStart;
            _currentItem.Complete -= OnComplete;
            _currentItem.Error -= OnError;
        }

        private void Connect()
        {
            if (_currentItem == null)
                return;

            _currentItem.Start += OnStart;
            _currentItem.Complete += OnComplete;
            _currentItem.Error += OnError;
        }

        private void OnSelectedItemChanged(SearchObjectPayload searchObjectPayload)
        {
            _currentData = searchObjectPayload.Data as BaseSearchObject;
            if (_currentData == null || _currentData.TypeItem == TypeSearchItem.None)
                return;
            ShowPreview(Current);
        }

        private void ShowPreview(BaseSearchObject data)
        {
            if (data == null)
            {
                return;
            }
            ChooseStrategy(data);
            try
            {
                Enabled = false;

                switch (data.TypeItem)
                {
                    case TypeSearchItem.Contact:
                        Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => ShowPreviewForPreviewObject(data)),
                            null);
                        break;

                    default:
                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(ShowPreviewForCurrentItem), null);
                        break;
                }

                IsBusy = true;
                DataVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "ShowPreview", ex.Message));
            }
        }

        private void ShowPreviewForCurrentItem()
        {
            try
            {
                string filename = SearchItemHelper.GetFileName(_currentData);
                var previewView = _container.Resolve<IPreviewView>();
                if (previewView != null)
                {
                    previewView.Model = this;
                    if (!string.IsNullOrEmpty(filename))
                    {
                        if (_currentData.TypeItem == TypeSearchItem.Email)
                        {
                            previewView.SetFullFolderPath(SearchItemHelper.GetFullFolderPath(_currentData));
                        }

                        previewView.SetSearchPattern( _contactDetails != null ? _contactDetails.SearchCriteria : _currentItem != null
                            ? _currentItem.SearchString
                            : string.Empty);
                        previewView.SetPreviewFile(filename);
                        MoveToLeft(previewView);
                    }
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
                OnPropertyChanged(() => BackButtonVisibility);
            }
        }

        private void ShowPreviewForPreviewObject(BaseSearchObject previewData)
        {
            if (previewData == null)
                return;
            try
            {
                _contactDetails = _container.Resolve<IContactDetailsViewModel>();
                MoveToLeft(_contactDetails.View);
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => _contactDetails.SetDataObject(previewData)));

            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "ShowPreviewForPreviewObject", ex.Message));
            }
            finally
            {
                Enabled = true;
                IsBusy = false;
                OnPropertyChanged(() => BackButtonVisibility);
            }
        }

        private void OnStart(object sender, EventArgs e)
        {
            EventHandler temp = Start;
            if (temp != null)
                temp(this, null);
            Enabled = _currentItem.Enabled;
            IsBusy = true;
            if (_navigationService != null && _navigationService.IsBackButtonVisible)
            {
                ResetNavigation();
            }
            OnPropertyChanged(() => BackButtonVisibility);
        }

        private void ResetNavigation()
        {
            _navigationService.MoveToFirstDataView();
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
            if (BackCommand.CanExecute(null))
                BackCommand.Execute(null);
            CurrentKindChanged(_currentItem);
            if (!string.IsNullOrEmpty(searchString) && searchString != _currentItem.SearchString)
            {
                _currentItem.SearchString = searchString;
                _currentItem.FilterData();
            }
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
            SelectKind(KindsConstName.Everything);
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
                    if (_navigationService.IsPreviewVisible)
                    {
                        _navigationService.PreviewView.PassActionForPreview(action);
                    }
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
            if (_navigationService.IsPreviewVisible)
            {
                _navigationService.PreviewView.ClearPreview();
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
                OnPropertyChanged(() => IsBusy);
            }
        }

        public bool IsMainUiActive { get; private set; }

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

        public Visibility BackButtonVisibility
        {
            get
            {
                var visibility = _navigationService != null && _navigationService.IsBackButtonVisible ? Visibility.Visible : Visibility.Collapsed;
                Debug.WriteLine(visibility);
                return visibility;
            }
        }

        private Transition _currentTransition;

        public Transition CurrenTransition
        {
            get { return _currentTransition; }
            set
            {
                _currentTransition = value;
                OnPropertyChanged(() => CurrenTransition);
            }
        }

        public void ShowOutlookFolder(string folder)
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<WSUIShowFolder>().Publish(folder);
        }

        public void ShowContactPreview(object tag)
        {
            ShowPreview(tag as BaseSearchObject);
        }

        public BaseSearchObject Current
        {
            get { return _currentData; }
        }

        public BaseSearchObject CurrentTracked 
        {
            get { return GetContactDetailsTrackedObject() ?? GetKindItemTrackedObject(); }
        }

        public IEnumerable<MenuItem> EmailsMenuItems 
        {
            get { return _menuItems[TypeSearchItem.Email]; }
        }

        public IEnumerable<MenuItem> FileMenuItems
        {
            get { return _menuItems[TypeSearchItem.File]; }
        }

        public bool IsPreviewVisible 
        {
            get { return _navigationService != null && _navigationService.IsPreviewVisible; }
        }

        #endregion Implementation of IMainViewModel

        public virtual void Init()
        {
            Application.Current.Dispatcher.BeginInvoke(new Action(InitializeInThread), null);
            OutlookPreviewHelper.Instance.PreviewHostType = HostType.Plugin;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            BuyCommand = new DelegateCommand(InternalBuy);
            ActivateCommand = new DelegateCommand(RunInternalActivate);
            BackCommand = new WSUIRelayCommand(InternalBack, CanInternalBack);
        }

        private void InternalBack(object arg)
        {
            MoveToRight();
            OnPropertyChanged(() => BackButtonVisibility);
        }

        private bool CanInternalBack(object arg)
        {
            return BackButtonVisibility == Visibility.Visible;
        }

        private void MoveToLeft(object view)
        {
            if (_navigationService == null)
                return;
            BeforeMoveToLeft(view);
            _navigationService.MoveToLeft(view as INavigationView);
            OnPropertyChanged(() => IsKindsVisible);
        }

        private void BeforeMoveToLeft(object view)
        {
            if (view is IContactDetailsView)
            {
                ContactUIItems = new ObservableCollection<UIItem>(_contactDetails.ContactUIItemCollection);
                _contactDetails.PropertyChanged += OnContactDetailsPropertyChanged;
                SelectedUIItemIndex = 0;
                OnPropertyChanged(() => ContactUIItems);
                OnPropertyChanged(() => SelectedUIItemIndex);
            }
        }

        private void MoveToRight()
        {
            if (_navigationService == null)
                return;
            BeforeMoveToRight();
            _navigationService.MoveToRight();
            OnPropertyChanged(() => IsKindsVisible);
        }

        private void BeforeMoveToRight()
        {
            if (_navigationService.CurrentView is IContactDetailsView)
            {
                ResetContactDetails();
            }
        }

        private void ResetContactDetails()
        {
            if (_contactDetails == null)
                return;
            _contactDetails.PropertyChanged -= OnContactDetailsPropertyChanged;
            _contactDetails = null;
            ContactUIItems = null;
            OnPropertyChanged(() => ContactUIItems);
        }

        private void OnContactDetailsPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "SelectedIndex":
                    if (_contactDetails != null)
                    {
                        _selectedUIItemIndex = _contactDetails.SelectedIndex;
                        OnPropertyChanged(() => SelectedUIItemIndex);
                    }
                    break;
            }
        }

        private void PreviewViewOnStopLoad(object sender, EventArgs eventArgs)
        {
        }

        private void ChooseStrategy(BaseSearchObject current)
        {
            if (current == null)
                return;
            _currentStrategy = _commandStrategies.ContainsKey(current.TypeItem) ? _commandStrategies[current.TypeItem] : null;

            OnPropertyChanged(() => Commands);
        }

        private BaseSearchObject GetContactDetailsTrackedObject()
        {
            return _contactDetails != null ?  _contactDetails.TrackedElement as BaseSearchObject :  null;
        }

        private BaseSearchObject GetKindItemTrackedObject()
        {
            return _currentItem != null ? _currentItem.CurrentTrackedObject : null;
        }

    }
}