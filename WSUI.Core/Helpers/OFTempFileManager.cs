using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using OF.Core.Core.LimeLM;
using OF.Core.Data;
using OF.Core.Enums;
using OF.Core.Interfaces;
using OF.Core.Logger;

namespace OF.Core.Helpers
{
    public class OFTempFileManager
    {
        #region static

        private static OFTempFileManager _instance = null;
        private static string TempFileName = "{0}\\{1}";
        private static string TempFolder = "{0}OutlookFinder";
        private static string TempFolderFile = "{0}OutlookFinder\\{1}";
        private static readonly object _lockObject = new object();

        #endregion static

        #region fields

        private Dictionary<Guid, string> _tempFileList;
        private Dictionary<Guid, string> _temppFileEmlForEmails; 

        #endregion fields

        private OFTempFileManager()
        {
            _tempFileList = new Dictionary<Guid, string>();
            _temppFileEmlForEmails = new Dictionary<Guid, string>();
            string temp = string.Format(TempFolder, Path.GetTempPath());
            if (!File.Exists(temp))
            {
                Directory.CreateDirectory(temp);
                OFLogger.Instance.LogDebug(string.Format("{0}: {1}", "Create folder", temp));
            }
        }

        public static OFTempFileManager Instance
        {
            get
            {
                lock (_lockObject)
                {
                    if (_instance == null)
                        _instance = new OFTempFileManager();
                    return _instance;
                }
            }
        }

        #region public

        public string GenerateTempFileName(ISearchObject searchitem)
        {
            return InternalGetTempFilename(searchitem.Id, GetFilename(searchitem), GetExtension(searchitem));
        }

        public string GetTemporaryFilename(ISearchObject searchObject)
        {
            return GetFilename(searchObject);
        }

        public string GenerateTempFolderForObject(ISearchObject searchItem)
        {
            return InternalGenerateForlderForObject(searchItem.Id);
        }

        public string GenerateHtmlTempFileName(Guid id)
        {
            return GenerateTempFileName(id, ".html");
        }

        public string GenerateTempFileName(Guid id, string ext)
        {
            return InternalGetTempFilename(id, id.ToString(), ext);
        }

        private string InternalGenerateForlderForObject(Guid id)
        {
            string path = CheckAndGetExistTempFilepath(id);
            if (!string.IsNullOrEmpty(path))
                return path;
            string tempFolder = CreateAndGetTempFolder(id);
            return tempFolder;
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

        public string GenerateTempFileNameWithCopy(OFBaseSearchObject searchitem, string filename)
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
                OFLogger.Instance.LogError(string.Format("{0}: {1} - {2}", "Copy file", tempFilename, ex.ToString()));
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
                        OFLogger.Instance.LogError(ex.ToString());
                    }
                }
            }
        }

        public void SetEmlFileForEmailObject(ISearchObject email, string filename)
        {
            if (!_temppFileEmlForEmails.ContainsKey(email.Id))
            {
                _temppFileEmlForEmails.Add(email.Id, filename);
            }
            else
            {
                _temppFileEmlForEmails[email.Id] = filename;
            }
        }

        public bool IsEmlFileExistForEmailObject(ISearchObject email)
        {
            return _temppFileEmlForEmails.ContainsKey(email.Id);
        }

        public string GetExistEmlFileForEmailObject(ISearchObject email)
        {
            return _temppFileEmlForEmails.ContainsKey(email.Id) ? _temppFileEmlForEmails[email.Id] : "";
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
                OFLogger.Instance.LogDebug(string.Format("{0}: {1}", "Create folder", di.FullName));
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
                OFLogger.Instance.LogError(string.Format("{0} - {1}", "CopyFile", ex.ToString()));
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
                case Enums.OFTypeSearchItem.Email:
                    ext = ".msg";
                    break;

                case Enums.OFTypeSearchItem.Attachment:
                    string filename = searchItem.ItemName;
                    ext = Path.GetExtension(filename);
                    break;

                case Enums.OFTypeSearchItem.Contact:
                    ext = Path.GetExtension(searchItem.ItemName); // TODO: should check!! ItenUrl was changed yo ItemName
                    break;

                case Enums.OFTypeSearchItem.Calendar:
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
                case OFTypeSearchItem.Attachment:
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