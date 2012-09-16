using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;


namespace C4F.DevKit.PreviewHandler.Service.Logger
{
    public class WSSqlLogger
    {
        #region fields
        private ILog _log;
        #endregion

        private enum LevelLogging
        {
            Info,
            Warning,
            Error
        }


        #region fields static 

        private static WSSqlLogger _instance = null;
        private static object _lock = new object();

        #endregion

        private WSSqlLogger()
        {
            XmlConfigurator.Configure();
            _log = log4net.LogManager.GetLogger(typeof(WSSqlLogger));
        }

        public static WSSqlLogger Instance
        {
            get
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new WSSqlLogger();
                    }
                    return _instance;
                }
            }
        }


        #region public

        public void LogError(string message)
        {
            WriteLog(LevelLogging.Error, message);
        }

        public void LogInfo(string message)
        {
            WriteLog(LevelLogging.Info, message);
        }

        public void LogWarning(string message)
        {
            WriteLog(LevelLogging.Warning, message);
        }
        #endregion

        #region private

        private void WriteLog(LevelLogging level, string message)
        {
            if(_log == null)
                return;
            switch(level)
            {
                case LevelLogging.Info:
                    _log.Info(message);
                    break;
                case LevelLogging.Warning:
                    _log.Warn(message);
                    break;
                case LevelLogging.Error:
                    _log.Error(message);
                    break;
                default:
                    break;
            }
        }

        #endregion

    }
}
