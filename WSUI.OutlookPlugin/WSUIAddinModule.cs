using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using AddinExpress.MSO;
using AddinExpress.OL;
using Microsoft.Office.Core;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using WSPreview.PreviewHandler.Service.OutlookPreview;
using WSUI.Control;
using WSUI.Control.Interfaces;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Events;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUI.Core.Extensions;
using WSUI.Infrastructure.Controls.Application;
using WSUIOutlookPlugin.Core;
using WSUIOutlookPlugin.Events;
using WSUIOutlookPlugin.Interfaces;
using WSUIOutlookPlugin.Managers;
using Application = System.Windows.Forms.Application;
using Outlook = Microsoft.Office.Interop.Outlook;

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
        private IEventAggregator _eventAggregator = null;
        private Outlook.MAPIFolder _lastMapiFolder;
        private IWSUICommandManager _commandManager;
        private ICommandManager _aboutCommandManager;
        private ISidebarForm _sidebarForm;
        private bool IsLoading = true;
        private int _initHashCode;
        private string _previewReferenceSelected = null;
        private Stopwatch _watch;

        #region [const]

        private const string ADXHTMLFileName = "ADXOlFormGeneral.html";
        private const int WM_USER = 0x0400;
        private const int WM_CLOSE = 0x0010;
        private const int WM_QUIT = 0x0012;
        private const int WM_DESTROY = 0x0002;
        private ADXOlExplorerCommandBar adxMainPluginCommandBar;

        // Outlook 2007
        private ADXCommandBarButton buttonShow2007;

        private ADXCommandBarButton buttonHide2007;
        private ADXCommandBarEdit adxCommandBarEditSearchText;
        private ADXCommandBarButton adxCommandBarButtonSearch;

        //Outlook 2010
        private ADXRibbonGroup groupSearch;

        private ADXRibbonEditBox adxRibbonEditBoxSearch;
        private ADXRibbonButton adxRibbonButtonSearch;
        private ADXRibbonBox adxRibbonBoxSearch;
        private ADXRibbonTab wsuiTab;
        private ADXRibbonButton buttonShow;
        private ADXRibbonTab wsuiMainTab;
        private ADXRibbonGroup wsuiMainGroup;
        private ADXRibbonButton wsuiButtonSwitch;
        private ADXRibbonEditBox wsuiHomeSearch;

        private const int WM_LOADED = WM_USER + 1001;
        private ImageList wsuiImageList;
        private ADXRibbonBox adxMainBox;
        private ADXRibbonButton wsuiButtonSearch;
        private ADXRibbonSplitButton btnSplit;
        private ADXRibbonMenu menuHelp;
        private ADXRibbonButton btnHelp;
        private ADXRibbonMenuSeparator adxRibbonMenuSeparator1;
        private ADXRibbonButton btnAbout;
        private ADXRibbonSplitButton btnMainSplit;
        private ADXRibbonMenu mnuMain;
        private ADXRibbonButton btnMainHelp;
        private ADXRibbonMenuSeparator btnMainMenuSeparatotr;
        private ADXRibbonButton btnMainAbout;
        private ADXOutlookAppEvents OutlookFinderEvents;
        private AddinExpress.OL.ADXOlFormsCollectionItem formRightSidebar;

        private const string DefaultNamespace = "MAPI";

        #endregion [const]

        public WSUIAddinModule()
        {
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "false";
            Application.EnableVisualStyles();
            InitializeComponent();
            Init();
            this.OnSendMessage += WSUIAddinModule_OnSendMessage;
            this.AddinInitialize += OnAddinInitialize;

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;
        }

        private void OnAddinInitialize(object sender, EventArgs eventArgs)
        {
            if (adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
            {
                adxMainPluginCommandBar.UseForRibbon = false;
            }
            //CreateInboxSubFolder((Outlook.Application)OutlookApp);
        }

        private void WSUIAddinModule_OnSendMessage(object sender, ADXSendMessageEventArgs e)
        {
            switch (e.Message)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WSUIAddinModule));
            this.outlookFormManager = new AddinExpress.OL.ADXOlFormsManager(this.components);
            this.formRightSidebar = new AddinExpress.OL.ADXOlFormsCollectionItem(this.components);
            this.wsuiTab = new AddinExpress.MSO.ADXRibbonTab(this.components);
            this.groupSearch = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.adxRibbonBoxSearch = new AddinExpress.MSO.ADXRibbonBox(this.components);
            this.adxRibbonEditBoxSearch = new AddinExpress.MSO.ADXRibbonEditBox(this.components);
            this.adxRibbonButtonSearch = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.buttonShow = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.wsuiImageList = new System.Windows.Forms.ImageList(this.components);
            this.btnSplit = new AddinExpress.MSO.ADXRibbonSplitButton(this.components);
            this.menuHelp = new AddinExpress.MSO.ADXRibbonMenu(this.components);
            this.btnHelp = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxRibbonMenuSeparator1 = new AddinExpress.MSO.ADXRibbonMenuSeparator(this.components);
            this.btnAbout = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxMainPluginCommandBar = new AddinExpress.MSO.ADXOlExplorerCommandBar(this.components);
            this.buttonShow2007 = new AddinExpress.MSO.ADXCommandBarButton(this.components);
            this.buttonHide2007 = new AddinExpress.MSO.ADXCommandBarButton(this.components);
            this.adxCommandBarEditSearchText = new AddinExpress.MSO.ADXCommandBarEdit(this.components);
            this.adxCommandBarButtonSearch = new AddinExpress.MSO.ADXCommandBarButton(this.components);
            this.wsuiMainTab = new AddinExpress.MSO.ADXRibbonTab(this.components);
            this.wsuiMainGroup = new AddinExpress.MSO.ADXRibbonGroup(this.components);
            this.adxMainBox = new AddinExpress.MSO.ADXRibbonBox(this.components);
            this.wsuiHomeSearch = new AddinExpress.MSO.ADXRibbonEditBox(this.components);
            this.wsuiButtonSearch = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.wsuiButtonSwitch = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.btnMainSplit = new AddinExpress.MSO.ADXRibbonSplitButton(this.components);
            this.mnuMain = new AddinExpress.MSO.ADXRibbonMenu(this.components);
            this.btnMainHelp = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.btnMainMenuSeparatotr = new AddinExpress.MSO.ADXRibbonMenuSeparator(this.components);
            this.btnMainAbout = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.OutlookFinderEvents = new AddinExpress.MSO.ADXOutlookAppEvents(this.components);
            // 
            // outlookFormManager
            // 
            this.outlookFormManager.Items.Add(this.formRightSidebar);
            this.outlookFormManager.OnInitialize += new AddinExpress.OL.ADXOlFormsManager.OnComponentInitialize_EventHandler(this.outlookFormManager_OnInitialize);
            this.outlookFormManager.SetOwner(this);
            // 
            // formRightSidebar
            // 
            this.formRightSidebar.Cached = AddinExpress.OL.ADXOlCachingStrategy.OneInstanceForAllFolders;
            this.formRightSidebar.DefaultRegionState = AddinExpress.OL.ADXRegionState.Hidden;
            this.formRightSidebar.ExplorerAllowedDropRegions = AddinExpress.OL.ADXOlExplorerAllowedDropRegions.DockRight;
            this.formRightSidebar.ExplorerItemTypes = ((AddinExpress.OL.ADXOlExplorerItemTypes)((((((((AddinExpress.OL.ADXOlExplorerItemTypes.olMailItem | AddinExpress.OL.ADXOlExplorerItemTypes.olAppointmentItem) 
            | AddinExpress.OL.ADXOlExplorerItemTypes.olContactItem) 
            | AddinExpress.OL.ADXOlExplorerItemTypes.olTaskItem) 
            | AddinExpress.OL.ADXOlExplorerItemTypes.olJournalItem) 
            | AddinExpress.OL.ADXOlExplorerItemTypes.olNoteItem) 
            | AddinExpress.OL.ADXOlExplorerItemTypes.olPostItem) 
            | AddinExpress.OL.ADXOlExplorerItemTypes.olDistributionListItem)));
            this.formRightSidebar.ExplorerLayout = AddinExpress.OL.ADXOlExplorerLayout.DockRight;
            this.formRightSidebar.FormClassName = "WSUIOutlookPlugin.WSUISidebar";
            this.formRightSidebar.IsMinimizedStateAllowed = false;
            this.formRightSidebar.RegionBorder = AddinExpress.OL.ADXRegionBorderStyle.None;
            this.formRightSidebar.UseOfficeThemeForBackground = true;
            // 
            // wsuiTab
            // 
            this.wsuiTab.Caption = "Outlook Finder";
            this.wsuiTab.Controls.Add(this.groupSearch);
            this.wsuiTab.Id = "adxRibbonTab_500b5beadf3a45d9b11245e305940d6c";
            this.wsuiTab.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            this.wsuiTab.PropertyChanging += new AddinExpress.MSO.ADXRibbonPropertyChanging_EventHandler(this.wsuiTab_PropertyChanging);
            // 
            // groupSearch
            // 
            this.groupSearch.AutoScale = true;
            this.groupSearch.Caption = "Outlook Finder";
            this.groupSearch.Controls.Add(this.adxRibbonBoxSearch);
            this.groupSearch.Controls.Add(this.buttonShow);
            this.groupSearch.Controls.Add(this.btnSplit);
            this.groupSearch.Id = "adxRibbonGroup_c94173390d39441fa25cda51a851cd7a";
            this.groupSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.groupSearch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // adxRibbonBoxSearch
            // 
            this.adxRibbonBoxSearch.Controls.Add(this.adxRibbonEditBoxSearch);
            this.adxRibbonBoxSearch.Controls.Add(this.adxRibbonButtonSearch);
            this.adxRibbonBoxSearch.Id = "adxRibbonBox_dc294efeac8340e788f9e0ebd39ab866";
            this.adxRibbonBoxSearch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // adxRibbonEditBoxSearch
            // 
            this.adxRibbonEditBoxSearch.Id = "adxRibbonEditBox_decd82b7448b4bbe9853a4a4dee51263";
            this.adxRibbonEditBoxSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonEditBoxSearch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            this.adxRibbonEditBoxSearch.ShowCaption = false;
            this.adxRibbonEditBoxSearch.SizeString = "This is the width";
            this.adxRibbonEditBoxSearch.Tag = 200121;
            // 
            // adxRibbonButtonSearch
            // 
            this.adxRibbonButtonSearch.Caption = "Search";
            this.adxRibbonButtonSearch.Id = "adxRibbonButton_f0ca75ee177a4886a26d3c9246518516";
            this.adxRibbonButtonSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxRibbonButtonSearch.ParseMsoXmlTypeAs = AddinExpress.MSO.ADXParseMsoXmlTypeAs.pxtControl;
            this.adxRibbonButtonSearch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // buttonShow
            // 
            this.buttonShow.Caption = "Show/Hide";
            this.buttonShow.Id = "adxRibbonButton_dcb0aa6e6fd442c79ea44b4006d84643";
            this.buttonShow.Image = 1;
            this.buttonShow.ImageList = this.wsuiImageList;
            this.buttonShow.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonShow.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // wsuiImageList
            // 
            this.wsuiImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("wsuiImageList.ImageStream")));
            this.wsuiImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.wsuiImageList.Images.SetKeyName(0, "application-plus-red.png");
            this.wsuiImageList.Images.SetKeyName(1, "logo_64.png");
            this.wsuiImageList.Images.SetKeyName(2, "gear.png");
            this.wsuiImageList.Images.SetKeyName(3, "question.png");
            // 
            // btnSplit
            // 
            this.btnSplit.Caption = "More";
            this.btnSplit.Controls.Add(this.menuHelp);
            this.btnSplit.Id = "adxRibbonSplitButton_54dfcdd5b26847d19e5fb423558dbdcd";
            this.btnSplit.Image = 2;
            this.btnSplit.ImageList = this.wsuiImageList;
            this.btnSplit.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.btnSplit.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // menuHelp
            // 
            this.menuHelp.Controls.Add(this.btnHelp);
            this.menuHelp.Controls.Add(this.adxRibbonMenuSeparator1);
            this.menuHelp.Controls.Add(this.btnAbout);
            this.menuHelp.Id = "adxRibbonMenu_0e49baf968b443deaa578f34388228fc";
            this.menuHelp.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.menuHelp.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            this.menuHelp.ShowCaption = false;
            // 
            // btnHelp
            // 
            this.btnHelp.Caption = "Help";
            this.btnHelp.Id = "adxRibbonButton_32e3a315cb2240e690bd8a996bff0ffc";
            this.btnHelp.Image = 3;
            this.btnHelp.ImageList = this.wsuiImageList;
            this.btnHelp.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.btnHelp.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // adxRibbonMenuSeparator1
            // 
            this.adxRibbonMenuSeparator1.Id = "adxRibbonMenuSeparator_0f5aee07dd7b4c2689f6d1e780c9f15a";
            this.adxRibbonMenuSeparator1.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // btnAbout
            // 
            this.btnAbout.Caption = "About";
            this.btnAbout.Id = "adxRibbonButton_ca1899db162c46dca40d0d2791d231c7";
            this.btnAbout.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.btnAbout.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
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
            this.adxMainPluginCommandBar.UpdateCounter = 18;
            // 
            // buttonShow2007
            // 
            this.buttonShow2007.Caption = "Show OutlookFinder";
            this.buttonShow2007.ControlTag = "e314f48a-f1c6-4e22-9e96-2f526c649798";
            this.buttonShow2007.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonShow2007.Temporary = true;
            this.buttonShow2007.UpdateCounter = 7;
            // 
            // buttonHide2007
            // 
            this.buttonHide2007.Caption = "Close OutlookFinder";
            this.buttonHide2007.ControlTag = "2a1559ac-9958-47f2-bae7-141679f598e8";
            this.buttonHide2007.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.buttonHide2007.Temporary = true;
            this.buttonHide2007.UpdateCounter = 5;
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
            // wsuiMainTab
            // 
            this.wsuiMainTab.Caption = "Main Tab";
            this.wsuiMainTab.Controls.Add(this.wsuiMainGroup);
            this.wsuiMainTab.Id = "adxRibbonTab_9bda0e619ca1438d9effcf3f083e92e8";
            this.wsuiMainTab.IdMso = "TabMail";
            this.wsuiMainTab.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // wsuiMainGroup
            // 
            this.wsuiMainGroup.Caption = "Outlook Finder";
            this.wsuiMainGroup.Controls.Add(this.adxMainBox);
            this.wsuiMainGroup.Controls.Add(this.wsuiButtonSwitch);
            this.wsuiMainGroup.Controls.Add(this.btnMainSplit);
            this.wsuiMainGroup.Id = "adxRibbonGroup_f065ec953c074c6a9e1ba8cae6b9b786";
            this.wsuiMainGroup.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.wsuiMainGroup.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            this.wsuiMainGroup.PropertyChanging += new AddinExpress.MSO.ADXRibbonPropertyChanging_EventHandler(this.wsuiMainGroup_PropertyChanging);
            // 
            // adxMainBox
            // 
            this.adxMainBox.Controls.Add(this.wsuiHomeSearch);
            this.adxMainBox.Controls.Add(this.wsuiButtonSearch);
            this.adxMainBox.Id = "adxRibbonBox_650813691cb74c5db775e919c877f3ff";
            this.adxMainBox.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // wsuiHomeSearch
            // 
            this.wsuiHomeSearch.Id = "adxRibbonEditBox_8043c3682f36418996476af3affdd7e5";
            this.wsuiHomeSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.wsuiHomeSearch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            this.wsuiHomeSearch.ScreenTip = "Search";
            this.wsuiHomeSearch.ShowCaption = false;
            this.wsuiHomeSearch.SizeString = "This is the width";
            this.wsuiHomeSearch.SuperTip = "Enter search criteria";
            // 
            // wsuiButtonSearch
            // 
            this.wsuiButtonSearch.Caption = "Search";
            this.wsuiButtonSearch.Id = "adxRibbonButton_c993f00bdccb44988791d19e0e30e00a";
            this.wsuiButtonSearch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.wsuiButtonSearch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // wsuiButtonSwitch
            // 
            this.wsuiButtonSwitch.Caption = "Show/Hide";
            this.wsuiButtonSwitch.Id = "adxRibbonButton_295c2b7151ed437382c104f4c3d542ce";
            this.wsuiButtonSwitch.Image = 1;
            this.wsuiButtonSwitch.ImageList = this.wsuiImageList;
            this.wsuiButtonSwitch.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.wsuiButtonSwitch.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // btnMainSplit
            // 
            this.btnMainSplit.Caption = "More";
            this.btnMainSplit.Controls.Add(this.mnuMain);
            this.btnMainSplit.Id = "adxRibbonSplitButton_9b5edc2e0af14b188c856fdddb98dcbe";
            this.btnMainSplit.Image = 2;
            this.btnMainSplit.ImageList = this.wsuiImageList;
            this.btnMainSplit.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.btnMainSplit.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // mnuMain
            // 
            this.mnuMain.Controls.Add(this.btnMainHelp);
            this.mnuMain.Controls.Add(this.btnMainMenuSeparatotr);
            this.mnuMain.Controls.Add(this.btnMainAbout);
            this.mnuMain.Id = "adxRibbonMenu_eef51eb086df4c2886195868fb76cef8";
            this.mnuMain.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mnuMain.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // btnMainHelp
            // 
            this.btnMainHelp.Caption = "Help";
            this.btnMainHelp.Id = "adxRibbonButton_e8264c95d9934ff5a85549d2fbeecd7e";
            this.btnMainHelp.Image = 3;
            this.btnMainHelp.ImageList = this.wsuiImageList;
            this.btnMainHelp.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.btnMainHelp.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // btnMainMenuSeparatotr
            // 
            this.btnMainMenuSeparatotr.Id = "adxRibbonMenuSeparator_1001098bd1ed42f7837be7469856176d";
            this.btnMainMenuSeparatotr.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // btnMainAbout
            // 
            this.btnMainAbout.Caption = "About";
            this.btnMainAbout.Id = "adxRibbonButton_610ffe76131f40c3b097201812603434";
            this.btnMainAbout.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.btnMainAbout.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // OutlookFinderEvents
            // 
            this.OutlookFinderEvents.Quit += new System.EventHandler(this.OutlookFinderEvents_Quit);
            this.OutlookFinderEvents.NewExplorer += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_NewExplorer);
            this.OutlookFinderEvents.ExplorerSelectionChange += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerSelectionChange);
            // 
            // WSUIAddinModule
            // 
            this.AddinName = "Outlook Finder";
            this.HandleShortcuts = true;
            this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaOutlook;
            this.AddinStartupComplete += new AddinExpress.MSO.ADXEvents_EventHandler(this.WSUIAddinModule_AddinStartupComplete);
            this.OnKeyDown += new AddinExpress.MSO.ADXKeyDown_EventHandler(this.WSUIAddinModule_OnKeyDown);

        }

        #endregion Component Designer generated code

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

        #endregion Add-in Express automatic code

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

        #region my own initialization

        private void Init()
        {
            StartWatch();
            WSSqlLogger.Instance.LogInfo("Plugin is loading...");
            outlookFormManager.ADXFolderSwitchEx += OutlookFormManagerOnAdxFolderSwitchEx;

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
            StopWatch("Init");
        }

        private void OutlookFormManagerOnAdxFolderSwitchEx(object sender, FolderSwitchExEventArgs args)
        {
            StartWatch();
            if (IsLoading)
                return;
            HidaSidebarDuringSwitching();
            StopWatch("OutlookFormManagerOnAdxFolderSwitchEx");
        }

        private void HidaSidebarDuringSwitching()
        {
            var app = (AppEmpty)System.Windows.Application.Current;
            if (app.IsNull())
                return;
            app.Dispatcher.BeginInvoke(new Action(() =>
            {
                _sidebarForm = GetSidebarForm();

                if (!IsMainUIVisible)
                {
                    HideUi(false);
                }
            }));
        }

        #endregion my own initialization

        private void CheckUpdate()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (!ReferenceEquals(_updatable, null))
            {
                _updatable.Update();
            }
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("Check for update: {0}ms", watch.ElapsedMilliseconds));
        }

        private void RunPluginUI()
        {
            StartWatch();
            DllPreloader.Instance.PreloadDll();
            _wsuiBootStraper = new PluginBootStraper();
            _wsuiBootStraper.Run();
            _eventAggregator = _wsuiBootStraper.Container.Resolve<IEventAggregator>();
            ((AppEmpty)System.Windows.Application.Current).MainControl = (System.Windows.Controls.UserControl)_wsuiBootStraper.View;
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
            SetEventAggregatorToManager();
            SubscribeToEvents();

            IsLoading = false;
            StopWatch("RunPluginUI");
        }

        private ISidebarForm GetSidebarForm()
        {

            ISidebarForm form = null;
            for (int i = 0; i < formRightSidebar.FormInstanceCount; i++)
            {
                form = formRightSidebar.FormInstances(i) as ISidebarForm;
                if (form != null)
                {
                    break;
                }
            }
            return form;
        }

        private void SubscribeToEvents()
        {
            if (_eventAggregator == null)
                return;
            _eventAggregator.GetEvent<WSUIOpenWindow>().Subscribe(ShowUi);
            _eventAggregator.GetEvent<WSUIHideWindow>().Subscribe(HideUi);
            _eventAggregator.GetEvent<WSUISearch>().Subscribe(StartSearch);
            _eventAggregator.GetEvent<WSUIShowFolder>().Subscribe(ShowOutlookFolder);
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
            StartWatch();
            _sidebarForm = GetSidebarForm();
            if (_sidebarForm != null && !_sidebarForm.IsDisposed)
            {
                _sidebarForm.Show();
                _wsuiBootStraper.PassAction(new WSAction(WSActionType.Show, null));
                IsMainUIVisible = true;
                RegistryHelper.Instance.SetIsPluginUiVisible(IsMainUIVisible);
            }
            StopWatch("ShowUi");
        }

        private void HideUi(bool hide)
        {
            StartWatch();
            _sidebarForm = GetSidebarForm();
            if (_sidebarForm != null && !_sidebarForm.IsDisposed)
            {
                _sidebarForm.Hide();
                IsMainUIVisible = false;
                RegistryHelper.Instance.SetIsPluginUiVisible(IsMainUIVisible);
            }
            StopWatch("HideUi");
        }

        private void StartSearch(string criteria)
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

        private void CreateCommandManager()
        {
            _commandManager = adxMainPluginCommandBar.UseForRibbon
                              ? (IWSUICommandManager)new WSUICommandBarManager(buttonShow2007, buttonHide2007, adxCommandBarEditSearchText,
                                  adxCommandBarButtonSearch)
                              : new WSUIRibbonManager(buttonShow, wsuiButtonSwitch, adxRibbonButtonSearch, adxRibbonEditBoxSearch, wsuiHomeSearch, wsuiButtonSearch);
            if (!adxMainPluginCommandBar.UseForRibbon)
            {
                _aboutCommandManager = new WSUIAboutCommandManager(btnHelp, btnAbout, btnMainHelp, btnMainAbout);
            }
            else if (RegistryHelper.Instance.GetIsPluginUiVisible())
            {
                _commandManager.SetShowHideButtonsEnabling(false, true);
            }
        }

        private void SetEventAggregatorToManager()
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

        #region Outlook Object Model routines

        private enum OutlookPane
        {
            olFolderList = 2,
            olNavigationPane = 4,
            olOutlookBar = 1,
            olPreview = 3,
            olToDoPane = 5
        }


        private void SetExplorerFolder(Outlook.Explorer Explorer, Outlook.MAPIFolder Folder)
        {
            try
            {
                if (_outlookVersion == 2000 || _outlookVersion == 2002)
                    //|| _outlookVersion == 2007 || _outlookVersion == 2010
                    Explorer.CurrentFolder = Folder;
                else
                    Explorer.SelectFolder(Folder);
                // Explorer.GetType().InvokeMember("SelectFolder", BindingFlags.InvokeMethod, null, Explorer, new object[] { Folder });
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("SetExplorerFolder");
                WSSqlLogger.Instance.LogError(ex.Message);
            }
        }

        #endregion Outlook Object Model routines

        private void WSUIAddinModule_AddinStartupComplete(object sender, EventArgs e)
        {
            StartWatch();
            RestoreOutlookFolder();
            //CheckUpdate(); // TODO: just for testing
            this.SendMessage(WM_LOADED, IntPtr.Zero, IntPtr.Zero);
            OutlookPreviewHelper.Instance.OutlookApp = OutlookApp;
            OutlookHelper.Instance.OutlookApp = OutlookApp;
            WSSqlLogger.Instance.LogInfo("WSUI AddinModule Startup Complete...");
            StopWatch("WSUIAddinModule_AddinStartupComplete");
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WSSqlLogger.Instance.LogError("Unhandled Exception (plugin): {0}", e.ExceptionObject.ToString());
        }

        private void CurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs firstChanceExceptionEventArgs)
        {
            if (firstChanceExceptionEventArgs.Exception is ReflectionTypeLoadException)
            {
                foreach (var item in (firstChanceExceptionEventArgs.Exception as ReflectionTypeLoadException).LoaderExceptions)
                {
                    WSSqlLogger.Instance.LogError("Reflection Type Load: {0}", item.Message.ToString());
                }
            }
            WSSqlLogger.Instance.LogError("Exception: {0}\nStacktrace: {1}", firstChanceExceptionEventArgs.Exception.Message, firstChanceExceptionEventArgs.Exception.StackTrace);
        }

        #region [event handlers for ribbon]

        private void PassSearchActionToSearchEngine(string text)
        {
            if (string.IsNullOrEmpty(text) || _wsuiBootStraper.IsPluginBusy)
                return;
            if (!WSUIAddinModule.CurrentInstance.IsMainUIVisible)
            {
                ShowMainUiAfterSearchAction();
            }
            _wsuiBootStraper.PassAction(new WSAction(WSActionType.Search, text));
        }

        private void ShowMainUiAfterSearchAction()
        {

            if (!adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
            {
                buttonShow2007.Enabled = false;
                buttonHide2007.Enabled = true;
            }
        }

        #endregion [event handlers for ribbon]

        private void outlookFormManager_OnInitialize()
        {
            StartWatch();
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
            StopWatch("outlookFormManager_OnInitialize");
        }

        private void OutlookFinderEvents_Quit(object sender, EventArgs e)
        {
            StartWatch();
            _wsuiBootStraper.PassAction(new WSAction(WSActionType.Quit, null));
            SetOutlookFolderProperties(string.Empty, string.Empty);
            WSSqlLogger.Instance.LogInfo("Shutdown...");
            StopWatch("OutlookFinderEvents_Quit");
        }

        private void SetOutlookFolderProperties(string folderName, string folderWebUrl)
        {
            if (!string.IsNullOrEmpty(folderName))
                RegistryHelper.Instance.SetOutlookFolderName(folderName);
            if (!string.IsNullOrEmpty(folderWebUrl))
                RegistryHelper.Instance.SetOutlookFolderWebUrl(folderWebUrl);
        }

        public void RestoreOutlookFolder()
        {
            if (RegistryHelper.Instance.IsShouldRestoreOutlookFolder())
            {
                WSSqlLogger.Instance.LogInfo("{0}", "OUtlookFolder is empty");
                return;
            }

            string id = RegistryHelper.Instance.GetOutllokFolderName();
            Outlook.NameSpace outlookNamespace = OutlookApp.GetNamespace(DefaultNamespace);
            if (outlookNamespace == null || string.IsNullOrEmpty(id))
                return;
            WSSqlLogger.Instance.LogInfo("OutlookFolder ID: {0}", id);
            Outlook.MAPIFolder folder = outlookNamespace.GetFolderFromID(id, Type.Missing);
            if (folder == null)
                return;
            folder.WebViewURL = RegistryHelper.Instance.GetOutlookFolderWebUrl();
            folder.WebViewOn = true;

            WSSqlLogger.Instance.LogInfo("WebViewURL: {0}", folder.WebViewURL);

            Marshal.ReleaseComObject(folder);
            Marshal.ReleaseComObject(outlookNamespace);
        }

        // TODO: uncomment
        private void WSUIAddinModule_OnKeyDown(object sender, ADXKeyDownEventArgs e)
        {
            _sidebarForm = GetSidebarForm();
            if (_sidebarForm == null)
                return;
            if (e.Ctrl && e.VirtualKey == (int)Keys.C)
            {
                _sidebarForm.SendAction(WSActionType.Copy);
            }
        }

        private Outlook.MAPIFolder GetFolderByFullPath(string folderPath)
        {
            Outlook.NameSpace ns = this.OutlookApp.GetNamespace("MAPI");
            Outlook.MAPIFolder fld = null;
            foreach (var folder in ns.Folders.OfType<Outlook.MAPIFolder>())
            {
                fld = GetOutlookFolders(folder, folderPath);
                if (fld != null)
                    break;
            }
            return fld;
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
                    }
                    catch (Exception e)
                    {
                        WSSqlLogger.Instance.LogError(string.Format("{0} '{1}' - {2}", "Get Folders", subfolder.Name, e.Message));
                    }
                }
                return mapiFolder;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
            }
            return null;
        }

        private void OutlookFinderEvents_NewExplorer(object sender, object explorer)
        {
            StartWatch();
            var exp = explorer as Outlook._Explorer;
            if (IsLoading && exp != null)
            {
                _initHashCode = exp.GetHashCode();
            }
            StopWatch("OutlookFinderEvents_NewExplorer");
        }

        private void wsuiTab_PropertyChanging(object sender, ADXRibbonPropertyChangingEventArgs e)
        {
            if (e.PropertyType == ADXRibbonControlPropertyType.Visible && e.Context.GetHashCode() != _initHashCode)
            {
                e.Value = false;
            }
        }

        private void wsuiMainGroup_PropertyChanging(object sender, ADXRibbonPropertyChangingEventArgs e)
        {
            if (e.PropertyType == ADXRibbonControlPropertyType.Visible && e.Context.GetHashCode() != _initHashCode)
            {
                e.Value = false;
            }
        }

        private void StartWatch()
        {
            (_watch = new Stopwatch()).Start();
        }

        private void StopWatch(string method)
        {
            _watch.Stop();
            WSSqlLogger.Instance.LogInfo("--------------- {0} => {1}",method,_watch.ElapsedMilliseconds);
        }


        private void OutlookFinderEvents_ExplorerSelectionChange(object sender, object explorer)
        {
            return;

            var exp = explorer as Outlook._Explorer;
            if (exp == null || exp.Selection.Count == 0)
                return;

            string email = string.Empty;
            string[] names = default(string[]);
            dynamic item = null;
            if (exp.Selection[1] is Outlook.MailItem)
            {
                var itemMail = exp.Selection[1] as Outlook.MailItem;
                var senderContact = itemMail.Sender as Outlook.AddressEntry;
                names = senderContact.Name.Split(' ');
                email = senderContact.GetEmailAddress();
                item = itemMail;
            }
            else if (exp.Selection[1] is Outlook.AppointmentItem)
            {
                var itemAppointment = exp.Selection[1] as Outlook.AppointmentItem;
                var organizer = itemAppointment.GetOrganizer() as Outlook.AddressEntry;
                names = organizer.Name.Split(' ');
                email = organizer.GetEmailAddress();
                item = itemAppointment;
            }
            else if (exp.Selection[1] is Outlook.MeetingItem)
            {
                var itemMeeting = exp.Selection[1] as Outlook.MeetingItem;
                names = itemMeeting.SenderName.Split(' ');
                if (itemMeeting.SenderEmailAddress.IsEmail())
                {
                    email = itemMeeting.SenderEmailAddress;
                }
                else if (names.Length > 1)
                {
                    var contact = OutlookHelper.Instance.GetContact(names[0], names[1]);
                    if (contact.Any())
                    {
                        var ct = contact.FirstOrDefault(c => !string.IsNullOrEmpty(c.Email1Address));
                        if (ct.IsNotNull() && ct.Email1Address.IsEmail())
                        {
                            email = ct.Email1Address;
                        }
                    }
                }
                item = itemMeeting;
            }
            if (item == null)
            {
                return;
            }
            var referenceItem = item.EntryID;
            if (_previewReferenceSelected != null && _previewReferenceSelected == referenceItem)
                return;
            _previewReferenceSelected = referenceItem;

            if (_wsuiBootStraper == null)
                return;

            if(names.Length > 1)
            {
                var tag = new ContactSearchObject() { FirstName = names[0], LastName = names[1], EmailAddress = email };
                _wsuiBootStraper.PassAction(new WSAction(WSActionType.ShowContact, tag));
            }
            else if (names.Length > 0)
            {
                var tag = new EmailContactSearchObject() { ContactName = names[0], EMail = email };
                _wsuiBootStraper.PassAction(new WSAction(WSActionType.ShowContact, tag));
            }
        }


    }
}