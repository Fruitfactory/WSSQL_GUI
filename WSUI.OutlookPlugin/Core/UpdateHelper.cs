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
using OF.Core.Helpers;
using OF.Core.Logger;
using OFOutlookPlugin.Interfaces;
using Timer = System.Timers.Timer;

namespace OFOutlookPlugin.Core
{
    public class UpdateHelper : IUpdatable
    {
        #region [const]

        private const string OFPluginUpdatetingMutexName = "Global\\OFPluginUpdating";

        private const string UpdatedFilename = "OutlookFinderSetup.msi";
        private const string ManifestFilename = "adxloader.dll.manifest";
        private const string VersionFilename = "version_info.xml";
        private const string TempFolder = "{0}\\Update\\{1}";
        private const string TempFolderCreate = "{0}\\Update";

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

#if !TRIAL
        private const string InstalationUrl = "http://outlookfinder.com/downloads/clicktwice/full/";
        //private const string InstalationUrl = "http://readyshare.routerlogin.net/shares/NetShare/FTP/downloads/clicktwice/full/";
#else
        private const string InstalationUrl = "http://outlookfinder.com/downloads/clicktwice/trial/";
        //private const string InstalationUrl = "http://readyshare.routerlogin.net/shares/NetShare/FTP/downloads/clicktwice/trial/";
#endif

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
            _path = Assembly.GetAssembly(typeof(OFAddinModule)).Location;
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
            OFLogger.Instance.LogDebug("Module  != null: {0}",Module != null);
            if (Module != null)
            {
                OFLogger.Instance.LogDebug("Module.IsMSINetworkDeployed(): {0}", Module.IsMSINetworkDeployed());
                OFLogger.Instance.LogDebug("Module.IsMSIUpdatable(): {0}", Module.IsMSIUpdatable());    
            }
            return Module != null && Module.IsMSINetworkDeployed() && Module.IsMSIUpdatable();
        }

        public void Lock()
        {
            try
            {
                File.Create(string.Format("{0}{1}", _path, LocFilename)).Close();
                RegistryHelper.Instance.StartSilentUpdate();
                RegistryHelper.Instance.SetCallIndexKey(RegistryHelper.CallIndex.First);
                OFLogger.Instance.LogError("Lock silent update !!!!");
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("Lock: " + ex.Message);
            }
            
        }

        public void Unlock()
        {
            try
            {
            	File.Delete(string.Format("{0}{1}", _path, LocFilename));
                RegistryHelper.Instance.FinishSilentUpdate();
                OFLogger.Instance.LogError("Unlock silent update !!!!");
            }
            catch (System.Exception ex)
            {
            	OFLogger.Instance.LogError("Unlock: " + ex.Message);
            }
        }

        public void Update()
        {
            _updateTimer = new Timer(IntervalForUpdate);
            _updateTimer.Elapsed += UpdateTimerOnElapsed;
            _updateTimer.Start();
        }

        public void DeleteTempoparyFolders()
        {
            string temp = GetTempUserPath();
            DeleteTempFolder(string.Format(TempFolderCreate, temp));
        }

        #endregion

        private void SilentUpdate()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            SynchronizationContext.SetSynchronizationContext(new DispatcherSynchronizationContext(Dispatcher.CurrentDispatcher));
            try
            {
                OFLogger.Instance.LogDebug("Check for updates...");
                string url = Module.CheckForMSIUpdates();
                OFLogger.Instance.LogDebug("End checking updates...");
                OFLogger.Instance.LogDebug("Url after checking: " + url);
                if (String.IsNullOrEmpty(url))
                {
                    Unlock();
                    OFLogger.Instance.LogDebug("No updates...");
                    return;
                }

                string filename = url.Substring(0, url.LastIndexOf('/') + 1);
                filename = filename + UpdatedFilename;
                OFLogger.Instance.LogDebug(String.Format("File: {0}...", filename));
                string shadow = string.Format(ShadowCopyFolder, _path);
                _instalatonUrl = GetInstalationPath(_path);
                OFLogger.Instance.LogDebug(string.Format("Instalation Url: {0}...", _instalatonUrl));
                string temp = GetTempUserPath();
                CreateTempFolder(string.Format(TempFolderCreate, temp));
                //CreateTempFolder(shadow);
                string localmsi = string.Format(TempFolder, temp, UpdatedFilename);
                OFLogger.Instance.LogDebug(String.Format("Msi local path: {0}...", localmsi));
                string localversion = string.Format(TempFolder, temp, VersionFilename);
                OFLogger.Instance.LogDebug(String.Format("Version local path: {0}...", localversion));
                string urlVersion = _instalatonUrl + VersionFilename;
                OFLogger.Instance.LogDebug(String.Format("Version Url: {0}...", urlVersion));

                WebClient webClient = new WebClient();
                OFLogger.Instance.LogDebug("Download update...");
                webClient.DownloadFile(filename, localmsi);
                webClient.DownloadFile(urlVersion, localversion);

                Process process = new Process();
                process.StartInfo.FileName = "msiexec.exe";
                process.StartInfo.Arguments = String.Format(" /i \"{0}\" /qb /norestart /log {1}install.log ", localmsi, _path); //REINSTALL=\"ALL\"
                process.StartInfo.Verb = "runas";
                OFLogger.Instance.LogDebug("Installing update...");
                process.Start();
                process.WaitForExit();
                //Copy(shadow, path);
                OFLogger.Instance.LogDebug(process.ExitCode == 0
                                                 ? "Update is done..."
                                                 : "Something wrong. Check exit code...");
                OFLogger.Instance.LogDebug(String.Format("Exit code: {0}", process.ExitCode));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(String.Format("Exception during updates: {0}...", ex.Message));
            }
            finally
            {
                watch.Stop();
                OFLogger.Instance.LogDebug(String.Format("Silent updated lasts: {0}ms", watch.ElapsedMilliseconds));
            }
        }

        private string GetInstalationPath(string localpath)
        {
            string manifest = localpath + UpdateHelper.ManifestFilename;
            XDocument doc = XDocument.Load(manifest);
            var msi = doc.Descendants("msi").FirstOrDefault();
            if (msi == null)
                return string.Empty;
            
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

        private string GetTempUserPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        private void UpdateTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_updateTimer == null)
                return;
            _updateTimer.Elapsed -= UpdateTimerOnElapsed;
            _updateTimer.Stop();
            _updateTimer.Dispose();
            _updateTimer = null;
            UpdateOnTimer();
        }

        private void UpdateOnTimer()
        {
            if (!IsMsiNodePresent())
            {
                OFLogger.Instance.LogWarning("MSI node is absent...");
                CreateMSINode();
            }

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
                OFLogger.Instance.LogDebug(string.Format("Can update = {0}", CanUpdate()));
                OFLogger.Instance.LogInfo("Not updatable...");
            }
        }

        private void CreateMSINode()
        {
            string remoteFile = Path.Combine(InstalationUrl, VersionFilename);
            string localFile = Path.Combine(_path, VersionFilename);
            WebClient web = new WebClient();
            try
            {
                web.DownloadFile(remoteFile, localFile);
                if (!File.Exists(localFile))
                    return;
                XDocument docVersion = XDocument.Load(localFile);
                XDocument docManifest = XDocument.Load(Path.Combine(_path,ManifestFilename));

                string language = docVersion.Descendants("product").First().Attribute("language").Value;
                var listVersions =
                    docVersion.Descendants("version").Select(
                        el => el.Attribute("name") != null ? el.Attribute("name").Value : string.Empty).ToList();
                var sortedList = listVersions.Select(s => s.Split('.').Select(str => int.Parse(str)).ToArray())
                .OrderBy(arr => arr[0])
                .ThenBy(arr => arr[1])
                .ThenBy(arr => arr[2])
                .Select(arr => string.Join(".", arr)).ToList();
                string newversion = sortedList[sortedList.Count - 1];
                var element = docVersion.Descendants("version").Where(el => el.Attribute("name").Value == newversion).First();
                CreateAndAddNode(docManifest,element,language);
                docManifest.Save(Path.Combine(_path, ManifestFilename));
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError("CreateMSINode: {0}",ex.Message);
            }
            finally
            {
                if (web != null)
                {
                    web.Dispose();
                    web = null;
                }
                if(File.Exists(localFile))
                    File.Delete(localFile);
            }
        }

        private void CreateAndAddNode(XDocument docManifest, XElement elementNewestVersion, string language)
        {
            XElement msi = new XElement(MSINode);
            string prodCode = elementNewestVersion.Attribute(ProductCodeNode).Value;
            string installUrl = elementNewestVersion.Attribute(InstallPathNodeName).Value;
            string newVersion = elementNewestVersion.Attribute("name").Value;
            XElement pc = new XElement(ProductCodeNode, prodCode);
            XElement iu = new XElement(InstallPathNodeName, installUrl);
            XElement ver = new XElement(VersionNode, newVersion);
            XElement lan = new XElement(LanguageNode, language);
            msi.Add(pc, iu, ver, lan);
            docManifest.Root.Add(msi);
        }

        private bool IsMsiNodePresent()
        {
            return !string.IsNullOrEmpty(GetInstalationPath(_path));
        }
    }
}