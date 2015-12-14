using System;
using System.Threading;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Win32;
using OF.Infrastructure.Implements.ElasticSearch.Clients;

namespace OF.Infrastructure.Service.Helpers
{
    public class OFUserActivityTracker : IUserActivityTracker
    {
        private OFElasticTrackingClient _elasticSearchClient;
        private Thread _userActivityThread;
        private volatile bool _stop;

        public OFUserActivityTracker()
        {
            _elasticSearchClient = new OFElasticTrackingClient();
            _userActivityThread = new Thread(UserActivityProcess);
        }


        public void Start()
        {
            _userActivityThread.Start();
        }

        public void Stop()
        {
            if (!_userActivityThread.IsAlive)
            {
                return;
            }
            _stop = true;
            _userActivityThread.Join();
        }

        private void UserActivityProcess(object arg)
        {
            IIndexExistsRequest request = new IndexExistsRequest(OFElasticSearchClientBase.DefaultInfrastructureName);

            while (!_stop)
            {
                try
                {
                    var resp = _elasticSearchClient.IndexExists(request);
                    if (!resp.Exists)
                    {
                        OFLogger.Instance.LogDebug("ES Exist = " + resp.Exists);
                        Thread.Sleep(2000);
                        continue;
                    }
                    uint idleTimeSec = WindowsFunction.GetIdleTime() / 1000;
                    OFLogger.Instance.LogDebug("Input = " + idleTimeSec);
                    _elasticSearchClient.SetUserActivityTime((int)idleTimeSec);
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.ToString());
                }
                Thread.Sleep(1000);
            }            
        }

    }
}