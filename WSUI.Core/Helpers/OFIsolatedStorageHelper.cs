using System;
using System.IO;
using System.IO.IsolatedStorage;

namespace OF.Core.Helpers
{
    public class OFIsolatedStorageHelper
    {

        #region [ctor]

        public OFIsolatedStorageHelper()
        {
            
        } 

        #endregion

        #region [static]

        private static string RiverSettingsFile = GlobalConst.SettingsRiverFile + ".json";

        private static string ApplicationSettings = GlobalConst.SettingsApplication + ".json";
                                          

        private static Lazy<OFIsolatedStorageHelper> _instance = new Lazy<OFIsolatedStorageHelper>(() => new OFIsolatedStorageHelper());

        public static OFIsolatedStorageHelper Instance
        {
            get { return _instance.Value; }
        }
 

        #endregion

        #region [methods]

        public string GetElasticSearchSettings()
        {
            return GetFileContent(RiverSettingsFile);
        }

        public void SaveElasticSearchSettings(string content)
        {
            SetFileContent(RiverSettingsFile, content);
        }

        public string GetApplicationSettings()
        {
            return GetFileContent(ApplicationSettings);
        }

        public void SetApplicationSettings(string content)
        {
            SetFileContent(ApplicationSettings,content);
        }

        public void RemoveApplicationSettings()
        {
            using (IsolatedStorageFile isolatedStorage = GetIsolatedStorage())
            {
                if (isolatedStorage.FileExists(ApplicationSettings))
                {
                    isolatedStorage.DeleteFile(ApplicationSettings);
                }
            }
        }


        public string GetData(string filename)
        {
            return GetFileContent(filename);
        }

        public void SetData(string filename, string content)
        {
            SetFileContent(filename,content);
        }



        
        private string GetFileContent(string filename)
        {
            using (var isolatedStorage = GetIsolatedStorage())
            {
                if (isolatedStorage.FileExists(filename))
                {
                    using (
                        IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(filename,
                            FileMode.Open, isolatedStorage))
                    {
                        using (StreamReader reader = new StreamReader(isoStream))
                        {
                            return reader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        private void SetFileContent(string filename, string content)
        {
            using (IsolatedStorageFile isolatedStorage = GetIsolatedStorage())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(filename, isolatedStorage.FileExists(filename) ? FileMode.Create : FileMode.CreateNew, isolatedStorage))
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        writer.WriteLine(content);
                    }
                }
            }
        }


        private IsolatedStorageFile GetIsolatedStorage()
        {
            return IsolatedStorageFile.GetStore(IsolatedStorageScope.User | IsolatedStorageScope.Assembly,null,null);
        }

        #endregion





    }
}