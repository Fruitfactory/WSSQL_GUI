using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using C4F.DevKit.PreviewHandler.Service.Logger;

namespace WSUIOutlookPlugin.Core
{
    public class DllPreloader
    {
        #region [needs]

        private const string DllExt = "*.dll";
        private List<string> _listPartNameOfDll = new List<string>();

        #endregion
        
        #region [ctor]
        
        protected DllPreloader()
        {

        }
        #endregion

        #region [static]
         
        private static Lazy<DllPreloader> _instance = new Lazy<DllPreloader>(() =>
                                                                                 {
                                                                                     var inst = new DllPreloader();
                                                                                     inst.Init();
                                                                                     return inst;
                                                                                 });

        public static DllPreloader Instance
        {
            get { return _instance.Value; }
        }

        #endregion

        #region [public]

        public void PreloadDll()
        {
            var preLoadThread = new Thread(PreloadDllInThread);
            //ThreadPool.QueueUserWorkItem(PreloadDllInThread);
            preLoadThread.Start(null);
        }

        #endregion

        #region [private]

        private void Init()
        {
            // add part of dll name which should be loaded during loading plugin
            _listPartNameOfDll.Add("MahApps");
            _listPartNameOfDll.Add("Practices");
            _listPartNameOfDll.Add("WSUI");
            _listPartNameOfDll.Add("WSPreview");
        }

        private void PreloadDllInThread(object state)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(currentFolder, DllExt, SearchOption.TopDirectoryOnly);//.Where(filename => _listPartNameOfDll.Any(s => Path.GetFileName(filename).Contains(s)));
            if (!files.Any())
                return;
            foreach (var file in files)
            {
                try
                {
                    //_listPartNameOfDll.Any(s => assembleName.Name.Contains(s))
                    //    &&
                    var assembleName = AssemblyName.GetAssemblyName(file);
                    if ( !assemblies.Any(a => AssemblyName.ReferenceMatchesDefinition(assembleName, a.GetName())))
                    {
                        WSSqlLogger.Instance.LogInfo(string.Format("Preload: {0}",file));
                        Assembly.LoadFrom(file);
                    }
                }
                catch (Exception ex)
                {
                    WSSqlLogger.Instance.LogError(string.Format("Preload: {0}",ex.Message));
                }
                
            }
        }

        #endregion

    }
}