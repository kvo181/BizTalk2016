using System;
using System.ComponentModel;
using System.Management;

namespace Winterdom.BizTalk.Adapters
{
    [RunInstaller(true)]
    public partial class NullAdapterInstaller : System.Configuration.Install.Installer
    {
        public NullAdapterInstaller()
        {
            InitializeComponent();
        }
        public override void Install(System.Collections.IDictionary stateSaver)
        {
            base.Install(stateSaver);
        }

        public override void Uninstall(System.Collections.IDictionary savedState)
        {
            base.Uninstall(savedState);
            removeAdapterFromBizTalk();
        }

        public override void Commit(System.Collections.IDictionary savedState)
        {
            base.Commit(savedState);
            registerAdapterInBizTalk();
        }

        public override void Rollback(System.Collections.IDictionary savedState)
        {
            base.Rollback(savedState);
            removeAdapterFromBizTalk();
        }

        private void registerAdapterInBizTalk()
        {
            try
            {
                PutOptions options = new PutOptions();
                options.Type = PutType.CreateOnly;

                //create a ManagementClass object and spawn a ManagementObject instance
                ManagementClass newAdapterClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_AdapterSetting", null);
                ManagementObject newAdapterObject = newAdapterClass.CreateInstance();

                //set the properties for the Managementobject
                newAdapterObject["Name"] = "NULL";
                newAdapterObject["Comment"] = "Discard BizTalk messages.";
                newAdapterObject["Constraints"] = "8210";  //see the registry file!! 
                newAdapterObject["MgmtCLSID"] = "{C98D3C74-F722-4F50-8AFC-4C2A9CB1D961}"; //see the registry file!!

                //create the Managementobject
                newAdapterObject.Put(options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("NullAdapterInstaller", "Unable to register adapter in BizTalk: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        private void removeAdapterFromBizTalk()
        {
            try
            {
                //create a ManagementClass object and spawn a ManagementObject instance
                ManagementClass newAdapterClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_AdapterSetting", null);
                ManagementObject newAdapterObject = newAdapterClass.CreateInstance();

                //set the properties for the Managementobject
                newAdapterObject["Name"] = "NULL";
                newAdapterObject["Comment"] = "Discard BizTalk messages.";
                newAdapterObject["Constraints"] = "8210";  //see the registry file!! 
                newAdapterObject["MgmtCLSID"] = "{C98D3C74-F722-4F50-8AFC-4C2A9CB1D961}"; //see the registry file!!

                //create the Managementobject
                newAdapterObject.Delete();
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("NullAdapterInstaller", "Unable to unregister adapter from BizTalk: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
