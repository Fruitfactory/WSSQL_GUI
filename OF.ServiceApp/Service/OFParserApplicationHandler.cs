﻿using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using OF.Core.Extensions;
using OF.Core.Logger;

namespace OF.ServiceApp.Service
{
    public class OFParserApplicationHandler
    {

        private readonly string ParserApplicationFileName = "emailparser-1.0-SNAPSHOT.jar";
        private readonly string ParserApplicationPathTemplate = "{0}\\java\\{1}";
        private readonly int MinMemory = 125;
        private readonly int MaxMemory = 256;

        private Process _parserProcess;
        

        private static Lazy<OFParserApplicationHandler> _instance = new Lazy<OFParserApplicationHandler>(() => new OFParserApplicationHandler());

        public static OFParserApplicationHandler Instance => _instance.Value;

        private string CurrentPath
        {
            get
            {
                string codeBase = typeof(OFParserApplicationHandler).Assembly.CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }

        public void StartParser()
        {
            var currentpath = CurrentPath;
            var fullPathOFApplication = $"{currentpath}\\java\\{ParserApplicationFileName}";
            if (!File.Exists(fullPathOFApplication))
            {
                OFLogger.Instance.LogWarning($"Parser application is not exist...");
                return;
            }
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("java.exe ", $" -jar -Xms{MinMemory}m -Xmx{MaxMemory}m {fullPathOFApplication}");
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;

                _parserProcess = Process.Start(psi);

                Thread.Sleep(1000);
            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        public void StopParser()
        {
            if (_parserProcess.IsNotNull())
            {
                try
                {
                    _parserProcess.Kill();
                }
                catch (Exception e)
                {
                    OFLogger.Instance.LogError(e.ToString());
                }
            }
        }

    }
}