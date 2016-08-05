using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NetFwTypeLib;
using OF.Core.Logger;
using OF.FirewallRules.Firewall;

namespace OF.FirewallRules
{
    class Program
    {

        private static string FIREWALL_PREFIX = "Firewall Rules: {0}";

        private static string OUTLOOKFINDER_FIREWALL_RULE = "OutlookfinderFIrewallRule {0}"; 
            
        static void Main(string[] args)
        {
            System.Diagnostics.Debugger.Launch();
            if (args.Length == 0)
            {
                OFEventLogger.Instance.LogInfo(GetMessage("The argument list is empty."));
                return;
            }
            switch (args[0])
            {
                case "install":
                    InstallRules(args.Skip(1).ToArray());
                    break;
                case "uninstall":
                    UnistallRules(args.Skip(1).ToArray());
                    break;
            }
        }

        private static void UnistallRules(string[] toArray)
        {
            try
            {
                //OFEventLogger.Instance.LogInfo(GetMessage("Uninstall rules..."));
                if (toArray != null && toArray.Any())
                {
                    foreach (var path in toArray)
                    {
                        OFFirewallSettings.RemoveAuthorizeApplication(path);
                    }
                }
                OFFirewallSettings.RemoveOpenPort(9200);
                OFFirewallSettings.RemoveOpenPort(11223);
            }
            catch (Exception ex)
            {
                //OFEventLogger.Instance.LogError(GetMessage(ex.ToString()));
            }
        }

        static void InstallRules(string[] paths)
        {
            try
            {
                //OFEventLogger.Instance.LogInfo(GetMessage("Install rules..."));
                if (paths != null && paths.Any())
                {
                    var index = 1;
                    foreach (var path in paths)
                    {
                        OFFirewallSettings.AuthorizeApplication(string.Format(OUTLOOKFINDER_FIREWALL_RULE, index++), path,
                            NET_FW_SCOPE_.NET_FW_SCOPE_ALL, NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY);
                    }
                }
                OFFirewallSettings.AddOpenPort("ELASTICSEARCH_PORT", 9200);
                OFFirewallSettings.AddOpenPort("OUTLOOKFINDER_SERVICEAPP_PORT", 11223);
            }
            catch (Exception ex)
            {
                //OFEventLogger.Instance.LogError(GetMessage(ex.ToString()));
            }
        }




        static string GetMessage(string message)
        {
            return string.Format(FIREWALL_PREFIX, message);
        }
    }
}
