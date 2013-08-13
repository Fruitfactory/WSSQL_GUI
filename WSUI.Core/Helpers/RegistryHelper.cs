using System;
using Microsoft.Win32;

namespace WSUI.Core.Helpers
{
    public class RegistryHelper
    {
        #region [needs]

        private const string ProductSubKey = "SOFTWARE\\WSUIOutlookPlugin";
        private const string SilentUpdateKey = "IsSilentUpdate";

        private RegistryKey _baseRegistry = Registry.CurrentUser;

        #endregion


        #region [instance]

        protected RegistryHelper()
        {}

        protected void Init()
        {
            
        }

        private static Lazy<RegistryHelper> _instance = new Lazy<RegistryHelper>(() =>
                                                                                     {
                                                                                         var helper =
                                                                                             new RegistryHelper();
                                                                                         helper.Init();
                                                                                         return helper;
                                                                                     });
        public static RegistryHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        public bool IsSilendUpdate()
        {
            var v = ReadKey(SilentUpdateKey);
            if (string.IsNullOrEmpty(v))
                return false;
            bool parseresult = false;
            bool.TryParse(v, out parseresult);
            return parseresult;
        }

        public void StartSilentUpdate()
        {
            Write(SilentUpdateKey,true);
        }

        public void FinishSilelntUpdate()
        {
            Write(SilentUpdateKey,false);
        }


        private string ReadKey(string key)
        {
            RegistryKey temp = _baseRegistry;
            RegistryKey subKey = temp.OpenSubKey(ProductSubKey);
            if (subKey == null)
                return null;
            
            try
            {
                return subKey.GetValue(key.ToLower()) as string;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private void Write(string key, object value)
        {
            RegistryKey temp = _baseRegistry;
            RegistryKey subKey = temp.CreateSubKey(ProductSubKey);
            if (subKey == null)
                return;

            try
            {
                if (value is bool)
                {
                    subKey.SetValue(key.ToLower(), (bool)value ? bool.TrueString : bool.FalseString);    
                }
                else
                    subKey.SetValue(key.ToLower(),value);
                
            }
            catch (Exception)
            {
                return;
            }
        }



    }
}