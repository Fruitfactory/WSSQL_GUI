using System;
using System.Net.Configuration;
using Microsoft.Win32;

namespace WSUI.Core.Helpers
{
    public class RegistryHelper
    {
        public enum CallIndex
        {
            None,
            First,
            Second
        };

        #region [needs]

        private const string ProductSubKey = "SOFTWARE\\WSUIOutlookPlugin";
        private const string SilentUpdateKey = "IsSilentUpdate";
        private const string IsOutlookClosedByInstallerKey = "IsOutlookClosedByInstaller";
        private const string CallIndexKey = "CallIndex";


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
            return ProcessBoolKey(SilentUpdateKey);
        }

        public void StartSilentUpdate()
        {
            Write(SilentUpdateKey,true);
        }

        public void FinishSilentUpdate()
        {
            Write(SilentUpdateKey,false);
        }

        public bool IsOutlookClosedByInstaller()
        {
            return ProcessBoolKey(IsOutlookClosedByInstallerKey);
        }

        public void SetFlagClosedOutlookApplication()
        {
            Write(IsOutlookClosedByInstallerKey,true);
        }

        public void ResetFlagClosedOutlookApplication()
        {
            Write(IsOutlookClosedByInstallerKey,false);
        }

        private bool ProcessBoolKey(string key)
        {
            var v = ReadKey(key);
            if (string.IsNullOrEmpty(v))
                return false;
            bool parseresult = false;
            bool.TryParse(v, out parseresult);
            return parseresult;
        }

        public void SetCallIndexKey(CallIndex index)
        {
            Write(CallIndexKey,index);
        }

        public CallIndex GetCallIndex()
        {
            var val = ReadKey(CallIndexKey);
            var parse = (CallIndex)Enum.Parse(typeof (CallIndex), val);
            return parse;
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