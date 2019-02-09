using bizilante.ManagementConsole.SSO.Properties;
using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;
using System;
using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

namespace bizilante.ManagementConsole.SSO
{
    [SnapInSettings("{4F6C666A-99F6-4159-AAB2-E39D27391085}"
        , DisplayName = "SSO Application Configuration"
        , Description = "Allows a BizTalk Administrator to Configure the SSO Configuration Store"
        , Vendor = "bizilante"
        , ConfigurationFile = "bizilante.SSOMMCSnapIn.dll.config")]
    public class ActionsSnapIn : SnapIn
    {
        private static bool _hasSecurityRights;

        public static bool HasSecurityRights
        {
            get
            {
                return _hasSecurityRights;
            }
            set
            {
                _hasSecurityRights = value;
            }
        }

        public ActionsSnapIn()
        {
            //System.Diagnostics.Debugger.Launch();
            ApplicationScopeNode applicationScopeNode = new ApplicationScopeNode(true);
            applicationScopeNode.ApplicationImported += new EventHandler(rootNode_ApplicationImported);
            RootNode = applicationScopeNode;
            if (HasSecurityRights)
            {
                InitializeRootApplications();
            }
            SmallImages.Add(Resources.authority_16);
            SmallImages.Add(Resources.add_scope);
        }

        private void InitializeRootApplications()
        {
            RootNode.Children.Clear();
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            string[] applications = sSO.GetApplications();
            for (int i = 0; i < applications.Length; i++)
            {
                ApplicationScopeNode applicationScopeNode = new ApplicationScopeNode(applications[i]);
                applicationScopeNode.ApplicationImported += new EventHandler(this.rootNode_ApplicationImported);
                base.RootNode.Children.Add(applicationScopeNode);
            }
        }

        protected override bool OnShowInitializationWizard()
        {
            bizilante.SSO.Helper.SSO sSO = new bizilante.SSO.Helper.SSO();
            sSO.GetSecretServerName();
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool flag = windowsPrincipal.IsInRole(sSO.SSOAdminGroup);
            bool flag2 = windowsPrincipal.IsInRole(sSO.AffiliateAppMgrGroup);
            return flag || flag2;
        }

        protected override void OnInitialize()
        {
            if (!HasSecurityRights)
            {
                EventLog.WriteEntry("SSO MMC Snap-In", "User does not have proper rights.  SSO Administrators required.", EventLogEntryType.Error);
                MessageBoxParameters messageBoxParameters = new MessageBoxParameters();
                messageBoxParameters.Buttons = MessageBoxButtons.OK;
                messageBoxParameters.Caption = "Access Denied";
                messageBoxParameters.Text = "User is not an SSO Administrator";
                messageBoxParameters.Icon = MessageBoxIcon.Hand;
                Console.ShowDialog(messageBoxParameters);
            }
            base.OnInitialize();
        }

        private void rootNode_ApplicationImported(object sender, EventArgs e)
        {
            InitializeRootApplications();
        }
    }
}
