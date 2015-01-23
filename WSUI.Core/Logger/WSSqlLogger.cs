using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Practices.Prism.Logging;

namespace WSUI.Core.Logger
{
    public class WSSqlLogger : ILoggerFacade
    {
        private const string Filename = "log4net.config";

        #region fields

        private ILog _log;

        #endregion fields

        private enum LevelLogging
        {
            Info,
            Warning,
            Error
        }

        #region fields static

        private static WSSqlLogger _instance = null;
        private static object _lock = new object();

        #endregion fields static

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
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            string tempMessage = string.Format("{0}: {1}", methodBase.Name, message);    
            WriteLog(LevelLogging.Error, tempMessage);
        }

        public void LogError(string format, params object[] args)
        {
            var message = string.Format(format, args);

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            string tempMessage = string.Format("{0}: {1}", methodBase.Name, message);
            WriteLog(LevelLogging.Error, tempMessage);
        }

        public void LogInfo(string message)
        {
            WriteLog(LevelLogging.Info, message);
        }

        public void LogInfo(string format, params object[] args)
        {
            var message = string.Format(format, args);
            LogInfo(message);
        }

        public void LogWarning(string message)
        {
            WriteLog(LevelLogging.Warning, message);
        }

        public void LogWarning(string format, params object[] args)
        {
            var message = string.Format(format, args);
            LogWarning(message);
        }

        #endregion public

        #region private

        private void WriteLog(LevelLogging level, string message)
        {
            if (_log == null)
                return;
            switch (level)
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

        #endregion private

        public void Log(string message, Category category, Priority priority)
        {
            //if (_watch != null && _watch.IsRunning)
            //{
            //    _watch.Stop();
            //    WriteLog(LevelLogging.Warning, string.Format("Last Elapsed: {0}ms", _watch.ElapsedMilliseconds));
            //}

            switch (category)
            {
                case Category.Warn:
                    WriteLog(LevelLogging.Warning, message);
                    break;

                case Category.Debug:
                    WriteLog(LevelLogging.Info, message);
                    break;

                case Category.Info:
                    WriteLog(LevelLogging.Info, message);
                    break;

                case Category.Exception:
                    WriteLog(LevelLogging.Error, message);
                    break;
            }
            //(_watch = new Stopwatch()).Start();
        }
    }
}