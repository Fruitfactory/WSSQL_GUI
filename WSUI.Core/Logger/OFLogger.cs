using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Practices.Prism.Logging;
using OF.Core.Helpers;

namespace OF.Core.Logger
{
    public class OFLogger : ILoggerFacade
    {
        private const string Filename = "log4net.config";
#if DEBUG
        private static readonly int DefaultLogginfLevels = 15;
#else
        private static readonly int DefaultLogginfLevels = 5;
#endif
        #region fields

        private ILog _log;

#endregion fields

        [Flags]
        public enum LevelLogging
        {
            Info = 0x01,
            Warning = 0x02,
            Error = 0x04,
            Debug = 0x08,
        }

#region fields static

        private static OFLogger _instance = null;
        private static object _lock = new object();

#endregion fields static

        private OFLogger()
        {
            string path = Assembly.GetAssembly(typeof(OFLogger)).Location;
            path = path.Substring(0, path.LastIndexOf('\\') + 1);
            path = path + Filename;
            System.Diagnostics.Debug.WriteLine(path);
            FileInfo fi = new FileInfo(path);
            if (fi.Exists)
            {
                var col = XmlConfigurator.Configure(fi);
                _log = log4net.LogManager.GetLogger("OFLogger");
            }
#if DEBUG
            int levels = DefaultLogginfLevels;
#else
            int levels = OFRegistryHelper.Instance.GetLoggingSettings();
#endif
            if (levels == default(int))
            {
                OFRegistryHelper.Instance.SetLoggingsettings(DefaultLogginfLevels);
            }
        }

        public static OFLogger Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ?? (_instance = new OFLogger());
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine(tempMessage);
#endif
#if CONSOLE
            Console.WriteLine(message);
#endif
        }

        public void LogError(string format, params object[] args)
        {
            var message = string.Format(format, args);

            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            string tempMessage = string.Format("{0}: {1}", methodBase.Name, message);
            WriteLog(LevelLogging.Error, tempMessage);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(tempMessage);
#endif
#if CONSOLE
            Console.WriteLine(message);
#endif
        }

        public void LogInfo(string message)
        {
            WriteLog(LevelLogging.Info, message);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
#if CONSOLE
            Console.WriteLine(message);
#endif
        }

        public void LogInfo(string format, params object[] args)
        {
            var message = string.Format(format, args);
            LogInfo(message);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
#if CONSOLE
            Console.WriteLine(message);
#endif
        }

        public void LogWarning(string message)
        {
            WriteLog(LevelLogging.Warning, message);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
        }

        public void LogWarning(string format, params object[] args)
        {
            var message = string.Format(format, args);
            LogWarning(message);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
#if CONSOLE
            Console.WriteLine(message);
#endif
        }

        public void LogDebug(string message)
        {
            WriteLog(LevelLogging.Debug, message);
#if DEBUG
            System.Diagnostics.Debug.WriteLine(message);
#endif
#if CONSOLE
          Console.WriteLine(message);
#endif

        }

        public void LogDebug(string format, params object[] args)
        {
            string message = string.Format(format, args);
            LogDebug(message);
        }



#endregion public

#region private

        private void WriteLog(LevelLogging level, string message)
        {
            if (_log == null || !IsEnabledLogLevel(level))
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
                case LevelLogging.Debug:
                    _log.Debug(message);
                    break;
            }
        }

        private bool IsEnabledLogLevel(LevelLogging level)
        {
            int levels = OFRegistryHelper.Instance.GetLoggingSettings();
            return level == (LevelLogging)((int)level & levels);
        }

#endregion private

        public void Log(string message, Category category, Priority priority)
        {

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