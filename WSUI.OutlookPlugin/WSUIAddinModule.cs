using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Windows.Forms;
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

        #region elements fo manifest
        private const string InstallPathNodeName = "installationUrl";
        private const string VersionNodeName = "version";
        private const string ProductCodeNodeName = "productCode";
        #endregion

        private const string UpdatedFilename = "WSUI.OutlookPluginSetup.msi";
        private const string ManifestFilename = "adxloader.dll.manifest";
        private const string VersionFilename = "version_info.xml";
        private const string TempFolder = "{0}Update\\{1}";
        private const string TempFolderCreate = "{0}Update";

        private const string ShadowCopyFolder = "{0}\\Shadow";

        private const string ADXHTMLFileName = "ADXOlFormGeneral.html";

        public WSUIAddinModule()
        {
            AppDomain.CurrentDomain.SetupInformation.ShadowCopyFiles = "false"; 
            Application.EnableVisualStyles();
            InitializeComponent();
            // Please add any initialization code to the AddinInitialize event handler
            Init();
        }

        private AddinExpress.OL.ADXOlFormsManager outlookFormManager;
        private AddinExpress.OL.ADXOlFormsCollectionItem formWebPaneItem;
        private ADXRibbonTab wsuiTab;
        private ADXRibbonGroup managingCtrlGroup;
        private ADXRibbonButton buttonShow;
        private ADXRibbonButton buttonClose;
        private Thread taskUpdate;
 
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
            // 
            // outlookFormManager
            // 
            this.outlookFormManager.Items.Add(this.formWebPaneItem);
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
            CheckUpdate();
        }

        #endregion


        private void CheckUpdate()
        {
            var isMSIDep = this.IsMSINetworkDeployed();
            var isMSIUpdatable = this.IsMSIUpdatable();

            if (isMSIDep && isMSIUpdatable)
            {
                taskUpdate = new Thread(new ThreadStart(SilentUpdate));
                taskUpdate.Start();
            }
            else
            {
                WSSqlLogger.Instance.LogInfo(string.Format("IsMSIDep = {0}; IsMSIUpdatable = {1}", isMSIDep, isMSIUpdatable));
                WSSqlLogger.Instance.LogInfo("Not updatable...");
            }
        }

        private void SilentUpdate()
        {
            try
            {
                WSSqlLogger.Instance.LogInfo("Check for updates...");
                string url = this.CheckForMSIUpdates();
                WSSqlLogger.Instance.LogInfo("End checking updates...");
                if (string.IsNullOrEmpty(url))
                {
                    WSSqlLogger.Instance.LogInfo("No updates...");
                    return;
                }
                
                string filename = url.Substring(0, url.LastIndexOf('/') + 1);
                filename = filename + UpdatedFilename;
                WSSqlLogger.Instance.LogInfo(string.Format("File: {0}...", filename));
                string path = Assembly.GetAssembly(typeof(WSUIAddinModule)).Location;
                path = path.Substring(0, path.LastIndexOf('\\') + 1);
                string shadow = string.Format(ShadowCopyFolder, path);
                _instalatonUrl = GetInstalationPath(path);
                WSSqlLogger.Instance.LogInfo(string.Format("Instalation Url: {0}...", _instalatonUrl));
                CreateTempFolder(string.Format(TempFolderCreate, path));
                //CreateTempFolder(shadow);
                string localmsi = string.Format(TempFolder, path,UpdatedFilename);
                WSSqlLogger.Instance.LogInfo(string.Format("Msi local path: {0}...", localmsi));
                string localversion = string.Format(TempFolder, path, VersionFilename);
                WSSqlLogger.Instance.LogInfo(string.Format("Version local path: {0}...", localversion));
                string urlVersion = _instalatonUrl + VersionFilename;
                WSSqlLogger.Instance.LogInfo(string.Format("Version Url: {0}...",urlVersion));

                WebClient webClient = new WebClient();
                WSSqlLogger.Instance.LogInfo("Download update...");
                webClient.DownloadFile(filename, localmsi);
                webClient.DownloadFile(urlVersion,localversion);

                Process process = new Process();
                process.StartInfo.FileName = "msiexec.exe";
                process.StartInfo.Arguments = string.Format(" /i \"{0}\" /qb /norestart /log {1}install.log ", localmsi,path); //REINSTALL=\"ALL\"
                process.StartInfo.Verb = "runas";
                WSSqlLogger.Instance.LogInfo(string.Format("TARGETDIR = {0}",shadow));
                //process.StartInfo.Arguments = string.Format(" /a \"{0}\" /qb TARGETDIR={1} ", localmsi,shadow);
                WSSqlLogger.Instance.LogInfo("Installing update...");
                process.Start();
                process.WaitForExit();
                WSSqlLogger.Instance.LogInfo(string.Format("Exit code: {0}",process.ExitCode));
                Copy(shadow,path);
                if(process.ExitCode == 0)
                    UpdatedInstallationInfo(path,localversion);
                DeleteTempFolder(string.Format(TempFolderCreate, path));
                WSSqlLogger.Instance.LogInfo("Update is done...");
            }
            catch(Exception ex)
            {
                WSSqlLogger.Instance.LogInfo(string.Format("Exception during updates: {0}...",ex.Message));
            }
        }

        void Copy(string sourceDir, string targetDir)
        {
            Directory.CreateDirectory(targetDir);
            foreach (var file in Directory.GetFiles(sourceDir))
                try
                {
                    File.Copy(file, Path.Combine(targetDir, Path.GetFileName(file)));
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError(ex.Message);
                }
        }

        private bool IsAdmin()
        {
            WindowsIdentity current = WindowsIdentity.GetCurrent();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(current);
            return windowsPrincipal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        private string GetInstalationPath(string localpath)
        {
            string manifest = localpath + ManifestFilename;
            XDocument doc = XDocument.Load(manifest);
            var msi = doc.Descendants("msi").First();

            if (msi.Descendants(InstallPathNodeName).Count() > 0)
            {
                var installPath = msi.Descendants(InstallPathNodeName).First();
                return installPath.Value;
            }
            return string.Empty;
        }

        private void UpdatedInstallationInfo(string localpath, string localversion)
        {
            string manifest = localpath + ManifestFilename;
            XDocument docManifest = XDocument.Load(manifest);
            XDocument docVersion = XDocument.Load(localversion);

            var listVersions = docVersion.Descendants("version").Select(el => el.Attribute("name") != null ? el.Attribute("name").Value : string.Empty).ToList();
            listVersions.Sort();
            string newversion = listVersions[listVersions.Count - 1];
            var element = docVersion.Descendants("version").Where(el => el.Attribute("name").Value == newversion).First();
            var productCode = element.Attribute(ProductCodeNodeName).Value;


            var msi = docManifest.Descendants("msi").First();
            if (msi.Descendants(VersionNodeName).Count() > 0)
            {
                var el = msi.Descendants(VersionNodeName).First();
                el.Value = newversion;
                WSSqlLogger.Instance.LogInfo(string.Format("New version: {0}",newversion));
            }
            if (msi.Descendants(ProductCodeNodeName).Count() > 0)
            {
                var el = msi.Descendants(ProductCodeNodeName).First();
                el.Value = productCode;
                WSSqlLogger.Instance.LogInfo(string.Format("Product code: {0}", productCode));
            }
            docManifest.Save(manifest);
        }

        private void CreateTempFolder(string temp)
        {
            if (!Directory.Exists(temp))
            {
                Directory.CreateDirectory(temp);
            }
        }

        private void DeleteTempFolder(string temp)
        {
            if (Directory.Exists(temp))
            {
                Directory.Delete(temp, true);
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

    }
}

