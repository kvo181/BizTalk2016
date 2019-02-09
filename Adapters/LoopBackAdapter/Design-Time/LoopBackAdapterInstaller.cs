using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.Management;

namespace bizilante.BizTalk.Adapters.LoopBackDesignTime
{
    [RunInstaller(true)]
    public partial class LoopBackAdapterInstaller : Installer
    {
        public LoopBackAdapterInstaller()
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
                newAdapterObject["Name"] = "LoopBack";
                newAdapterObject["Comment"] = "Bounce a message back to the message box through a solicit-response send port.";
                newAdapterObject["Constraints"] = "9482";  //see the registry file!! 
                newAdapterObject["MgmtCLSID"] = "{D44A2A6E-3B4F-452A-8007-CAB27D2D0B95}"; //see the registry file!!

                //create the Managementobject
                newAdapterObject.Put(options);
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("LoopBackAdapterInstaller", "Unable to register adapter in BizTalk: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
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
                newAdapterObject["Name"] = "LoopBack";
                newAdapterObject["Comment"] = "Bounce a message back to the message box through a solicit-response send port.";
                newAdapterObject["Constraints"] = "9482";  //see the registry file!! 
                newAdapterObject["MgmtCLSID"] = "{D44A2A6E-3B4F-452A-8007-CAB27D2D0B95}"; //see the registry file!!

                //create the Managementobject
                newAdapterObject.Delete();
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("LoopBackAdapterInstaller", "Unable to unregister adapter from BizTalk: " + ex.Message, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}