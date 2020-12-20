﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Threading;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using OFPreview.PreviewHandler.Service.OutlookPreview;
using OF.Control;
using OF.Control.Interfaces;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Events;
using OF.Core.Helpers;
using OF.Core.Logger;
using OF.Core.Extensions;
using OF.Infrastructure.Controls.Application;
using OFOutlookPlugin.Core;
using OFOutlookPlugin.Events;
using OFOutlookPlugin.Interfaces;
using OFOutlookPlugin.Managers;
using Application = System.Windows.Forms.Application;
using Outlook = Microsoft.Office.Interop.Outlook;
using Microsoft.Win32;
using OF.Core;
using OF.Core.Data.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Win32;
using OF.Infrastructure.Events;
using OF.Infrastructure.Implements.ElasticSearch.Clients;
using OF.Infrastructure.Payloads;
using OFOutlookPlugin.Hooks;

namespace OFOutlookPlugin
{
    public class OFAddinModule 
    {
        #region [need]

        private int _outlookVersion = 0;
        private string _officeVersion = string.Empty;

        private IUpdatable _updatable = null;
        private IPluginBootStraper _wsuiBootStraper = null;
        private IEventAggregator _eventAggregator = null;
        private IOFCommandManager _commandManager;
        private ICommandManager _aboutCommandManager;
        private ISidebarForm _sidebarForm;
        private bool IsLoading = true;
        private int _initHashCode;
        private IOFMailRemovingManager _mailRemovingManager;
        private IOFEmailSuggesterManager _emailSuggesterManager;
        private bool _canConnect = true;

        private static string SERVICE_APP = "SERVICEAPP";

        #endregion

        #region [const]

        public const string ProgIdDefault = OFRegistryHelper.ProgIdKey;

        private const string ADXHTMLFileName = "ADXOlFormGeneral.html";
        private const int WM_USER = 0x0400;
        private const int WM_CLOSE = 0x0010;
        private const int WM_QUIT = 0x0012;
        private const int WM_DESTROY = 0x0002;
        private const int WM_LOADED = WM_USER + 1001;
        private ImageList wsuiImageList;
        private const string DefaultNamespace = "MAPI";

        #endregion [const]

        public OFAddinModule()
        {
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "false";
            Application.EnableVisualStyles();
            InitializeComponent();
            Init();
            //this.OnSendMessage += OFAddinModule_OnSendMessage;
            //this.AddinInitialize += OnAddinInitialize;
            //this.AddinBeginShutdown += OnAddinBeginShutdown;
            //_mailRemovingManager = new OFOutlookItemsRemovingManager(this);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;
        }

        public void AddinBeginShutdown(object sender, EventArgs eventArgs)
        {

        }

        public void AddinInitialize(object sender, EventArgs eventArgs)
        {
            try
            {
                //TODO !!!!
                CheckAndStartServiceApp();

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        // TODO: change EventArgs to something particular which transfer the message itself
        private void OFAddinModuleOnSendMessage(object sender, EventArgs e)
        {
            //switch(e.Message)
            //{
            //    case WM_LOADED:
            //        lock(this)
            //        {
            //            RunPluginUI();
            //        }
            //        break;
            //}
        }
        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.wsuiImageList = new System.Windows.Forms.ImageList(null);
            // 
            // wsuiImageList
            // 
            this.wsuiImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(Properties.Resources.ResourceManager.GetObject("wsuiImageList.ImageStream")));            
            this.wsuiImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.wsuiImageList.Images.SetKeyName(0, "application-plus-red.png");
            this.wsuiImageList.Images.SetKeyName(1, "logo_64.png");
            this.wsuiImageList.Images.SetKeyName(2, "gear.png");
            this.wsuiImageList.Images.SetKeyName(3, "question.png");
            // 
            // OutlookFinderEvents
            // 
            //this.OutlookFinderEvents.ItemSend += new AddinExpress.MSO.ADXOlItemSend_EventHandler(this.OutlookFinderEvents_ItemSend);
            //this.OutlookFinderEvents.Quit += new System.EventHandler(this.OutlookFinderEvents_Quit);
            //this.OutlookFinderEvents.InspectorActivate += new AddinExpress.MSO.ADXOlInspector_EventHandler(this.OutlookFinderEvents_InspectorActivate);
            //this.OutlookFinderEvents.InspectorClose += new AddinExpress.MSO.ADXOlInspector_EventHandler(this.OutlookFinderEvents_InspectorClose);
            //this.OutlookFinderEvents.NewExplorer += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_NewExplorer);
            //this.OutlookFinderEvents.ExplorerActivate += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerActivate);
            //this.OutlookFinderEvents.ExplorerClose += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerClose);
            //this.OutlookFinderEvents.ExplorerSelectionChange += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerSelectionChange);
            //this.OutlookFinderEvents.ExplorerInlineResponse += new AddinExpress.MSO.ADXOlExplorerInlineResponse_EventHandler(this.OutlookFinderEvents_ExplorerInlineResponse);
            //this.OutlookFinderEvents.ExplorerInlineResponseEx += new AddinExpress.MSO.ADXOlExplorerInlineResponseEx_EventHandler(this.OutlookFinderEvents_ExplorerInlineResponseEx);
            //this.OutlookFinderEvents.ExplorerInlineResponseCloseEx += new AddinExpress.MSO.ADXOlExplorerInlineResponseCloseEx_EventHandler(this.OutlookFinderEvents_ExplorerInlineResponseCloseEx);
        }

        #endregion Component Designer generated code


        public Outlook._Application OutlookApp
        {
            get
            {
                return null; // TODO: we will get this instance from constructor or somehow else
            }
        }

        public IPluginBootStraper BootStraper
        {
            get
            {
                return _wsuiBootStraper;
            }
        }


        public bool IsMainUIVisible
        {
            get;
            set;
        }

        //#region my own initialization

        private void Init()
        {
            try
            {
                //StartWatch();
                OFLogger.Instance.LogDebug("Plugin is loading...");
                //outlookFormManager.ADXFolderSwitchEx += OutlookFormManagerOnAdxFolderSwitchEx;
                OFRegistryHelper.Instance.ResetShutdownNotification();
                new OFAppEmpty();
                if (System.Windows.Application.Current != null)
                {
                    // to avoid Application was shutdown exception (WALLBASH)
                    System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                    System.Windows.Application.Current.DispatcherUnhandledException += WPFApplicationOnDispatcherUnhandledException;
                }

                if (_updatable == null)
                {
                    _updatable = OFUpdateHelper.Instance;
                    //_updatable.Module = this;
                }
                //StopWatch("Init");
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        // TODO: change EventArgs to something particular
        public void FolderSwitch(object sender, EventArgs args)
        {
            try
            {
                //var folder = args.FolderObj as Outlook.Folder;
                //_mailRemovingManager.ConnectTo(folder);
                if (IsLoading)
                    return;
                HideSidebarDuringSwitching();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void HideSidebarDuringSwitching()
        {
            var app = (OFAppEmpty)System.Windows.Application.Current;
            if (app.IsNull())
                return;

            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    _sidebarForm = GetSidebarForm();
                    if (!IsMainUIVisible)
                    {
                        HideUi(false);
                    }
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
            }));
        }

        private void RunPluginUI()
        {
            try
            {

                //StartWatch();
                OFDllPreloader.Instance.PreloadDll();
                _wsuiBootStraper = new PluginBootStraper();
                _wsuiBootStraper.Run();
                _eventAggregator = _wsuiBootStraper.Container.Resolve<IEventAggregator>();
                ((OFAppEmpty)System.Windows.Application.Current).MainControl = (System.Windows.Controls.UserControl)_wsuiBootStraper.View;
                if (_wsuiBootStraper.View is IWSMainControl)
                {
                    (_wsuiBootStraper.View as IWSMainControl).Close += (o, e) =>
                    {
                        HideUi(false);
                        if (_commandManager != null)
                            _commandManager.SetShowHideButtonsEnabling(true, false);
                    };
                }
                CreateCommandManager();
                CreateEmailSuggesterManager();
                SetEventAggregatorToManager();
                SubscribeToEvents();
                LogVersions();

                IsLoading = false;
                //StopWatch("RunPluginUI");

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void LogVersions()
        {
            var currentOFVersion = typeof(OFAddinModule).Assembly.GetName().Version;
            OFLogger.Instance.LogInfo("OF Version: {0}", currentOFVersion);
            OFLogger.Instance.LogInfo("OS Version: {0}", Environment.OSVersion);
            OFLogger.Instance.LogInfo("OS x64 Version: {0}", Environment.Is64BitOperatingSystem);
            OFLogger.Instance.LogInfo("Outlook x64 Version: {0}", Environment.Is64BitProcess);

        }

        private ISidebarForm GetSidebarForm()
        {
            try
            {
                ISidebarForm form = null;
                //for (int i = 0; i < formRightSidebar.FormInstanceCount; i++)
                //{
                //    form = formRightSidebar.FormInstances(i) as ISidebarForm;
                //    if (form != null)
                //    {
                //        break;
                //    }
                //}
                return form;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return null;
        }

        private void SubscribeToEvents()
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<OFShowHideWindow>().Subscribe(ShowUi);
            _eventAggregator.GetEvent<OFHideWindow>().Subscribe(HideUi);
            _eventAggregator.GetEvent<OFSearch>().Subscribe(StartSearch);
            _eventAggregator.GetEvent<OFShowFolder>().Subscribe(ShowOutlookFolder);
            _eventAggregator.GetEvent<OFSuggestedEmailEvent>().Subscribe(OnSuggestedEmail);
        }
        private void OnSuggestedEmail(OFSuggestedEmailPayload ofSuggestedEmailPayload)
        {
            if (_emailSuggesterManager.IsNull() || ofSuggestedEmailPayload.IsNull() || ofSuggestedEmailPayload.Data.IsNull())
            {
                return;
            }
            _emailSuggesterManager.SuggestedEmail(ofSuggestedEmailPayload.Data);
        }

        private void ShowOutlookFolder(string s)
        {
            if (string.IsNullOrEmpty(s))
                return;
            var folder = GetFolderByFullPath(s);
            if (folder != null)
            {
                Outlook.Explorer activeExplorer = null;
                try
                {
                    activeExplorer = (OutlookApp as Outlook._Application).ActiveExplorer();
                    SetExplorerFolder(activeExplorer, folder);
                }
                finally
                {
                    if (activeExplorer != null)
                        Marshal.ReleaseComObject(activeExplorer);
                    if (folder != null)
                        Marshal.ReleaseComObject(folder);
                }
            }
        }

        private void ShowUi(bool show)
        {
            try
            {
                //StartWatch();
                _sidebarForm = GetSidebarForm();
                if (_sidebarForm != null && !_sidebarForm.IsDisposed)
                {
                    _sidebarForm.Show();
                    _wsuiBootStraper.PassAction(new OFAction(OFActionType.Show, null));
                    IsMainUIVisible = true;
                    OFRegistryHelper.Instance.SetIsPluginUiVisible(IsMainUIVisible);
                }
                //StopWatch("ShowUi");
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void HideUi(bool hide)
        {
            try
            {
                //StartWatch();
                _sidebarForm = GetSidebarForm();
                if (_sidebarForm != null && !_sidebarForm.IsDisposed)
                {
                    _sidebarForm.Hide();
                    IsMainUIVisible = false;
                    OFRegistryHelper.Instance.SetIsPluginUiVisible(IsMainUIVisible);
                }
                //StopWatch("HideUi");
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void StartSearch(string criteria)
        {
            try
            {
                _sidebarForm = GetSidebarForm();
                PassSearchActionToSearchEngine(criteria);
                if (!IsMainUIVisible)
                {
                    if (!_sidebarForm.IsDisposed)
                        _sidebarForm.Show();
                    ShowUi(true);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void CreateEmailSuggesterManager()
        {
            _emailSuggesterManager = new OFEmailSuggesterManager(_wsuiBootStraper, _eventAggregator);
        }

        //TODO: this will be ractored when I will add command bars
        private void CreateCommandManager()
        {
            //try
            //{
            //    _commandManager = adxMainPluginCommandBar.UseForRibbon
            //                          ? (IOFCommandManager)new OFCommandBarManager(buttonShow2007, buttonHide2007, adxCommandBarEditSearchText,
            //                              adxCommandBarButtonSearch)
            //                          : new OFRibbonManager(buttonShow, wsuiButtonSwitch, adxRibbonButtonSearch, adxRibbonEditBoxSearch, wsuiHomeSearch, wsuiButtonSearch, adxSendLogOFTab);
            //    if (!adxMainPluginCommandBar.UseForRibbon)
            //    {
            //        _aboutCommandManager = new OFMainRibbonCommandManager(btnHelp, btnAbout, mnuSettings, btnMainHelp, btnMainAbout, mnuMainSettings, adxSendLogMain);
            //    }
            //    else if (OFRegistryHelper.Instance.GetIsPluginUiVisible())
            //    {
            //        _commandManager.SetShowHideButtonsEnabling(false, true);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    OFLogger.Instance.LogError(ex.ToString());
            //}
        }

        private void SetEventAggregatorToManager()
        {
            try
            {
                if (_eventAggregator == null)
                    return;
                var manager = _commandManager as ICommandManager;
                if (manager != null)
                {
                    manager.SetEventAggregator(_eventAggregator);
                }
                if (_aboutCommandManager != null)
                {
                    _aboutCommandManager.SetEventAggregator(_eventAggregator);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        //#region Outlook Object Model routines

        //private enum OutlookPane
        //{
        //    olFolderList = 2,
        //    olNavigationPane = 4,
        //    olOutlookBar = 1,
        //    olPreview = 3,
        //    olToDoPane = 5
        //}


        private void SetExplorerFolder(Outlook.Explorer Explorer, Outlook.MAPIFolder Folder)
        {
            try
            {
                if (_outlookVersion == 2000 || _outlookVersion == 2002)
                    Explorer.CurrentFolder = Folder;
                else
                    Explorer.SelectFolder(Folder);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        //#endregion Outlook Object Model routines

        public void AddInStartupComplete(object sender, EventArgs e)
        {
            try
            {
                //StartWatch();
                //RestoreOutlookFolder();
                //CheckUpdate(); // TODO: just for testing

                //this.SendMessage(WM_LOADED, IntPtr.Zero, IntPtr.Zero);
                OutlookPreviewHelper.Instance.OutlookApp = OutlookApp;
                OFOutlookHelper.Instance.OutlookApp = OutlookApp;
                _mailRemovingManager.Initialize();
                OFLogger.Instance.LogInfo("OF AddinModule Startup Complete...");
                //StopWatch("OFAddinModule_AddinStartupComplete");
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OFLogger.Instance.LogDebug("Domain Unhandled Exception (plugin): {0}", e.ExceptionObject.ToString());
        }

        private void CurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs firstChanceExceptionEventArgs)
        {
            if (firstChanceExceptionEventArgs.Exception is ReflectionTypeLoadException)
            {
                foreach (var item in (firstChanceExceptionEventArgs.Exception as ReflectionTypeLoadException).LoaderExceptions)
                {
                    OFLogger.Instance.LogError("Reflection Type Load: {0}", item.ToString());
                }
            }
            OFLogger.Instance.LogDebug("Domain Exception: {0}", firstChanceExceptionEventArgs.Exception.ToString());
        }


        private void WPFApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs dispatcherUnhandledExceptionEventArgs)
        {
            OFLogger.Instance.LogDebug("WPF Exception: {0}", dispatcherUnhandledExceptionEventArgs.Exception.ToString());
        }

        //#region [event handlers for ribbon]

        private void PassSearchActionToSearchEngine(string text)
        {
            try
            {
                if (string.IsNullOrEmpty(text) || _wsuiBootStraper.IsPluginBusy)
                    return;
                if (!IsMainUIVisible)
                {
                    ShowMainUiAfterSearchAction();
                }
                _wsuiBootStraper.PassAction(new OFAction(OFActionType.Search, text));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        //TODO: refatore after adding UI
        private void ShowMainUiAfterSearchAction()
        {

            //if (!adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
            //{
            //    buttonShow2007.Enabled = false;
            //    buttonHide2007.Enabled = true;
            //}
        }

        //#endregion [event handlers for ribbon]

        private void outlookFormManager_OnInitialize()
        {
            try
            {
                if ((OutlookApp != null) && (_outlookVersion == 0))
                {
                    string hostVersion = OutlookApp.Version;
                    if (hostVersion.StartsWith("9.0"))
                    {
                        _outlookVersion = 2000;
                        _officeVersion = "9.0";
                        GlobalConst.CurrentOutlookVersion = OutlookVersions.None;
                    }
                    if (hostVersion.StartsWith("10.0"))
                    {
                        _outlookVersion = 2002;
                        _officeVersion = "10.0";
                        GlobalConst.CurrentOutlookVersion = OutlookVersions.None;
                    }
                    if (hostVersion.StartsWith("11.0"))
                    {
                        _outlookVersion = 2003;
                        _officeVersion = "11.0";
                        GlobalConst.CurrentOutlookVersion = OutlookVersions.None;
                    }
                    if (hostVersion.StartsWith("12.0"))
                    {
                        _outlookVersion = 2007;
                        _officeVersion = "12.0";
                        GlobalConst.CurrentOutlookVersion = OutlookVersions.Outlook2007;
                    }
                    if (hostVersion.StartsWith("14.0"))
                    {
                        _outlookVersion = 2010;
                        _officeVersion = "14.0";
                        GlobalConst.CurrentOutlookVersion = OutlookVersions.Outlook2010;
                    }
                    if (hostVersion.StartsWith("15.0"))
                    {
                        _outlookVersion = 2013;
                        _officeVersion = "15.0";
                        GlobalConst.CurrentOutlookVersion = OutlookVersions.Otlook2013;
                    }
                    OFLogger.Instance.LogInfo("Outlook Version: {0}, {1}, {2}", hostVersion, _outlookVersion, _officeVersion);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void Quit(object sender, EventArgs e)
        {
            FinalizeComponents();

            OFRegistryHelper.Instance.ResetLoadingAddinMode();
            OFRegistryHelper.Instance.ResetAdxStartMode();
            ResetLoadingTime();
            ResetAddIn();
            ResetDisabling();
            _mailRemovingManager.Dispose();
            _emailSuggesterManager.Dispose();
            if (_wsuiBootStraper != null)
            {
                _wsuiBootStraper.PassAction(new OFAction(OFActionType.Quit, null));
            }
#if DEBUG
            CheckAndCloseServiceApp();
#endif
            SetOutlookFolderProperties(string.Empty, string.Empty);
            OFLogger.Instance.LogInfo("Shutdown...");
        }

        // TODO: refactor after adding UI
        private void FinalizeComponents()
        {
            //if (adxMainPluginCommandBar.IsNotNull())
            //{
            //    adxMainPluginCommandBar.Dispose();
            //    adxCommandBarButtonSearch = null;
            //}
        }

        private void SetOutlookFolderProperties(string folderName, string folderWebUrl)
        {
            try
            {
                if (!string.IsNullOrEmpty(folderName))
                    OFRegistryHelper.Instance.SetOutlookFolderName(folderName);
                if (!string.IsNullOrEmpty(folderWebUrl))
                    OFRegistryHelper.Instance.SetOutlookFolderWebUrl(folderWebUrl);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        //public void RestoreOutlookFolder()
        //{
        //    try
        //    {
        //        if (OFRegistryHelper.Instance.IsShouldRestoreOutlookFolder())
        //        {
        //            OFLogger.Instance.LogDebug("{0}", "OutlookFolder is empty");
        //            return;
        //        }

        //        string id = OFRegistryHelper.Instance.GetOutllokFolderName();
        //        Outlook.NameSpace outlookNamespace = OutlookApp.GetNamespace(DefaultNamespace);
        //        if (outlookNamespace == null || string.IsNullOrEmpty(id))
        //            return;
        //        OFLogger.Instance.LogDebug("OutlookFolder ID: {0}", id);
        //        Outlook.MAPIFolder folder = outlookNamespace.GetFolderFromID(id, Type.Missing);
        //        if (folder == null)
        //            return;
        //        folder.WebViewURL = OFRegistryHelper.Instance.GetOutlookFolderWebUrl();
        //        folder.WebViewOn = true;

        //        OFLogger.Instance.LogDebug("WebViewURL: {0}", folder.WebViewURL);

        //        Marshal.ReleaseComObject(folder);
        //        Marshal.ReleaseComObject(outlookNamespace);
        //    }
        //    catch (Exception ex)
        //    {
        //        OFLogger.Instance.LogError(ex.ToString());
        //    }
        //}

        //// TODO: refactore and add support key events after adding hooks
        private void KeyDown(object sender, EventArgs e)
        {
            try
            {
                //_sidebarForm = GetSidebarForm();
                //if (_sidebarForm == null)
                //    return;
                //if (e.Ctrl && e.VirtualKey == (int)Keys.C)
                //{
                //    _sidebarForm.SendAction(OFActionType.Copy);
                //}

                //if (System.Windows.Application.Current != null)
                //{
                //    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                //    {
                //        if (_emailSuggesterManager.IsNotNull() && OFRegistryHelper.Instance.CheckAutoCompleateState())
                //        {
                //            System.Diagnostics.Debug.WriteLine($"Key Down: {Enum.GetName(typeof(Keys), e.VirtualKey)}");
                //            _emailSuggesterManager.ProcessKeyDown(e);
                //        }
                //    }));
                //}
                //if (e.VirtualKey == (int)Keys.Escape && _emailSuggesterManager.IsSuggestWindowVisible())
                //{
                //    e.Handled = true;
                //}
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private Outlook.MAPIFolder GetFolderByFullPath(string folderPath)
        {
            try
            {
                Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
                Outlook.MAPIFolder fld = null;
                foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
                {
                    fld = GetOutlookFolders(folder, folderPath);
                    if (fld != null)
                        break;
                    Marshal.ReleaseComObject(folder);
                }
                return fld;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return null;
        }

        private Outlook.MAPIFolder GetOutlookFolders(Outlook.MAPIFolder folder, string fullpath)
        {
            try
            {
                if (folder.FullFolderPath == fullpath)
                    return folder;
                Outlook.MAPIFolder mapiFolder = null;
                foreach (var subfolder in folder.Folders.OfType<Outlook.MAPIFolder>())
                {
                    try
                    {
                        mapiFolder = GetOutlookFolders(subfolder, fullpath);
                        if (mapiFolder != null)
                            break;
                        Marshal.ReleaseComObject(subfolder);
                    }
                    catch (Exception e)
                    {
                        OFLogger.Instance.LogError(string.Format("{0} {1}", subfolder.Name, e.Message));
                    }
                }
                return mapiFolder;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return null;
        }

        private void OutlookFinderEvents_NewExplorer(object sender, object explorer)
        {
            //StartWatch();
            var exp = explorer as Outlook._Explorer;
            if (IsLoading && exp != null)
            {
                _initHashCode = exp.GetHashCode();
            }
            //StopWatch("OutlookFinderEvents_NewExplorer");
        }

        //private void wsuiTab_PropertyChanging(object sender, ADXRibbonPropertyChangingEventArgs e)
        //{
        //    if (IsMainUIVisible && e.PropertyType == ADXRibbonControlPropertyType.Visible && e.Context.GetHashCode() != _initHashCode)
        //    {
        //        e.Value = false;
        //    }
        //}

        //private void wsuiMainGroup_PropertyChanging(object sender, ADXRibbonPropertyChangingEventArgs e)
        //{
        //    if (IsMainUIVisible && e.PropertyType == ADXRibbonControlPropertyType.Visible && e.Context.GetHashCode() != _initHashCode)
        //    {
        //        e.Value = false;
        //    }
        //}

        //private void StartWatch()
        //{
        //    (_watch = new Stopwatch()).Start();
        //}

        //private void StopWatch(string method)
        //{
        //    _watch.Stop();
        //    OFLogger.Instance.LogDebug("--------------- {0} => {1}", method, _watch.ElapsedMilliseconds);
        //}


        private void ExplorerSelectionChange(object sender, object explorer)
        {
            ConnectToSelectedItem(explorer);
        }

        private void ResetLoadingTime()
        {
            const string AddInLoadTimesKey = "Software\\Microsoft\\Office\\{0}\\Outlook\\AddInLoadTimes";
            const string ModuleKey = "OFOutlookPlugin.AddinModule";

            var CurrentOulookVersion = string.Format(AddInLoadTimesKey, _officeVersion);
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (key.IsNull())
                    return;
                var value = (byte[])key.GetValue(ModuleKey, null, RegistryValueOptions.DoNotExpandEnvironmentNames);
                if (value.IsNull())
                    return;
                var buffer = new byte[value.Length];
                key.SetValue(ModuleKey, buffer, RegistryValueKind.Binary);
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void ResetAddIn()
        {
            const string AddInLoadTimesKey = "Software\\Microsoft\\Office\\{0}\\Outlook\\AddIns\\OFOutlookPlugin.AddinModule";
            var CurrentOulookVersion = string.Format(AddInLoadTimesKey, _officeVersion);

            try
            {
                var key = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (key.IsNull())
                    return;

                foreach (var valueName in key.GetValueNames())
                {
                    var value = key.GetValue(valueName, null, RegistryValueOptions.DoNotExpandEnvironmentNames) as byte[];
                    if (value.IsNull())
                        return;
                    var buffer = new byte[value.Length];
                    key.SetValue(valueName, buffer, RegistryValueKind.Binary);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void ResetDisabling()
        {
            const string DisableKey = "Software\\Microsoft\\Office\\{0}\\Outlook\\Resiliency\\DisabledItems";
            var CurrentOulookVersion = string.Format(DisableKey, _officeVersion);
            RegistryKey registry = null;
            try
            {
                registry = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (registry != null)
                {

                    foreach (var item in registry.GetValueNames())
                    {
                        var val =
                            (byte[])registry.GetValue(item, null, RegistryValueOptions.DoNotExpandEnvironmentNames);

                        var temp = System.Text.Encoding.Unicode.GetString(val);
                        if (temp.Length > 0 && temp.IndexOf(ProgIdDefault) > -1)
                        {
                            registry.DeleteValue(item);
                        }
                        OFLogger.Instance.LogDebug("Disabled Add-ins: {0}; {1}", item, temp);
                    }
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            finally
            {
                if (registry.IsNotNull())
                {
                    registry.Close();
                }
            }
        }


        private void ConnectToSelectedItem(object explorer)
        {
            Outlook.Selection sel = null;
            try
            {
                Outlook._Explorer expl = explorer as Outlook._Explorer;
                sel = expl.Selection;
                if (sel.Count == 1)
                {
                    var mail = sel[1] as Outlook.MailItem;
                    if (mail != null)
                    {
                        _mailRemovingManager.ConnectTo(mail);
                    }
                    else
                    {
                        if (mail != null)
                        {
                            Marshal.ReleaseComObject(mail);
                        }
                    }
                }
            }
            catch (COMException com)
            {
                OFLogger.Instance.LogDebug("ErrorCode: {0}; Message: {1}", com.ErrorCode.GetErrorCode(), com.ToString());
            }
            finally
            {
                if (sel != null)
                {
                    Marshal.ReleaseComObject(sel);
                }
            }
        }

        private void OutlookFinderEvents_ExplorerClose(object sender, object explorer)
        {
            int count = 0;
            Outlook._Explorers expls = null;
            try
            {
                expls = OutlookApp.Explorers;
                count = expls.Count;
            }
            finally
            {
                if (expls != null)
                {
                    Marshal.ReleaseComObject(expls);
                }
            }
            if (count == 0)
            {
                _canConnect = false;
            }
        }

        private void OutlookFinderEvents_ExplorerActivate(object sender, object explorer)
        {
            ConnectToSelectedItem(explorer);
        }

        private void OutlookFinderEvents_InspectorClose(object sender, object inspector, string folderName)
        {
            StopProcessSuggestings();
        }

        private void StopProcessSuggestings()
        {
            if (_emailSuggesterManager.IsNotNull())
            {
                _emailSuggesterManager.UnsubscribeMailWindow();
                _wsuiBootStraper.PassAction(new OFAction(OFActionType.HideSuggestEmail, null));
            }
        }

        private void CheckAndStartServiceApp()
        {
#if !DEBUG
            int count = Process.GetProcesses().Count(p => p.ProcessName.ToUpperInvariant().Contains(SERVICE_APP));
            if (count > 0)
            {
                return;
            }
            try
            {
                string filename = string.Format("{0}\\{1}", Path.GetDirectoryName(typeof(OFAddinModule).Assembly.Location), "serviceapp.exe");
                if (File.Exists(filename))
                {
                    ProcessStartInfo si = new ProcessStartInfo(filename);
                    Process.Start(si);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
#endif
        }

        private void CheckAndCloseServiceApp()
        {
            var app = Process.GetProcesses().Where(p => p.ProcessName.ToUpperInvariant().Contains(SERVICE_APP)).FirstOrDefault();
            if (app.IsNotNull())
            {
                app.Kill();
            }
        }

        private IntPtr GetOutlookWindowHandle()
        {
            var outlook =
                Process.GetProcesses()
                    .FirstOrDefault(p => p.ProcessName.ToUpperInvariant().Contains("outlook".ToUpperInvariant()));
            return outlook.IsNotNull() ? outlook.MainWindowHandle : IntPtr.Zero;
        }

        private void OutlookFinderEvents_ExplorerInlineResponse(object sender, object itemObject)
        {
            if (_emailSuggesterManager.IsNotNull())
            {
                _emailSuggesterManager.SubscribeMailWindow();
            }
        }

        private void OutlookFinderEvents_ExplorerInlineResponseEx(object sender, object itemObject, object sourceObject)
        {

        }

        private void OutlookFinderEvents_ExplorerInlineResponseCloseEx(object sender, object sourceObject)
        {
            StopProcessSuggestings();
        }
    }
}