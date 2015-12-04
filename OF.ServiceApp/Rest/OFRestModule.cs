using Microsoft.Practices.Prism.Events;
using Nancy;
using OF.ServiceApp.Events;
using OF.ServiceApp.Interfaces;

namespace OF.ServiceApp.Rest
{
    public class OFRestModule : NancyModule, IOFRestModule
    {

        #region [needs]

        private IEventAggregator _eventAggregator;

        #endregion

        public OFRestModule(IEventAggregator eventAggregator)
            :base("/serviceapp")
        {
            _eventAggregator = eventAggregator;
            Get["/stop"] = Stop;
            Get["/status"] = Status;
            
        }

        public object Stop(object arg)
        {
            _eventAggregator.GetEvent<OFStopEvent>().Publish(true);
            return null;
        }

        public object Status(object arg)
        {
            return true;
        }
    }
}