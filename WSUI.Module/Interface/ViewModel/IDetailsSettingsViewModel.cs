using System.ComponentModel;

namespace OF.Module.Interface.ViewModel
{
    public interface IDetailsSettingsViewModel : INotifyPropertyChanged
    {
        bool IsRequiredAdminRights { get; }

        void ApplySettings();

        object View { get; }

        void Initialize();

        bool HasDetailsChanges { get; }

            
    }
}