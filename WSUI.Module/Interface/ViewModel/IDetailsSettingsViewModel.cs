namespace OF.Module.Interface.ViewModel
{
    public interface IDetailsSettingsViewModel
    {
        void ApplySettings();

        object View { get; }

        void Initialize();

        bool HasDetailsChanges { get; }

            
    }
}