using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Timers;
using System.Windows.Threading;
using System.Xml.Linq;
using AddinExpress.MSO;
using WSUI.Core.Helpers;
using WSUI.Core.Logger;
using WSUIOutlookPlugin.Interfaces;
using Timer = System.Timers.Timer;

namespace WSUIOutlookPlugin.Core
{
    public class UpdateHelper : IUpdatable
    {
        #region [const]

        private const string WSUIPluginUpdatetingMutexName = "Global\\WSUIPluginUpdating";

        private const string UpdatedFilename = "WSUI.OutlookPluginSetup.msi";
        private const string ManifestFilename = "adxloader.dll.manifest";
        private const string VersionFilename = "version_info.xml";
        private const string TempFolder = "{0}Update\\{1}";
        private const string TempFolderCreate = "{0}Update";

        private const string ShadowCopyFolder = "{0}\\Shadow";
        private const string LocFilename = "install.loc";




        //<msi>
        //    <installationUrl>http://fruitfactory.github.io/WSSQL_GUI/downloads/clicktwice/</installationUrl>
        //    <language>1033</language>
        //    <version>1.0.0</version>
        //    <productCode>{59D4722B-CF84-4019-A70A-E027430514A7}</productCode>
        //</msi>

        private const string MSINode = "msi";
        private const string IstallationUrlNode = "installationUrl";
        private const string LanguageNode = "language";
        private const string VersionNode = "version";
        private const string ProductCodeNode = "productCode";
        private const int IntervalForUpdate = 30*1000;

        #endregion

        #region elements fo manifest
        private const string InstallPathNodeName = "installationUrl";
        private const string VersionNodeName = "version";
        private const string ProductCodeNodeName = "productCode";
        #endregion

        #region [private]

        private Thread _taskUpdate;
        private string _instalatonUrl = string.Empty;
        private string _path = string.Empty;
        private Timer _updateTimer = null;

        #endregion

        #region [static]
        private static readonly Lazy<IUpdatable> _instance = new Lazy<IUpdatable>(() =>
                                                                                      {
                                                                                          UpdateHelper upd = new UpdateHelper();
                                                                                          upd.Init();
                                                                                          return upd;
                                                                                      });

        public static IUpdatable Instance
        {
            get { return _instance.Value; }
        }
        #endregion

        #region [ctor]

        private UpdateHelper()
        {
                    
        }

        #endregion

        private void Init()
        {
            _path = Assembly.GetAssembly(typeof(WSUIAddinModule)).Location;
            _path = _path.Substring(0, _path.LastIndexOf('\\') + 1);
        }

        #region Implementation of IUpdatable

        public ADXAddinModule Module { get; set; }

        public bool IsUpdating()
        {
            return File.Exists(string.Format("{0}{1}",_path,LocFilename));
        }

        public void RunSilentUpdate()
        {
            _taskUpdate = new Thread(new ThreadStart(SilentUpdate));
            _taskUpdate.Start();
        }

        public bool CanUpdate()
        {
            return Module != null && Module.IsMSINetworkDeployed() && Module.IsMSIUpdatable();

        }

        public void Lock()
        {
            try
            {
                File.Create(string.Format("{0}{1}", _path, LocFilename)).Close();
                RegistryHelper.Instance.StartSilentUpdate();
                RegistryHelper.Instance.SetCallIndexKey(RegistryHelper.CallIndex.First);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError("Lock: " + ex.Message);
            }
            
        }

        public void Unlock()
        {
            try
            {
            	File.Delete(string.Format("{0}{1}", _path, LocFilename));
                RegistryHelper.Instance.FinishSilentUpdate();
                DeleteTempFolder(string.Format(TempFolderCreate, _path));
            }
            catch (System.Exception ex)
            {
            	WSSqlLogger.Instance.LogError("Unlock: " + ex.Message);
            }
        }

        public void Update()
        {
            _updateTimer = new Timer(IntervalForUpdate);
            _updateTimer.Elapsed += UpdateTimerOnElapsed;
            _updateTimer.Start();
        }

        #endregion

        private void SilentUpdate()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            try
            {
                WSSqlLogger.Instance.LogInfo("Check for updates...");
                string url = Module.CheckForMSIUpdates();
                WSSqlLogger.Instance.LogInfo("End checking updates...");
                WSSqlLogger.Instance.LogInfo("Url after checking: " + url);
                if (String.IsNullOrEmpty(url))
                {
                    Unlock();
                    WSSqlLogger.Instance.LogInfo("No updates...");
                    return;
                }

                string filename = url.Substring(0, url.LastIndexOf('/') + 1);
                filename = filename + UpdatedFilename;
                WSSqlLogger.Instance.LogInfo(String.Format("File: {0}...", filename));
                string shadow = string.Format(ShadowCopyFolder, _path);
                _instalatonUrl = GetInstalationPath(_path);
                WSSqlLogger.Instance.LogInfo(string.Format("Instalation Url: {0}...", _instalatonUrl));
                CreateTempFolder(string.Format(TempFolderCreate, _path));
                //CreateTempFolder(shadow);
                string localmsi = string.Format(TempFolder, _path, UpdatedFilename);
                WSSqlLogger.Instance.LogInfo(String.Format("Msi local path: {0}...", localmsi));
                string localversion = string.Format(TempFolder, _path, VersionFilename);
                WSSqlLogger.Instance.LogInfo(String.Format("Version local path: {0}...", localversion));
                string urlVersion = _instalatonUrl + VersionFilename;
                WSSqlLogger.Instance.LogInfo(String.Format("Version Url: {0}...", urlVersion));

                WebClient webClient = new WebClient();
                WSSqlLogger.Instance.LogInfo("Download update...");
                webClient.DownloadFile(filename, localmsi);
                webClient.DownloadFile(urlVersion, localversion);

                Process process = new Process();
                process.StartInfo.FileName = "msiexec.exe";
                process.StartInfo.Arguments = String.Format(" /i \"{0}\" /qb /norestart /log {1}install.log ", localmsi, _path); //REINSTALL=\"ALL\"
                process.StartInfo.Verb = "runas";
                WSSqlLogger.Instance.LogInfo("Installing update...");
                process.Start();
                process.WaitForExit();
                //Copy(shadow, path);
                WSSqlLogger.Instance.LogInfo(process.ExitCode == 0
                                                 ? "Update is done..."
                                                 : "Something wrong. Check exit code...");
                WSSqlLogger.Instance.LogInfo(String.Format("Exit code: {0}", process.ExitCode));
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogInfo(String.Format("Exception during updates: {0}...", ex.Message));
            }
            finally
            {
                watch.Stop();
                WSSqlLogger.Instance.LogInfo(String.Format("Silent updated lasts: {0}ms", watch.ElapsedMilliseconds));
            }
        }

        private string GetInstalationPath(string localpath)
        {
            string manifest = localpath + UpdateHelper.ManifestFilename;
            XDocument doc = XDocument.Load(manifest);
            var msi = doc.Descendants("msi").First();

            if (msi.Descendants(InstallPathNodeName).Count() > 0)
            {
                var installPath = msi.Descendants(InstallPathNodeName).First();
                return installPath.Value;
            }
            return string.Empty;
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

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            UpdateOnTimer();
            if (_updateTimer == null)
                return;
            _updateTimer.Elapsed -= UpdateTimerOnElapsed;
            _updateTimer.Stop();
            _updateTimer.Dispose();
            _updateTimer = null;
        }

        private void UpdateOnTimer()
        {
            if (CanUpdate())
            {
                if (!IsUpdating())
                {
                    Lock();
                    RunSilentUpdate();
                }
            }
            else
            {
                WSSqlLogger.Instance.LogInfo(string.Format("Can update = {0}", CanUpdate()));
                WSSqlLogger.Instance.LogInfo("Not updatable...");
            }
        }
    }
}