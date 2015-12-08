using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;
using OF.Core.Logger;

namespace OF.ServiceApp.Rest
{
    public class OFRestHosting : IDisposable
    {
        private NancyHost _host;


        public OFRestHosting(INancyBootstrapper bootstrapper)
        {
            var hostConfiguration = new HostConfiguration(){UrlReservations =  new UrlReservations(){CreateAutomatically =  true}};
            _host = new NancyHost(new Uri("http://localhost:11223"),bootstrapper,hostConfiguration);    
        }

        public void Start()
        {
            try
            {
                _host.Start();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
            
        }

        public void Stop()
        {
            try
            {
                _host.Stop();
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
            }
        }

        public void Dispose()
        {
            Stop();
            _host.Dispose();
            _host = null;
        }
    }
}