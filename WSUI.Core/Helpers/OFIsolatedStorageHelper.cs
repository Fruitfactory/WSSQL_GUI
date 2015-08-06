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
                                                  

        private static Lazy<OFIsolatedStorageHelper> _instance = new Lazy<OFIsolatedStorageHelper>(() => new OFIsolatedStorageHelper());

        public static OFIsolatedStorageHelper Instance
        {
            get { return _instance.Value; }
        }
 

        #endregion

        #region [methods]

        public string GetElasticSearchSettings()
        {
            using (var isolatedStorage = GetIsolatedStorage())
            {
                if (isolatedStorage.FileExists(RiverSettingsFile))
                {
                    using (
                        IsolatedStorageFileStream isoStream = new IsolatedStorageFileStream(RiverSettingsFile,
                            FileMode.Open,isolatedStorage))
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

        public void SaveElasticSearchSettings(string content)
        {
            using (IsolatedStorageFile isolatedStorage = GetIsolatedStorage())
            {
                using (IsolatedStorageFileStream stream = new IsolatedStorageFileStream(RiverSettingsFile,isolatedStorage.FileExists(RiverSettingsFile) ? FileMode.Create : FileMode.CreateNew,isolatedStorage))
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