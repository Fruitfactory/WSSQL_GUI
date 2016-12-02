using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace OF.Core.Helpers
{
    public class OFRegistryHelper
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
        private const string LoggingSettings = "Logging";
        private const string OFPath = "ofpath";

        private const string AddInOutlookSubKey =
            @"Software\Microsoft\Office\Outlook\Addins\OFOutlookPlugin.AddinModule";

        private const string RequireShutdownNotificationKey = "RequireShutdownNotification";
        private const string LoadBehaviorKey = "LoadBehavior"; // value => 3
        private const int LoadBehaviorDefaultValue = 3;
        private const string ADXStartModeKey = "ADXStartMode"; // value => NORMAL
        private const string ADXStartModeDefaultValue = "NORMAL"; // value => NORMAL

        private RegistryKey _baseRegistry = Registry.CurrentUser;

        private const string OUTLOOKFINDER_HELPER_APPLICATION = "OutlookFinder Helper Application";


        private const string DefaultNamespace = "MAPI";

        public const string ProgIdKey = "OFOutlookPlugin.AddinModule";


        Dictionary<string, int> _keyDisableWarningsValues = new Dictionary<string, int>()
        {
            {"objectmodelguard",2 },
            {"promptsimplemapisend", 2},
            {"promptsimplemapinameresolve", 2},
            {"promptsimplemapiopenmessage", 2},
            {"promptoomsend", 2},
            {"promptoommeetingtaskrequestresponse", 2},
            {"promptoomaddressinformationaccess", 2},
            {"promptoomsaveas", 2},
            {"promptoomformulaaccess", 2},
            {"promptoomaddressbookaccess", 2},
            {"adminsecuritymode", 3}
        };


        #region [unistall]

        private static string UnistallKey = @"Software\Microsoft\Windows\CurrentVersion\Uninstall";
        private static string DisplayNameSubKey = "DisplayName";
        private static string DisplayVersionSubKey = "DisplayVersion";

        #endregion

        #endregion [needs]

        #region [instance]

        protected OFRegistryHelper()
        {
        }

        protected void Init()
        {
        }

        private static Lazy<OFRegistryHelper> _instance = new Lazy<OFRegistryHelper>(() =>
        {
            var helper = new OFRegistryHelper();
            helper.Init();
            return helper;
        });

        public static OFRegistryHelper Instance
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
                    Write(RequireShutdownNotificationKey, ShutdodwnNotificationNotRequired, false);
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
                Write(LoadBehaviorKey, LoadBehaviorDefaultValue, false);
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
                Write(ADXStartModeKey, ADXStartModeDefaultValue, false);
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

            var CurrentOulookVersion = String.Format(AddInLoadTimesKey, officeVersion);
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
            const string AddInLoadTimesKey =
                "Software\\Microsoft\\Office\\{0}\\Outlook\\AddIns\\OFOutlookPlugin.AddinModule";
            var CurrentOulookVersion = String.Format(AddInLoadTimesKey, officeVersion);

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
            var CurrentOulookVersion = String.Format(DisableKey, officeVersion);
            RegistryKey registry = null;
            try
            {
                registry = Registry.CurrentUser.OpenSubKey(CurrentOulookVersion, true);
                if (registry != null)
                {

                    foreach (var item in registry.GetValueNames())
                    {
                        var val =
                            (byte[]) registry.GetValue(item, null, RegistryValueOptions.DoNotExpandEnvironmentNames);

                        var temp = Encoding.Unicode.GetString(val);
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

        public void DisableOutlookSecurityWarning(string officeVersion, bool machineScope = true)
        {
            if (string.IsNullOrEmpty(officeVersion))
            {
                throw new ArgumentException("officeVersion");
            }
            var registry = machineScope ? Registry.LocalMachine : _baseRegistry;
            var key = machineScope ? string.Format(@"SOFTWARE\Microsoft\Office\{0}\Outlook\Security", officeVersion) : string.Format("Software\\Policies\\Microsoft\\Office\\{0}\\Outlook\\Security", officeVersion);
            var securityKey = registry.CreateSubKey(key);
            if (securityKey != null)
            {
                foreach (var keyDisableWarningsValue in _keyDisableWarningsValues)
                {
                    securityKey.SetValue(keyDisableWarningsValue.Key, keyDisableWarningsValue.Value);
                }
            }
            else
            {
                throw new NullReferenceException("SecutiryKey '" + key+"'");
            }
        }

        public void FixBeforeItemMoveEvent(string officeVersion)
        {
            //https://support.microsoft.com/en-us/kb/971367
            if (string.IsNullOrEmpty(officeVersion))
            {
                return;
            }
            var registry = _baseRegistry;
            try
            {
                var key = string.Format(@"SOFTWARE\Microsoft\Office\{0}\Outlook\Security", officeVersion);
                var preferenceKey = registry.CreateSubKey(key);
                if (preferenceKey != null)
                {
                    preferenceKey.SetValue("ReliableFolderMoveEvent", 1);
                }
            }
            catch (Exception)
            {

            }
        }


        public void DeleteOutlookSecuritySettings(string officeVersion,bool machineScope = true)
        {
            if (string.IsNullOrEmpty(officeVersion))
            {
                return;
            }
            var registry = machineScope ? Registry.LocalMachine : _baseRegistry;
            try
            {
                var key = machineScope ?string.Format(@"SOFTWARE\Microsoft\Office\{0}\Outlook\Security", officeVersion) : string.Format("Software\\Policies\\Microsoft\\Office\\{0}\\Outlook\\Security", officeVersion);
                var securityKey = registry.OpenSubKey(key, RegistryKeyPermissionCheck.ReadWriteSubTree);
                if (securityKey != null)
                {
                    foreach (var pair in _keyDisableWarningsValues)
                    {
                        securityKey.DeleteValue(pair.Key);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        public bool IsOutlookAutoCompleateDisabled(string officeVersion)
        {
            if (string.IsNullOrEmpty(officeVersion))
            {
                return false;
            }
            try
            {
                var registry = _baseRegistry;
                var key = string.Format("Software\\Microsoft\\Office\\{0}\\Outlook\\Preferences", officeVersion);
                var preferences = registry.OpenSubKey(key);
                var valObj = preferences.GetValue("ShowAutoSug");
                if (valObj == null)
                {
                    return false;
                }
                int valInt;
                int.TryParse(valObj.ToString(), out valInt);
                return valInt == 0;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public void DisableAutoCompleateEmailsToCcBcc(string officeVersion)
        {
            SetOutlookAutoCompleateValue(officeVersion,0);
        }

        public void EnableAutoCompleateEmailsToCcBcc(string officeVersion)
        {
            SetOutlookAutoCompleateValue(officeVersion, 1);
        }

        private void SetOutlookAutoCompleateValue(string officeVersion, int value)
        {
            if (string.IsNullOrEmpty(officeVersion))
            {
                return;
            }
            try
            {
                var registry = _baseRegistry;
                var key = string.Format("Software\\Microsoft\\Office\\{0}\\Outlook\\Preferences", officeVersion);
                var preferences = registry.OpenSubKey(key);
                if (preferences == null)
                {
                    return;
                }
                preferences.SetValue("ShowAutoSug", value);
            }
            catch (Exception ex)
            {

            }
        }


        public bool IsSecurityWarningDisable(string officeVersion)
        {
            if (string.IsNullOrEmpty(officeVersion))
            {
                return false;
            }
            try
            {
                var registry = _baseRegistry;
                var key = string.Format("Software\\Policies\\Microsoft\\Office\\{0}\\Outlook\\Security", officeVersion);
                var securityKey = registry.OpenSubKey(key);
                return securityKey != null;
            }
            catch (Exception)
            {

            }
            return false;
        }

        public bool IsAllSecuritySettingsApply(string officeVersion)
        {
            if (string.IsNullOrEmpty(officeVersion))
            {
                return false;
            }
            try
            {
                var registry = _baseRegistry;
                var key = string.Format("Software\\Policies\\Microsoft\\Office\\{0}\\Outlook\\Security", officeVersion);
                var securityKey = registry.OpenSubKey(key);
                if (securityKey != null)
                {
                    var valuenames = securityKey.GetValueNames().Select(name => name.ToUpperInvariant());
                    var keysPresent = _keyDisableWarningsValues.Keys.ToDictionary(p => p.ToUpperInvariant(), p => false);
                    foreach (var valuename in valuenames)
                    {
                        if (keysPresent.ContainsKey(valuename))
                        {
                            keysPresent[valuename] = true;
                        }
                    }
                    return keysPresent.Values.All(v => v);
                }
            }
            catch (Exception)
            {
            }
            return false;
        }


        public Tuple<string, int> GetOutlookVersion()
        {
            try
            {
                RegistryKey reg = Registry.ClassesRoot.OpenSubKey("Outlook.Application");
                if (reg == null)
                {
                    return new Tuple<string, int>(String.Empty, -1);
                }
                var curVer = reg.OpenSubKey("CurVer");

                if (curVer == null)
                {
                    return new Tuple<string, int>(String.Empty, -1);
                }

                var val = curVer.GetValue(null) as string;

                if (String.IsNullOrEmpty(val))
                {
                    return new Tuple<string, int>(String.Empty, -1);
                }
                var arr = val.Split('.');
                if (arr == null || arr.Length == 0)
                {
                    return new Tuple<string, int>(String.Empty, -1);
                }

                int version = 0;
                Int32.TryParse(arr[arr.Length - 1], out version);
                return new Tuple<string, int>(String.Format("{0:0.0}", version), version);
            }
            catch (Exception)
            {
            }
            return new Tuple<string, int>(String.Empty, -1);
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
            if (String.IsNullOrEmpty(v))
                return false;
            bool parseresult = false;
            Boolean.TryParse(v, out parseresult);
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
            return String.IsNullOrEmpty(res) || Boolean.Parse(res);
        }

        public void SetIsPluginUiVisible(bool visible)
        {
            Write(IsPluginUiVisible, visible);
        }

        public bool IsPluginUiVisibleKeyPresent()
        {
            return IsKeyPresent(IsPluginUiVisible);
        }

        public void DeletePluginUiKey()
        {
            DeleteKey(IsPluginUiVisible);
        }

        public string GetElasticSearchpath()
        {
            return ReadKey<string>(ElastiSearchPath);
        }

        public void SetElasticSearchPath(string path)
        {
            Write(ElastiSearchPath, path);
        }

        public string GetMachineOfPath()
        {
            return ReadMachineKey<string>(OFPath);
        }

        public string GetOfPath()
        {
            return ReadKey<string>(OFPath);
        }

        public void SetMachineOfPath(string path)
        {
            WriteMachine(OFPath, path);
        }

        public void SetOfPath(string path)
        {
            Write(OFPath, path);
        }


        public void SetLoggingsettings(int value)
        {
            Write(LoggingSettings, value);
        }

        public int GetLoggingSettings()
        {
            return ReadKey<int>(LoggingSettings);
        }


        public static void DeleteMachineProductKey()
        {
            try
            {
                var software = GetSoftwareKey(Registry.LocalMachine);
                if (software == null)
                {
                    return;
                }
                software.DeleteSubKey(ProductSubKey);
            }
            catch (Exception)
            {
            }
        }

        public static void DeleteUserProductKey()
        {
            try
            {
                var software = GetSoftwareKey(Registry.CurrentUser);
                if (software == null)
                {
                    return;
                }
                software.DeleteSubKey(ProductSubKey);
            }
            catch (Exception)
            {
            }
        }


        public void SetAutoRunHelperApplication(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }
            try
            {
                var filename = Path.Combine(path, "serviceapp.exe");
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.SetValue(OUTLOOKFINDER_HELPER_APPLICATION, "\"" + filename + "\"");
            }
            catch (Exception)
            {
            }
        }

        public bool IsServiceApplicationAutoRunExist()
        {
            try
            {
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                var value = add.GetValue(OUTLOOKFINDER_HELPER_APPLICATION);
                return value != null;
            }
            catch (Exception)
            {
            }
            return false;
        }


        public void DeleteAutoRunHelperApplication()
        {
            try
            {
                RegistryKey add = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                add.DeleteValue(OUTLOOKFINDER_HELPER_APPLICATION);
            }
            catch (Exception)
            {
            }
        }

        public void DeleteUnistallKeyNotCurrentVersion(string version, string productName)
        {
            try
            {
                RegistryKey unistall = Registry.CurrentUser.OpenSubKey(UnistallKey, true);
                if (unistall == null)
                {
                    return;
                }
                foreach (var subKeyName in unistall.GetSubKeyNames())
                {
                    var subKey = unistall.OpenSubKey(subKeyName);
                    if (subKey == null)
                        continue;
                    if ((string) subKey.GetValue(DisplayNameSubKey) == productName &&
                        (string) subKey.GetValue(DisplayVersionSubKey) != version)
                    {
                        unistall.DeleteSubKey(subKeyName);
                    }
                }
            }
            catch (UnauthorizedAccessException un)
            {
                throw un;
            }
            catch (NullReferenceException nul)
            {
                throw nul;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteUnistallKeyCurrentVersion(string version, string productName)
        {
            try
            {
                RegistryKey unistall = Registry.CurrentUser.OpenSubKey(UnistallKey, true);
                foreach (var subKeyName in unistall.GetSubKeyNames())
                {
                    var subKey = unistall.OpenSubKey(subKeyName);
                    if (subKey == null)
                        continue;
                    if ((string)subKey.GetValue(DisplayNameSubKey) == productName && (string)subKey.GetValue(DisplayVersionSubKey) == version)
                    {
                        unistall.DeleteSubKey(subKeyName);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region [restore outlook folders]

        public bool IsShouldRestoreOutlookFolder()
        {
            return !String.IsNullOrEmpty(ReadKey<string>(OutlookFolderName));
        }

        #endregion [restore outlook folders]


        private static RegistryKey GetSoftwareKey(RegistryKey rootKey)
        {
            return rootKey.CreateSubKey("SOFTWARE");
        }

        private T ReadKey<T>(string key, bool IsProduct = true)
        {
            try
            {
                RegistryKey subKey = GetRegistrySubKey(IsProduct);
                if (subKey == null)
                    return default(T);
                var objVal = subKey.GetValue(key.ToLower());
                return objVal != null ? (T)objVal : default(T);
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        private T ReadMachineKey<T>(string key)
        {
            try
            {
                RegistryKey subkey = GetMachineRegistryKey();
                if (subkey == null)
                    return default(T);

                var objVal = subkey.GetValue(key.ToLower());
                return objVal != null ? (T)objVal : default(T);
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
                RegistryKey subKey = GetRegistrySubKey(IsProduct);
                if (subKey == null)
                    return;
                if (value is bool)
                {
                    subKey.SetValue(key.ToLower(), (bool)value ? Boolean.TrueString : Boolean.FalseString);
                }
                else
                    subKey.SetValue(key.ToLower(), value);
            }
            catch (Exception)
            {
                return;
            }
        }

        private void WriteMachine(string key, object value)
        {
            try
            {
                RegistryKey registryKey = GetMachineRegistryKey();
                if (registryKey == null)
                    return;
                if (value is bool)
                {
                    registryKey.SetValue(key.ToLower(), (bool)value ? Boolean.TrueString : Boolean.FalseString);
                }
                else
                    registryKey.SetValue(key.ToLower(), value);
            }
            catch (Exception)
            {
                return;
            }
        }

        private bool IsKeyPresent(string key, bool IsProduct = true)
        {
            try
            {
                RegistryKey subKey = GetRegistrySubKey(IsProduct);
                if (subKey == null)
                {
                    return false;
                }
                var values = subKey.GetValueNames();
                var lowerKey = key.ToLowerInvariant();
                return values != null && values.Any(v => v.ToLowerInvariant().Equals(lowerKey));
            }
            catch
            {
                return false;
            }
        }

        private void DeleteKey(string key, bool isProduct = true)
        {
            try
            {
                RegistryKey subKey = GetRegistrySubKey(isProduct);
                if (subKey == null)
                {
                    return;
                }
                subKey.DeleteValue(key.ToLowerInvariant());
            }
            catch (Exception)
            {
            }
        }



        private RegistryKey GetRegistrySubKey(bool IsProduct = true)
        {
            try
            {
                RegistryKey temp = _baseRegistry;
                RegistryKey subKey = temp.CreateSubKey(IsProduct ? ProductSubKey : AddInOutlookSubKey);
                return subKey;
            }
            catch (Exception)
            {
                return null;
            }
        }



        private RegistryKey GetMachineRegistryKey()
        {
            try
            {
                RegistryKey temp = Registry.LocalMachine;
                RegistryKey subKey = temp.CreateSubKey(ProductSubKey);
                return subKey;
            }
            catch (Exception)
            {
                return null;
            }
        }


        public String GetJavaInstallationPath()
        {
            String javaKey = "SOFTWARE\\JavaSoft\\Java Runtime Environment";
            String javaWow3264Node = "SOFTWARE\\Wow6432Node\\JavaSoft\\Java Runtime Environment";

            var machineKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            if (machineKey == null)
            {
                return "";
            }

            using (var baseKey = machineKey.OpenSubKey(javaKey))
            {

                if (baseKey != null)
                {
                    String currentVersion = baseKey.GetValue("CurrentVersion").ToString();
                    using (var homeKey = baseKey.OpenSubKey(currentVersion))
                        return homeKey.GetValue("JavaHome").ToString();
                }
            }
            using (var baseWowKey = machineKey.OpenSubKey(javaWow3264Node))
            {
                if (baseWowKey != null)
                {
                    String currentVersion = baseWowKey.GetValue("CurrentVersion").ToString();
                    using (var homeKey = baseWowKey.OpenSubKey(currentVersion))
                        return homeKey.GetValue("JavaHome").ToString();
                }
            }


            return "";
        }
    }
}