using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms.VisualStyles;
using System.Xml.Linq;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Windows.Forms;
using WSUI.Core.Helpers;

namespace WSUI.CA
{
    public class CustomActions
    {
        #region [needs]

        private const string UpdateFolder = "\\Update";
        private const string LocFilename = "install.loc";
        private const string LogFilename = "install.log";

        private const string OutllokAppName = "outlook";

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
                var list = new List<string>() {LocFilename};
                foreach (var filename in list)
                {
                    DeleteFile(session,path, filename);
                }
                DeleteRootFolder(session,path);
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
            Process outlookProcess;
            ActionResult res = ActionResult.Success;
            if (IsOutlookOpen(out outlookProcess,session))
            {
                try
                {
                    if (outlookProcess != null)
                    {
                        session.Log("Close outlook. IsOutllokClosedByInstaller = " + RegistryHelper.Instance.IsOutlookClosedByInstaller().ToString());
                        outlookProcess.Kill();
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
            session.Log("Open outlook. IsOutllokClosedByInstaller = " + RegistryHelper.Instance.IsOutlookClosedByInstaller().ToString());
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

        private static bool IsOutlookOpen(out Process pr, Session session)
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
                    pr = outlook.ElementAt(0);
                }

            }
            catch (Exception ex)
            {
                session.Log("Error during checking: "+ ex.Message);
                
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
                session.Message(InstallMessage.User | InstallMessage.Info | (InstallMessage)(MessageBoxIcon.Information) | (InstallMessage)MessageBoxButtons.OK, rec);
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
                Unlock(path,session);
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
                    UpdateMSINode(msi, productCode, newversion,session);
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

        public static void Unlock(string path,Session session)
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
