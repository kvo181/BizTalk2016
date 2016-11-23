using Microsoft.Build.Utilities;
using Microsoft.RuleEngine;

namespace bizilante.BuildGenerator.Policies.Tasks
{
    public class DeployPolicy : Task
    {
        private string databaseName = string.Empty;
        private bool deploy;
        private string policyFileName;
        private string policyName;
        private string serverName = string.Empty;

        public override bool Execute()
        {
            bool flag;
            try
            {
                RuleSet ruleSet = RuleLoader.LoadRuleFromFile(this.PolicyFileName, this.PolicyName);
                if ((this.databaseName != string.Empty) && (this.serverName != string.Empty))
                {
                    RuleLoader.DeployRuleSet(ruleSet, this.serverName, this.databaseName, base.Log, this.deploy);
                }
                else
                {
                    RuleLoader.DeployRuleSet(ruleSet, string.Empty, string.Empty, base.Log, this.deploy);
                }
                flag = true;
            }
            catch
            {
                throw;
            }
            base.Log.LogMessage("", new object[] { });
            return flag;
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

        public bool Deploy
        {
            get
            {
                return this.deploy;
            }
            set
            {
                this.deploy = value;
            }
        }

        public string PolicyFileName
        {
            get
            {
                return this.policyFileName;
            }
            set
            {
                this.policyFileName = value;
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

