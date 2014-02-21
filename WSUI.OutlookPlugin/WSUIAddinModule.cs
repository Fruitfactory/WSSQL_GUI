using System;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Forms;
using AddinExpress.MSO;
using Microsoft.Practices.Prism.Events;
using Microsoft.Practices.Unity;
using WSPreview.PreviewHandler.Service.OutlookPreview;
using WSUI.Control.Interfaces;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Logger;
using WSUI.Infrastructure.Service.Helpers;
using WSUIOutlookPlugin.Events;
using WSUIOutlookPlugin.Interfaces;
using WSUIOutlookPlugin.Managers;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Globalization;
using System.Reflection;
using WSUI.Control;
using System.Diagnostics;
using WSUIOutlookPlugin.Core;
using WSUI.Infrastructure.Controls.Application;
using Application = System.Windows.Forms.Application;
using MessageBox = System.Windows.Forms.MessageBox;

namespace WSUIOutlookPlugin {
/// <summary>
///   Add-in Express Add-in Module
/// </summary>
[GuidAttribute("E854FABB-353C-4B9A-8D18-F66E61F6FCA5"), ProgId("WSUIOutlookPlugin.AddinModule")]
public class WSUIAddinModule : AddinExpress.MSO.ADXAddinModule {

  private int _outlookVersion = 0;
  private bool _refreshCurrentFolderExecuting = false;
  private IUpdatable _updatable = null;
  private IPluginBootStraper _wsuiBootStraper = null;
  private IEventAggregator _eventAggregator = null;
  private Outlook.MAPIFolder _lastMapiFolder;
  private IWSUICommandManager _commandManager;
  private ICommandManager _aboutCommandManager;

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
    WSSqlLogger.Instance.LogInfo(string.Format("InitializeComponent [ctor]: {0}ms", watch.ElapsedMilliseconds));
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
    }
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
  private AddinExpress.OL.ADXOlFormsCollectionItem formWebPaneItem;

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
    this.formWebPaneItem = new AddinExpress.OL.ADXOlFormsCollectionItem(this.components);
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
    this.wsuiTab.Caption = "Outlook Finder";
    this.wsuiTab.Controls.Add(this.groupSearch);
    this.wsuiTab.Id = "adxRibbonTab_500b5beadf3a45d9b11245e305940d6c";
    this.wsuiTab.Ribbons = AddinExpress.MSO.ADXRibbons.msrOutlookExplorer;
    //
    // groupSearch
    //
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
    this.buttonShow2007.Caption = "Show Windows Search";
    this.buttonShow2007.ControlTag = "e314f48a-f1c6-4e22-9e96-2f526c649798";
    this.buttonShow2007.ImageTransparentColor = System.Drawing.Color.Transparent;
    this.buttonShow2007.Temporary = true;
    this.buttonShow2007.UpdateCounter = 6;
    //
    // buttonHide2007
    //
    this.buttonHide2007.Caption = "Close Windows Search";
    this.buttonHide2007.ControlTag = "2a1559ac-9958-47f2-bae7-141679f598e8";
    this.buttonHide2007.ImageTransparentColor = System.Drawing.Color.Transparent;
    this.buttonHide2007.Temporary = true;
    this.buttonHide2007.UpdateCounter = 4;
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
    //
    // WSUIAddinModule
    //
    this.AddinName = "Outlook Finder";
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
    get {
      return _wsuiBootStraper;
    }
  }

  public bool IsMainUIVisible {
    get;
    set;
  }

  #region my own initialization

  private void Init()
  {
    WSSqlLogger.Instance.LogInfo("Plugin is loading...");
    outlookFormManager.ADXBeforeFolderSwitchEx += outlookFormManager_ADXBeforeFolderSwitchEx;

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
    if (!ReferenceEquals(_updatable, null))
    {
      _updatable.Update();
    }
    watch.Stop();
    WSSqlLogger.Instance.LogInfo(string.Format("Check for update: {0}ms", watch.ElapsedMilliseconds));
  }

  private void RunPluginUI()
  {
    DllPreloader.Instance.PreloadDll();
    _wsuiBootStraper = new PluginBootStraper();
    _wsuiBootStraper.Run();
    _eventAggregator = _wsuiBootStraper.Container.Resolve<IEventAggregator>();
    ((AppEmpty)System.Windows.Application.Current).MainControl = (System.Windows.Controls.UserControl)_wsuiBootStraper.View;
    if (_wsuiBootStraper.View is IWSMainControl)
    {
      (_wsuiBootStraper.View as IWSMainControl).Close += (o, e) =>
      {
        DoHideWebViewPane();
        if (_commandManager != null)
          _commandManager.SetShowHideButtonsEnabling(true, false);
      };
    }
    CreateCommandManager();
    SetEventAggregatorToManager();
    SubscribeToEvents();
  }

  private void SubscribeToEvents()
  {
    if (_eventAggregator == null)
      return;
    _eventAggregator.GetEvent<WSUIOpenWindow>().Subscribe(ShowUI);
    _eventAggregator.GetEvent<WSUIHideWindow>().Subscribe(HideUI);
    _eventAggregator.GetEvent<WSUISearch>().Subscribe(StartSearch);
  }

  private void ShowUI(bool show)
  {
    DoShowWebViewPane();
  }

  private void HideUI(bool hide)
  {
    DoHideWebViewPane();
  }

  private void StartSearch(string criteria)
  {
    PassSearchActionToSearchEngine(criteria);
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

          if (_commandManager != null)
            _commandManager.SetShowHideButtonsEnabling(true, false);
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
    _wsuiBootStraper.PassAction(new WSAction(WSActionType.Show, null));
  }

  public void DoHideWebViewPane()
  {
    if (!ExistsVisibleForm(formWebPaneItem))
      return;
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
    _wsuiBootStraper.PassAction(new WSAction(WSActionType.Hide, null));
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
    return folder != null ? GetFullFolderName(folder) : string.Empty;
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
      if (_outlookVersion == 2000 || _outlookVersion == 2002)//|| _outlookVersion == 2007 || _outlookVersion == 2010
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
            Marshal.Release(ifolder);
            ifolder = IntPtr.Zero;
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

  private Tuple<string, string> GetFullNameOfCurrentFolder()
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
    return default(Tuple<string, string>);
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

  private void WSUIAddinModule_AddinStartupComplete(object sender, EventArgs e)
  {
    CheckUpdate();
    this.SendMessage(WM_LOADED, IntPtr.Zero, IntPtr.Zero);
    OutlookPreviewHelper.Instance.OutlookApp = OutlookApp;
    OutlookHelper.Instance.OutlookApp = OutlookApp;
    WSSqlLogger.Instance.LogInfo("WSUI AddinModule Startup Complete...");
  }

  void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
  {
    WSSqlLogger.Instance.LogError("Unhandled Exception (plugin): {0}", e.ExceptionObject.ToString());
  }

  private void CurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs firstChanceExceptionEventArgs)
  {
    WSSqlLogger.Instance.LogError("First Chance Exception (plugin): {0}\n{1}", firstChanceExceptionEventArgs.Exception.Message, firstChanceExceptionEventArgs.Exception.StackTrace);
    if (firstChanceExceptionEventArgs.Exception is ReflectionTypeLoadException)
    {

      foreach (var item in (firstChanceExceptionEventArgs.Exception as ReflectionTypeLoadException).LoaderExceptions)
      {
        WSSqlLogger.Instance.LogError("Reflection Type Load: {0}", item.Message.ToString());
      }
    }
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
    DoShowWebViewPane();
    if (!adxMainPluginCommandBar.UseForRibbon && this.HostMajorVersion > 12)
    {
      buttonShow2007.Enabled = false;
      buttonHide2007.Enabled = true;
    }

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

  private void OutlookFinderEvents_Quit(object sender, EventArgs e)
  {
    DoHideWebViewPane();
    _wsuiBootStraper.PassAction(new WSAction(WSActionType.Quit,null));
    WSSqlLogger.Instance.LogInfo("Shutdown...");
  }

}
}

