using System;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Management;
using Microsoft.BizTalk.ExplorerOM;

namespace BizUnit.TestSteps.BizTalk
{
    public class BizTalkHelper
    {
        [RegistryPermission(SecurityAction.Demand, Read = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration")]
        public static BizTalkServerRegistry GetMgmtServerInfo()
        {
            BizTalkServerRegistry registry2;
            BizTalkServerRegistry registry = new BizTalkServerRegistry();

            string name = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration";
            RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
            if (key == null)
                throw new Exception(string.Format("Registry key '{0}' not found", new object[] { name }));

            registry.BizTalkMgmtDbName = (string)key.GetValue("MgmtDBName");
            registry.BizTalkMgmtDb = (string)key.GetValue("MgmtDBServer");
            key.Close();
            key = null;
            registry2 = registry;

            return registry2;
        }

        public static BizTalkServerMsgBox GetMsgboxServerInfo()
        {
            BizTalkServerMsgBox box = new BizTalkServerMsgBox();
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(@"ROOT\MicrosoftBizTalkServer", "Select * from MSBTS_GroupSetting"))
            {
                foreach (ManagementObject obj2 in searcher.Get())
                {
                    box.BizTalkMsgBoxDbName = (string)obj2["SubscriptionDBName"];
                    box.BizTalkMsgBoxDb = (string)obj2["SubscriptionDBServerName"];
                }
            }
            return box;
        }

        private static string ConnectionString(BizTalkServerRegistry btsServerRegistry)
        {
            string str2;
            str2 = "Data Source=" + btsServerRegistry.BizTalkMgmtDb + ";Initial Catalog=" + btsServerRegistry.BizTalkMgmtDbName + ";Integrated Security=SSPI;Connect Timeout=120";
            return str2;
        }

        public static BtsCatalogExplorer GetBtsCatalogExplorer()
        {
            BtsCatalogExplorer explorer = new BtsCatalogExplorer();
            explorer.ConnectionString = ConnectionString(GetMgmtServerInfo());
            return explorer;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BizTalkServerRegistry
    {
        private string _BizTalkMgmtDbName;
        private string _BizTalkMgmtDb;
        public string BizTalkMgmtDbName
        {
            get
            {
                return this._BizTalkMgmtDbName;
            }
            set
            {
                this._BizTalkMgmtDbName = value;
            }
        }
        public string BizTalkMgmtDb
        {
            get
            {
                return this._BizTalkMgmtDb;
            }
            set
            {
                this._BizTalkMgmtDb = value;
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BizTalkServerMsgBox
    {
        private string _BizTalkMsgBoxDbName;
        private string _BizTalkMsgBoxDb;
        public string BizTalkMsgBoxDbName
        {
            get
            {
                return this._BizTalkMsgBoxDbName;
            }
            set
            {
                this._BizTalkMsgBoxDbName = value;
            }
        }
        public string BizTalkMsgBoxDb
        {
            get
            {
                return this._BizTalkMsgBoxDb;
            }
            set
            {
                this._BizTalkMsgBoxDb = value;
            }
        }
    }
}
