using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using C4F.DevKit.PreviewHandler.PreviewHandlerFramework;
using System.Diagnostics;
using System.IO;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace C4F.DevKit.PreviewHandler
{
    public class HelperPreviewHandlers
    {

        #region private static fields

        private static string HKLMWinSdk32 = "SOFTWARE\\Microsoft\\Microsoft SDKs\\Windows";
        private static string HKLMWinSdk64 = "SOFTWARE\\Wow6432Node\\Microsoft\\Microsoft SDKs\\Windows";
        private static string InstallationFolder = "InstallationFolder";

        private static Dictionary<string, Type> _handlersDictionary = null;

        #endregion

        public static Dictionary<string, Type> HandlersDictionary
        {
            get
            {
                if (_handlersDictionary == null)
                {
                    InitDictiohary();
                }
                return _handlersDictionary;
            }
        }


        private static void InitDictiohary()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(PreviewHandler.PreviewHandlerFramework.PreviewHandler));
            var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
            _handlersDictionary = new Dictionary<string, Type>();
            foreach (var type in listTypes)
            {
                object[] attr = (object[])type.GetCustomAttributes(typeof(PreviewHandlerAttribute), true);
                if (attr == null || attr.Length == 0)
                    continue;
                PreviewHandlerAttribute prewAttr = attr[0] as PreviewHandlerAttribute;
                string[] exts = prewAttr.Extension.Split(';');
                foreach (var ext in exts)
                {
                    if(!_handlersDictionary.ContainsKey(ext))
                        _handlersDictionary.Add(ext, type);
                }
            }
        }


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

            //var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
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

            //var listTypes = assembly.GetTypes().Where(type => type.IsSealed && (type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.FileBasedPreviewHandler)) || type.IsSubclassOf(typeof(C4F.DevKit.PreviewHandler.PreviewHandlerFramework.StreamBasedPreviewHandler)))).ToList();
            //listTypes.ForEach(type => PreviewHandler.PreviewHandlerFramework.PreviewHandler.Register(type));


            return true;
        }

        #region private

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
