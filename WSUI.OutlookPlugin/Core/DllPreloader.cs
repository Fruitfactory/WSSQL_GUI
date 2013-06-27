using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

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
            ThreadPool.QueueUserWorkItem(PreloadDllInThread);
        }

        #endregion

        #region [private]

        private void Init()
        {
            // add part of dll name which should be loaded during loading plugin
            _listPartNameOfDll.Add("MahApps");
            _listPartNameOfDll.Add("Microsoft");
            _listPartNameOfDll.Add("WSUI");
            _listPartNameOfDll.Add("WSPreview");
        }

        private void PreloadDllInThread(object state)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            string currentFolder = AppDomain.CurrentDomain.BaseDirectory;
            var files = Directory.GetFiles(currentFolder, DllExt, SearchOption.TopDirectoryOnly);
            if (files.Length == 0)
                return;
            foreach (var file in files)
            {
                var assembleName = AssemblyName.GetAssemblyName(file);
                if (_listPartNameOfDll.Any(s => assembleName.Name.Contains(s)) 
                    && !assemblies.Any(a => AssemblyName.ReferenceMatchesDefinition(assembleName, a.GetName())))
                {
                    Assembly.LoadFrom(file);
                }
            }
        }

        #endregion

    }
}