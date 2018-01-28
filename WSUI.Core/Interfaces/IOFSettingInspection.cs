using OF.Core.Enums;

namespace OF.Core.Interfaces
{
    public interface IOFSettingInspection
    {
        bool? IsValidValueOfSetting { get; }

        OFSettingsType SettingsType { get; }

        void FixSetting(object value);
    }
}