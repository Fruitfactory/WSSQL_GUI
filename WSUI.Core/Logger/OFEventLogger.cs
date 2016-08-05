using System;
using System.Diagnostics;

namespace OF.Core.Logger
{
    public class OFEventLogger
    {

        #region [needs]

        private static readonly string SOURCE = "OutlookFinder";
        private static readonly string LOG = "Application";
        private static readonly string ERROR_EVENT = "Error";
        private static readonly string WARNING_EVENT = "Warning";
        private static readonly string Info_EVENT = "Info";

        #endregion

        #region [static]

        private static Lazy<OFEventLogger> _instance = new Lazy<OFEventLogger>(() =>
        {
            var inst = new OFEventLogger();
            inst.Initialize();
            return inst;
        });

        public static OFEventLogger Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        private OFEventLogger()
        {
            
        }


        private void Initialize()
        {
            if (!EventLog.SourceExists(SOURCE))
            {
                EventLog.CreateEventSource(SOURCE,LOG);
            }
        }


        public void LogError(string message)
        {
            EventLog.WriteEntry(SOURCE,message,EventLogEntryType.Error,0);
        }


        public void LogWarning(string message)
        {
            EventLog.WriteEntry(SOURCE,message,EventLogEntryType.Warning,1);
        }

        public void LogInfo(string message)
        {
            EventLog.WriteEntry(SOURCE,message,EventLogEntryType.Information,2);
        }

    }
}