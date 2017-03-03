using System.ComponentModel;
using OF.Core.Data.Settings;

namespace OF.Module.Interface.ViewModel
{
    public interface IDetailsSettingsViewModel : INotifyPropertyChanged
    {
        bool IsRequiredAdminRights { get; }

        void ApplySettings();

        object View { get; }

        void Initialize();

        bool HasDetailsChanges { get; }

        OFTypeInspectionPayloadSettings GetAdminSettings();


    }
}