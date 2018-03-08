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
            MessageBox.Show("Init custom install");
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            MessageBox.Show("Start custom install");
            try
            {
                base.Install(stateSaver);

                // Get the configuration xml file
                string targetDirectory = Context.Parameters["targetdir"];
                string exePath = string.Format("{0}BTSDeployHost.exe", targetDirectory);
                Configuration config = ConfigurationManager.OpenExeConfiguration(exePath);

                // Set the DeploymentDb connection string
                string deploymentDb = Context.Parameters["DeploymentDb"];
                string deploymentDbName = Context.Parameters["DeploymentDbName"];
                if (string.IsNullOrWhiteSpace(deploymentDbName))
                    deploymentDbName = "BizTalkDeploymentDb";
                if (string.IsNullOrWhiteSpace(deploymentDb))
                {
                    config.ConnectionStrings.ConnectionStrings["LogDeployment.Properties.Settings.DeploymentDb"].ConnectionString =
                        string.Format("Data Source=.;Initial Catalog={0};Integrated Security=True", deploymentDbName);
                }
                else
                {
                    config.ConnectionStrings.ConnectionStrings["LogDeployment.Properties.Settings.DeploymentDb"].ConnectionString =
                        string.Format("Data Source={0};Initial Catalog={1};Integrated Security=True", deploymentDb, deploymentDbName);
                }

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
                string biztalk_env1 = Context.Parameters["BTSEnv1"];
                string biztalk_env2 = Context.Parameters["BTSEnv2"];
                string biztalk_env3 = Context.Parameters["BTSEnv3"];
                string biztalk_env4 = Context.Parameters["BTSEnv4"];
                string biztalk_env5 = Context.Parameters["BTSEnv5"];
                string biztalk_env6 = Context.Parameters["BTSEnv6"];
                config.AppSettings.Settings.Remove("BizTalk_LOC");
                if (string.IsNullOrEmpty(biztalk_env1))
                    config.AppSettings.Settings.Remove("BizTalk_DEV");
                else
                    config.AppSettings.Settings["BizTalk_DEV"].Value = biztalk_env1;
                config.AppSettings.Settings.Remove("BizTalk_TST");
                config.AppSettings.Settings.Remove("BizTalk_EDU");
                config.AppSettings.Settings.Remove("BizTalk_HFX");
                if (string.IsNullOrEmpty(biztalk_env2))
                    config.AppSettings.Settings.Remove("BizTalk_PRD");
                else
                    config.AppSettings.Settings["BizTalk_PRD"].Value = biztalk_env2;

                // Save the config
                config.Save();
            }
            catch (Exception ex)
            {
                if (null != ex.InnerException)
                    MessageBox.Show(ex.InnerException.Message);
                else
                    MessageBox.Show(ex.Message);

            }

        }
    }
}
