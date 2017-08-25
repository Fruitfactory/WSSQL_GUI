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

        public bool SaveElasticSearchSettings(object obj)
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

        public T ReadElasticSearchSettings<T>() where T : class
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

        public bool SaveApplicationSettings(object obj)
        {
            var result = false;
            try
            {
                OFIsolatedStorageHelper.Instance.SetApplicationSettings(JsonConvert.SerializeObject(obj));
                result = true;
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
            return result;
        }

        public T ReadApplicationSettings<T>() where T : class
        {
            try
            {
                var str = OFIsolatedStorageHelper.Instance.GetApplicationSettings();
                if (string.IsNullOrEmpty(str))
                {
                    return default(T);
                }
                return JsonConvert.DeserializeObject(str, typeof(T)) as T;
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
            return default(T);
        }

    }
}