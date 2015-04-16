using System;
using System.Data;
using Microsoft.Win32;

namespace OF.Core.Helpers
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


        private const string ProductSubKey = "SOFTWARE\\OFOutlookPlugin";
        private const string SilentUpdateKey = "IsSilentUpdate";
        private const string IsOutlookClosedByInstallerKey = "IsOutlookClosedByInstaller";
        private const string CallIndexKey = "CallIndex";
        private const string OutlookFolderName = "OutlookFolderName";
        private const string OutlookFolderWebUrl = "OutlookFolderWebUrl";
        private const string PKeyId = "Id";
        private const string IsPluginUiVisible = "IsPluginUiVisible";
        private const string ElastiSearchPath = "ElasticSearchPath";

        private const string AddInOutlookSubKey = @"Software\Microsoft\Office\Outlook\Addins\OFOutlookPlugin.AddinModule";
        private const string RequireShutdownNotificationKey = "RequireShutdownNotification";
        private const string LoadBehaviorKey = "LoadBehavior"; // value => 3
        private const int LoadBehaviorDefaultValue = 3;
        private const string ADXStartModeKey = "ADXStartMode"; // value => NORMAL
        private const string ADXStartModeDefaultValue = "NORMAL"; // value => NORMAL

        private RegistryKey _baseRegistry = Registry.CurrentUser;

        private const string DefaultNamespace = "MAPI";

        public const string ProgIdKey = "OFOutlookPlugin.AddinModule";


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
            catch (Exception)
            {
                return;
            }
        }

        public void ResetLoadingAddinMode()
        {
            try
            {
                Write(LoadBehaviorKey,LoadBehaviorDefaultValue,false);
            }
            catch (Exception)
            {
                return;
            }
        }

        public void ResetAdxStartMode()
        {
            try
            {
                Write(ADXStartModeKey,ADXStartModeDefaultValue, false);
            }
            catch (Exception)
            {
                return;
            }
        }

        public void DeleteLoadingTime(string officeVersion)
        {
            const string AddInLoadTimesKey = "Software\\Microsoft\\Office\\{0}\\Outlook\\AddInLoadTimes";
            const string ModuleKey = "OFOutlookPlugin.AddinModule";

            var CurrentOulookVersion = string.Format(AddInLoadTimesKey, officeVersion);
            try
            {
                var key = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (key == null)
                    return;
                key.DeleteValue(ModuleKey);
            }
            catch (Exception)
            {
            }
        }

        public void DeleteAddIn(string officeVersion)
        {
            const string AddInLoadTimesKey = "Software\\Microsoft\\Office\\{0}\\Outlook\\AddIns\\OFOutlookPlugin.AddinModule";
            var CurrentOulookVersion = string.Format(AddInLoadTimesKey, officeVersion);

            try
            {
                var key = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (key == null)
                    return;

                foreach (var valueName in key.GetValueNames())
                {
                    key.DeleteValue(valueName);
                }
            }
            catch (Exception)
            {
            }
        }

        public void DeleteDisabling(string officeVersion)
        {
            const string DisableKey = "Software\\Microsoft\\Office\\{0}\\Outlook\\Resiliency\\DisabledItems";
            var CurrentOulookVersion = string.Format(DisableKey, officeVersion);
            RegistryKey registry = null;
            try
            {
                registry = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (registry != null)
                {

                    foreach (var item in registry.GetValueNames())
                    {
                        var val =
                            (byte[])registry.GetValue(item, null, RegistryValueOptions.DoNotExpandEnvironmentNames);

                        var temp = System.Text.Encoding.Unicode.GetString(val);
                        if (temp.Length > 0 && temp.IndexOf(ProgIdKey) > -1)
                        {
                            registry.DeleteValue(item);
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            finally
            {
                if (registry != null)
                {
                    registry.Close();
                }
            }
        }



        public Tuple<string, int> GetOutlookVersion()
        {
            try
            {
                RegistryKey reg = Registry.ClassesRoot.OpenSubKey("Outlook.Application");
                if (reg == null)
                {
                    return new Tuple<string, int>(string.Empty,-1);
                }
                var curVer = reg.OpenSubKey("CurVer");

                if (curVer == null)
                {
                    return new Tuple<string, int>(string.Empty, -1);
                }

                var val = curVer.GetValue(null) as string;

                if (string.IsNullOrEmpty(val))
                {
                    return new Tuple<string, int>(string.Empty, -1);
                }
                var arr = val.Split('.');
                if (arr == null || arr.Length == 0)
                {
                    return new Tuple<string, int>(string.Empty, -1);
                }

                int version = 0;
                int.TryParse(arr[arr.Length - 1], out version);
                return new Tuple<string, int>(string.Format("{0:0.0}",version),version);
            }
            catch (Exception)
            {   
            }
            return new Tuple<string, int>(string.Empty, -1);
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

        public string GetElasticSearchpath()
        {
            return ReadKey<string>(ElastiSearchPath);
        }

        public void SetElasticSearchPath(string path)
        {
            Write(ElastiSearchPath,path);
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