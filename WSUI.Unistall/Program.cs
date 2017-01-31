using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;
using OF.Core.Core.LimeLM;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Interfaces;

namespace OF.Unistall
{
    internal class Program
    {
        private const string ParamName = "uninstall";
        private const string PstPluginName = "pstriver";
        private const string UnistallArguments = " --remove {0} \"{1}\"";
        private static StreamWriter logWriter;

        private static void Main(string[] args)
        {

            if (args == null || !args.Any())
                return;

            string param = args[0];
            if (param != ParamName)
                return;
            IOFTurboLimeActivate turboLimeActivate = null;
            try
            {
                turboLimeActivate = new TurboLimeActivate();
                turboLimeActivate.Init();
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            //using (logWriter = File.CreateText("C:\\of_install.log"))
            {
                try
                {
                     
                    turboLimeActivate.Deactivate(true);
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
                try
                {
                    StopServiceAndApplication();
                    UnInstallElasticSearch();
                    ApplyRules(ParamName, args[1], args[2]);

                    DeleteRegistrySettings();

                    Log("Done...");
                }
                catch (Exception ex)
                {
                    Log(ex.ToString());
                }
            }
        }

        private static void Log(string message)
        {
            if (logWriter == null)
            {
                return;
            }
            logWriter.WriteLine(message);
        }

        private static void DeleteRegistrySettings()
        {
            var versions = OFRegistryHelper.Instance.GetOutlookVersion();
            OFRegistryHelper.Instance.DeleteOutlookSecuritySettings(versions.Item1);
        }

        public static void StopServiceAndApplication()
        {
            StopESService();
            StopServiceApp();
        }

        private static void StopESService()
        {
            try
            {
                ServiceController sct =
                    ServiceController.GetServices()
                        .FirstOrDefault(
                            s => s.ServiceName.IndexOf("elasticsearch", StringComparison.InvariantCultureIgnoreCase) > -1);
                if (sct != null && sct.Status == ServiceControllerStatus.Running)
                {
                    KillTask(sct.ServiceName + ".exe", "elasticsearch");
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        public static void StopServiceApp()
        {
            try
            {
                Process process =
                    Process.GetProcesses().FirstOrDefault(p => p.ProcessName.ToUpperInvariant().Contains("SERVICEAPP"));
                if (process != null)
                {
                    Log("Stopping Service Application....");
                    OFRegistryHelper.Instance.DeleteAutoRunHelperApplication();
                    KillTask("serviceapp.exe", "serviceapp");
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static bool KillTask(string taskName, string processName)
        {
            var result = true;
            try
            {
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = Path.Combine(Environment.SystemDirectory, "taskkill.exe");
                Log("!!!! Kill task ");
                info.Verb = "runas";
                info.Arguments = string.Format(" /F /IM {0}", taskName);
                info.WindowStyle = ProcessWindowStyle.Hidden;
                Log(string.Format("!!!! {0}", info.Arguments));
                Process p = new Process() { StartInfo = info };
                p.Start();
                p.WaitForExit();
                while (true)
                {
                    Process elasticProcess =
                        Process.GetProcesses()
                            .FirstOrDefault(pp => pp.ProcessName.ToUpper().IndexOf(processName.ToUpper()) > -1);

                    if (elasticProcess == null)
                    {
                        break;
                    }
                    Log(string.Format("!!!! Waiting for {0}", elasticProcess.ProcessName));
                    Thread.Sleep(100);
                }
                return result;
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
                result = false;
            }

            return result;
        }


        public static void UnInstallElasticSearch()
        {
            try
            {
                string javaHome = OFRegistryHelper.Instance.GetJavaInstallationPath();
                var elasticSearchPath = OFRegistryHelper.Instance.GetElasticSearchpath();
                if (!string.IsNullOrEmpty(elasticSearchPath))
                {
                    UnregisterPlugin(elasticSearchPath, javaHome);

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(elasticSearchPath, "service.bat");
                    si.UseShellExecute = false;
                    si.Verb = "runas";
                    si.CreateNoWindow = true;
                    si.WorkingDirectory = elasticSearchPath;

                    si.Arguments = string.Format(" {0} \"{1}\"", "remove", javaHome);
                    Process processRemove = new Process {StartInfo = si};
                    processRemove.Start();
                    processRemove.WaitForExit();
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
            finally
            {
            }
        }

        private static void UnregisterPlugin(string elasticSearchPath, string javaHome)
        {
            try
            {
                ProcessStartInfo si = new ProcessStartInfo();
                si.FileName = Path.Combine(elasticSearchPath, "removeplugin.bat");
                si.Arguments = string.Format(UnistallArguments, PstPluginName, javaHome);
                si.UseShellExecute = false;
                si.Verb = "runas";
                si.CreateNoWindow = true;
                si.WorkingDirectory = elasticSearchPath;
                Process pInstall = new Process();
                pInstall.StartInfo = si;
                pInstall.Start();
                pInstall.WaitForExit();
                Log("PST plugin was unistalled.");
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        private static void ApplyRules(string action,string esBinFolder, string installFolder)
        {
            var es86 = Path.Combine(esBinFolder, "elasticsearch-service-x86.exe");
            var es64 = Path.Combine(esBinFolder, "elasticsearch-service-x64.exe");
            var serviceApp = Path.Combine(installFolder, "serviceapp.exe");
            ProcessStartInfo si = new ProcessStartInfo(Path.Combine(installFolder, "firewallrules.exe"))
            {
                Arguments = string.Format(" {0} \"{1}\" \"{2}\" \"{3}\"", action, es86, es64, serviceApp),
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var process = Process.Start(si);
            process.WaitForExit();
        }
    }
}