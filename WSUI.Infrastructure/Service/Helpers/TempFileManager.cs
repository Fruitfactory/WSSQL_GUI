﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using WSPreview.PreviewHandler.Service.Logger;
using WSUI.Infrastructure.Service.Enums;


namespace WSUI.Infrastructure.Service.Helpers
{
    public class TempFileManager
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


        public string GenerateTempFileName(WSUI.Infrastructure.Core.BaseSearchData searchitem)
        {
            if (_tempFileList.ContainsKey(searchitem.ID))
            {
                return _tempFileList[searchitem.ID];
            }
            string filename = string.Format("{0}{1}",GetFilename(searchitem),GetExtension(searchitem));
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

        public string GenerateTempFileNameWithCopy(WSUI.Infrastructure.Core.BaseSearchData searchitem, string filename)
        {
            if (_tempFileList.ContainsKey(searchitem.ID))
            {
                return _tempFileList[searchitem.ID];
            }
            string file = Path.GetFileName(filename);
            string tempFolder = string.Format(TempFolderFile, Path.GetTempPath(), searchitem.ID.ToString());
            DirectoryInfo di = new DirectoryInfo(tempFolder);
            if (!di.Exists)
            {
                di.Create();
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Create folder", di.FullName));
            }
            string tempFilename = string.Format(TempFileName, Path.GetTempPath(), searchitem.ID.ToString(), file);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            try
            {
                if (!File.Exists(filename))
                    throw new FileNotFoundException("File doesn't exist", filename);
                CopyFile(filename,tempFilename);    
                _tempFileList.Add(searchitem.ID, tempFilename);
            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Copy file", tempFilename, ex.Message));
                return null;
            }
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
                        WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "ClearTempFolder", ex.Message));
                    }
                }
            }
        }

        #endregion


        #region private 


        private void CopyFile(string source, string destination)
        {
            FileStream fileSource = null;
            FileStream fileDestination = null;
            try
            {
                fileSource = new FileStream(source, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                fileDestination = new FileStream(destination, FileMode.Create, FileAccess.Write, FileShare.None);
                int numBytes;
                const int size = 4096;
                byte[] buffer = new byte[size];
                while ((numBytes = fileSource.Read(buffer, 0, size)) > 0)
                {
                    fileDestination.Write(buffer, 0, numBytes);
                }

            }
            catch (Exception ex)
            {
                WSSqlLogger.Instance.LogError(string.Format("{0} - {1}", "CopyFile", ex.Message));
            }
            finally
            {
                if(fileSource !=  null)
                    fileSource.Close();
                if (fileDestination != null)
                {
                    fileDestination.Flush();
                    fileDestination.Close();
                }
            }
        }

        private string GetExtension(WSUI.Infrastructure.Core.BaseSearchData searchItem)
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
                case Enums.TypeSearchItem.Calendar:
                    ext = ".html";
                    break;
            }
            return ext;
        }

        private string GetFilename(WSUI.Infrastructure.Core.BaseSearchData searchItem)
        {
            string filename = string.Empty;
            switch (searchItem.Type)
            {
                case TypeSearchItem.Attachment:
                    try
                    {
                        filename = Path.GetFileNameWithoutExtension(searchItem.Name);
                    }
                    catch (Exception)
                    { //TODO should changed for getting from the path
                        filename = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                    }
                    break;
                default:
                    filename = Path.GetFileNameWithoutExtension(Path.GetTempFileName());
                    break;

            }
            return filename;
        }

        #endregion

    }
}
