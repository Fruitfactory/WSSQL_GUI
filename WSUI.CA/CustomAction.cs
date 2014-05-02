﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Office.Interop.Outlook;
using Microsoft.Win32;
using WSUI.Core.Core.LimeLM;
using WSUI.Core.Helpers;
using Exception = System.Exception;
using View = Microsoft.Deployment.WindowsInstaller.View;

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
                if (RegistryHelper.Instance.IsSilendUpdate())
                {
                    session.Log("Silent update...");
                    return ActionResult.Success;
                }
                session.Log("Delete  files...");
                DeleteUpdateFolder(session);
                var list = new List<string> { LocFilename };
                foreach (string filename in list)
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

        #endregion [CA ClearFiles]

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

        #endregion [private for delete files]

        #region [CA operation with Outlook]

        [CustomAction]
        public static ActionResult CloseOutlook(Session session)
        {
            _Application outlookProcess;
            var res = ActionResult.Success;
            if (IsOutlookOpen())
            {
                try
                {
                    session.Log("Close outlook. IsOutllokClosedByInstaller = " +
                                RegistryHelper.Instance.IsOutlookClosedByInstaller());
                    CloseAllOutlookInstancesLite(session);
                    Thread.Sleep(1000); // wait for closing
                    if (IsOutlookOpen())
                        CloseAllOutlookInstancesHard(session);
                    RegistryHelper.Instance.SetFlagClosedOutlookApplication();
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
                        RegistryHelper.Instance.IsOutlookClosedByInstaller());
            var res = ActionResult.Success;
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

            if (!RegistryHelper.Instance.IsSilendUpdate())
            {
                session.Log("Isn't silent update.");
                return result;
            }
            //session.Log("Second entry to SuccesMessage...");
            try
            {
                var rec = new Record();
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
                RegistryHelper.Instance.FinishSilentUpdate();
                RegistryHelper.Instance.SetCallIndexKey(RegistryHelper.CallIndex.None);
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
                string email = session["EMAILVALUE"];
                bool res = Regex.IsMatch(email, EmailPattern, RegexOptions.IgnoreCase);
                session[EmailValidProperty] = res.ToString();
                session.Log(session[EmailValidProperty]);
                if (!res)
                {
                    session["EMAILVALUE"] = string.Empty;
                    session["WIXUI_EXITDIALOGOPTIONALTEXT"] = "Email is not valid.";
                }
                
            }
            catch (Exception ex)
            {
                session.Log(ex.Message);
            }
            return ActionResult.Success;
        }


        [CustomAction]
        public static ActionResult ActivatePlugin(Session session)
        {
            if (RegistryHelper.Instance.IsSilendUpdate())
                return ActionResult.Success;
            try
            {
                string path = Path.GetDirectoryName(typeof (CustomActions).Assembly.Location);
                session.Log(string.Format("Path {0}", path));
                ExtractFiles(session, path);
                string email = session["EMAILVALUE"];

                if (string.IsNullOrEmpty(email))
                {
                    session.Log("Email is empty");
                    return ActionResult.Failure;
                }
                try
                {
                    var key = LimeLMApi.GenerateAndReturnKey(email);
                    if (string.IsNullOrEmpty(key))
                    {
                        session.Log("The key hasn't been generated.");
                    }
                    else
                    {
                        session.Log("KEY " + key);
                        TurboActivate.CheckAndSavePKey(key.Trim(), TurboActivate.TA_Flags.TA_USER);
                        TurboActivate.Activate();
                    }
                }
                catch (Exception ex)
                {
                    session.Log(ex.Message);
                }
                CopyDatFileToInstallationFolder(session, path, GetInstallationFolder(session));
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
            if (RegistryHelper.Instance.IsSilendUpdate())
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
                session.Log("Deactivate: {0}", ex.Message);
                return ActionResult.Failure;
            }
            return ActionResult.Success;
        }

        #endregion [deactivate CA]

        #region [error message]

        public static ActionResult ErrorMessage(Session session)
        {
            var result = ActionResult.Success;
            if (!RegistryHelper.Instance.IsSilendUpdate())
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
            if (!RegistryHelper.Instance.IsSilendUpdate())
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
    }
}