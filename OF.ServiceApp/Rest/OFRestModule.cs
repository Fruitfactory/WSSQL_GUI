using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Practices.Prism.Events;
using Nancy;
using OF.Core.Logger;
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
            Post["/stop"] = Stop;
            Get["/startread"] = StartRead;
            Get["/startread/{arg}"] = StartRead;
            Get["/stopread"] = StopRead;
            Get["/suspendread"] = SuspendRead;
            Get["/resumeread"] = ResumeRead;
            Get["/resumeread/{arg}"] = ResumeRead;
        }

        public object Stop(object arg)
        {
            Task.Factory.StartNew(() => _eventAggregator.GetEvent<OFStopEvent>().Publish(true));
            return Response.AsJson(new{Result = true});
        }

        public object StartRead(object arg)
        {
            var dict = arg as DynamicDictionary;
            if (dict == null)
            {
                return Response.AsJson(new { Result = false });
            }
            try
            {
                if (dict.Keys.Any())
                {
                    OFLogger.Instance.LogDebug("Update of attachments...");
                    DateTime date = DateTime.Parse(dict.Values.First());
                    _eventAggregator.GetEvent<OFStartReadEvent>().Publish(date);
                }
                else
                {
                    OFLogger.Instance.LogDebug("Initial reading of attachments...");
                    _eventAggregator.GetEvent<OFStartReadEvent>().Publish(null);
                }
                return Response.AsJson(new { Result = true });
            }
            catch (Exception ex)
            {
                OFLogger.Instance.LogError(ex.ToString());
                return Response.AsJson(new { Result = false });
            }
        }

        public object SuspendRead(object arg)
        {
            _eventAggregator.GetEvent<OFSuspendReadEvent>().Publish(true);
            OFLogger.Instance.LogDebug("Suspend Read...");
            return Response.AsJson(new { Result = true });
        }

        public object ResumeRead(object arg)
        {
            var lastDate = GetLastDate(arg);
            _eventAggregator.GetEvent<OFResumeReadEvent>().Publish(lastDate);
            OFLogger.Instance.LogDebug("Resume Read...");
            return Response.AsJson(new { Result = true });
        }

        public object StopRead(object arg)
        {
            _eventAggregator.GetEvent<OFStopReadEvent>().Publish(true);
            OFLogger.Instance.LogDebug("Stop Read...");
            return Response.AsJson(new { Result = true });
        }

        public object Status(object arg)
        {
            return Response.AsJson(new{Result = true});
        }

        private DateTime? GetLastDate(object arg)
        {
            var dict = arg as DynamicDictionary;
            if (dict == null || !dict.Keys.Any())
            {
                return null;
            }
            DateTime date = DateTime.Parse(dict.Values.First());
            return (DateTime?)date;
        }

    }
}