using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Win32;
using OF.CA.ClosePromt;
using OF.CA.DeleteDataPromt;
using OF.CA.DownloadInstallJava;
using OF.CA.EmailValidate;
using OF.Core.Core.LimeLM;
using OF.Core.Helpers;
using wyDay.TurboActivate;
using Exception = System.Exception;
using View = Microsoft.Deployment.WindowsInstaller.View;

namespace OF.CA
{
    public class CustomActions
    {
        #region [needs]

        private const string UpdateFolder = "\\Update";
        private const string LocFilename = "install.loc";
        private const string LogFilename = "install.log";

        private const string OutllokAppName = "outlook";
        private const string OutlookId = "Outlook.Application";

        private const string ManifestFilename = "adxloader.dll.manifest";
        private const string VersionFilename = "version_info.xml";
        private const string TempFolder = "{0}\\Update\\{1}";

        private const string InstallFolder = "INSTALLFOLDER";
        private const string ElasticSearchInstallFolder = "ELASTICSEASRCHINSTALLFOLDER";
        private const string ElasticSearchInstallBinFolder = "ELASTICSEASRCHINSTALLFOLDERBIN";

        private const string ElasticSearchDataFolder = "data";


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

        #endregion [needs]

        #region elements fo manifest

        private const string InstallPathNodeName = "installationUrl";
        private const string VersionNodeName = "version";
        private const string ProductCodeNodeName = "productCode";

        #endregion elements fo manifest





        #region [CA ClearFiles]

        [CustomAction]
        public static ActionResult ClearFiles(Session session)
        {
            try
            {
                string path = GetInstallationFolder(session);
                session.Log("Delete  files...");
                DeleteUpdateFolder(session);
                var list = new List<string> { LocFilename };
                foreach (string filename in list)
                {
                    DeleteFile(session, path, filename);
                }
                DeleteRootFolder(session, path);

                DeleteElasticSearchFiles(session, session[ElasticSearchInstallFolder]);
                DeleteRegistryKeys();
                DeleteUnistallInfo(session);

            }
            catch (Exception)
            {
                return ActionResult.Failure;
            }
            finally
            {
            }
            return ActionResult.Success;
        }

        private static void DeleteRegistryKeys()
        {
            if (OFRegistryHelper.Instance.IsPluginUiVisibleKeyPresent())
            {
                OFRegistryHelper.Instance.DeletePluginUiKey();
            }
        }

        private static void DeleteUnistallInfo(Session session)
        {
            try
            {
                var version = session["ProductVersion"];
                var productName = session["ProductName"];
                OFRegistryHelper.Instance.DeleteUnistallKeyCurrentVersion(version, productName);
            }
            catch (Exception ex)
            {
                session.Log(ex.ToString());
            }
        }

        #endregion [CA ClearFiles]

        private static string GetInstallationFolder(Session session)
        {
            return session[InstallFolder];
        }

        private static string GetElasticSearchBinFolder(Session session)
        {
            return session[ElasticSearchInstallBinFolder];
        }

        private static string GetTempPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        }

        #region [private for delete files]

        private static bool DeleteUpdateFolder(Session session)
        {
            session.Log("DeleteUpdateFolder...");
            string temp = GetTempPath();
            string fullpath = string.Format("{0}{1}", temp, UpdateFolder);
            if (!Directory.Exists(fullpath))
                return false;
            try
            {
                Directory.Delete(fullpath, true);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static bool DeleteRootFolder(Session session, string root)
        {
            string fullpath = string.Format("{0}", root);
            session.Log("Delete Root file...");
            if (!Directory.Exists(fullpath))
                return false;
            try
            {
                Directory.Delete(fullpath, true);
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static bool DeleteFile(Session session, string root, string filename)
        {
            string fullpath = string.Format("{0}{1}", root, filename);
            if (!File.Exists(fullpath))
                return false;
            try
            {
                File.Delete(fullpath);
                session.Log("Delete File...");
            }
            catch
            {
                return false;
            }
            return true;
        }

        private static void DeleteElasticSearchFiles(Session session, string root)
        {
            session.Log("Delete ElasticSearch files...");
            session.Log("ElasticSearch Root => " + root);
            try
            {
                var rootDir = new DirectoryInfo(root);
                bool dataFolderPresent = rootDir.EnumerateDirectories().Any(d => d.Name.ToLowerInvariant().Equals(ElasticSearchDataFolder));
                if (dataFolderPresent)
                {
                    foreach (var enumerateDirectory in rootDir.EnumerateDirectories())
                    {
                        if (enumerateDirectory.Name.ToLowerInvariant().Equals(ElasticSearchDataFolder))
                        {
                            using (var form = new DeleteDataPromtApplication(session["ProductName"]))
                            {
                                var result = form.PromtDeleteFolder();
                                if (!result)
                                    continue;
                            }
                        }
                        session.Log("Deleting " + enumerateDirectory.Name + "...");
                        Directory.Delete(enumerateDirectory.FullName, true);
                    }
                    foreach (var enumerateFile in rootDir.EnumerateFiles())
                    {
                        session.Log("Deleting " + enumerateFile.Name + "...");
                        enumerateFile.Delete();
                    }
                    if (rootDir.GetDirectories().Length == 0 && rootDir.GetFiles().Length == 0)
                    {
                        rootDir.Delete(true);
                    }
                }
                else
                {
                    rootDir.Delete(true);
                }
            }
            catch (Exception ex)
            {
                session.Log(ex.Message);
            }
        }

        #endregion [private for delete files]

        #region [CHECK VERSION]

        [CustomAction]
        public static ActionResult CheckVersion(Session session)
        {

            try
            {
                var version = session["ProductVersion"];
                var productName = session["ProductName"];
                OFRegistryHelper.Instance.DeleteUnistallKeyNotCurrentVersion(version, productName);
            }
            catch (Exception ex)
            {
                session.Log(ex.ToString());
            }
            return ActionResult.Success;
        }

        #endregion


        #region [CA operation with Outlook]


        [CustomAction]
        public static ActionResult ClosePrompt(Session session)
        {
            session.Log("Begin PromptToCloseApplications");
            try
            {
                var productName = session["ProductName"];
                var processes = session["PromptToCloseProcesses"].Split(',');
                var displayNames = session["PromptToCloseDisplayNames"].Split(',');

                if (processes.Length != displayNames.Length)
                {
                    session.Log(@"Please check that 'PromptToCloseProcesses' and 'PromptToCloseDisplayNames' exist and have same number of items.");
                    return ActionResult.Failure;
                }

                DeleteRegistryKeys();
                for (var i = 0; i < processes.Length; i++)
                {
                    session.Log("Prompting process {0} with name {1} to close.", processes[i], displayNames[i]);
                    using (var prompt = new PromptCloseApplication(productName, processes[i], displayNames[i]))
                        if (!prompt.Prompt())
                        {
                            session.Log("User press cancel...");
                            return ActionResult.UserExit;
                        }
                }
            }
            catch (Exception ex)
            {
                session.Log("Missing properties or wrong values. Please check that 'PromptToCloseProcesses' and 'PromptToCloseDisplayNames' exist and have same number of items. \nException:" + ex.ToString());
                return ActionResult.Failure;
            }

            session.Log("End PromptToCloseApplications");
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult OpenOutlook(Session session)
        {
            session.Log("Open outlook. IsOutllokClosedByInstaller = " +
                        OFRegistryHelper.Instance.IsOutlookClosedByInstaller());
            var res = ActionResult.Success;
            if (OFRegistryHelper.Instance.IsOutlookClosedByInstaller() && IsOutlookInstalled())
            {
                try
                {
                    Process.Start("OUTLOOK.EXE");
                    OFRegistryHelper.Instance.ResetFlagClosedOutlookApplication();
                }
                catch (Exception ex)
                {
                    session.Log("Error during open outlook: " + ex.Message);
                    res = ActionResult.Failure;
                }
            }
            return res;
        }

        private static bool IsOutlookOpen()
        {
            return GetAllOutlookInstances().Any();
        }

        private static IEnumerable<Process> GetAllOutlookInstances()
        {
            return Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutllokAppName.ToUpper()));
        }

        private static _Application GetOutlook()
        {
            return Marshal.GetActiveObject(OutlookId) as _Application;
        }

        private static void CloseAllOutlookInstancesLite(Session session)
        {
            foreach (Process allOutlookInstance in GetAllOutlookInstances())
            {
                try
                {
                    _Application app = GetOutlook();
                    if (app != null)
                        app.Quit();
                }
                catch (Exception ex)
                {
                    allOutlookInstance.Kill();
                    session.Log("{0}", ex.Message);
                }
            }
        }

        private static void CloseAllOutlookInstancesHard(Session session)
        {
            foreach (Process allOutlookInstance in GetAllOutlookInstances())
            {
                CloseHardOutlook(allOutlookInstance, session);
            }
        }

        private static bool IsOutlookInstalled()
        {
            const string SubKey = "Software\\microsoft\\windows\\currentversion\\app paths\\OUTLOOK.EXE";
            RegistryKey subKey = Registry.LocalMachine.OpenSubKey(SubKey);
            var path = subKey.GetValue("Path") as string;
            return !string.IsNullOrEmpty(path);
        }

        private static void CloseHardOutlook(Process app, Session session)
        {
            session.Log("Hard close of Outlook");
            try
            {
                session.Log("Kill the Outlook");
                app.Kill();
                session.Log("Good kill the Outlook");
            }
            catch (Exception ex)
            {
                session.Log(ex.Message);
            }
        }

        #endregion [CA operation with Outlook]

        #region [succes message]

        [CustomAction]
        public static ActionResult SuccesMessage(Session session)
        {
            var result = ActionResult.Success;

            if (!OFRegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            //session.Log("Second entry to SuccesMessage...");
            try
            {
                var rec = new Record();
                rec.FormatString = "Update was successful.";
                if (OFRegistryHelper.Instance.IsOutlookClosedByInstaller())
                {
                    rec.FormatString += "\nOutlook will be opened soon.";
                }
                UpdateAndDeleteLock(session);
                session.Message(
                    InstallMessage.User | InstallMessage.Info | (InstallMessage)(MessageBoxIcon.Information) |
                    (InstallMessage)MessageBoxButtons.OK, rec);
                OpenOutlook(session);
            }
            catch (Exception)
            {
                result = ActionResult.NotExecuted;
            }
            return result;
        }

        #endregion [succes message]

        #region [update installation info]

        private static void UpdateAndDeleteLock(Session session)
        {
            string path = GetInstallationFolder(session);
            string temp = GetTempPath();
            try
            {
                string localversion = string.Format(TempFolder, temp, VersionFilename);
                if (File.Exists(localversion))
                {
                    session.Log(String.Format("Version local path: {0}...", localversion));
                    UpdatedInstallationInfo(path, localversion, session);
                }
                else
                {
                    session.Log(String.Format("Version local path: {0} is not exist...", localversion));
                }
                Unlock(path, session);
            }
            catch (Exception ex)
            {
                session.Log("Update Installation Info: " + ex.Message);
            }
        }

        private static void UpdatedInstallationInfo(string localpath, string localversion, Session session)
        {
            try
            {
                string manifest = localpath + ManifestFilename;
                XDocument docManifest = XDocument.Load(manifest);
                XDocument docVersion = XDocument.Load(localversion);

                string language = docVersion.Descendants("product").First().Attribute("language").Value;

                List<string> listVersions =
                    docVersion.Descendants("version").Select(
                        el => el.Attribute("name") != null ? el.Attribute("name").Value : string.Empty).ToList();
                List<string> sortedList = listVersions.Select(s => s.Split('.').Select(str => int.Parse(str)).ToArray())
                    .OrderBy(arr => arr[0])
                    .ThenBy(arr => arr[1])
                    .ThenBy(arr => arr[2])
                    .Select(arr => string.Join(".", arr)).ToList();
                string newversion = sortedList[sortedList.Count - 1];
                XElement element =
                    docVersion.Descendants("version").Where(el => el.Attribute("name").Value == newversion).First();
                string productCode = element.Attribute(ProductCodeNodeName).Value;

                if (docManifest.Descendants(MSINode).Any())
                {
                    XElement msi = docManifest.Descendants("msi").First();
                    UpdateMSINode(msi, productCode, newversion, session);
                }
                else
                {
                    CreateMSINode(docManifest, element, language);
                }

                docManifest.Save(manifest);
            }
            catch (Exception ex)
            {
                session.Log("Update info: " + ex.Message);
            }
        }

        private static void UpdateMSINode(XElement msi, string productCode, string newversion, Session session)
        {
            if (msi.Descendants(VersionNodeName).Count() > 0)
            {
                XElement el = msi.Descendants(VersionNodeName).First();
                el.Value = newversion;
                session.Log(string.Format("New version: {0}", newversion));
            }
            if (msi.Descendants(ProductCodeNodeName).Count() > 0)
            {
                XElement el = msi.Descendants(ProductCodeNodeName).First();
                el.Value = productCode;
                session.Log(string.Format("Product code: {0}", productCode));
            }
        }

        private static void CreateMSINode(XDocument docManifest, XElement elementNewestVersion, string language)
        {
            var msi = new XElement(MSINode);
            string prodCode = elementNewestVersion.Attribute(ProductCodeNode).Value;
            string installUrl = elementNewestVersion.Attribute(InstallPathNodeName).Value;
            string newVersion = elementNewestVersion.Attribute("name").Value;
            var pc = new XElement(ProductCodeNode, prodCode);
            var iu = new XElement(InstallPathNodeName, installUrl);
            var ver = new XElement(VersionNode, newVersion);
            var lan = new XElement(LanguageNode, language);
            msi.Add(pc, iu, ver, lan);
            docManifest.Root.Add(msi);
        }

        public static void Unlock(string path, Session session)
        {
            try
            {
                File.Delete(string.Format("{0}{1}", path, LocFilename));
                DeleteUpdateFolder(session);
                session.Log("Silent updates is finished. Call index set up in default value (None).");
                OFRegistryHelper.Instance.FinishSilentUpdate();
                OFRegistryHelper.Instance.SetCallIndexKey(OFRegistryHelper.CallIndex.None);
            }
            catch (Exception ex)
            {
                session.Log("Unlock: " + ex.Message);
            }
        }

        //private static void DeleteTempFolder(string temp)
        //{
        //    if (Directory.Exists(temp))
        //    {
        //        Directory.Delete(temp, true);
        //    }
        //}

        #endregion [update installation info]

        #region [activate CA]

#if !TRIAL
        private const string ActivateFilesFolder = "ActivatePlugin";
        private const string TurboActivateExeKey = "Turbo_Activate_exe";
        private const string TurboActivateDllKey = "Turbo_Activate_dll";
        private const string TurboActivate64DllKey = "Turbo_Activate64_dll";
        private const string TurboActivateDatKey = "Turbo_Activate_dat";

        private const string TurboActivateExeFilename = "TurboActivate.exe";
        private const string TurboActivateDllFilename = "TurboActivate.dll";
        private const string TurboActivate64DllFilename = "TurboActivate64.dll";
        private const string TurboActivateDatFilename = "TurboActivate.dat";
        private const string TurboActivateTemplate = "TurboActivate";

        private const string QueryTemplate = "SELECT Data FROM Binary WHERE Name = '{0}' ";
        private const int SizeCopy = 1024 * 50;
        private const string EmailPattern = @"\b[A-Z0-9._%+-]+@(?:[A-Z0-9-]+\.)+[A-Z]{2,4}\b";

        [CustomAction]
        public static ActionResult EmailValidation(Session session)
        {
            const string EmailValidProperty = "EMAILVALID";
            try
            {
                var productName = session["ProductName"];
                string email = session["EMAILVALUE"];
                bool isValid = Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
                session.Log("Check Lime...");
                bool isPresent = LimeLMApi.IsEmailPresent(email);
                session.Log("Finish Lime...");
                bool res = isValid && isPresent;

                session.Log("IsValid Email: " + isValid);
                session.Log("IsPresent Email: " + isPresent);

                if (!res)
                {
                    using (var emailForm = new EmailValidateApplication(productName))
                    {
                        var result = emailForm.PromtEmail();
                        session.Log("Result validation: " + result.ToString());
                        if (result)
                        {
                            session[EmailValidProperty] = result.ToString();
                            session.Log(session[EmailValidProperty]);
                            var mail = emailForm.GetEmail();
                            session["EMAILVALUE"] = mail;
                            session.Log("Valid email: " + session["EMAILVALUE"]);
                        }
                        else
                        {
                            session[EmailValidProperty] = result.ToString();
                            session.Log(session[EmailValidProperty]);
                            session["EMAILVALUE"] = string.Empty;
                            session["WIXUI_EXITDIALOGOPTIONALTEXT"] = !isValid
                                ? "Email is not valid."
                                : !isPresent
                                    ? "Please, enter the same email you entered on the website to download the software next time."
                                    : string.Empty;
                            session["EMAILVALIDMESSAGE"] = session["WIXUI_EXITDIALOGOPTIONALTEXT"];
                        }
                    }
                }
                else
                {
                    session["EMAILVALUE"] = email;
                    session[EmailValidProperty] = bool.TrueString;
                    session.Log(session[EmailValidProperty]);
                }
            }
            catch (Exception ex)
            {
                session.Log(ex.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }
        
        private static void ExtractFiles(Session session, string path)
        {
            if (!CopyFileFromDatabase(session, path, TurboActivateDatKey, TurboActivateDatFilename))
                throw new FileLoadException(TurboActivateDatFilename);
            if (!CopyFileFromDatabase(session, path, TurboActivateDllKey, TurboActivateDllFilename))
                throw new FileLoadException(TurboActivateDllFilename);
            if (!CopyFileFromDatabase(session, path, TurboActivate64DllKey, TurboActivate64DllFilename))
                throw new FileLoadException(TurboActivate64DllFilename);
            if (!CopyFileFromDatabase(session, path, TurboActivateExeKey, TurboActivateExeFilename))
                throw new FileLoadException(TurboActivateExeFilename);
        }

        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        private static void RunAndWaitActivator(Session session, string path)
        {
            string file = Path.Combine(path, TurboActivateExeFilename);
            if (!File.Exists(file))
                return;
            try
            {
                var p = new Process
                {
                    StartInfo =
                    {
                        FileName = file
                    },
                    EnableRaisingEvents = true
                };
                p.Exited += POnExited;
                p.Start();
                BringToTopActivateWindow();
                p.WaitForExit();
                session.Log("Exit code {0}", p.ExitCode);
            }
            catch (Exception ex)
            {
                session.Log(string.Format("RunAndActivator: {0}", ex.Message));
            }
        }

        private static void BringToTopActivateWindow()
        {
            Task.Factory.StartNew(SetTop);
        }

        private static void SetTop()
        {
            while (true)
            {
                IEnumerable<Process> list =
                    Process.GetProcesses().Where(p => p.ProcessName.IndexOf(TurboActivateTemplate) > -1);
                if (!list.Any())
                    continue;
                Process ta = list.ElementAt(0);
                if (ta.MainWindowHandle == IntPtr.Zero)
                    continue;
                SetForegroundWindow(ta.MainWindowHandle);
                break;
            }
        }

        private static void CopyDatFileToInstallationFolder(Session session, string sourcepath, string destinationpath)
        {
            if (string.IsNullOrEmpty(sourcepath) || string.IsNullOrEmpty(destinationpath))
                return;
            string sourceFile = Path.Combine(sourcepath, TurboActivateDatFilename);
            string destinationFile = Path.Combine(destinationpath, TurboActivateDatFilename);
            session.Log("Copy from '{0}' to '{1}'", sourceFile, destinationFile);
            if (File.Exists(destinationFile))
                File.Delete(destinationFile);
            try
            {
                FileStream srcFile = File.Open(sourceFile, FileMode.Open);
                FileStream destFile = File.Create(destinationFile);
                var buf = new byte[SizeCopy];
                int len;
                while ((len = srcFile.Read(buf, 0, buf.Length)) > 0)
                {
                    destFile.Write(buf, 0, len);
                }
            }
            catch (Exception ex)
            {
                session.Log("CopyDatFileToInstallationFolder: {0}", ex.Message);
            }
        }

        private static void POnExited(object sender, EventArgs eventArgs)
        {
            ((Process)sender).Exited -= POnExited;
        }

        private static bool CopyFileFromDatabase(Session session, string path, string key, string filename)
        {
            bool result = false;
            try
            {
                string pathFile = path + "\\" + filename;
                session.Log(string.Format("PathFile {0}", pathFile));
                using (FileStream file = File.Create(pathFile))
                {
                    using (View view = session.Database.OpenView(QueryTemplate, key))
                    {
                        view.Execute();
                        using (Record rec = view.Fetch())
                        {
                            using (Stream stream = rec.GetStream("Data"))
                            {
                                var buf = new byte[SizeCopy];
                                int len;
                                while ((len = stream.Read(buf, 0, buf.Length)) > 0)
                                {
                                    file.Write(buf, 0, len);
                                }
                            }
                        }
                    }
                }
                result = true;
            }
            catch (Exception ex)
            {
                session.Log(string.Format("CopyFileFromDatabase: {0}", ex.Message));
                result = false;
            }
            return result;
        }

        private static void CreateFolder(string folder)
        {
            if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
        }

        private static void DeleteFolder(string folder, bool recurs = true)
        {
            if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
            {
                Directory.Delete(folder, recurs);
            }
        }

#endif

        #endregion [activate CA]

        #region [deactivate CA]

        private const string UnistallFile = "unistall.exe";

        [CustomAction]
        public static ActionResult Deactivate(Session session)
        {
            try
            {
                var esPath = GetElasticSearchBinFolder(session).TrimEnd(Path.DirectorySeparatorChar);
                var ofPath = GetInstallationFolder(session).TrimEnd(Path.DirectorySeparatorChar);
                string fullname = Path.Combine(ofPath, UnistallFile);
                if (string.IsNullOrEmpty(fullname) || !File.Exists(fullname))
                {
                    session.Log("Couldn't call unistall.exe");
                    return ActionResult.Success;
                }
                DeleteRiverSettings(session);
                var startinfo = new ProcessStartInfo(fullname, "uninstall");
                startinfo.Arguments = string.Format(" {0} \"{1}\" \"{2}\"", "uninstall", esPath, ofPath);
                startinfo.Verb = "runas";
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(startinfo);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                session.Log("Deactivate: {0}", ex.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        #endregion [deactivate CA]

        #region [Start Service Application]

        [CustomAction]
        public static ActionResult StartServiceApp(Session session)
        {
            try
            {
                var ofPath = GetInstallationFolder(session);
                var filename = Path.Combine(ofPath, "serviceapp.exe");
                if (File.Exists(filename))
                {
                    OFRegistryHelper.Instance.SetAutoRunHelperApplication(ofPath);
                }
                
            }
            catch (Exception ex)
            {
                session.Log(ex.ToString());
            }
            return ActionResult.Success;
        }
        
        #endregion


        #region [error message]

        public static ActionResult ErrorMessage(Session session)
        {
            var result = ActionResult.Success;
            if (!OFRegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            try
            {
                var rec = new Record();
                rec.FormatString = "Update was finished with error.\nPlease, see log.";
                session.Message(
                    InstallMessage.User | InstallMessage.Error | (InstallMessage)(MessageBoxIcon.Error) |
                    (InstallMessage)MessageBoxButtons.OK, rec);
            }
            catch (Exception)
            {
                result = ActionResult.NotExecuted;
            }
            return result;
        }

        #endregion [error message]

        #region [cancel message]

        public static ActionResult CancelMessage(Session session)
        {
            var result = ActionResult.Success;
            if (!OFRegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            try
            {
                var rec = new Record();
                rec.FormatString = "Update was canceled by user.";
                session.Message(
                    InstallMessage.User | InstallMessage.Info | (InstallMessage)(MessageBoxIcon.Warning) |
                    (InstallMessage)MessageBoxButtons.OK, rec);
            }
            catch (Exception)
            {
                result = ActionResult.NotExecuted;
            }
            return result;
        }

        #endregion [cancel message]

        #region [clear registry (loading times, disabling etc)]

        [CustomAction]
        public static ActionResult ResetRegistry(Session session)
        {
            try
            {
                var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion();
                OFRegistryHelper.Instance.DeleteLoadingTime(officeVersion.Item1);
                OFRegistryHelper.Instance.DeleteAddIn(officeVersion.Item1);
                OFRegistryHelper.Instance.DeleteDisabling(officeVersion.Item1);
            }
            catch (Exception)
            {
            }
            return ActionResult.Success;
        }

        #endregion

        #region [elastc search]


        private readonly static string InstallFile = "install.exe";

        [CustomAction]
        public static ActionResult InstallApp(Session session)
        {
            try
            {
                var esPath = GetElasticSearchBinFolder(session).TrimEnd(Path.DirectorySeparatorChar);
                var ofPath = GetInstallationFolder(session).TrimEnd(Path.DirectorySeparatorChar);
                OFRegistryHelper.Instance.SetElasticSearchPath(esPath);
                OFRegistryHelper.Instance.SetOfPath(ofPath);
                
                string fullname = string.Format("{0}\\{1}", GetInstallationFolder(session), InstallFile);
                if (string.IsNullOrEmpty(fullname) || !File.Exists(fullname))
                {
                    session.Log("Couldn't call install.exe");
                    return ActionResult.Success;
                }
                DeleteRiverSettings(session);
                var args = string.Format(" \"{0}\"  \"{1}\" ", esPath, ofPath);
                var startinfo = new ProcessStartInfo(fullname,args);
                startinfo.Verb = "runas";
                startinfo.WindowStyle = ProcessWindowStyle.Hidden;
                Process p = Process.Start(startinfo);
                p.WaitForExit();

            }
            catch (Exception ex)
            {
                session.Log("Install service and firewall rules: {0}", ex.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        #endregion

        #region [delete river settings - isolated storage]

        static string Filename = "riversettings.json".ToUpperInvariant();

        private static void DeleteRiverSettings(Session session)
        {
            var isolatedFolder = "IsolatedStorage";
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            var fullIsolatedParh = Path.Combine(localAppData, isolatedFolder);
            if (Directory.Exists(fullIsolatedParh))
            {
                ScanDirectory(fullIsolatedParh,session);
            }
        }

        private static void ScanDirectory(string path, Session session)
        {

            var dirInfo = new DirectoryInfo(path);

            try
            {
                var dirs = dirInfo.GetDirectories();
                if (dirs.Length > 0)
                {
                    foreach (var dir in dirs)
                    {
                        ScanDirectory(dir.FullName, session);
                    }
                }
            }
            catch (Exception e)
            {
                session.Log(e.ToString());
            }
            try
            {
                var files = dirInfo.GetFiles();
                if (files.Length > 0)
                {
                    foreach (var fileInfo in files)
                    {
                        if (fileInfo.Name.ToUpperInvariant().Equals(Filename))
                        {
                            fileInfo.Delete();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                session.Log(e.ToString());
            }
        }
        
        #endregion
    }
}