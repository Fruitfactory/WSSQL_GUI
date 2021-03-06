using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MahApps.Metro;
using Microsoft.Practices.Prism.Commands;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OF.Core;
using Transitionals;
using OFPreview.PreviewHandler.Service.OutlookPreview;
using OF.Core.Core.LimeLM;
using OF.Core.Core.MVVM;
using OF.Core.Data;
using OF.Core.Data.ElasticSearch.Response;
using OF.Core.Data.NamedPipeMessages;
using OF.Core.Data.UI;
using OF.Core.Enums;
using OF.Core.Events;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Utils.Dialog;
using OF.Core.Win32;
using OF.Infrastructure.Events;
using OF.Infrastructure.NamedPipes;
using OF.Infrastructure.Payloads;
using OF.Infrastructure.Service.Helpers;
using OF.Infrastructure.Services;
using OF.Module.Core;
using OF.Module.Events;
using OF.Module.Interface.Service;
using OF.Module.Interface.View;
using OF.Module.Interface.ViewModel;
using OF.Module.Service;
using OF.Module.Service.Dialogs.Message;
using OF.Module.Strategy;
using Application = System.Windows.Application;

namespace OF.Module.ViewModel
{
    public class MainViewModel : OFViewModelBase, IMainViewModel
    {
        #region [urls]

        private const string BuyUrl = "https://outlookfinder.com/buy/";

        #endregion [urls]

        private const string Interface = "OF.Module.Interface.ViewModel.IKindItem";

        private readonly IUnityContainer _container;
        private readonly IEventAggregator _eventAggregator;
        private readonly object LOCK = new object();

        private OFBaseSearchObject _currentData;
        private IKindItem _currentItem;
        private bool _enabled = true;
        private List<OFLazyKind> _listItems;
        private ILazyKind _selectedLazyKind;
        private Visibility _dataVisibility;


        private bool _isBusy;
        private INavigationService _navigationService;
        private SubscriptionToken _token;

        private Dictionary<OFTypeSearchItem, ICommandStrategy> _commandStrategies;
        private Dictionary<OFTypeSearchItem, IEnumerable<MenuItem>> _menuItems;
        private ICommandStrategy _currentStrategy;
        private int _selectedUIItemIndex;
        private IElasticSearchViewModel _elasticSearchViewModel;
        private IOFTurboLimeActivate _turboLimeActivate;
        private OFNamedPipeServer<OFReaderStatus> _readerStatusServer;
        private IOFEmailSuggestViewModel _suggestViewModel;


        public MainViewModel(IUnityContainer container, IKindsView kindView, IEventAggregator eventAggregator)
        {
            _container = container;
            _eventAggregator = eventAggregator;
            KindsView = kindView;
            kindView.Model = this;
            MainDataSource = new List<OFBaseSearchObject>();
            Host = ReferenceEquals(Application.Current.MainWindow, null) ? OFHostType.Plugin : OFHostType.Application;
            DataVisibility = Visibility.Visible;
            _navigationService = _container.Resolve<INavigationService>();
            if (_navigationService != null)
            {
                _navigationService.SetMainViewModel(this);
            }
            _token = _eventAggregator.GetEvent<OFSelectedChangedPayloadEvent>().Subscribe(OnSelectedItemChanged);
            _elasticSearchViewModel = _container.Resolve<IElasticSearchViewModel>();
            _elasticSearchViewModel.IndexingFinished += ElasticSearchViewModelOnIndexingFinished;
            _elasticSearchViewModel.Closed += ElasticSearchViewModelOnIndexingFinished;
            Monitoring = _container.Resolve<IElasticSearchMonitoringViewModel>();
            Monitoring.StatusChanged += MonitoringOnStatusChanged;
            _turboLimeActivate = _container.Resolve<IOFTurboLimeActivate>();

            _suggestViewModel = _container.Resolve<IOFEmailSuggestViewModel>();
            _suggestViewModel.MainViewModel = this;
            _eventAggregator.GetEvent<OFElasticsearchServiceStartedEvent>().Subscribe(OnElasticSearchServiceStarted);

            _readerStatusServer = new OFNamedPipeServer<OFReaderStatus>(GlobalConst.PluginServer);
            _readerStatusServer.Attach(_elasticSearchViewModel as IOFNamedPipeObserver<OFReaderStatus>);
            _readerStatusServer.Attach(Monitoring as IOFNamedPipeObserver<OFReaderStatus>);

        }

        


        public IElasticSearchMonitoringViewModel Monitoring { get; private set; }

        public IKindsView KindsView { get; protected set; }

        public ObservableCollection<OFLazyKind> KindsCollection { get; protected set; }

        public ObservableCollection<OFUIItem> ContactUIItems { get; private set; }

        public int SelectedUIItemIndex
        {
            get { return _selectedUIItemIndex; }
            set
            {
                _selectedUIItemIndex = value;
                if (_navigationService.IsContactDetailsVisible)
                {
                    _navigationService.ContactDetailsViewModel.ApplyIndexForShowing(value);
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

        public ObservableCollection<IOFCommand> Commands
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
            try
            {
                _listItems = new List<OFLazyKind>();
                KindsCollection = new ObservableCollection<OFLazyKind>();
                GetAllKinds();
                if (_listItems.Count > 0)
                {
                    _listItems.ForEach(k =>
                    {
                        if (k.IsVisibleByDefault)
                            KindsCollection.Add(k);
                    });
                    SelectedLazyKind = _listItems.FirstOrDefault(k => k.IsVisibleByDefault);
                }
                OnPropertyChanged(() => KindsCollection);
                InitCommandStrategies();


            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void InitCommandStrategies()
        {
            try
            {
                _commandStrategies = new Dictionary<OFTypeSearchItem, ICommandStrategy>();
                _commandStrategies.Add(OFTypeSearchItem.Email,
                    CommadStrategyFactory.CreateStrategy(OFTypeSearchItem.Email, this));
                ICommandStrategy fileAttach = CommadStrategyFactory.CreateStrategy(OFTypeSearchItem.FileAll, this);
                _commandStrategies.Add(OFTypeSearchItem.File, fileAttach);
                _commandStrategies.Add(OFTypeSearchItem.Attachment, fileAttach);
                _commandStrategies.Add(OFTypeSearchItem.Picture, fileAttach);
                _commandStrategies.Add(OFTypeSearchItem.FileAll, fileAttach);

                InitMenuItems(_commandStrategies);

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void InitMenuItems(Dictionary<OFTypeSearchItem, ICommandStrategy> commandStrategies)
        {
            if (commandStrategies == null || !commandStrategies.Any())
                return;
            _menuItems = new Dictionary<OFTypeSearchItem, IEnumerable<MenuItem>>();
            if (commandStrategies.ContainsKey(OFTypeSearchItem.Email))
            {
                var list = commandStrategies[OFTypeSearchItem.Email].Commands.Select(wsCommand => new MenuItem()
                {
                    Command = wsCommand,
                    Header = wsCommand.Caption,
                    Icon = new Image() { Source = new BitmapImage(new Uri(wsCommand.Icon)) }
                }).ToList();
                _menuItems.Add(OFTypeSearchItem.Email, list);
            }
            if (commandStrategies.ContainsKey(OFTypeSearchItem.File))
            {
                var list = commandStrategies[OFTypeSearchItem.File].Commands.Select(wsCommand => new MenuItem()
                {
                    Command = wsCommand,
                    Header = wsCommand.Caption,
                    Icon = new Image() { Source = new BitmapImage(new Uri(wsCommand.Icon)) }
                }).ToList();
                _menuItems.Add(OFTypeSearchItem.File, list);
                _menuItems.Add(OFTypeSearchItem.Attachment, list);
                _menuItems.Add(OFTypeSearchItem.Picture, list);
                _menuItems.Add(OFTypeSearchItem.FileAll, list);

            }
        }

        private void UpdatedActivatedStatus()
        {
            try
            {
                ActivateStatus = _turboLimeActivate.State;
                OFLogger.Instance.LogDebug("Activated Status: {0}", ActivateStatus.ToString());
                OnPropertyChanged(() => ActivateStatus);
                OnPropertyChanged(() => VisibleTrialLabel);
                OnPropertyChanged(() => DaysLeft);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void CheckStateAndShowActivatedForm()
        {
            try
            {
                _turboLimeActivate.TryCheckAgain();
                ActivateStatus = _turboLimeActivate.State;
                switch (ActivateStatus)
                {
#if !TRIAL
                    //case ActivationState.Trial:
#endif
                    case OFActivationState.TrialEnded:
                    case OFActivationState.NonActivated:
                        RunInternalActivate();
                        break;

                    case OFActivationState.Trial:
                    case OFActivationState.Activated:

                        break;
                }
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => _turboLimeActivate.IncreaseTimeUsedFlag()));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void RunInternalActivate()
        {

            try
            {
                _turboLimeActivate.Activate(UpdatedActivatedStatus);
                //string path = Path.Combine(Path.GetDirectoryName(typeof(TurboLimeActivate).Assembly.Location), "TurboActivate.exe");
                //OFLogger.Instance.LogDebug("Path Activate: {0}", path);
                //Process activationProcess = new Process()
                //{
                //    StartInfo =
                //                {
                //                    FileName = path
                //                },
                //    EnableRaisingEvents = true
                //};
                //activationProcess.Exited += ActivationProcessOnExited;
                //activationProcess.Start();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogDebug("----- !!!! <<<<<< ERROR >>>>>>> !!! -----");
                OFLogger.Instance.LogError(ex.ToString());
            }

            //TurboLimeActivate.Instance.Activate(UpdatedActivatedStatus);
        }

        private void ActivationProcessOnExited(object sender, EventArgs eventArgs)
        {
            ((Process)sender).Exited -= ActivationProcessOnExited;
            UpdatedActivatedStatus();
        }

        private void GetAllKinds()
        {
            try
            {
                IEnumerable<Type> types =
                    this.GetType()
                        .Assembly.GetTypes()
                        .Where(t => !t.IsAbstract && t.IsClass && t.GetInterface(Interface, true) != null);
                foreach (Type type in types)
                {
                    var kind = new OFLazyKind(_container, type, this, null, OnPropertyChanged);
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
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }



        private void Disconnect()
        {
            if (_currentItem == null)
                return;

            _currentItem.Start -= OnStart;
            _currentItem.Complete -= OnComplete;
            _currentItem.Error -= OnError;
            _currentItem.SearchStringCleared -= CurrentItemOnSearchStringCleared;

        }

        private void Connect()
        {
            if (_currentItem == null)
                return;

            _currentItem.Start += OnStart;
            _currentItem.Complete += OnComplete;
            _currentItem.Error += OnError;
            _currentItem.SearchStringCleared += CurrentItemOnSearchStringCleared;
        }

        private void CurrentItemOnSearchStringCleared(object sender, EventArgs eventArgs)
        {
            foreach (var ofLazyKind in _listItems)
            {
                if (!ReferenceEquals(ofLazyKind.Kind, _currentItem))
                {
                    ofLazyKind.Kind.SetSearchString(string.Empty);
                }
            }
        }

        private void OnSelectedItemChanged(OFSearchObjectPayload searchObjectPayload)
        {
            _currentData = searchObjectPayload.Data as OFBaseSearchObject;
            if (_currentData == null || _currentData.TypeItem == OFTypeSearchItem.None)
                return;
            ShowPreview(Current);
        }

        private void ShowPreview(OFBaseSearchObject data, bool useTransaction = true)
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
                    case OFTypeSearchItem.Contact:
                        Dispatcher.CurrentDispatcher.BeginInvoke(
                            (Action)(() => ShowPreviewForPreviewObject(data, useTransaction)),
                            null);
                        break;

                    default:
                        Dispatcher.CurrentDispatcher.BeginInvoke((Action)(ShowPreviewForCurrentItem));
                        break;
                }

                IsBusy = true;
                DataVisibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void ShowPreviewForCurrentItem()
        {
            try
            {
                var previewView = _container.Resolve<IPreviewView>();
                if (previewView == null)
                {
                    return;
                }
                System.Diagnostics.Debug.WriteLine("Preview " + previewView.GetHashCode());
                previewView.Model = this;
                previewView.SetSearchPattern(_navigationService.IsContactDetailsVisible
                    ? _navigationService.ContactDetailsViewModel.SearchCriteria
                    : _currentItem != null
                        ? _currentItem.GetSearchPattern()
                        : string.Empty);
                switch (_currentData.TypeItem)
                {
                    case OFTypeSearchItem.Email:
                        if (_currentData.TypeItem == OFTypeSearchItem.Email)
                        {
                            previewView.SetFullFolderPath(
                                OFSearchItemHelper.GetFullFolderPath(_currentData as OFEmailSearchObject));
                        }
                        previewView.SetPreviewObject(_currentData);
                        break;
                    default:
                        string filename = OFSearchItemHelper.GetFileName(_currentData);
                        previewView.SetPreviewFile(filename);
                        break;
                }
                MoveToLeft(previewView);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
            finally
            {
                Enabled = true;
                IsBusy = false;
                OnPropertyChanged(() => BackButtonVisibility);
            }
        }

        private void ShowPreviewForPreviewObject(OFBaseSearchObject previewData, bool useTransaction = true)
        {
            if (previewData == null)
                return;
            try
            {
                var contactDetails = _container.Resolve<IContactDetailsViewModel>();
                MoveToLeft(contactDetails.View, useTransaction);
                Dispatcher.CurrentDispatcher.BeginInvoke((Action)(() => contactDetails.SetDataObject(previewData)));

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
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
            try
            {
                EventHandler temp = Start;
                if (temp != null)
                    temp(this, null);
                Enabled = _currentItem.Enabled;
                IsBusy = true;
                if (_navigationService.IsNotNull())
                {
                    ResetNavigation();
                }
                OnPropertyChanged(() => BackButtonVisibility);
                OnPropertyChanged(() => IsKindsVisible);

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void ResetNavigation()
        {
            _navigationService.MoveToFirstDataView(false);
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
            try
            {
                Disconnect();
                string searchString = string.Empty;
                if (_currentItem != null && !(_currentItem is IAdvancedSearchViewModel))
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
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void CurrentKindChanged(object kindItem)
        {
            try
            {
                if (_navigationService != null)
                {
                    if (_navigationService.IsContactDetailsVisible)
                    {
                        ResetContactDetails();
                    }
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
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
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
                OFLogger.Instance.LogError("Buy: {0}", ex.Message);
            }
        }

        private void InternalTryAgain()
        {
            try
            {
                _turboLimeActivate.TryCheckAgain();
                UpdatedActivatedStatus();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void InternalDeactivate()
        {
            try
            {
                if (_turboLimeActivate.Deactivate(true))
                {
                    UpdatedActivatedStatus();
                }
                else
                {
                    OFMessageBoxService.Instance.Show("Warning", "Something wrong during Deactivate",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void ClearTextCriteriaForAllKinds()
        {
            _listItems.ForEach(k =>
            {
                if (k.Kind != null)
                    k.Kind.SearchString = string.Empty;
            });
            SelectKind(OFKindsConstName.Everything);
        }

        #endregion private

        #region Implementation of IMainViewModel

        public string DaysLeft
        {
            get
            {
#if !TRIAL
                return _turboLimeActivate.DaysRemain.ToString(); //return string.Empty;
#else
      return TurboLimeActivate.Instance.DaysRemain.ToString();
#endif
            }
        }

        public event EventHandler Start;

        public event EventHandler Complete;
#pragma warning disable 0067
        public event EventHandler<OFSlideDirectionEventArgs> Slide;
#pragma warning restore 0067

        public List<OFBaseSearchObject> MainDataSource { get; protected set; }

        public void Clear()
        {
            OFTempFileManager.Instance.ClearTempFolder();
        }

        public void SelectKind(string name)
        {
            if (string.IsNullOrEmpty(name))
                return;

            OFLazyKind lazyKind = _listItems.Find(lk => lk.UIName == name);
            if (lazyKind != null)
            {
                SelectedLazyKind = lazyKind;
            }
        }

        public void PassAction(IWSAction action)
        {
            try
            {
                switch (action.Action)
                {
                    case OFActionType.Copy:
                    case OFActionType.Cut:
                    case OFActionType.Paste:
                    case OFActionType.ShowContextMenu:
                        if (_navigationService.IsPreviewVisible)
                        {
                            _navigationService.PreviewView.PassActionForPreview(action);
                        }
                        break;

                    case OFActionType.Search:
                        RunSearchFromExternal(action);
                        break;

                    case OFActionType.Show:
                        CheckStateAndShowActivatedForm();
                        break;

                    case OFActionType.Hide:
                        break;

                    case OFActionType.Quit:
                        //StopUserActivityTracker();
                        Clear();
                        NotifyServerPluginShutdown();
                        break;

                    case OFActionType.ClearText:
                        ClearTextCriteriaForAllKinds();
                        break;
                    case OFActionType.ShowContact:
                        ShowSenderOfSelectedOutlookEmail(action);
                        break;
                    case OFActionType.Settings:
                        ShowSettings();
                        break;
                    case OFActionType.DeleteMail:
                        RemoveEmail(action);
                        break;
                    case OFActionType.ShowSuggestEmail:
                        OFLogger.Instance.LogDebug("Show Suggested emails... " + action.Data);
                        _suggestViewModel.Show((Tuple<IntPtr, string>)action.Data);
                        break;
                    case OFActionType.HideSuggestEmail:
                        OFLogger.Instance.LogDebug("Hide Suggested emails... " + action.Data);
                        _suggestViewModel.Hide();
                        break;
                    case OFActionType.UpSuggestEmail:
                    case OFActionType.DownSuggestEmail:
                    case OFActionType.SelectSuggestEmail:
                        _suggestViewModel.ProcessSelection(action.Action);
                        break;
                    case OFActionType.SendLogFile:
                        SendLogFiles();
                        break;
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void ShowSettings()
        {
            var wnd = _container.Resolve<IMainSettingsWindow>();

            wnd.ShowModal();
        }

        private void SendLogFiles()
        {
            var sendlogManager = _container.Resolve<IOFLogFilesSenderManager>();
            if (sendlogManager.IsNull())
            {
                throw new NullReferenceException("Sender LogSender Manager");
            }
            if (sendlogManager.SendLogFiles())
            {
                OFLogger.Instance.LogDebug("Log files were sent...");
                OFMessageBoxService.Instance.Show("Send Logs Files", "Log files were sent.");
            }
            else
            {
                OFLogger.Instance.LogDebug("Log files weren't sent...");
                OFMessageBoxService.Instance.Show("Send Logs Files", "Log files weren't sent.");
            }
        }

        private void RemoveEmail(IWSAction action)
        {
            var entryId = action.Data as string;
            if (string.IsNullOrEmpty(entryId))
            {
                return;
            }

            using (var removingClient = _container.Resolve<IOFElasticSearchRemovingClient>())
            {
                if (removingClient.IsNotNull())
                {
                    removingClient.RemoveEmail(entryId);
                }
            }
        }

        private void RunSearchFromExternal(IWSAction action)
        {
            try
            {
                var searchCriteria = action.Data as string;
                if (string.IsNullOrEmpty(searchCriteria))
                    return;
                _currentItem.SetSearchString(searchCriteria);
                _currentItem.SearchCommand.Execute(null);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void ShowSenderOfSelectedOutlookEmail(IWSAction action)
        {
            try
            {
                if (_navigationService != null && _navigationService.IsContactDetailsVisible &&
                    !_navigationService.ContactDetailsViewModel.IsSameData(action.Data as ISearchObject))
                {
                    _navigationService.MoveToFirstDataView(false);
                    ShowContactPreview(action.Data, false);
                }
                else if (!_navigationService.IsContactDetailsVisible)
                {
                    _navigationService.MoveToFirstDataView(false);
                    ShowContactPreview(action.Data, false);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        public void ForceClosePreview()
        {
            if (_navigationService.IsPreviewVisible)
            {
                _navigationService.PreviewView.ClearPreview();
            }
        }

        public OFActivationState ActivateStatus { get; private set; }

        public bool IsActivated => ActivateStatus == OFActivationState.Activated || ActivateStatus == OFActivationState.Trial;

        public ICommand BuyCommand { get; private set; }

        public ICommand ActivateCommand { get; private set; }

        public ICommand BackCommand { get; private set; }

        public bool IsBusy
        {
            get { return _isBusy; }
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
                return ActivateStatus == OFActivationState.Activated ? Visibility.Collapsed : Visibility.Visible;
                //return Visibility.Collapsed;
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
                var visibility = _navigationService != null && _navigationService.IsBackButtonVisible
                    ? Visibility.Visible
                    : Visibility.Collapsed;
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
            _eventAggregator.GetEvent<OFShowFolder>().Publish(folder);
        }

        public void ShowContactPreview(object tag, bool useTransaction = true)
        {
            ShowPreview(tag as OFBaseSearchObject, useTransaction);
        }

        public void ShowAdvancedSearch(object tag)
        {
            var advancedSearch = _listItems.FirstOrDefault(k => k.Kind is IAdvancedSearchViewModel);
            if (advancedSearch.IsNull())
                return;
            SelectKind(advancedSearch.UIName);
        }

        public OFBaseSearchObject Current
        {
            get { return _currentData; }
        }

        public OFBaseSearchObject CurrentTracked
        {
            get { return GetContactDetailsTrackedObject() ?? GetKindItemTrackedObject(); }
        }

        public IEnumerable<MenuItem> EmailsMenuItems
        {
            get { return _menuItems[OFTypeSearchItem.Email]; }
        }

        public IEnumerable<MenuItem> FileMenuItems
        {
            get { return _menuItems[OFTypeSearchItem.File]; }
        }

        public bool IsPreviewVisible
        {
            get { return _navigationService != null && _navigationService.IsPreviewVisible; }
        }

        public bool IsMenuEnabled
        {
            get { return true; }
        }


        #endregion Implementation of IMainViewModel

        public virtual void Init()
        {
            try
            {
                var mem = WindowsFunction.GetAvailableMemory();
                _turboLimeActivate.Init();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            try
            {
                _readerStatusServer.Start();
                NotifyServerPluginRunning();
                Application.Current.Dispatcher.BeginInvoke(new Action(InitializeInThread), null);
                OutlookPreviewHelper.Instance.PreviewHostType = OFHostType.Plugin;
                UpdatedActivatedStatus();
                InitializeCommands();
                if (_elasticSearchViewModel.IsNotNull())
                {
                    _elasticSearchViewModel.Initialize();
                    if (!_elasticSearchViewModel.IsInitialIndexinginProgress &&
                        (!_elasticSearchViewModel.IsServiceInstalled ||
                        !_elasticSearchViewModel.IsServiceRunning ||
                        !_elasticSearchViewModel.IsIndexExisted))
                    {
                        _elasticSearchViewModel.Show(false);
                    }
                    else if (_elasticSearchViewModel.IsInitialIndexinginProgress)
                    {
                        _elasticSearchViewModel.Show(true);
                    }
                    else
                    {
                        Monitoring.Start();
                    }
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.Message);
            }
        }

        private void InitializeCommands()
        {
            BuyCommand = new DelegateCommand(InternalBuy);
            ActivateCommand = new DelegateCommand(RunInternalActivate);
            BackCommand = new OFRelayCommand(InternalBack, CanInternalBack);
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

        private void MoveToLeft(object view, bool useTransaction = true)
        {
            if (_navigationService == null)
                return;
            BeforeMoveToLeft(view);
            _navigationService.MoveToLeft(view as INavigationView, useTransaction);
            OnPropertyChanged(() => IsKindsVisible);
        }

        private void BeforeMoveToLeft(object view)
        {
            if (view is IContactDetailsView)
            {
                var contacDetailsModel = (view as IContactDetailsView).Model;
                ContactUIItems = new ObservableCollection<OFUIItem>(contacDetailsModel.ContactUIItemCollection);
                contacDetailsModel.PropertyChanged += OnContactDetailsPropertyChanged;
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
            AfterMoveToRight();
            OnPropertyChanged(() => IsKindsVisible);
        }

        private void AfterMoveToRight()
        {
            if (_navigationService.IsContactDetailsVisible)
            {
                var contacDetailsModel = _navigationService.ContactDetailsViewModel;
                ContactUIItems = new ObservableCollection<OFUIItem>(contacDetailsModel.ContactUIItemCollection);
                //SelectedUIItemIndex = 0; // TODO: cause the reseting contact details to first tab.
                OnPropertyChanged(() => ContactUIItems);
                OnPropertyChanged(() => SelectedUIItemIndex);
            }
        }

        private void BeforeMoveToRight()
        {
            if (_navigationService.IsContactDetailsVisible)
            {
                ResetContactDetails();
            }
        }

        private void ResetContactDetails()
        {

            if (_navigationService.ContactDetailsViewModel == null)
                return;
            _navigationService.ContactDetailsViewModel.PropertyChanged -= OnContactDetailsPropertyChanged;
            ContactUIItems = null;
            OnPropertyChanged(() => ContactUIItems);
        }

        private void OnContactDetailsPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            switch (propertyChangedEventArgs.PropertyName)
            {
                case "SelectedIndex":
                    if (_navigationService.IsContactDetailsVisible)
                    {
                        _selectedUIItemIndex = _navigationService.ContactDetailsViewModel.SelectedIndex;
                        OnPropertyChanged(() => SelectedUIItemIndex);
                    }
                    break;
            }
        }

        private void PreviewViewOnStopLoad(object sender, EventArgs eventArgs)
        {
        }

        private void ChooseStrategy(OFBaseSearchObject current)
        {
            if (current == null)
                return;
            _currentStrategy = _commandStrategies.IsNotNull() && _commandStrategies.ContainsKey(current.TypeItem) ? _commandStrategies[current.TypeItem] : null;

            OnPropertyChanged(() => Commands);
        }

        private OFBaseSearchObject GetContactDetailsTrackedObject()
        {
            return _navigationService.IsContactDetailsVisible ? _navigationService.ContactDetailsViewModel.TrackedElement as OFBaseSearchObject : null;
        }

        private OFBaseSearchObject GetKindItemTrackedObject()
        {
            return _currentItem != null ? _currentItem.CurrentTrackedObject : null;
        }

        private void ElasticSearchViewModelOnIndexingFinished(object sender, EventArgs eventArgs)
        {
            lock (LOCK)
            {
                if (Monitoring.IsNotNull() && !Monitoring.IsRunning)
                {
                    Monitoring.Start();
                }
                NotifyServerPluginRunning();
                _readerStatusServer.Deattach(_elasticSearchViewModel as IOFNamedPipeObserver<OFReaderStatus>);
            }
        }


        private void NotifyServerPluginRunning()
        {
            try
            {
                var ofPluginStatus = _container.Resolve<IServiceAppOFPluginStatusClient>();
                if (ofPluginStatus.IsNull())
                {
                    return;
                }
                ofPluginStatus.OFPluginStatus(true);
            }
            catch (WebException we)
            {
                OFLogger.Instance.LogError(we.ToString());
            }

        }

        private void NotifyServerPluginShutdown()
        {
            var ofPluginStatus = _container.Resolve<IServiceAppOFPluginStatusClient>();
            if (ofPluginStatus.IsNull())
            {
                return;
            }
            ofPluginStatus.OFPluginStatus(false);
        }

        private void OnElasticSearchServiceStarted(bool b)
        {
            if (!b)
            {
                return;
            }
            if (_elasticSearchViewModel.IsVisible)
            {
                _elasticSearchViewModel.Close();
            }
            else if (Monitoring.IsNotNull() && !Monitoring.IsRunning)
            {
                Monitoring.Start();
            }
            NotifyServerPluginRunning();
        }

        private void MonitoringOnStatusChanged(object sender, EventArgs<OFRiverStatus> eventArgs)
        {
            if (eventArgs.Value != OFRiverStatus.Busy && eventArgs.Value != OFRiverStatus.InitialIndexing)
            {
                _suggestViewModel.UpdateSuggectingList();
            }
        }

    }
}