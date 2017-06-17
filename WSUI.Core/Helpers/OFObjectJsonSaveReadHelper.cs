using System;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using OF.Core.Extensions;
using OF.Core.Logger;

namespace OF.Core.Helpers
{
    public class OFObjectJsonSaveReadHelper
    {
        private OFObjectJsonSaveReadHelper()
        {
        }

        #region [static]

        private static Lazy<OFObjectJsonSaveReadHelper> _instance = new Lazy<OFObjectJsonSaveReadHelper>(() => new OFObjectJsonSaveReadHelper());

        public static OFObjectJsonSaveReadHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        public bool Save(object obj, string filename)
        {
            bool result = false;
            try
            {
                var str = JsonConvert.SerializeObject(obj);
                OFIsolatedStorageHelper.Instance.SaveElasticSearchSettings(str);
                result = true;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return result;
        }

        public T Read<T>() where T : class
        {
            return Read<T>(typeof(T).Name);
        }

        public T Read<T>(string filename) where T : class
        {
            try
            {
                var strObject = OFIsolatedStorageHelper.Instance.GetElasticSearchSettings();
                if (strObject.IsEmpty())
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject(strObject, typeof(T)) as T;
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            return default(T);
        }



    }
}