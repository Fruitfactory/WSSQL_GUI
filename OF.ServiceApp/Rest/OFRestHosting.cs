using System;
using Nancy.Bootstrapper;
using Nancy.Hosting.Self;

namespace OF.ServiceApp.Rest
{
    public class OFRestHosting : IDisposable
    {
        private NancyHost _host;


        public OFRestHosting(INancyBootstrapper bootstrapper)
        {
            _host = new NancyHost(new Uri("http://localhost:11223"),bootstrapper);    
        }

        public void Start()
        {
            _host.Start();
        }

        public void Stop()
        {
            _host.Stop();
        }

        public void Dispose()
        {
            _host.Stop();
            _host.Dispose();
            _host = null;
        }
    }
}