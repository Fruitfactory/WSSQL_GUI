using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WSSQLGUI.Services
{
    class FileService
    {
        #region public

        public static bool IsDirectory(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                return true;
            FileInfo fi = new FileInfo(filename);

            return (fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory;
        }

        #endregion
    }
}
