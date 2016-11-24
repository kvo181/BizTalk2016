using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Configuration;
using System.Windows.Forms;


namespace bizilante.Deployment.BTSDeployHost
{
    [RunInstaller(true)]
    public partial class BTSDeployHostInstaller : Installer
    {
        public BTSDeployHostInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);

            try
            {
                // Get the configuration xml file
                string targetDirectory = Context.Parameters["targetdir"];
                string exePath = string.Format("{0}BTSDeployHost.exe", targetDirectory);
                Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);

                // Set the DeploymentDb connection string
                string deploymentDb = Context.Parameters["DeploymentDb"];
                if (deploymentDb == "1")
                    config.ConnectionStrings.ConnectionStrings["LogDeployment.Properties.Settings.DeploymentDb"].ConnectionString =
                        "Data Source=dboprod.biztalk2010.acv-csc.intranet;Initial Catalog=BizTalkDeploymentDb;Integrated Security=True";
                else
                    config.ConnectionStrings.ConnectionStrings["LogDeployment.Properties.Settings.DeploymentDb"].ConnectionString =
                        "Data Source=dboloc.biztalk2010.acv-csc.intranet;Initial Catalog=BizTalkDeploymentDb;Integrated Security=True";

                // Set the PowerShell info
                string psRootPath = Context.Parameters["PSRootPath"];
                string msiRootPath = Context.Parameters["MsiRootPath"];
                string bizTalkToolsFolder = Context.Parameters["BizTalkToolsFolder"];
                string bizTalkLogsFolder = Context.Parameters["BizTalkLogsFolder"];
                string bizTalkDomain = Context.Parameters["BizTalkDomain"];
                string bizTalkTmpInstall = Context.Parameters["BizTalkTmpInstall"];

                config.AppSettings.Settings["PSRootPath"].Value = psRootPath;
                config.AppSettings.Settings["MsiRootPath"].Value = msiRootPath;
                config.AppSettings.Settings["BizTalkToolsFolder"].Value = bizTalkToolsFolder;
                config.AppSettings.Settings["BizTalkLogsFolder"].Value = bizTalkLogsFolder;
                config.AppSettings.Settings["BizTalkDomain"].Value = bizTalkDomain;
                config.AppSettings.Settings["BizTalkTmpInstall"].Value = bizTalkTmpInstall;

                // Set the BizTalk groups
                string biztalk_loc = Context.Parameters["BTSEnv1"];
                string biztalk_dev = Context.Parameters["BTSEnv2"];
                string biztalk_tst = Context.Parameters["BTSEnv3"];
                string biztalk_edu = Context.Parameters["BTSEnv4"];
                string biztalk_hfx = Context.Parameters["BTSEnv5"];
                string biztalk_prd = Context.Parameters["BTSEnv6"];
                if (string.IsNullOrEmpty(biztalk_loc))
                    config.AppSettings.Settings.Remove("BizTalk_LOC");
                else
                    config.AppSettings.Settings["BizTalk_LOC"].Value = biztalk_loc;
                config.AppSettings.Settings["BizTalk_DEV"].Value = biztalk_dev;
                config.AppSettings.Settings["BizTalk_TST"].Value = biztalk_tst;
                config.AppSettings.Settings["BizTalk_EDU"].Value = biztalk_edu;
                config.AppSettings.Settings["BizTalk_HFX"].Value = biztalk_hfx;
                config.AppSettings.Settings["BizTalk_PRD"].Value = biztalk_prd;

                // Save the config
                config.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}
