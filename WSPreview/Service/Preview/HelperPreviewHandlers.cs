﻿using System;
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
        private readonly Dictionary<ControlsKey, Type> _poolPreviewControlsTypes; 
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
            _poolPreviewControlsTypes = new Dictionary<ControlsKey, Type>();
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

        public PreviewHandlerFramework.PreviewHandler GetReadyHandler(string ext)
        {
            var type = HandlersDictionary.ContainsKey(ext) ? HandlersDictionary[ext] : null;
            if (type == null)
                return null;
            var handler = Activator.CreateInstance(type) as PreviewHandlerFramework.PreviewHandler;
            return handler;
        }

        public IPreviewControl GetPreviewControl(ControlsKey key)
        {
            var ctrl = _poolPreviewControlsTypes.ContainsKey(key) ?  Activator.CreateInstance(_poolPreviewControlsTypes[key]) as IPreviewControl : null;
            return ctrl;
        }

        public void Inititialize()
        {
            Init();
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
                if(key == ControlsKey.None || _poolPreviewControlsTypes.ContainsKey(key))
                    continue;
                _poolPreviewControlsTypes.Add(key,listType);
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
