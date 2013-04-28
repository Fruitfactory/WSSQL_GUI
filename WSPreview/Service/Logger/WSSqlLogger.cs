using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using log4net.Config;
using System.Reflection;
using System.IO;


namespace C4F.DevKit.PreviewHandler.Service.Logger
{
    public class WSSqlLogger
    {

        private const string Filename = "log4net.config";

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
            string path = Assembly.GetAssembly(typeof(WSSqlLogger)).Location;
            path = path.Substring(0, path.LastIndexOf('\\') + 1);
            path = path + Filename;
            System.Diagnostics.Debug.WriteLine(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                var col = XmlConfigurator.Configure(fi);
                _log = log4net.LogManager.GetLogger("WSUILogger");
            }
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
