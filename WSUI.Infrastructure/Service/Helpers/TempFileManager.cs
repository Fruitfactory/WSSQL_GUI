using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using C4F.DevKit.PreviewHandler.Service.Logger;


namespace WSUI.Service.Helpers
{
    class TempFileManager
    {

        #region static 
        private static TempFileManager _instance = null;
        private static string TempFileName = "{0}WSSQL\\{1}\\{2}";
        private static string TempFolder = "{0}WSSQL";
        private static string TempFolderFile = "{0}WSSQL\\{1}";
        private static readonly object _lockObject = new object();

        #endregion

        #region fields
        
        private Dictionary<Guid, string> _tempFileList;

        

        #endregion

        private TempFileManager()
        {
            _tempFileList = new Dictionary<Guid,string>();
            string temp = string.Format(TempFolder, Path.GetTempPath());
            if (!File.Exists(temp))
            {
                Directory.CreateDirectory(temp);
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Create folder", temp));
            }
        }

        public static TempFileManager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                        _instance = new TempFileManager();
                    return _instance;
                }
            }
        }

        #region public 


        public string GenerateTempFileName(WSUI.Core.BaseSearchData searchitem)
        {
            if (_tempFileList.ContainsKey(searchitem.ID))
            {
                return _tempFileList[searchitem.ID];
            }
            string filename = string.Format("{0}{1}",Path.GetFileNameWithoutExtension(Path.GetTempFileName()),GetExtension(searchitem));
            string tempFolder = string.Format(TempFolderFile, Path.GetTempPath(), searchitem.ID.ToString());
            DirectoryInfo di = new DirectoryInfo(tempFolder);
            if (!di.Exists)
            {
                di.Create();
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Create folder", di.FullName));
            }
            string tempFilename = string.Format(TempFileName, Path.GetTempPath(), searchitem.ID.ToString(), filename);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            _tempFileList.Add(searchitem.ID, tempFilename);
            return tempFilename;
        }

        public void ClearTempFolder()
        {
            string temp = string.Format(TempFolder, Path.GetTempPath());
            DirectoryInfo di = new DirectoryInfo(temp);
            if (di.Exists && di.GetDirectories().Count() > 0 )
            {
                var dirCollection = di.GetDirectories();
                foreach (var dir in dirCollection)
                {
                    try
                    {
                        dir.Delete(true);
                    }
                    catch (Exception ex)
                    {
                        WSSqlLogger.Instance.LogError(ex.Message);
                    }
                }
            }
        }

        #endregion


        #region private 

        private string GetExtension(WSUI.Core.BaseSearchData searchItem)
        {
            string ext = null;
            switch (searchItem.Type)
            {
                case Enums.TypeSearchItem.Email:
                    ext = ".msg";
                    break;
                case Enums.TypeSearchItem.Attachment:
                    string filename = searchItem.Path.Substring(searchItem.Path.LastIndexOf(':') + 1);
                    ext = Path.GetExtension(filename);
                    break;
                case Enums.TypeSearchItem.Contact:
                    ext = Path.GetExtension(searchItem.Path);
                    break;
            }
            return ext;
        }

        #endregion

    }
}
