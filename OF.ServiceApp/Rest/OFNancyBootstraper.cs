using Microsoft.Practices.Prism.Events;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.TinyIoc;

namespace OF.ServiceApp.Rest
{
    public class OFNancyBootstraper : DefaultNancyBootstrapper
    {
        private IEventAggregator _eventAggregator;


        public OFNancyBootstraper(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
        }


        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
        {
            container.Register<IEventAggregator>(_eventAggregator);
            base.ApplicationStartup(container, pipelines);
        }
    }
}