using Microsoft.Win32;
using System.Globalization;
using System.Security.Permissions;

namespace bizilante.SSO.Helper
{
    class BizTalkServerRegistry
    {
        public string BizTalkMgmtDbName { get; set; }
        public string BizTalkMgmtDb { get; set; }
        public string InstallPath { get; set; }
    }
    class BizTalkHelper
    {
        [RegistryPermission(SecurityAction.Demand, Read = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration")]
        public static BizTalkServerRegistry GetMgmtServerInfo()
        {
            BizTalkServerRegistry registry = new BizTalkServerRegistry();
            try
            {
                string name = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                if (key == null)
                    throw new BizTalkRegistryMissingException(string.Format(CultureInfo.CurrentCulture, "Could not locate the BizTalk Registry key. The service must be deployed to a BizTalk Server.  Reg Key Lookup '{0}'.", new object[] { name }));
                registry.InstallPath = (string)key.GetValue("InstallPath");

                RegistryKey subKey = key.OpenSubKey("Administration");
                if (subKey == null)
                    throw new BizTalkRegistryMissingException(string.Format(CultureInfo.CurrentCulture, "Could not locate the BizTalk Registry key. The service must be deployed to a BizTalk Server.  Reg Key Lookup '{0}'.", new object[] { name + "\\Administration" }));
                registry.BizTalkMgmtDbName = (string)subKey.GetValue("MgmtDBName");
                registry.BizTalkMgmtDb = (string)subKey.GetValue("MgmtDBServer");

                key.Close();
                key = null;
            }
            catch { }

            return registry;
        }
    }
}
