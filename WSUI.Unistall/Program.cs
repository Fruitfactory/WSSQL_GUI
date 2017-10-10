using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
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
                    DeleteRegistrySettings();
                    StopServiceAndApplication();
                    UnInstallElasticSearch();
                    ApplyRules(ParamName, args[1], args[2]);
                    EnableOutlooAutoComplete();

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
            StopEmailParser();
        }

        private static void StopESService()
        {
            try
            {
                var sct = GetElasticSearchService();
                if (sct != null && sct.Status == ServiceControllerStatus.Running)
                {
                    KillTaskByTaskName(sct.ServiceName + ".exe", "elasticsearch");
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static ServiceController GetElasticSearchService()
        {
            ServiceController sct =
                ServiceController.GetServices()
                    .FirstOrDefault(
                        s => s.ServiceName.IndexOf("elasticsearch", StringComparison.InvariantCultureIgnoreCase) > -1);
            return sct;
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
                    KillTaskByTaskName("serviceapp.exe", "serviceapp");
                }
            }
            catch (Exception ex)
            {
                Log(ex.ToString());
            }
        }

        private static void StopEmailParser()
        {
            try
            {
                var searcher = new ManagementObjectSearcher("Select * From Win32_Process");
                var processList = searcher.Get();

                foreach (var process in processList)
                {
                    if (!process["Name"].ToString().ToUpperInvariant().Contains("JAVA"))
                        continue;

                    var cmd = process["CommandLine"].ToString().ToLowerInvariant();
                    if (string.IsNullOrEmpty(cmd))
                        continue;
                    if (cmd.Contains("emailparser"))
                    {
                        var processId = process["ProcessId"];
                        KillTaskByProcessId("/pid", processId.ToString());
                    }

                }
            }
            catch (Exception e)
            {

                Log(e.ToString());
            }

        }

        private static bool KillTaskByProcessId(string processId, string processName)
        {
            var result = true;
            try
            {
                KillProcess("/pid", processId);
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

        private static bool KillTaskByTaskName(string taskName, string processName)
        {
            var result = true;
            try
            {
                KillProcess("/IM", taskName);
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

        private static void KillProcess(string argument, string taskName)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = Path.Combine(Environment.SystemDirectory, "taskkill.exe");
            Log("!!!! Kill task ");
            info.Verb = "runas";
            info.Arguments = string.Format(" /F {0} {1}", argument, taskName);
            info.WindowStyle = ProcessWindowStyle.Hidden;
            Log(string.Format("!!!! {0}", info.Arguments));
            Process p = new Process() { StartInfo = info };
            p.Start();
            p.WaitForExit();
        }


        public static void UnInstallElasticSearch()
        {
            try
            {

                string javaHome = OFRegistryHelper.Instance.GetJavaInstallationPath();
                var elasticSearchPath = OFRegistryHelper.Instance.GetElasticSearchpath();
                if (!string.IsNullOrEmpty(elasticSearchPath))
                {

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(elasticSearchPath, "elasticsearch-service.bat");
                    si.UseShellExecute = false;
                    si.Verb = "runas";
                    si.CreateNoWindow = true;
                    si.WorkingDirectory = elasticSearchPath;

                    si.Arguments = string.Format(" {0} \"{1}\"", "remove", javaHome);
                    Process processRemove = new Process { StartInfo = si };
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

            try
            {
                var serviceESController = GetElasticSearchService();
                if (serviceESController != null)
                {
                    var processInfo = new ProcessStartInfo();
                    processInfo.FileName = Path.Combine(Environment.SystemDirectory, "sc.exe");
                    processInfo.Arguments = string.Format(" delete {0}", serviceESController.ServiceName);
                    var process = new Process() { StartInfo = processInfo };
                    process.Start();
                    process.WaitForExit();
                }
            }
            catch (Exception e)
            {
                Log(e.ToString());
            }

        }

        private static void ApplyRules(string action, string esBinFolder, string installFolder)
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

        private static void EnableOutlooAutoComplete()
        {
            var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
            OFRegistryHelper.Instance.EnableOutlookAutoCompleateEmailsToCcBcc(officeVersion);
        }
    }
}