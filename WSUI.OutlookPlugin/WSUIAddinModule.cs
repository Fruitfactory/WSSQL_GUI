﻿using System;
using System.Collections;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Forms;
using AddinExpress.MSO;
using WSPreview.PreviewHandler.Service.OutlookPreview;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUI.Infrastructure.Service.Interfaces;
using WSUIOutlookPlugin.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Globalization;
using System.Reflection;
using WSUI.Control;
using Microsoft.Win32;
using System.Diagnostics;
using AddinExpress.OL;
using WSUIOutlookPlugin.Core;
using ADXOlExplorerItemTypes = AddinExpress.OL.ADXOlExplorerItemTypes;

namespace WSUIOutlookPlugin
{
    /// <summary>
    ///   Add-in Express Add-in Module
    /// </summary>
    [GuidAttribute("E854FABB-353C-4B9A-8D18-F66E61F6FCA5"), ProgId("WSUIOutlookPlugin.AddinModule")]
    public class WSUIAddinModule : AddinExpress.MSO.ADXAddinModule
    {

        private int _outlookVersion = 0;
        private bool _refreshCurrentFolderExecuting = false;
        private IUpdatable _updatable = null;
        private IPluginBootStraper _wsuiBootStraper = null;
        private Outlook.MAPIFolder _lastMapiFolder;
        

        #region [const]
        
        private const string ADXHTMLFileName = "ADXOlFormGeneral.html";
        private const int WM_USER = 0x0400;
        private ADXOlExplorerCommandBar adxMainPluginCommandBar;
        private ADXCommandBarButton buttonShow2007;
        private ADXCommandBarButton buttonHide2007;
        private ADXRibbonGroup adxRibbonGroupSearch;
        private ADXRibbonEditBox adxRibbonEditBoxSearch;
        private ADXRibbonButton adxRibbonButtonSearch;
        private ADXRibbonBox adxRibbonBoxSearch;
        private ADXCommandBarEdit adxCommandBarEditSearchText;
        private ADXCommandBarButton adxCommandBarButtonSearch;
        private const int WM_LOADED = WM_USER + 1001;

        private const string DefaultNamespace = "MAPI";


        #endregion

        public WSUIAddinModule()
        {
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "false"; 
            Application.EnableVisualStyles();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            InitializeComponent();
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("InitializeComponent [ctor]: {0}ms",watch.ElapsedMilliseconds));
            // Please add any initialization code to the AddinInitialize event handler
            (watch = new Stopwatch()).Start();
            Init();
            watch.Stop();
            this.OnSendMessage += WSUIAddinModule_OnSendMessage;
            this.AddinInitialize += OnAddinInitialize;
            WSSqlLogger.Instance.LogInfo(string.Format("WSUIAddinModule [ctor]: {0}ms", watch.ElapsedMilliseconds));
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;
            
        }

        private void OnAddinInitialize(object sender, EventArgs eventArgs)
        {
            if (adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
            {
                adxMainPluginCommandBar.UseForRibbon = false;
                this.adxRibbonEditBoxSearch.OnChange +=
                    new AddinExpress.MSO.ADXRibbonOnActionChange_EventHandler(this.adxRibbonEditBoxSearch_OnChange);
                this.adxRibbonButtonSearch.OnClick += AdxRibbonButtonSearchOnOnClick;
            }
            else
            {
                // for outlook 2007 and less
                adxCommandBarEditSearchText.Change += AdxCommandBarEditSearchTextOnChange;
                adxCommandBarButtonSearch.Click += AdxCommandBarButtonSearchOnClick;
            }
        }

        private void WSUIAddinModule_OnSendMessage(object sender, ADXSendMessageEventArgs e)
        {
            switch(e.Message)
            {
                case WM_LOADED:
                    lock (this)
                    {
                        RunPluginUI();
                    }
                    break;
            }
        }

        private AddinExpress.OL.ADXOlFormsManager outlookFormManager;
        private AddinExpress.OL.ADXOlFormsCollectionItem formWebPaneItem;
        private ADXRibbonTab wsuiTab;
        private ADXRibbonGroup managingCtrlGroup;
        private ADXRibbonButton buttonShow;
        private ADXRibbonButton buttonClose;
        
 
        #region Component Designer generated code
        /// <summary>
        /// Required by designer
        /// </summary>
        private System.ComponentModel.IContainer components;
 
        /// <summary>
        /// Required by designer support - do not modify
        /// the following method
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.outlookFormManager = new AddinExpress.OL.ADXOlFormsManager(this.components);
            this.formWebPaneItem = new AddinExpress.OL.ADXOlFormsCollectionItem(this.components);
            this.wsuiTab = new AddinExpress.MSO.ADXRibbonTab(this.components);
            this.managingCtrlGroup = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.buttonShow = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.buttonClose = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonGroupSearch = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.adxRibbonBoxSearch = new AddinExpress.MSO.ADXRibbonBox(this.components);
            this.adxRibbonEditBoxSearch = new AddinExpress.MSO.ADXRibbonEditBox(this.components);
            this.adxRibbonButtonSearch = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxMainPluginCommandBar = new AddinExpress.MSO.ADXOlExplorerCommandBar(this.components);
            this.buttonShow2007 = new AddinExpress.MSO.ADXCommandBarButton(this.components);
            this.buttonHide2007 = new AddinExpress.MSO.ADXCommandBarButton(this.components);
            this.adxCommandBarEditSearchText = new AddinExpress.MSO.ADXCommandBarEdit(this.components);
            this.adxCommandBarButtonSearch = new AddinExpress.MSO.ADXCommandBarButton(this.components);
            // 
            // outlookFormManager
            // 
            this.outlookFormManager.Items.Add(this.formWebPaneItem);
            this.outlookFormManager.OnInitialize += new AddinExpress.OL.ADXOlFormsManager.OnComponentInitialize_EventHandler(this.outlookFormManager_OnInitialize);
            this.outlookFormManager.SetOwner(this);
            // 
            // formWebPaneItem
            // 
            this.formWebPaneItem.Cached = AddinExpress.OL.ADXOlCachingStrategy.OneInstanceForAllFolders;
            this.formWebPaneItem.ExplorerLayout = AddinExpress.OL.ADXOlExplorerLayout.WebViewPane;
            this.formWebPaneItem.FormClassName = "WSUIOutlookPlugin.WSUIForm";
            this.formWebPaneItem.IsMinimizedStateAllowed = false;
            this.formWebPaneItem.RegionBorder = AddinExpress.OL.ADXRegionBorderStyle.None;
            this.formWebPaneItem.UseOfficeThemeForBackground = true;
            // 
            // wsuiTab
            // 
            this.wsuiTab.Caption = "Windows Search";
            this.wsuiTab.Controls.Add(this.managingCtrlGroup);
            this.wsuiTab.Controls.Add(this.adxRibbonGroupSearch);
            this.wsuiTab.Id = "adxRibbonTab_500b5beadf3a45d9b11245e305940d6c";
            this.wsuiTab.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // managingCtrlGroup
            // 
            this.managingCtrlGroup.Caption = "Managing";
            this.managingCtrlGroup.Controls.Add(this.buttonShow);
            this.managingCtrlGroup.Controls.Add(this.buttonClose);
            this.managingCtrlGroup.Id = "adxRibbonGroup_8837e4bd15814fb2bef7a21b2d8784a3";
            this.managingCtrlGroup.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.managingCtrlGroup.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // buttonShow
            // 
            this.buttonShow.Caption = "Show Windows  Search";
            this.buttonShow.Id = "adxRibbonButton_dcb0aa6e6fd442c79ea44b4006d84643";
            this.buttonShow.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonShow.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // buttonClose
            // 
            this.buttonClose.Caption = "Close Windows Search";
            this.buttonClose.Id = "adxRibbonButton_28c7fe480c61454ca13d1a20e9ae3405";
            this.buttonClose.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonClose.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // adxRibbonGroupSearch
            // 
            this.adxRibbonGroupSearch.Caption = "Search";
            this.adxRibbonGroupSearch.Controls.Add(this.adxRibbonBoxSearch);
            this.adxRibbonGroupSearch.Id = "adxRibbonGroup_c94173390d39441fa25cda51a851cd7a";
            this.adxRibbonGroupSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonGroupSearch.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // adxRibbonBoxSearch
            // 
            this.adxRibbonBoxSearch.Controls.Add(this.adxRibbonEditBoxSearch);
            this.adxRibbonBoxSearch.Controls.Add(this.adxRibbonButtonSearch);
            this.adxRibbonBoxSearch.Id = "adxRibbonBox_dc294efeac8340e788f9e0ebd39ab866";
            this.adxRibbonBoxSearch.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // adxRibbonEditBoxSearch
            // 
            this.adxRibbonEditBoxSearch.Caption = "Enter search string:";
            this.adxRibbonEditBoxSearch.Id = "adxRibbonEditBox_decd82b7448b4bbe9853a4a4dee51263";
            this.adxRibbonEditBoxSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonEditBoxSearch.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            this.adxRibbonEditBoxSearch.SizeString = "The size of this string is the size of the edit box";
            // 
            // adxRibbonButtonSearch
            // 
            this.adxRibbonButtonSearch.Caption = "Start Search";
            this.adxRibbonButtonSearch.Id = "adxRibbonButton_f0ca75ee177a4886a26d3c9246518516";
            this.adxRibbonButtonSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonButtonSearch.ParseMsoXmlTypeAs = AddinExpress.MSO.ADXParseMsoXmlTypeAs.pxtControl;
            this.adxRibbonButtonSearch.Ribbons = ((AddinExpress.MSO.ADXRibbons)(((AddinExpress.MSO.ADXRibbons.msrOutlookMailRead | AddinExpress.MSO.ADXRibbons.msrOutlookMailCompose) 
            | AddinExpress.MSO.ADXRibbons.msrOutlookExplorer)));
            // 
            // adxMainPluginCommandBar
            // 
            this.adxMainPluginCommandBar.CommandBarName = "WSUIPluginCommandBar";
            this.adxMainPluginCommandBar.CommandBarTag = "674128a0-9ce1-485c-b1a5-f5ff6897bfc8";
            this.adxMainPluginCommandBar.Controls.Add(this.buttonShow2007);
            this.adxMainPluginCommandBar.Controls.Add(this.buttonHide2007);
            this.adxMainPluginCommandBar.Controls.Add(this.adxCommandBarEditSearchText);
            this.adxMainPluginCommandBar.Controls.Add(this.adxCommandBarButtonSearch);
            this.adxMainPluginCommandBar.Temporary = true;
            this.adxMainPluginCommandBar.UpdateCounter = 16;
            // 
            // buttonShow2007
            // 
            this.buttonShow2007.Caption = "Show Windows Search";
            this.buttonShow2007.ControlTag = "e314f48a-f1c6-4e22-9e96-2f526c649798";
            this.buttonShow2007.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonShow2007.Temporary = true;
            this.buttonShow2007.UpdateCounter = 6;
            this.buttonShow2007.Click += new AddinExpress.MSO.ADXClick_EventHandler(this.buttonShow2007_Click);
            // 
            // buttonHide2007
            // 
            this.buttonHide2007.Caption = "Close Windows Search";
            this.buttonHide2007.ControlTag = "2a1559ac-9958-47f2-bae7-141679f598e8";
            this.buttonHide2007.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonHide2007.Temporary = true;
            this.buttonHide2007.UpdateCounter = 4;
            this.buttonHide2007.Click += new AddinExpress.MSO.ADXClick_EventHandler(this.buttonHide2007_Click);
            // 
            // adxCommandBarEditSearchText
            // 
            this.adxCommandBarEditSearchText.BeginGroup = true;
            this.adxCommandBarEditSearchText.Caption = "Enter search string:";
            this.adxCommandBarEditSearchText.ControlTag = "84fc0dcb-c517-428b-a63f-73b7340c923a";
            this.adxCommandBarEditSearchText.Style = AddinExpress.MSO.ADXMsoComboStyle.adxMsoComboLabel;
            this.adxCommandBarEditSearchText.Temporary = true;
            this.adxCommandBarEditSearchText.UpdateCounter = 16;
            this.adxCommandBarEditSearchText.Width = 350;
            // 
            // adxCommandBarButtonSearch
            // 
            this.adxCommandBarButtonSearch.Caption = "Start search";
            this.adxCommandBarButtonSearch.ControlTag = "0c750b68-d191-4eec-9745-ff30dbac7360";
            this.adxCommandBarButtonSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxCommandBarButtonSearch.Temporary = true;
            this.adxCommandBarButtonSearch.UpdateCounter = 3;
            // 
            // WSUIAddinModule
            // 
            this.AddinName = "WS Plugin (Windows  Search)";
            this.HandleShortcuts = true;
            this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaOutlook;
            this.AddinStartupComplete += new AddinExpress.MSO.ADXEvents_EventHandler(this.WSUIAddinModule_AddinStartupComplete);

        }
        #endregion
 
        #region Add-in Express automatic code
 
        // Required by Add-in Express - do not modify
        // the methods within this region
 
        public override System.ComponentModel.IContainer GetContainer()
        {
            if (components == null)
                components = new System.ComponentModel.Container();
            return components;
        }
 
        [ComRegisterFunctionAttribute]
        public static void AddinRegister(Type t)
        {
            AddinExpress.MSO.ADXAddinModule.ADXRegister(t);
        }
 
        [ComUnregisterFunctionAttribute]
        public static void AddinUnregister(Type t)
        {
            AddinExpress.MSO.ADXAddinModule.ADXUnregister(t);
        }
 
        public override void UninstallControls()
        {
            base.UninstallControls();
        }

        #endregion

        public static new WSUIAddinModule CurrentInstance 
        {
            get
            {
                return AddinExpress.MSO.ADXAddinModule.CurrentInstance as WSUIAddinModule;
            }
        }

        public Outlook._Application OutlookApp
        {
            get
            {
                return (HostApplication as Outlook._Application);
            }
        }

        public IPluginBootStraper BootStraper
        {
            get { return _wsuiBootStraper; }
        }

        public bool IsMainUIVisible { get; set; }

        #region my own initialization

        private void Init()
        {
            WSSqlLogger.Instance.LogInfo("Plugin is loading...");
            outlookFormManager.ADXBeforeFolderSwitchEx += outlookFormManager_ADXBeforeFolderSwitchEx;
            buttonShow.OnClick += buttonShow_OnClick;
            buttonClose.OnClick += buttonClose_OnClick;
            buttonShow.Enabled = buttonShow2007.Enabled = true;
            buttonClose.Enabled = buttonHide2007.Enabled = false;

            if (System.Windows.Application.Current == null)
            {
                new AppEmpty();
                if (System.Windows.Application.Current != null)
                {
                    // to avoid Application was shutdown exception (WALLBASH)
                    System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                }
                    
            }
            if (_updatable == null)
            {
                _updatable = UpdateHelper.Instance;
                _updatable.Module = this;
            }
            
        }

        #endregion

        private void CheckUpdate()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (!ReferenceEquals(_updatable,null))
            {   
                _updatable.Update();    
            }
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("Check for update: {0}ms",watch.ElapsedMilliseconds));
        }

        private void RunPluginUI()
        {
            DllPreloader.Instance.PreloadDll();
            _wsuiBootStraper = new PluginBootStraper();
            _wsuiBootStraper.Run();
        }


        private void outlookFormManager_ADXBeforeFolderSwitchEx(object sender, AddinExpress.OL.BeforeFolderSwitchExEventArgs args)
        {
            if (!_refreshCurrentFolderExecuting)
            {
                if (args.SrcFolder != null)
                {
                    string folderName = GetFullFolderName(args.SrcFolder);

                    if (IsADXWebViewUrl((Outlook.MAPIFolder)args.SrcFolder)
                        && (CultureInfo.InvariantCulture.CompareInfo.IndexOf(folderName,
                        formWebPaneItem.FolderName,
                        System.Globalization.CompareOptions.IgnoreCase) == 0))
                    {
                        formWebPaneItem.FolderName = string.Empty;
                        ClearFolderWebViewProperties((Outlook.MAPIFolder)args.SrcFolder);
                        
                        if(!adxMainPluginCommandBar.UseForRibbon)
                            ApplyButtonsRibbonEnable();
                        else
                            ApplyCommandBarButtons();
                    }
                }
            }
        }

        public void DoShowWebViewPane()
        {
            string currentFullFolderName = GetFullPathOfDefaultFolder();
            formWebPaneItem.FolderName = currentFullFolderName;
            //Apply the WebViewPane layout
            RefreshCurrentFolder();
        }

        public void DoHideWebViewPane()
        {
            if (ExistsVisibleForm(formWebPaneItem))
            {
                Outlook.MAPIFolder currentFolder = GetCurrentFolder();
                if (currentFolder != null)
                    try
                    {
                        formWebPaneItem.FolderName = string.Empty;
                        ClearFolderWebViewProperties(currentFolder);
                        //Restore Standard View
                        RefreshCurrentFolder(false);
                    }
                    finally
                    {
                        Marshal.ReleaseComObject(currentFolder);
                    }
            }
        }

        #region Outlook Object Model routines

        private enum OutlookPane
        {
            olFolderList = 2,
            olNavigationPane = 4,
            olOutlookBar = 1,
            olPreview = 3,
            olToDoPane = 5
        }

        private Outlook.MAPIFolder GetCurrentFolder()
        {
            Outlook.Explorer activeExplorer = (OutlookApp as Outlook._Application).ActiveExplorer();
            if (activeExplorer != null)
                try
                {
                    return activeExplorer.CurrentFolder;
                }
                finally
                {
                    Marshal.ReleaseComObject(activeExplorer);
                }
            return null;
        }

        private Outlook.MAPIFolder GetDefaultFolder()
        {
            Outlook.NameSpace outlookNqamespace = OutlookApp.GetNamespace(DefaultNamespace);
            return outlookNqamespace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
        }

        private string GetFullPathOfDefaultFolder()
        {
            var folder = GetDefaultFolder();
            return folder != null ?  GetFullFolderName(folder) : string.Empty;
        }

        private void RefreshCurrentFolder(bool isOpenUI = true)
        {
            _refreshCurrentFolderExecuting = true;
            try
            {
                Outlook.Explorer activeExplorer = (OutlookApp as Outlook._Application).ActiveExplorer();
                Outlook.MAPIFolder currentFolder = isOpenUI ? GetDefaultFolder() : _lastMapiFolder;//activeExplorer.CurrentFolder;
                Outlook.NameSpace nameSpace = (OutlookApp as Outlook._Application).GetNamespace(DefaultNamespace);
                Outlook.MAPIFolder outboxFolder = nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderOutbox);
                _lastMapiFolder = isOpenUI ? activeExplorer.CurrentFolder : null;
                try
                {
                    SetExplorerFolder(activeExplorer, outboxFolder);
                    Application.DoEvents();
                    SetExplorerFolder(activeExplorer, currentFolder);
                }
                finally
                {
                    if (nameSpace != null)
                        Marshal.ReleaseComObject(nameSpace);
                    if (currentFolder != null)
                        Marshal.ReleaseComObject(currentFolder);
                    if (outboxFolder != null)
                        Marshal.ReleaseComObject(outboxFolder);
                    if (activeExplorer != null)
                        Marshal.ReleaseComObject(activeExplorer);
                }
            }
            finally
            {
                _refreshCurrentFolderExecuting = false;
            }
        }

        private void SetExplorerFolder(Outlook.Explorer Explorer, Outlook.MAPIFolder Folder)
        {
            try
            {
                if (_outlookVersion == 2000 || _outlookVersion == 2002 )//|| _outlookVersion == 2007 || _outlookVersion == 2010
                    Explorer.CurrentFolder = Folder;
                else
                    Explorer.GetType().InvokeMember("SelectFolder", BindingFlags.InvokeMethod, null, Explorer, new object[] { Folder });
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("SetExplorerFolder");
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            
        }

        private void ClearFolderWebViewProperties(Outlook.MAPIFolder Folder)
        {
            if (Folder != null)
            {
                Folder.WebViewURL = string.Empty;
                Folder.WebViewOn = false;
            }
        }

        private bool IsExplorerPaneVisible(object Explorer, OutlookPane OlPane)
        {
            return Convert.ToBoolean(Explorer.GetType().InvokeMember("IsPaneVisible", System.Reflection.BindingFlags.InvokeMethod, null, Explorer, new object[] { (int)OlPane }));
        }

        private string GetFullFolderName(object FolderObj)
        {
            if (FolderObj == null)
                return string.Empty;

            string fullName = String.Empty;

            if (_outlookVersion == 2007 || _outlookVersion == 2003)
            {
                try
                {
                    fullName = Convert.ToString(FolderObj.GetType().InvokeMember("FolderPath", BindingFlags.GetProperty, null, FolderObj, null));
                }
                catch (Exception err)
                {
                    MessageBox.Show("GetFullFolderName.FolderPath Error: " + err.Message,
                        "Switching WebViewPane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                try
                {
                    object _folder = null;
                    object folderObj = null;
                    IntPtr ifolder = new IntPtr();
                    string tmp;
                    Guid folderGuid = new Guid("00063006-0000-0000-C000-000000000046");

                    IntPtr unk = Marshal.GetIUnknownForObject(FolderObj);
                    if (unk == IntPtr.Zero) return String.Empty;
                    try
                    {
                        folderObj = Marshal.GetObjectForIUnknown(unk);
                        _folder = folderObj;
                        Marshal.QueryInterface(unk, ref folderGuid, out ifolder);
                        Marshal.Release(unk);
                        while (ifolder != IntPtr.Zero)
                        {
                            Marshal.Release(ifolder); ifolder = IntPtr.Zero;
                            tmp = Convert.ToString(folderObj.GetType().InvokeMember("Name", BindingFlags.GetProperty, null, folderObj, null));
                            fullName = "\\" + tmp + fullName;
                            try
                            {
                                _folder = folderObj.GetType().InvokeMember("Parent", BindingFlags.GetProperty, null, folderObj, null);
                            }
                            catch
                            {
                                _folder = null;
                            }
                            finally
                            {
                                Marshal.ReleaseComObject(folderObj);
                                folderObj = null;
                            }
                            if (_folder != null)
                            {
                                unk = Marshal.GetIUnknownForObject(_folder);
                                Marshal.QueryInterface(unk, ref folderGuid, out ifolder);
                                Marshal.Release(unk);
                                folderObj = _folder;
                            }
                        }
                    }
                    finally
                    {
                        if (folderObj != null)
                            Marshal.ReleaseComObject(folderObj);
                    }
                }
                catch (Exception err)
                {
                    MessageBox.Show("GetFullFolderName error: " + err.Message,
                        "Switching WebViewPane", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            while (fullName != String.Empty && fullName[0] == '\\')
            {
                fullName = fullName.Remove(0, 1);
            }
            return fullName;
        }

        private Tuple<string,string> GetFullNameOfCurrentFolder()
        {
            Outlook.Explorer activeExplorer = (OutlookApp as Outlook._Application).ActiveExplorer();
            if (activeExplorer != null)
                try
                {
                    Outlook.MAPIFolder currentFolder = activeExplorer.CurrentFolder;
                    if (currentFolder != null)
                        try
                        {

                            return new Tuple<string, string>(currentFolder.EntryID, GetFullFolderName(currentFolder));
                        }
                        finally
                        {
                            Marshal.ReleaseComObject(currentFolder);
                        }
                }
                finally
                {
                    Marshal.ReleaseComObject(activeExplorer);
                }
            return default(Tuple<string,string>);
        }

        private bool IsADXWebViewUrl(Outlook.MAPIFolder folder)
        {
            if (folder == null || folder.WebViewURL == null || folder.WebViewURL.Length <= 0)
                return false;

            return (CultureInfo.InvariantCulture.CompareInfo.IndexOf(folder.WebViewURL,
                ADXHTMLFileName, System.Globalization.CompareOptions.IgnoreCase) > 0);
        }

        private bool ExistsVisibleForm(AddinExpress.OL.ADXOlFormsCollectionItem Item)
        {
            for (int i = 0; i < Item.FormInstanceCount; i++)
            {
                if (Item.FormInstances(i).Visible)
                    return true;
            }
            return false;
        }

        #endregion

        private void buttonShow_OnClick(object sender, IRibbonControl control, bool pressed)
        {
            DoShowWebViewPane();
            ApplyButtonsRibbonEnable(false);
        }

        private void buttonClose_OnClick(object sender, IRibbonControl control, bool pressed)
        {
            DoHideWebViewPane();
            ApplyButtonsRibbonEnable();
        }

        private void ApplyButtonsRibbonEnable(bool isShowEnable = true)
        {
            buttonShow.Enabled = isShowEnable;
            buttonClose.Enabled = !isShowEnable;
        }


        private void WSUIAddinModule_AddinStartupComplete(object sender, EventArgs e)
        {
            CheckUpdate();
            this.SendMessage(WM_LOADED,IntPtr.Zero,IntPtr.Zero);
            OutlookPreviewHelper.Instance.OutlookApp = OutlookApp;
            OutlookHelper.Instance.OutlookApp = OutlookApp;
            WSSqlLogger.Instance.LogInfo("WSUI AddinModule Startup Complete...");
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WSSqlLogger.Instance.LogError("Unhandled Exception (plugin): " + e.ExceptionObject.ToString());
        }

        private void CurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs firstChanceExceptionEventArgs)
        {
            WSSqlLogger.Instance.LogError("First Chance Exception (plugin): " + firstChanceExceptionEventArgs.Exception.Message);
        }

        private void buttonShow2007_Click(object sender)
        {
            DoShowWebViewPane();
            ApplyCommandBarButtons(false);
        }

        private void buttonHide2007_Click(object sender)
        {
            DoHideWebViewPane();
            ApplyCommandBarButtons();
        }

        private void ApplyCommandBarButtons(bool isOpenEnable = true)
        {
            buttonShow2007.Enabled = isOpenEnable;
            buttonHide2007.Enabled = !isOpenEnable;
        }

        
        #region [event handlers for ribbon]

        private void adxRibbonEditBoxSearch_OnChange(object sender, IRibbonControl Control, string text)
        {
            PassSearchActionToSearchEngine(text);
        }

        private void AdxRibbonButtonSearchOnOnClick(object sender, IRibbonControl control, bool pressed)
        {
            PassSearchActionToSearchEngine(adxRibbonEditBoxSearch.Text);
        }

        private void PassSearchActionToSearchEngine(string text)
        {
            if (string.IsNullOrEmpty(text))
                return;
            if (!WSUIAddinModule.CurrentInstance.IsMainUIVisible)
            {
                ShowMainUiAfterSearchAction();
            }
            _wsuiBootStraper.PassAction(new WSAction(WSActionType.Search, text));
        }

        private void ShowMainUiAfterSearchAction()
        {
            DoShowWebViewPane();
            buttonShow.Enabled = false;
            buttonClose.Enabled = true;
            if (!adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
            {
                buttonShow2007.Enabled = false;
                buttonHide2007.Enabled = true;    
            }
            
        }

        #endregion

        #region [event handlers for  command bar]

        private void AdxCommandBarButtonSearchOnClick(object sender)
        {
            PassSearchActionToSearchEngine(adxCommandBarEditSearchText.Text);
        }

        private void AdxCommandBarEditSearchTextOnChange(object sender)
        {
            if (!(sender is AddinExpress.MSO.ADXCommandBarEdit))
                return;
            var editBox = sender as AddinExpress.MSO.ADXCommandBarEdit;
            PassSearchActionToSearchEngine(editBox.Text);
        }

        #endregion 

        private void outlookFormManager_OnInitialize()
        {
            if ((OutlookApp != null) && (_outlookVersion == 0))
            {
                string hostVersion = OutlookApp.Version;
                if (hostVersion.StartsWith("9.0"))
                    _outlookVersion = 2000;
                if (hostVersion.StartsWith("10.0"))
                    _outlookVersion = 2002;
                if (hostVersion.StartsWith("11.0"))
                    _outlookVersion = 2003;
                if (hostVersion.StartsWith("12.0"))
                    _outlookVersion = 2007;
                if (hostVersion.StartsWith("14.0"))
                    _outlookVersion = 2010;
                if (hostVersion.StartsWith("15.0"))
                    _outlookVersion = 2013;
            }
        }

    }
}
