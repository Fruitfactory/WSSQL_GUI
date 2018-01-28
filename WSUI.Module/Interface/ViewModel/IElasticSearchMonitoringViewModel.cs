using System;
using OF.Core.Data.ElasticSearch.Response;
using OF.Infrastructure.Services;

namespace OF.Module.Interface.ViewModel
{
    public interface IElasticSearchMonitoringViewModel
    {
        object View { get; }
        void Start();
        void Stop();

        bool IsRunning { get; }
        event EventHandler<EventArgs<OFRiverStatus>> StatusChanged;
    }
}