using OF.Core.Enums;
using OF.Core.Interfaces;

namespace OF.Core.Core.Inspections
{
    public abstract class OFSettingInspection : IOFSettingInspection
    {
        public virtual bool IsValidValueOfSetting { get { return true; } }

        public abstract void FixSetting(object value);

        public abstract OFSettingsType SettingsType { get; }
    }
}