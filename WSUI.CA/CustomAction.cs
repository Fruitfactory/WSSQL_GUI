using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Windows.Forms;
using WSUI.Core.Helpers;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace WSUI.CA
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

        #endregion

        #region elements fo manifest

        private const string InstallPathNodeName = "installationUrl";
        private const string VersionNodeName = "version";
        private const string ProductCodeNodeName = "productCode";

        #endregion

        #region [CA ClearFiles]

        [CustomAction]
        public static ActionResult ClearFiles(Session session)
        {
            try
            {
                var path = GetInstallationFolder(session);
                if (RegistryHelper.Instance.IsSilendUpdate())
                {
                    session.Log("Silent update...");
                    return ActionResult.Success;
                }
                session.Log("Delete  files...");
                DeleteUpdateFolder(session);
                var list = new List<string>() { LocFilename };
                foreach (var filename in list)
                {
                    DeleteFile(session, path, filename);
                }
                DeleteRootFolder(session, path);
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

        #endregion

        private static string GetInstallationFolder(Session session)
        {
            return session[InstallFolder];
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

        #endregion

        #region [CA operation with Outlook]

        [CustomAction]
        public static ActionResult CloseOutlook(Session session)
        {
            Outlook._Application outlookProcess;
            ActionResult res = ActionResult.Success;
            if (IsOutlookOpen(out outlookProcess, session))
            {
                try
                {
                    if (outlookProcess != null)
                    {
                        session.Log("Close outlook. IsOutllokClosedByInstaller = " +
                                    RegistryHelper.Instance.IsOutlookClosedByInstaller().ToString());
                        outlookProcess.Quit();
                        WaitForClosingOutlook(session);
                        RegistryHelper.Instance.SetFlagClosedOutlookApplication();
                    }
                    res = ActionResult.Success;
                }
                catch (Exception ex)
                {
                    session.Log("Error during closing outlook: " + ex.Message);
                    res = ActionResult.Failure;
                }
            }
            return res;
        }

        [CustomAction]
        public static ActionResult OpenOutlook(Session session)
        {
            session.Log("Open outlook. IsOutllokClosedByInstaller = " +
                        RegistryHelper.Instance.IsOutlookClosedByInstaller().ToString());
            ActionResult res = ActionResult.Success;
            if (RegistryHelper.Instance.IsOutlookClosedByInstaller() && IsOutlookInstalled())
            {
                try
                {
                    Process.Start("OUTLOOK.EXE");
                    RegistryHelper.Instance.ResetFlagClosedOutlookApplication();

                }
                catch (Exception ex)
                {
                    session.Log("Error during open outlook: " + ex.Message);
                    res = ActionResult.Failure;
                }
            }
            return res;
        }

        private static bool IsOutlookOpen(out Outlook._Application pr, Session session)
        {
            bool res = false;
            pr = null;
            try
            {
                var outlook =
                    Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutllokAppName.ToUpper()));
                res = outlook.Any();
                if (res)
                {
                    pr = System.Runtime.InteropServices.Marshal.GetActiveObject(OutlookId) as Outlook._Application;
                }

            }
            catch (Exception ex)
            {
                session.Log("Error during checking: " + ex.Message);

            }
            return res;
        }

        private static bool IsOutlookInstalled()
        {
            const string SubKey = "Software\\microsoft\\windows\\currentversion\\app paths\\OUTLOOK.EXE";
            RegistryKey subKey = Registry.LocalMachine.OpenSubKey(SubKey);
            string path = subKey.GetValue("Path") as string;
            return !string.IsNullOrEmpty(path);
        }

        private static void WaitForClosingOutlook( Session session)
        {
            const int Times = 35; // => 3.5 sek
            int count = 0;
            Outlook._Application app;
            while (IsOutlookOpen(out app, session))
            {
                Thread.Sleep(100);
                if (Times == count)
                    break;
                count++;
            };
            if (IsOutlookOpen(out app, session))
            {
                CloseHardOutlook(app,session);
            }
        }

        private static void CloseHardOutlook(Outlook._Application application, Session session)
        {
            session.Log("Hard close of Outlook");
            var process = Process.GetProcesses().Where(p => p.ProcessName.ToUpper().StartsWith(OutllokAppName.ToUpper()));
            if (!process.Any())
                return;
            try
            {
                session.Log("Kill the Outlook");
                process.ElementAt(0).Kill();
                session.Log("Good kill the Outlook");
            }
            catch (Exception ex)
            {
                session.Log(ex.Message);
            }
        }

        #endregion

        #region [succes message]

        [CustomAction]
        public static ActionResult SuccesMessage(Session session)
        {
            ActionResult result = ActionResult.Success;

            if (!RegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            //session.Log("Second entry to SuccesMessage...");
            try
            {
                Record rec = new Record();
                rec.FormatString = "Update was successful.";
                if (RegistryHelper.Instance.IsOutlookClosedByInstaller())
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

        #endregion

        #region [update installation info]

        private static void UpdateAndDeleteLock(Session session)
        {
            var path = GetInstallationFolder(session);
            var temp = GetTempPath();
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

                var listVersions =
                    docVersion.Descendants("version").Select(
                        el => el.Attribute("name") != null ? el.Attribute("name").Value : string.Empty).ToList();
                listVersions.Sort();
                string newversion = listVersions[listVersions.Count - 1];
                var element =
                    docVersion.Descendants("version").Where(el => el.Attribute("name").Value == newversion).First();
                var productCode = element.Attribute(ProductCodeNodeName).Value;

                if (docManifest.Descendants(MSINode).Any())
                {
                    var msi = docManifest.Descendants("msi").First();
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
                var el = msi.Descendants(VersionNodeName).First();
                el.Value = newversion;
                session.Log(string.Format("New version: {0}", newversion));
            }
            if (msi.Descendants(ProductCodeNodeName).Count() > 0)
            {
                var el = msi.Descendants(ProductCodeNodeName).First();
                el.Value = productCode;
                session.Log(string.Format("Product code: {0}", productCode));
            }
        }

        private static void CreateMSINode(XDocument docManifest, XElement elementNewestVersion, string language)
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

        public static void Unlock(string path, Session session)
        {
            try
            {
                File.Delete(string.Format("{0}{1}", path, LocFilename));
                DeleteUpdateFolder(session);
                session.Log("Silent updates is finished. Call index set up in default value (None).");
                RegistryHelper.Instance.FinishSilentUpdate();
                RegistryHelper.Instance.SetCallIndexKey(RegistryHelper.CallIndex.None);
            }
            catch (System.Exception ex)
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

        #endregion



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



        [CustomAction]
        public static ActionResult ActivatePlugin(Session session)
        {
            if (RegistryHelper.Instance.IsSilendUpdate())
                return ActionResult.Success;
            try
            {
                string path = Path.Combine(Path.GetTempPath(), ActivateFilesFolder);
                CreateFolder(path);
                session.Log(string.Format("Path {0}", path));
                ExtractFiles(session, path);
                RunAndWaitActivator(session,path);
                //CopyDatFileToInstallationFolder(session,path,GetInstallationFolder(session));
                DeleteFolder(path);
                session.Log(string.Format("Success"));
                return ActionResult.Success;
            }
            catch (Exception ex)
            {
                session.Log("Activate: {0}", ex.Message);
                return ActionResult.Failure;
            }
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

        private static void RunAndWaitActivator(Session session,string path)
        {
            string file = Path.Combine(path, TurboActivateExeFilename);
            if (!File.Exists(file))
                return;
            try
            {
                Process p = new Process()
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
                session.Log(string.Format("RunAndActivator: {0}",ex.Message));   
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
                var list = Process.GetProcesses().Where(p => p.ProcessName.IndexOf(TurboActivateTemplate) > -1);
                if(!list.Any())
                    continue;
                var ta = list.ElementAt(0);
                if(ta.MainWindowHandle == IntPtr.Zero)
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
            session.Log("Copy from '{0}' to '{1}'",sourceFile,destinationFile);
            if(File.Exists(destinationFile))
                File.Delete(destinationFile);
            try
            {
                var srcFile = File.Open(sourceFile, FileMode.Open);
                var destFile = File.Create(destinationFile);
                byte[] buf = new byte[SizeCopy];
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
            ((Process) sender).Exited -= POnExited;
        }

        private static bool CopyFileFromDatabase(Session session, string path, string key, string filename)
        {
            bool result = false;
            try
            {
                string pathFile = path + "\\" + filename;
                session.Log(string.Format("PathFile {0}", pathFile));
                using (var file = File.Create(pathFile))
                {
                    using (var view = session.Database.OpenView(QueryTemplate, key))
                    {
                        view.Execute();
                        using (Record rec = view.Fetch())
                        {
                            using (Stream stream = rec.GetStream("Data"))
                            {
                                byte[] buf = new byte[SizeCopy];
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
        #endregion

        #region [deactivate CA]

        private const string UnistallFile = "unistall.exe";

        [CustomAction]
        public static ActionResult Deactivate(Session session)
        {
            if(RegistryHelper.Instance.IsSilendUpdate())
                return ActionResult.Success;
            try
            {
                string fullname = string.Format("{0}\\{1}", GetInstallationFolder(session), UnistallFile);
                if (string.IsNullOrEmpty(fullname) || !File.Exists(fullname))
                {
                    session.Log("Couldn't call unistall.exe");
                    return ActionResult.Success;
                }
                var startinfo = new ProcessStartInfo(fullname, "unistall");
                Process p = Process.Start(startinfo);
                p.WaitForExit();
            }
            catch (Exception ex)
            {
                session.Log("Deactivate: {0}",ex.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        #endregion


        #region [error message]

        public static ActionResult ErrorMessage(Session session)
        {
            ActionResult result = ActionResult.Success;
            if (!RegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            try
            {
                Record rec = new Record();
                rec.FormatString = "Update was finished with error.\nPlease, see log.";
                session.Message(InstallMessage.User | InstallMessage.Error | (InstallMessage)(MessageBoxIcon.Error) | (InstallMessage)MessageBoxButtons.OK, rec);
            }
            catch (Exception)
            {
                result = ActionResult.NotExecuted;
            }
            return result;
        }

        #endregion

        #region [cancel message]

        public static ActionResult CancelMessage(Session session)
        {
            ActionResult result = ActionResult.Success;
            if (!RegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            try
            {
                Record rec = new Record();
                rec.FormatString = "Update was canceled by user.";
                session.Message(InstallMessage.User | InstallMessage.Info | (InstallMessage)(MessageBoxIcon.Warning) | (InstallMessage)MessageBoxButtons.OK, rec);
            }
            catch (Exception)
            {
                result = ActionResult.NotExecuted;
            }
            return result;
        }

        #endregion

    }
}
