using System.ComponentModel;

namespace OF.Module.Interface.ViewModel
{
    public interface IDetailsSettingsViewModel : INotifyPropertyChanged
    {
        void ApplySettings();

        object View { get; }

        void Initialize();

        bool HasDetailsChanges { get; }

            
    }
}