using Microsoft.Build.Utilities;

namespace bizilante.BuildGenerator.Policies.Tasks
{
    public class RemovePolicy : Task
    {
        private string databaseName = string.Empty;
        private string policyName;
        private string serverName = string.Empty;

        public override bool Execute()
        {
            RuleLoader.UnDeployRuleSet(this.policyName, this.serverName, this.databaseName, base.Log);
            base.Log.LogMessage("", new object[] { });
            return true;
        }

        public string DatabaseName
        {
            get
            {
                return this.databaseName;
            }
            set
            {
                this.databaseName = value;
            }
        }

        public string PolicyName
        {
            get
            {
                return this.policyName;
            }
            set
            {
                this.policyName = value;
            }
        }

        public string ServerName
        {
            get
            {
                return this.serverName;
            }
            set
            {
                this.serverName = value;
            }
        }
    }
}

