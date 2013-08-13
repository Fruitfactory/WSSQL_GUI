using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Deployment.WindowsInstaller;
using WSUI.Core.Helpers;

namespace WSUI.CA
{
    public class CustomActions
    {
        #region [needs]

        private const string UpdateFolder = "Update";
        private const string LocFilename = "install.loc";
        private const string LogFilename = "install.log";

        #endregion
        
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
    }
}
