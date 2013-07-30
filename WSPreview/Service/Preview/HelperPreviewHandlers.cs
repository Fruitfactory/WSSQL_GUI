using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Windows.Threading;
using Threads = System.Threading;
using Tasks = System.Threading.Tasks;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace WSPreview.PreviewHandler.Service.Preview
{
    public class HelperPreviewHandlers
    {

        #region private static fields

        private static string HKLMWinSdk32 = "SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows";
        private static string HKLMWinSdk64 = "SOFTWARE\\Wow6432Node\\Microsoft\\Microsoft SDKs\\Windows";
        private static string InstallationFolder = "InstallationFolder";
        #endregion

        #region [needs]

        private readonly string InterfaceName = "WSPreview.PreviewHandler.PreviewHandlerFramework.IPreviewControl";

        private readonly Dictionary<string, Type> _handlersDictionary = null;
        private readonly Dictionary<string, PreviewHandler.PreviewHandlerFramework.PreviewHandler> _poolPreviewHandlers;
        private readonly Dictionary<ControlsKey, IPreviewControl> _poolPreviewControls; 
        private readonly static Lazy<HelperPreviewHandlers>  _instance = new Lazy<HelperPreviewHandlers>(() =>
                                                                                                    {
                                                                                                        var inst = new HelperPreviewHandlers ();
                                                                                                        return inst;
                                                                                                    },Threads.LazyThreadSafetyMode.ExecutionAndPublication);

        #endregion

        #region [ctor]

        private HelperPreviewHandlers()
        {   
            _handlersDictionary = new Dictionary<string, Type>();
            _poolPreviewHandlers = new Dictionary<string, PreviewHandlerFramework.PreviewHandler>();
            _poolPreviewControls = new Dictionary<ControlsKey, IPreviewControl>();
        }

        #endregion

        public static HelperPreviewHandlers Instance
        {
            get { return _instance.Value; }
        }

        public Dictionary<string, Type> HandlersDictionary
        {
            get { return _handlersDictionary; }
        }

        public Dictionary<string,PreviewHandlerFramework.PreviewHandler> HandlersInstances
        {
            get { return _poolPreviewHandlers; }
        }

        public PreviewHandlerFramework.PreviewHandler GetReadyHandler(string ext)
        {
            var type = HandlersDictionary.ContainsKey(ext) ? HandlersDictionary[ext] : null;
            if (type == null)
                return null;
            string key = type.FullName;
            var handler = HandlersInstances.ContainsKey(key) ? HandlersInstances[key] : null;
            return handler;
        }

        public IPreviewControl GetPreviewControl(ControlsKey key)
        {
            var ctrl = _poolPreviewControls.ContainsKey(key) ? _poolPreviewControls[key] : null;
            return ctrl;
        }

        public void Inititialize()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Init), null);
        }

        #region [private]

        private void Init()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(PreviewHandlerFramework.PreviewHandler));
            InitPreviewHandlers(assembly);
            InitPreviewControls(assembly);
        }
        
        private void InitPreviewHandlers(Assembly assembly)
        {
            var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(StreamBasedPreviewHandler)))).ToList();
            foreach (var type in listTypes)
            {
                object[] attr = (object[])type.GetCustomAttributes(typeof(PreviewHandlerAttribute), true);
                if (attr == null || attr.Length == 0)
                    continue;
                PreviewHandlerAttribute prewAttr = attr[0] as PreviewHandlerAttribute;
                string[] exts = prewAttr.Extension.Split(';');
                foreach (var ext in exts)
                {
                    if (!_handlersDictionary.ContainsKey(ext))
                        _handlersDictionary.Add(ext, type);
                }
                var handler = Activator.CreateInstance(type) as PreviewHandlerFramework.PreviewHandler;
                _poolPreviewHandlers.Add(type.FullName, handler);
            } 
        }

        private void InitPreviewControls(Assembly assembly)
        {
            var listTypes = assembly.GetTypes().Where(type => type.GetInterface(InterfaceName,true) != null);
            if(!listTypes.Any())
                return;
            foreach (var listType in listTypes)
            {
                var key = GetControlKey(listType);
                if(key == ControlsKey.None)
                    continue;
                var ctrl = Activator.CreateInstance(listType) as IPreviewControl;
                _poolPreviewControls.Add(key,ctrl);
            }
        }

        private ControlsKey GetControlKey(Type type)
        {
            var attr = type.GetCustomAttributes(typeof (KeyControlAttribute), true);
            if(attr.Length == 0)
                return ControlsKey.None;
            var a = attr[0] as KeyControlAttribute;
            return a.Key;
        }


        #endregion

        #region [static public]

        public static bool RegisterHandlers()
        {
            string gac = GetPathToGacutil();
            string reg = GetPathToRegasm();

            if(string.IsNullOrEmpty(gac) || string.IsNullOrEmpty(reg))
                return false;

            Assembly assembly = Assembly.GetAssembly(typeof(PreviewHandler.PreviewHandlerFramework.PreviewHandler));
            string arg = string.Format(" -i {0}", assembly.Location);
            ProcessStartInfo processInfo = new ProcessStartInfo(gac, arg);
            processInfo.UseShellExecute = false;
            Process process = Process.Start(processInfo);
            process.WaitForExit();

            arg = string.Format(" /codebase {0}", assembly.Location);
            processInfo = new ProcessStartInfo(reg, arg);
            processInfo.UseShellExecute = false;
            process = Process.Start(processInfo);
            process.WaitForExit();

            //var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(WSPreview.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(WSPreview.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
            //listTypes.ForEach(type => PreviewHandler.PreviewHandlerFramework.PreviewHandler.Register(type));

            return true;
        }

        
        

        public static bool UnregisterHandlers()
        {
            string gac = GetPathToGacutil();
            string reg = GetPathToRegasm();

            if (string.IsNullOrEmpty(gac) || string.IsNullOrEmpty(reg))
                return false;

            Assembly assembly = Assembly.GetAssembly(typeof(PreviewHandler.PreviewHandlerFramework.PreviewHandler));
            string arg = string.Format(" /unregister {0}", assembly.Location);
            ProcessStartInfo processInfo = new ProcessStartInfo(reg, arg);
            processInfo.UseShellExecute = false;
            Process process = Process.Start(processInfo);
            process.WaitForExit();

            arg = string.Format(" -u {0}", assembly.Location);
            processInfo = new ProcessStartInfo(gac, arg);
            processInfo.UseShellExecute = false;
            process = Process.Start(processInfo);
            process.WaitForExit();

            //var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(WSPreview.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(WSPreview.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
            //listTypes.ForEach(type => PreviewHandler.PreviewHandlerFramework.PreviewHandler.Register(type));


            return true;
        }

        #endregion

        #region private static

        private static string GetPathToRegasm()
        {
            string pathTemplate = "{0}\\regasm.exe";
            string path = string.Format(pathTemplate, ToolLocationHelper.GetPathToDotNetFramework(TargetDotNetFrameworkVersion.VersionLatest));
            if (File.Exists(path))
                return path;
            else
                return null;
        }


        private static string GetPathToGacutil()
        {
            string pathTemplate = "{0}bin\\gacutil.exe";
            string path = null;
            RegistryKey winSdk = null;
            if (Environment.Is64BitOperatingSystem)
            {
                winSdk = Registry.LocalMachine.OpenSubKey(HKLMWinSdk64);
                
            }
            else
            {
                winSdk = Registry.LocalMachine.OpenSubKey(HKLMWinSdk32);
            }
            if (winSdk != null)
            {
                string[] sdks = winSdk.GetSubKeyNames();
                foreach (string sub in sdks)
                {
                    RegistryKey k = winSdk.OpenSubKey(sub);
                    if (k == null) continue;
                    var folder = k.GetValue(InstallationFolder) as string;
                    if (folder != null)
                    {
                        path = string.Format(pathTemplate, folder);
                        break;
                    }
                }
            }
            if (string.IsNullOrEmpty(path) || !File.Exists(path))
                return null;
            else
                return path;
        }



        #endregion

    }
}
