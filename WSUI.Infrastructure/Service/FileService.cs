using System;
using System.IO;
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace WSUI.Infrastructure.Services
{
    public class FileService
    {
        #region public

        public static bool IsDirectory(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return true;
            try
            {
                FileInfo fi = new FileInfo(filename);
                return (fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(ex.Message);
                return true;
            }
        }

        #endregion
    }
}
