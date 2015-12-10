using System;
using System.IO;
using OF.Core.Logger;


namespace OF.Infrastructure.Services
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "IsDirectory", ex.ToString()));
                return true;
            }
        }

        #endregion
    }
}
