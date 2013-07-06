using System;
using System.Collections;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Threading;
using System.Xml.Linq;
using AddinExpress.MSO;
using C4F.DevKit.PreviewHandler.Service;
using WSUIOutlookPlugin.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;
using System.Globalization;
using System.Reflection;
using WSUI.Control;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using C4F.DevKit.PreviewHandler.Service.Logger;
using System.IO;
using AddinExpress.OL;
using System.Security.Principal;
using System.Threading;
using WSUIOutlookPlugin.Core;

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
        private string _instalatonUrl = string.Empty;
        private IUpdatable _updatable = null;

        

        #region [const]
        
        private const string ADXHTMLFileName = "ADXOlFormGeneral.html";
        private const int WM_USER = 0x0400;
        private ADXOlFormsCollectionItem wpfHostForm;
        private const int WM_LOADED = WM_USER + 1001;


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
            WSSqlLogger.Instance.LogInfo(string.Format("WSUIAddinModule [ctor]: {0}ms", watch.ElapsedMilliseconds));
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.FirstChanceException += CurrentDomainOnFirstChanceException;

        }

        private void WSUIAddinModule_OnSendMessage(object sender, ADXSendMessageEventArgs e)
        {
            switch(e.Message)
            {
                case WM_LOADED:
                    lock (this)
                    {
                        PrecreateForm(null);
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
        private ElementHost hostElementFake;
        
 
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
            this.hostElementFake = new System.Windows.Forms.Integration.ElementHost();
            this.wpfHostForm = new AddinExpress.OL.ADXOlFormsCollectionItem(this.components);
            // 
            // outlookFormManager
            // 
            this.outlookFormManager.Items.Add(this.formWebPaneItem);
            this.outlookFormManager.Items.Add(this.wpfHostForm);
            this.outlookFormManager.SetOwner(this);
            // 
            // formWebPaneItem
            // 
            this.formWebPaneItem.Cached = AddinExpress.OL.ADXOlCachingStrategy.OneInstanceForAllFolders;
            this.formWebPaneItem.ExplorerLayout = AddinExpress.OL.ADXOlExplorerLayout.WebViewPane;
            this.formWebPaneItem.FormClassName = "WSUIOutlookPlugin.WSUIForm";
            this.formWebPaneItem.UseOfficeThemeForBackground = true;
            // 
            // wsuiTab
            // 
            this.wsuiTab.Caption = "Windows Search";
            this.wsuiTab.Controls.Add(this.managingCtrlGroup);
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
            // hostElementFake
            // 
            this.hostElementFake.Location = new System.Drawing.Point(0, 0);
            this.hostElementFake.Name = "hostElementFake";
            this.hostElementFake.Size = new System.Drawing.Size(200, 100);
            this.hostElementFake.TabIndex = 0;
            this.hostElementFake.Child = null;
            // 
            // wpfHostForm
            // 
            this.wpfHostForm.ExplorerLayout = AddinExpress.OL.ADXOlExplorerLayout.BottomSubpane;
            this.wpfHostForm.FormClassName = "WSUIOutlookPlugin.WPFHost";
            this.wpfHostForm.RegionBorder = AddinExpress.OL.ADXRegionBorderStyle.None;
            this.wpfHostForm.UseOfficeThemeForBackground = true;
            // 
            // WSUIAddinModule
            // 
            this.AddinName = "WS Plugin (Windows  Search)";
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

        #region my own initialization

        private void Init()
        {
            WSSqlLogger.Instance.LogInfo("Plugin is loading...");
            outlookFormManager.ADXBeforeFolderSwitchEx += outlookFormManager_ADXBeforeFolderSwitchEx;
            buttonShow.OnClick += buttonShow_OnClick;
            buttonClose.OnClick += buttonClose_OnClick;

            if (System.Windows.Application.Current == null)
            {
                new AppEmpty();
            }
            if (_updatable == null)
            {
                _updatable = UpdateHelper.Instance;
                _updatable.Module = this;
            }
            CheckUpdate();
            DllPreloader.Instance.PreloadDll();
        }

        #endregion

        private void CheckUpdate()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            
            if (!ReferenceEquals(_updatable,null) && _updatable.CanUpdate())
            {   
                if(!_updatable.IsUpdating())
                {
                    _updatable.Lock();
                   _updatable.RunSilentUpdate();
                }
            }
            else
            {
                if (_updatable.IsUpdating())
                {
                     WSSqlLogger.Instance.LogInfo("Updating is running. Just update installation info and delete lock file...");
                    _updatable.UpdateInstalationInfo();
                    _updatable.Unlock();
                }
                WSSqlLogger.Instance.LogInfo(string.Format("Can update = {0}", _updatable != null && _updatable.CanUpdate()));
                WSSqlLogger.Instance.LogInfo("Not updatable...");
            }
            watch.Stop();
            WSSqlLogger.Instance.LogInfo(string.Format("Check for update: {0}ms",watch.ElapsedMilliseconds));
        }

        private void PrecreateForm(object state)
        {
            WSSqlLogger.Instance.LogInfo("Design Mode: " + this.DesignMode.ToString());
            if (this.DesignMode || formWebPaneItem == null)
                return;
            try
            {
                // get internal field
                var field = typeof(ADXOlFormsCollectionItem).GetField("formInstances",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance |
                                                                              BindingFlags.CreateInstance);
                Assembly assembly = typeof (WSUIAddinModule).Assembly;
                if (field != null && assembly != null)
                {
                    var val = (IList)field.GetValue(formWebPaneItem);
                    var form = (ADXOlForm) assembly.CreateInstance(formWebPaneItem.FormClassName);
                    val.Add(form);
                    // get internal  method
                    var miInitialize = form.GetType().GetMethods( BindingFlags.NonPublic |
                                                                                     BindingFlags.Instance |
                                                                                     BindingFlags.CreateInstance).Where(mi => mi.Name == "Initialize" && mi.GetParameters().Count() == 2).ToList();

                    /// get internal property
                    PropertyInfo piFormsManager = typeof(ADXOlFormsCollectionItem).GetProperty("FormsManager",
                                                                              BindingFlags.NonPublic |
                                                                              BindingFlags.Instance |
                                                                              BindingFlags.CreateInstance);
                    var formsManagerValue = piFormsManager.GetValue(formWebPaneItem,null);
                    if (miInitialize.Any() && formsManagerValue != null)
                    {
                        miInitialize.ElementAt(0).Invoke(form, new object[] {formsManagerValue, formWebPaneItem});
                    }
                }
 
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
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
                    }
                }
            }
        }

        public void DoShowWebViewPane()
        {
            string currentFullFolderName = GetFullNameOfCurrentFolder();
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
                        RefreshCurrentFolder();
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

        private void RefreshCurrentFolder()
        {
            _refreshCurrentFolderExecuting = true;
            try
            {
                Outlook.Explorer activeExplorer = (OutlookApp as Outlook._Application).ActiveExplorer();
                Outlook.MAPIFolder currentFolder = activeExplorer.CurrentFolder;
                Outlook.NameSpace nameSpace = (OutlookApp as Outlook._Application).GetNamespace("MAPI");
                Outlook.MAPIFolder outboxFolder = nameSpace.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderOutbox);
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
            if (_outlookVersion == 2000 || _outlookVersion == 2002)
                Explorer.CurrentFolder = Folder;
            else
                Explorer.GetType().InvokeMember("SelectFolder", BindingFlags.InvokeMethod, null, Explorer, new object[] { Folder });
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

        private string GetFullNameOfCurrentFolder()
        {
            Outlook.Explorer activeExplorer = (OutlookApp as Outlook._Application).ActiveExplorer();
            if (activeExplorer != null)
                try
                {
                    Outlook.MAPIFolder currentFolder = activeExplorer.CurrentFolder;
                    if (currentFolder != null)
                        try
                        {
                            return GetFullFolderName(currentFolder);
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
            return string.Empty;
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
        }

        private void buttonClose_OnClick(object sender, IRibbonControl control, bool pressed)
        {
            DoHideWebViewPane();
        }

        private void WSUIAddinModule_AddinStartupComplete(object sender, EventArgs e)
        {
            Outlook.NameSpace oNS = null;
            Outlook.MAPIFolder ppallf= null;
            Outlook.MAPIFolder pf = null;
            Outlook.MAPIFolder fs = null;
            try
            {
                oNS = OutlookApp.GetNamespace("mapi");
                //ppallf = oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olPublicFoldersAllPublicFolders);
                 pf = oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
                 //fs = oNS.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderSyncIssues);
                if (ppallf != null && pf != null && fs != null)
                {
                    SaveDefaultFoldersEntryIDToRegistry(pf.EntryID, "", "");
                }
            }
            finally
            {
                if (oNS != null)
                    Marshal.ReleaseComObject(oNS);
                if (ppallf != null)
                    Marshal.ReleaseComObject(ppallf);
                if (pf != null)
                    Marshal.ReleaseComObject(pf);
                if (fs != null)
                    Marshal.ReleaseComObject(fs);
             
                this.SendMessage(WM_LOADED,IntPtr.Zero,IntPtr.Zero);
                WSSqlLogger.Instance.LogInfo("WSUI AddinModule Startup Complete...");
            }
            
        }

        internal void SaveDefaultFoldersEntryIDToRegistry(string PublicFoldersEntryID,
                string PublicFoldersAllPublicFoldersEntryID,
                string FolderSyncIssuesEntryID)
        {
            RegistryKey ModuleKey = null;
            RegistryKey ADXXOLKey = null;
            RegistryKey WebViewPaneSpecialFoldersKey = null;
            try
            {
                ModuleKey = Registry.CurrentUser.OpenSubKey(this.RegistryKey, true);
                if (ModuleKey != null)
                {
                    ADXXOLKey = ModuleKey.CreateSubKey("ADXXOL");
                    if (ADXXOLKey != null)
                    {
                        WebViewPaneSpecialFoldersKey =
                            ADXXOLKey.CreateSubKey
                            ("FoldersForExcludingFromUseWebViewPaneLayout");
                        if (WebViewPaneSpecialFoldersKey != null)
                        {
                            if (PublicFoldersEntryID.Length >= 0)
                            {
                                WebViewPaneSpecialFoldersKey.
                                    SetValue("PublicFolders",
                                    PublicFoldersEntryID);
                            }
                            if (PublicFoldersAllPublicFoldersEntryID.Length >= 0)
                            {
                                WebViewPaneSpecialFoldersKey.
                                    SetValue("PublicFoldersAllPublicFolders",
                                    PublicFoldersAllPublicFoldersEntryID);
                            }
                            if (FolderSyncIssuesEntryID.Length >= 0)
                            {
                                WebViewPaneSpecialFoldersKey.
                                    SetValue("FolderSyncIssues",
                                    FolderSyncIssuesEntryID);
                            }
                        }
                    }
                }
            }
            finally
            {
                if (ModuleKey != null)
                {
                    ModuleKey.Close();
                }
                if (WebViewPaneSpecialFoldersKey != null)
                {
                    WebViewPaneSpecialFoldersKey.Close();
                }
                if (ADXXOLKey != null)
                {
                    ADXXOLKey.Close();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            
            if (formWebPaneItem.Collection.Count > 0)
            {
                formWebPaneItem.Collection.OfType<WSUIForm>().ToList().ForEach(frm =>
                                                                                   {
                                                                                       if (frm is ICleaneable)
                                                                                       {
                                                                                           (frm as ICleaneable).Clean();
                                                                                       }
                                                                                   });
            }
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            WSSqlLogger.Instance.LogError("Unhandled Exception (plugin): " + e.ExceptionObject.ToString());
        }

        private void CurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs firstChanceExceptionEventArgs)
        {
            WSSqlLogger.Instance.LogError("First Chance Exception (plugin): " + firstChanceExceptionEventArgs.Exception.Message);
        }


    }
}

