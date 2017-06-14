using System;
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
        private readonly int MinMemory = 256;
        private readonly int MaxMemory = 758;

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
            OFLogger.Instance.LogInfo($"Parser application: {fullPathOFApplication}");
            if (!File.Exists(fullPathOFApplication))
            {
                OFLogger.Instance.LogError($"Application {fullPathOFApplication} is not exist...");
                return;
            }
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo("java.exe ", $" -jar -Xms{MinMemory}m -Xmx{MaxMemory}m \"{fullPathOFApplication}\"");
                psi.CreateNoWindow = true;
                psi.WindowStyle = ProcessWindowStyle.Hidden;
                //psi.UseShellExecute = false;
                //psi.RedirectStandardError = true;

                _parserProcess = Process.Start(psi);
                _parserProcess.ErrorDataReceived += ParserProcessOnErrorDataReceived;

                Thread.Sleep(1000);

                OFLogger.Instance.LogInfo($"Parser application was started: {fullPathOFApplication}");

            }
            catch (Exception e)
            {
                OFLogger.Instance.LogError(e.ToString());
            }
        }

        private void ParserProcessOnErrorDataReceived(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            OFLogger.Instance.LogError($"Error Received: {dataReceivedEventArgs.Data}");
        }

        public void StopParser()
        {
            if (_parserProcess.IsNotNull())
            {
                
                try
                {
                    OFLogger.Instance.LogInfo($"Stopping email parser process {_parserProcess.ProcessName}");
                    _parserProcess.ErrorDataReceived -= ParserProcessOnErrorDataReceived;
                    _parserProcess.Kill();
                    _parserProcess = null;
                }
                catch (Exception e)
                {
                    OFLogger.Instance.LogError(e.ToString());
                }
            }
        }

    }
}