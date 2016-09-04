namespace OF.Module.Interface.ViewModel
{
    public interface IServiceApplicationSettingsViewModel : IDetailsSettingsViewModel
    {
        bool IsElasticSearchServiceInstalled { get; set; }
        bool IsElasticSearchServiceRunning { get; set; }
    }
}