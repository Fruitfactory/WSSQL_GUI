using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using Microsoft.Win32;
using System.Windows.Forms;
using WSUI.Core.Helpers;

namespace WSUI.CA
{
    public class CustomActions
    {
        #region [needs]

        private const string UpdateFolder = "Update";
        private const string LocFilename = "install.loc";
        private const string LogFilename = "install.log";

        private const string OutllokAppName = "outlook";

        #endregion

        #region [CA ClearFiles]
        
        [CustomAction]
        public static ActionResult ClearFiles(Session session)
        {
            try
            {
                var path = session["INSTALLFOLDER"];
                if (RegistryHelper.Instance.IsSilendUpdate())
                {
                    session.Log("Silent update...");
                    return ActionResult.Success;
                }
                session.Log("Delete  files...");
                DeleteUpdateFolder(path);
                var list = new List<string>() {LocFilename};
                foreach (var filename in list)
                {
                    DeleteFile(path, filename);
                }
                DeleteRootFolder(path);
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

        #region [private for delete files]

        private static bool DeleteUpdateFolder(string root)
        {
            string fullpath = string.Format("{0}{1}",root,UpdateFolder);
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

        private static bool DeleteRootFolder(string root)
        {
            string fullpath = string.Format("{0}", root);
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

        private static bool DeleteFile(string root,string filename)
        {
            string fullpath = string.Format("{0}{1}", root, filename);
            if (!File.Exists(fullpath))
                return false;
            try
            {
                File.Delete(fullpath);
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
            try
            {
                Record rec = new Record();
                rec.FormatString = "Update was successful.";
                if (RegistryHelper.Instance.IsOutlookClosedByInstaller())
                {
                    rec.FormatString += "\nOutlook will be opened soon.";
                }
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

        #region [error message]

        public static ActionResult ErrorMessage(Session session)
        {
            ActionResult result = ActionResult.Success;
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
