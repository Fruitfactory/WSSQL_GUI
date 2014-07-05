using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using Microsoft.Office.Interop.Word;
using Threads = System.Threading;
using Tasks = System.Threading.Tasks;
using WSPreview.PreviewHandler.PreviewHandlerFramework;
using System.Diagnostics;
using System.IO;
using Microsoft.Win32;

namespace WSPreview.PreviewHandler.Service.Preview
{
    public class HelperPreviewHandlers
    {

        #region private static fields
#pragma warning disable 414
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
            Init();
            //Dispatcher.CurrentDispatcher.BeginInvoke(new Action(Init), null);
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
    }
}
