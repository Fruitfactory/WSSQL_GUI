using System;
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

        private const int ShutdodwnNotificationNotRequired = 0;
        private const int ShutdodwnNotificationRequired = 1;


        private const string ProductSubKey = "SOFTWARE\\WSUIOutlookPlugin";
        private const string SilentUpdateKey = "IsSilentUpdate";
        private const string IsOutlookClosedByInstallerKey = "IsOutlookClosedByInstaller";
        private const string CallIndexKey = "CallIndex";
        private const string OutlookFolderName = "OutlookFolderName";
        private const string OutlookFolderWebUrl = "OutlookFolderWebUrl";
        private const string PKeyId = "Id";
        private const string IsPluginUiVisible = "IsPluginUiVisible";

        private const string AddInOutlookSubKey = @"Software\Microsoft\Office\Outlook\Addins\WSUIOutlookPlugin.AddinModule";
        private const string RequireShutdownNotificationKey = "RequireShutdownNotification";


        private RegistryKey _baseRegistry = Registry.CurrentUser;

        private const string DefaultNamespace = "MAPI";

        #endregion [needs]

        #region [instance]

        protected RegistryHelper()
        { }

        protected void Init()
        {
        }

        private static Lazy<RegistryHelper> _instance = new Lazy<RegistryHelper>(() =>
                                                                                     {
                                                                                         var helper = new RegistryHelper();
                                                                                         helper.Init();
                                                                                         return helper;
                                                                                     });

        public static RegistryHelper Instance
        {
            get { return _instance.Value; }
        }

        #endregion [instance]

        public void ResetShutdownNotification()
        {
            var valKey = ReadKey<int>(RequireShutdownNotificationKey, false);
            try
            {
                var valInt = Convert.ToInt32(valKey);
                if (valInt == ShutdodwnNotificationRequired)
                {
                    Write(RequireShutdownNotificationKey,ShutdodwnNotificationNotRequired,false);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public bool IsSilendUpdate()
        {
            return ProcessBoolKey(SilentUpdateKey);
        }

        public void StartSilentUpdate()
        {
            Write(SilentUpdateKey, true);
        }

        public void FinishSilentUpdate()
        {
            Write(SilentUpdateKey, false);
        }

        public bool IsOutlookClosedByInstaller()
        {
            return ProcessBoolKey(IsOutlookClosedByInstallerKey);
        }

        public void SetFlagClosedOutlookApplication()
        {
            Write(IsOutlookClosedByInstallerKey, true);
        }

        public void ResetFlagClosedOutlookApplication()
        {
            Write(IsOutlookClosedByInstallerKey, false);
        }

        private bool ProcessBoolKey(string key)
        {
            var v = ReadKey<string>(key);
            if (string.IsNullOrEmpty(v))
                return false;
            bool parseresult = false;
            bool.TryParse(v, out parseresult);
            return parseresult;
        }

        public void SetCallIndexKey(CallIndex index)
        {
            Write(CallIndexKey, index);
        }

        public CallIndex GetCallIndex()
        {
            var val = ReadKey<string>(CallIndexKey);
            var parse = (CallIndex)Enum.Parse(typeof(CallIndex), val);
            return parse;
        }

        public void SetOutlookFolderName(string folderName)
        {
            Write(OutlookFolderName, folderName);
        }

        public string GetOutllokFolderName()
        {
            return ReadKey<string>(OutlookFolderName);
        }

        public void SetOutlookFolderWebUrl(string url)
        {
            Write(OutlookFolderWebUrl, url);
        }

        public string GetOutlookFolderWebUrl()
        {
            return ReadKey<string>(OutlookFolderWebUrl);
        }

        public void SetPKetId(string id)
        {
            Write(PKeyId, id);
        }

        public string GetPKeyId()
        {
            return ReadKey<string>(PKeyId);
        }

        public bool GetIsPluginUiVisible()
        {
            var res = ReadKey<string>(IsPluginUiVisible);
            return string.IsNullOrEmpty(res) || bool.Parse(res);
        }

        public void SetIsPluginUiVisible(bool visible)
        {
            Write(IsPluginUiVisible, visible);
        }

        #region [restore outlook folders]

        public bool IsShouldRestoreOutlookFolder()
        {
            return !string.IsNullOrEmpty(ReadKey<string>(OutlookFolderName));
        }

        #endregion [restore outlook folders]

        private T ReadKey<T>(string key, bool IsProduct = true)
        {
            try
            {
                RegistryKey temp = _baseRegistry;
                RegistryKey subKey = temp.CreateSubKey(IsProduct ? ProductSubKey : AddInOutlookSubKey);
                if (subKey == null)
                    return default(T);
                var objVal = subKey.GetValue(key.ToLower());
                return objVal != null ? (T)objVal : default (T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private void Write(string key, object value, bool IsProduct = true)
        {
            try
            {
                RegistryKey temp = _baseRegistry;
                RegistryKey subKey = temp.CreateSubKey(IsProduct ? ProductSubKey : AddInOutlookSubKey);
                if (subKey == null)
                    return;
                if (value is bool)
                {
                    subKey.SetValue(key.ToLower(), (bool)value ? bool.TrueString : bool.FalseString);
                }
                else
                    subKey.SetValue(key.ToLower(), value);
            }
            catch (Exception)
            {
                return;
            }
        }

    }
}