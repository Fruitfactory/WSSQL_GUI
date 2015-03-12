﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WSUI.Core.Data;
using WSUI.Core.Enums;
using WSUI.Core.Interfaces;
using WSUI.Core.Logger;

namespace WSUI.Core.Helpers
{
    public class TempFileManager
    {
        #region static

        private static TempFileManager _instance = null;
        private static string TempFileName = "{0}\\{1}";
        private static string TempFolder = "{0}OutlookFinder";
        private static string TempFolderFile = "{0}OutlookFinder\\{1}";
        private static readonly object _lockObject = new object();

        #endregion static

        #region fields

        private Dictionary<Guid, string> _tempFileList;

        #endregion fields

        private TempFileManager()
        {
            _tempFileList = new Dictionary<Guid, string>();
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

        public string GenerateTempFileName(ISearchObject searchitem)
        {
            return InternalGetTempFilename(searchitem.Id, GetFilename(searchitem), GetExtension(searchitem));
        }

        public string GenerateHtmlTempFileName(Guid id)
        {
            return GenerateTempFileName(id, ".html");
        }

        public string GenerateTempFileName(Guid id, string ext)
        {
            return InternalGetTempFilename(id, id.ToString(), ext);
        }

        private string InternalGetTempFilename(Guid id, string file, string ext)
        {
            string path = CheckAndGetExistTempFilepath(id);
            if (!string.IsNullOrEmpty(path))
                return path;
            string filename = GetFileName(file, ext);
            string tempFolder = CreateAndGetTempFolder(id);
            return GetTempFilename(tempFolder, filename, id);
        }

        public string GenerateTempFileNameWithCopy(BaseSearchObject searchitem, string filename)
        {
            string path = CheckAndGetExistTempFilepath(searchitem.Id);
            if (!string.IsNullOrEmpty(path))
                return path;
            string file = Path.GetFileName(filename);
            string tempFolder = CreateAndGetTempFolder(searchitem.Id);
            string tempFilename = GetTempFilename(tempFolder, file, searchitem.Id);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            try
            {
                if (!File.Exists(filename))
                    throw new FileNotFoundException("File doesn't exist", filename);
                CopyFile(filename, tempFilename);
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
            if (di.Exists && di.GetDirectories().Count() > 0)
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
                        WSSqlLogger.Instance.LogInfo(ex.Message);
                    }
                }
            }
        }

        #endregion public

        #region private

        private string CheckAndGetExistTempFilepath(Guid id)
        {
            if (_tempFileList.ContainsKey(id))
            {
                return _tempFileList[id];
            }
            return string.Empty;
        }

        private string GetFileName(string filename, string extension)
        {
            return string.Format("{0}{1}", filename, extension);
        }

        private string CreateAndGetTempFolder(Guid id)
        {
            string tempFolder = string.Format(TempFolderFile, Path.GetTempPath(), id.ToString());
            DirectoryInfo di = new DirectoryInfo(tempFolder);
            if (!di.Exists)
            {
                di.Create();
                WSSqlLogger.Instance.LogWarning(string.Format("{0}: {1}", "Create folder", di.FullName));
            }
            return tempFolder;
        }

        private string GetTempFilename(string folder, string filename, Guid id)
        {
            string tempFilename = string.Format(TempFileName, folder, filename);
            if (string.IsNullOrEmpty(tempFilename))
                return null;
            _tempFileList.Add(id, tempFilename);
            return tempFilename;
        }

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
                if (fileSource != null)
                    fileSource.Close();
                if (fileDestination != null)
                {
                    fileDestination.Flush();
                    fileDestination.Close();
                }
            }
        }

        private string GetExtension(ISearchObject searchItem)
        {
            string ext = null;
            switch (searchItem.TypeItem)
            {
                case Enums.TypeSearchItem.Email:
                    ext = ".msg";
                    break;

                case Enums.TypeSearchItem.Attachment:
                    string filename = searchItem.ItemName;
                    ext = Path.GetExtension(filename);
                    break;

                case Enums.TypeSearchItem.Contact:
                    ext = Path.GetExtension(searchItem.ItemName); // TODO: should check!! ItenUrl was changed yo ItemName
                    break;

                case Enums.TypeSearchItem.Calendar:
                    ext = ".html";
                    break;
            }
            return ext;
        }

        private string GetFilename(ISearchObject searchItem)
        {
            string filename = string.Empty;
            switch (searchItem.TypeItem)
            {
                case TypeSearchItem.Attachment:
                    try
                    {
                        filename = Path.GetFileNameWithoutExtension(searchItem.ItemName);
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

        #endregion private
    }
}