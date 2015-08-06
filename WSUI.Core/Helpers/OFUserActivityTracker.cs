using System;
using System.Threading;
using Nest;
using OF.Core.Core.ElasticSearch;
using OF.Core.ElasticSearch.Clients;
using OF.Core.Interfaces;
using OF.Core.Logger;
using OF.Core.Win32;

namespace OF.Core.Helpers
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
            _stop = true;
            _userActivityThread.Join(100);
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
                        Thread.Sleep(2000);
                        continue;
                    }
                    uint idleTimeSec = WindowsFunction.GetIdleTime() / 1000;
                    _elasticSearchClient.SetUserActivityTime((int)idleTimeSec);
                }
                catch (Exception ex)
                {
                    OFLogger.Instance.LogError(ex.Message);
                }
                Thread.Sleep(1000);
            }            
        }

    }
}