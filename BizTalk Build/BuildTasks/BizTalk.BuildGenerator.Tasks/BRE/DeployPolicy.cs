using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Task to deploy a policy, if version details are supplied then a specific version is deployed, it just the 
    /// policy name is supplied then all versions will be deployed
    /// </summary>
    public class DeployPolicy : BaseBRETask
    {
        /// <summary>
        /// The PolicyName
        /// </summary>
        [Required]
        public string PolicyName { get; set; }

        /// <summary>
        /// Task execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var mgr = CreateManager();
            if (IsSpecificVersion)
                mgr.DeployRuleSet(PolicyName, MajorVersion, MinorVersion);
            else
                mgr.DeployRuleSet(PolicyName);
            
            return true;
        }
    }
}
