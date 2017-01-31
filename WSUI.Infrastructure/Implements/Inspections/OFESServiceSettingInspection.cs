using OF.Core.Enums;
using OF.Core.Extensions;
using OF.Core.Helpers;
using OF.Core.Core.Inspections;

namespace OF.Infrastructure.Implements.Inspections
{
    public sealed class OFESServiceSettingInspection : OFSettingInspection
    {
        public override bool IsValidValueOfSetting
        {
            get { return CheckSetting(); }
        }

        public override void FixSetting(object value)
        {
            string javaRuntime = OFRegistryHelper.Instance.GetJavaRuntimeLibPath();
            OFRegistryHelper.Instance.SetESJvmValue(javaRuntime);
        }

        public override OFSettingsType SettingsType
        {
            get { return OFSettingsType.CheckAndFixJvmServivePath; }
        }

        private bool CheckSetting()
        {
            string javaRuntime = OFRegistryHelper.Instance.GetJavaRuntimeLibPath();
            string esJvm = OFRegistryHelper.Instance.GetESJvmValue();
            return javaRuntime.IsNotNull() && esJvm.IsNotNull() && javaRuntime == esJvm;
        }

    }

}