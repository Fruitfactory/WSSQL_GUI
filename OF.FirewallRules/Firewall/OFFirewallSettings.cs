using System;
using NetFwTypeLib;


namespace OF.FirewallRules.Firewall
{
    public class OFFirewallSettings
    {
        private const string CLSID_FIREWALL_MANAGER =
             "{304CE942-6E39-40D8-943A-B913C40C9CD4}";

        private static INetFwMgr GetFirewallManager()
        {
            var objectType = Type.GetTypeFromCLSID(
                new Guid(CLSID_FIREWALL_MANAGER));
            return Activator.CreateInstance(objectType)
                as INetFwMgr;
        }

        #region [add remove trusted application]

        private const string PROGID_AUTHORIZED_APPLICATION =
            "HNetCfg.FwAuthorizedApplication";

        public static bool AuthorizeApplication(string title, string applicationPath,
            NET_FW_SCOPE_ scope, NET_FW_IP_VERSION_ ipVersion)
        {
            // Create the type from prog id
            var type = Type.GetTypeFromProgID(PROGID_AUTHORIZED_APPLICATION);
            var auth = Activator.CreateInstance(type)
                as INetFwAuthorizedApplication;
            auth.Name = title;
            auth.ProcessImageFileName = applicationPath;
            auth.Scope = scope;
            auth.IpVersion = ipVersion;
            auth.Enabled = true;


            var manager = GetFirewallManager();
            try
            {
                manager.LocalPolicy.CurrentProfile.AuthorizedApplications.Add(auth);
            }
            catch (Exception ex)
            {
                
                return false;
            }
            return true;
        }

        public static bool RemoveAuthorizeApplication(string filepath)
        {
            var manager = GetFirewallManager();
            try
            {
                manager.LocalPolicy.CurrentProfile.AuthorizedApplications.Remove(filepath);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion

        #region [add remove ports]

        private const string PROGID_OPEN_PORT =
            "HNetCfg.FwOpenPort";

        public static bool AddOpenPort(string name, int port)
        {

            Type openPortType = Type.GetTypeFromProgID(PROGID_OPEN_PORT);
            var openPort = Activator.CreateInstance(openPortType) as INetFwOpenPort;

            openPort.Enabled = true;
            openPort.IpVersion = NET_FW_IP_VERSION_.NET_FW_IP_VERSION_ANY;
            openPort.Protocol = NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP;
            openPort.Scope = NET_FW_SCOPE_.NET_FW_SCOPE_ALL;
            openPort.Port = port;
            openPort.Name = name;
            var manager = GetFirewallManager();
            try
            {
                manager.LocalPolicy.CurrentProfile.GloballyOpenPorts.Add(openPort);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public static bool RemoveOpenPort(int port)
        {

            var manager = GetFirewallManager();
            try
            {
                manager.LocalPolicy.CurrentProfile.GloballyOpenPorts.Remove(port, NET_FW_IP_PROTOCOL_.NET_FW_IP_PROTOCOL_TCP);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion 
    }
}