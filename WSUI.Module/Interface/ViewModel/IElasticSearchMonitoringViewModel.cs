namespace OF.Module.Interface.ViewModel
{
    public interface IElasticSearchMonitoringViewModel
    {
        object View { get; }
        void Start();
        void Stop();

        bool IsRunning { get; }
    }
}