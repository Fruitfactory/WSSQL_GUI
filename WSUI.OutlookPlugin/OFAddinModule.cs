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
using AddinExpress.MSO;
using AddinExpress.OL;
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
    /// <summary>
    ///   Add-in Express Add-in Module
    /// </summary>
    [GuidAttribute("E854FABB-353C-4B9A-8D18-F66E61F6FCA5"), ProgId(ProgIdDefault)]
    public class OFAddinModule : AddinExpress.MSO.ADXAddinModule
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
        private ADXRibbonButton mnuSettings;
        private ADXRibbonButton mnuMainSettings;
        private ADXRibbonButton adxSendLogOFTab;
        private ADXRibbonButton adxSendLogMain;
        private const string DefaultNamespace = "MAPI";

        #endregion [const]

        public OFAddinModule()
        {
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "false";
            Application.EnableVisualStyles();
            InitializeComponent();
            Init();
            this.OnSendMessage += OFAddinModule_OnSendMessage;
            this.AddinInitialize += OnAddinInitialize;
            this.AddinBeginShutdown += OnAddinBeginShutdown;
            _mailRemovingManager = new OFOutlookItemsRemovingManager(this);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;
        }

        private void OnAddinBeginShutdown(object sender, EventArgs eventArgs)
        {

        }

        private void OnAddinInitialize(object sender, EventArgs eventArgs)
        {
            try
            {
                if (adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
                {
                    adxMainPluginCommandBar.UseForRibbon = false;
                }
                //TODO !!!!
                CheckAndStartServiceApp();

            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        private void OFAddinModule_OnSendMessage(object sender, ADXSendMessageEventArgs e)
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OFAddinModule));
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
            this.mnuSettings = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxSendLogOFTab = new AddinExpress.MSO.ADXRibbonButton(this.components);
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
            this.mnuMainSettings = new AddinExpress.MSO.ADXRibbonButton(this.components);
            this.adxSendLogMain = new AddinExpress.MSO.ADXRibbonButton(this.components);
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
            this.formRightSidebar.FormClassName = "OFOutlookPlugin.OFSidebar";
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
            this.menuHelp.Controls.Add(this.mnuSettings);
            this.menuHelp.Controls.Add(this.adxSendLogOFTab);
            this.menuHelp.Controls.Add(this.btnHelp);
            this.menuHelp.Controls.Add(this.adxRibbonMenuSeparator1);
            this.menuHelp.Controls.Add(this.btnAbout);
            this.menuHelp.Id = "adxRibbonMenu_0e49baf968b443deaa578f34388228fc";
            this.menuHelp.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.menuHelp.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            this.menuHelp.ShowCaption = false;
            // 
            // mnuSettings
            // 
            this.mnuSettings.Caption = "Settings...";
            this.mnuSettings.Id = "adxRibbonButton_479736670ccc428b8fc2782bc97ca1f1";
            this.mnuSettings.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mnuSettings.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // adxSendLogOFTab
            // 
            this.adxSendLogOFTab.Caption = "Send log files...";
            this.adxSendLogOFTab.Id = "adxRibbonButton_a8c2fbe6e8a6444eb293329425e3d082";
            this.adxSendLogOFTab.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxSendLogOFTab.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
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
            this.adxMainPluginCommandBar.CommandBarName = "adxMainPluginCommandBar";
            this.adxMainPluginCommandBar.CommandBarTag = "674128a0-9ce1-485c-b1a5-f5ff6897bfc8";
            this.adxMainPluginCommandBar.Controls.Add(this.buttonShow2007);
            this.adxMainPluginCommandBar.Controls.Add(this.buttonHide2007);
            this.adxMainPluginCommandBar.Controls.Add(this.adxCommandBarEditSearchText);
            this.adxMainPluginCommandBar.Controls.Add(this.adxCommandBarButtonSearch);
            this.adxMainPluginCommandBar.Temporary = true;
            this.adxMainPluginCommandBar.UpdateCounter = 21;
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
            this.mnuMain.Controls.Add(this.mnuMainSettings);
            this.mnuMain.Controls.Add(this.adxSendLogMain);
            this.mnuMain.Controls.Add(this.btnMainHelp);
            this.mnuMain.Controls.Add(this.btnMainMenuSeparatotr);
            this.mnuMain.Controls.Add(this.btnMainAbout);
            this.mnuMain.Id = "adxRibbonMenu_eef51eb086df4c2886195868fb76cef8";
            this.mnuMain.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mnuMain.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // mnuMainSettings
            // 
            this.mnuMainSettings.Caption = "Settings...";
            this.mnuMainSettings.Id = "adxRibbonButton_bde112361dfb425b89e394ed664825cb";
            this.mnuMainSettings.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.mnuMainSettings.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
            // 
            // adxSendLogMain
            // 
            this.adxSendLogMain.Caption = "Send log files...";
            this.adxSendLogMain.Id = "adxRibbonButton_8aa9adf95aa842dc9066f039b4b67452";
            this.adxSendLogMain.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.adxSendLogMain.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
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
            this.OutlookFinderEvents.ItemSend += new AddinExpress.MSO.ADXOlItemSend_EventHandler(this.OutlookFinderEvents_ItemSend);
            this.OutlookFinderEvents.Quit += new System.EventHandler(this.OutlookFinderEvents_Quit);
            this.OutlookFinderEvents.InspectorActivate += new AddinExpress.MSO.ADXOlInspector_EventHandler(this.OutlookFinderEvents_InspectorActivate);
            this.OutlookFinderEvents.InspectorClose += new AddinExpress.MSO.ADXOlInspector_EventHandler(this.OutlookFinderEvents_InspectorClose);
            this.OutlookFinderEvents.NewExplorer += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_NewExplorer);
            this.OutlookFinderEvents.ExplorerActivate += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerActivate);
            this.OutlookFinderEvents.ExplorerClose += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerClose);
            this.OutlookFinderEvents.ExplorerSelectionChange += new AddinExpress.MSO.ADXOlExplorer_EventHandler(this.OutlookFinderEvents_ExplorerSelectionChange);
            this.OutlookFinderEvents.ExplorerInlineResponse += new AddinExpress.MSO.ADXOlExplorerInlineResponse_EventHandler(this.OutlookFinderEvents_ExplorerInlineResponse);
            this.OutlookFinderEvents.ExplorerInlineResponseEx += new AddinExpress.MSO.ADXOlExplorerInlineResponseEx_EventHandler(this.OutlookFinderEvents_ExplorerInlineResponseEx);
            this.OutlookFinderEvents.ExplorerInlineResponseCloseEx += new AddinExpress.MSO.ADXOlExplorerInlineResponseCloseEx_EventHandler(this.OutlookFinderEvents_ExplorerInlineResponseCloseEx);
            // 
            // OFAddinModule
            // 
            this.AddinName = "Outlook Finder";
            this.HandleShortcuts = true;
            this.SupportedApps = AddinExpress.MSO.ADXOfficeHostApp.ohaOutlook;
            this.AddinStartupComplete += new AddinExpress.MSO.ADXEvents_EventHandler(this.OFAddinModule_AddinStartupComplete);
            this.OnKeyDown += new AddinExpress.MSO.ADXKeyDown_EventHandler(this.OFAddinModule_OnKeyDown);

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

        public static new OFAddinModule CurrentInstance
        {
            get
            {
                return AddinExpress.MSO.ADXAddinModule.CurrentInstance as OFAddinModule;
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

        //#region my own initialization

        private void Init()
        {
            try
            {
                //StartWatch();
                OFLogger.Instance.LogDebug("Plugin is loading...");
                outlookFormManager.ADXFolderSwitchEx += OutlookFormManagerOnAdxFolderSwitchEx;
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
                    _updatable.Module = this;
                }
                //StopWatch("Init");
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }


        private void OutlookFormManagerOnAdxFolderSwitchEx(object sender, FolderSwitchExEventArgs args)
        {
            try
            {
                var folder = args.FolderObj as Outlook.Folder;
                _mailRemovingManager.ConnectTo(folder);
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

        //#endregion my own initialization

        //private void CheckUpdate()
        //{
        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    if (!ReferenceEquals(_updatable, null))
        //    {
        //        _updatable.Update();
        //    }
        //    watch.Stop();
        //    OFLogger.Instance.LogInfo(string.Format("Check for update: {0}ms", watch.ElapsedMilliseconds));
        //}

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

                var fieldINfo = typeof(ADXAddinModule).GetField("disconnectMode",
                    BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
                if (fieldINfo.IsNotNull())
                {
                    fieldINfo.SetValue(this, ADXDisconnectMode.adx_dm_UISetupComplete);
                }

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
            _eventAggregator.GetEvent<OFOpenWindow>().Subscribe(ShowUi);
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

        private void CreateCommandManager()
        {
            try
            {
                _commandManager = adxMainPluginCommandBar.UseForRibbon
                                      ? (IOFCommandManager)new OFCommandBarManager(buttonShow2007, buttonHide2007, adxCommandBarEditSearchText,
                                          adxCommandBarButtonSearch)
                                      : new OFRibbonManager(buttonShow, wsuiButtonSwitch, adxRibbonButtonSearch, adxRibbonEditBoxSearch, wsuiHomeSearch, wsuiButtonSearch, adxSendLogOFTab);
                if (!adxMainPluginCommandBar.UseForRibbon)
                {
                    _aboutCommandManager = new OFMainRibbonCommandManager(btnHelp, btnAbout, mnuSettings, btnMainHelp, btnMainAbout, mnuMainSettings, adxSendLogMain);
                }
                else if (OFRegistryHelper.Instance.GetIsPluginUiVisible())
                {
                    _commandManager.SetShowHideButtonsEnabling(false, true);
                }
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
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

        private void OFAddinModule_AddinStartupComplete(object sender, EventArgs e)
        {
            try
            {
                //StartWatch();
                //RestoreOutlookFolder();
                //CheckUpdate(); // TODO: just for testing
                this.SendMessage(WM_LOADED, IntPtr.Zero, IntPtr.Zero);
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
                if (!OFAddinModule.CurrentInstance.IsMainUIVisible)
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

        private void ShowMainUiAfterSearchAction()
        {

            if (!adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
            {
                buttonShow2007.Enabled = false;
                buttonHide2007.Enabled = true;
            }
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

        private void OutlookFinderEvents_Quit(object sender, EventArgs e)
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


        private void FinalizeComponents()
        {
            if (adxMainPluginCommandBar.IsNotNull())
            {
                adxMainPluginCommandBar.Dispose();
                adxCommandBarButtonSearch = null;
            }
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

        //// TODO: uncomment
        private void OFAddinModule_OnKeyDown(object sender, ADXKeyDownEventArgs e)
        {
            try
            {
                _sidebarForm = GetSidebarForm();
                if (_sidebarForm == null)
                    return;
                if (e.Ctrl && e.VirtualKey == (int)Keys.C)
                {
                    _sidebarForm.SendAction(OFActionType.Copy);
                }

                if (System.Windows.Application.Current != null)
                {
                    System.Windows.Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        if (_emailSuggesterManager.IsNotNull() && OFRegistryHelper.Instance.CheckAutoCompleateState())
                        {
                            System.Diagnostics.Debug.WriteLine($"Key Down: {Enum.GetName(typeof(Keys), e.VirtualKey)}");
                            _emailSuggesterManager.ProcessKeyDown(e);
                        }
                    }));
                }
                if (e.VirtualKey == (int)Keys.Escape && _emailSuggesterManager.IsSuggestWindowVisible())
                {
                    e.Handled = true;
                }
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

        private void wsuiTab_PropertyChanging(object sender, ADXRibbonPropertyChangingEventArgs e)
        {
            if (IsMainUIVisible && e.PropertyType == ADXRibbonControlPropertyType.Visible && e.Context.GetHashCode() != _initHashCode)
            {
                e.Value = false;
            }
        }

        private void wsuiMainGroup_PropertyChanging(object sender, ADXRibbonPropertyChangingEventArgs e)
        {
            if (IsMainUIVisible && e.PropertyType == ADXRibbonControlPropertyType.Visible && e.Context.GetHashCode() != _initHashCode)
            {
                e.Value = false;
            }
        }

        //private void StartWatch()
        //{
        //    (_watch = new Stopwatch()).Start();
        //}

        //private void StopWatch(string method)
        //{
        //    _watch.Stop();
        //    OFLogger.Instance.LogDebug("--------------- {0} => {1}", method, _watch.ElapsedMilliseconds);
        //}


        private void OutlookFinderEvents_ExplorerSelectionChange(object sender, object explorer)
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

        private void OutlookFinderEvents_InspectorActivate(object sender, object inspector, string folderName)
        {
            if (_canConnect)
            {
                Outlook._Inspector insp = (Outlook._Inspector)inspector;
                var item = insp.CurrentItem as Outlook.MailItem;
                if (item != null)
                {
                    _mailRemovingManager.ConnectTo(item);
                }
            }
            if (_emailSuggesterManager.IsNotNull())
            {
                _emailSuggesterManager.SubscribeMailWindow();
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

        private void OutlookFinderEvents_ItemSend(object sender, ADXOlItemSendEventArgs e)
        {
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