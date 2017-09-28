using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OF.Core.Core.LimeLM;
using OF.Core.Enums;
using OF.Core.Helpers;
using OF.Core.Win32;

namespace OF.Install
{
    class Program
    {
        private static readonly string JavaHomeVar = "JAVA_HOME";

        private static readonly string ElasticsearchServiceBat = "elasticsearch-service.bat";

        private static readonly string filename = "jvm.options";

        private static readonly double PAGE_VALUE = 4;

        static void Main(string[] args)
        {
            Console.WriteLine("Install settings...");
            try
            {
                AddEnviromentVariable();
                InstallElasticSearch(args[0], args[1]);
                ApplyRules("install", args[0], args[1]);
                RegistrySettings(args[1]);
                DisableOutlookAutoComplete();
                Console.WriteLine("Installing has been done...");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Console.Out.WriteLine(e.ToString());
            }
        }
        
        private static void AddEnviromentVariable()
        {
            string javaHome = OFRegistryHelper.Instance.GetJavaInstallationPath();
            try
            {
                var jvm = System.Environment.GetEnvironmentVariable(JavaHomeVar);
                if (string.IsNullOrEmpty(jvm))
                {
                    System.Environment.SetEnvironmentVariable(JavaHomeVar,javaHome);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private static void RegistrySettings(string ofpath)
        {
            try
            {
                SetRegistrySettings();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Out.WriteLine(ex.ToString());
            }
        }

        private static void SetRegistrySettings()
        {
            var versions = OFRegistryHelper.Instance.GetOutlookVersion();
            DisabeAccessPrompt(versions.Item1);
            FixBeforeItemMoveEvent(versions.Item1);
        }

        private static void DisabeAccessPrompt(string version)
        {
            OFRegistryHelper.Instance.DisableOutlookSecurityWarning(version);
        }

        private static void FixBeforeItemMoveEvent(string version)
        {
            OFRegistryHelper.Instance.FixBeforeItemMoveEvent(version);
        }

        #region [elastc search]

        public static void InstallElasticSearch(string espath, string ofpath)
        {
            try
            {
                string javaHome = OFRegistryHelper.Instance.GetJavaInstallationPath();

                var elasticSearchPath = espath;
                var ofPath = ofpath;
                OFRegistryHelper.Instance.SetMachineOfPath(ofPath);
                if (!String.IsNullOrEmpty(elasticSearchPath))
                {
                    ApplyMemoryForElasticSearch(elasticSearchPath);

                    ProcessStartInfo si = new ProcessStartInfo();
                    si.FileName = Path.Combine(elasticSearchPath, ElasticsearchServiceBat);
                    if (!File.Exists(si.FileName))
                    {
                        Console.Out.WriteLine("File not Exits: " + si.FileName);
                        return;
                    }
                    Console.Out.WriteLine("JAVA_HOME = " + javaHome);
                    Console.Out.WriteLine("ElasticSearch Path = " + elasticSearchPath);
                    si.Arguments = String.Format(" {0} \"{1}\"", "install", javaHome);
                    si.UseShellExecute = false;
                    si.CreateNoWindow = true;
                    si.Verb = "runas";
                    si.WorkingDirectory = String.Format("{0}", elasticSearchPath);
                    Process pInstall = new Process { StartInfo = si };
                    pInstall.Start();


                    pInstall.WaitForExit();

                    Console.Out.WriteLine("Install Elastic Search: install service");

                    si.Arguments = String.Format(" {0} \"{1}\"", "start", javaHome);
                    Process pStart = new Process { StartInfo = si };
                    pStart.Start();
                    pStart.WaitForExit();
                    Console.Out.WriteLine("Install Elastis Search: run service");
                }
                else
                {
                    Console.Out.WriteLine("ESPATH is empty.");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
                Console.Out.WriteLine("Install Elastis Search: " + exception.Message + "  => path : " + espath);
            }
        }
        
        #endregion

        #region [firewall]
        
        public static void InstallRules(string espath, string ofpath)
        {
            try
            {
                ApplyRules("install",espath,ofpath);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Console.Out.WriteLine(ex.ToString());
            }
        }

        private static void ApplyRules(string action,string espath,string ofpath)
        {
            var es86 = Path.Combine(espath,"elasticsearch-service-x86.exe");
            var es64 = Path.Combine(espath, "elasticsearch-service-x64.exe");
            var serviceApp = Path.Combine(ofpath, "serviceapp.exe");
            Console.Out.WriteLine(es86);
            Console.Out.WriteLine(es64);
            Console.Out.WriteLine(serviceApp);
            ProcessStartInfo si = new ProcessStartInfo(Path.Combine(ofpath, "firewallrules.exe"))
            {
                Arguments = String.Format(" {0} \"{1}\" \"{2}\" \"{3}\"", action, es86, es64, serviceApp),
                Verb = "runas",
                UseShellExecute = true,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            var process = Process.Start(si);
            process.WaitForExit();
        }
        
        private static void ApplyMemoryForElasticSearch(string espath)
        {
            try
            {
                string path = espath;
                var startIndex = path.IndexOf(@"\bin");
                path = path.Remove(startIndex, @"\bin".Length);


                var memoryInMb = WindowsFunction.GetAvailableMemory();

                var strings = System.IO.File.ReadAllLines(Path.Combine(path, "config\\") + filename);

                Regex regs = new Regex("-Xms.*");
                Regex regx = new Regex("-Xmx.*");
                var pageValue = (int)(memoryInMb / PAGE_VALUE);
                for (int i = 0; i < strings.Length; i++)
                {
                    if (!strings[i].StartsWith("##") && regs.IsMatch(strings[i]))
                    {
                        strings[i] = $"-Xms{pageValue}m";
                    }
                    if (!strings[i].StartsWith("##") && regx.IsMatch(strings[i]))
                    {
                        strings[i] = $"-Xmx{pageValue}m";
                    }
                }

                using (var writer = new StreamWriter(Path.Combine(path, "config\\") + filename))
                {
                    foreach (var s in strings)
                    {
                        writer.WriteLine(s);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        #endregion

        #region [disable outlook auto-complete]

        private static void DisableOutlookAutoComplete()
        {
            try
            {
                var officeVersion = OFRegistryHelper.Instance.GetOutlookVersion().Item1;
                OFRegistryHelper.Instance.DisableOutlookAutoCompleateEmailsToCcBcc(officeVersion);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        #endregion

    }
}
