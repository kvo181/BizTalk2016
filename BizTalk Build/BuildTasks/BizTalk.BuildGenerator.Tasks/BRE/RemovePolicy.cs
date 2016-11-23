using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Task to remove a policy, if the version is supplied then it will remove the specific version
    /// if not then all versions will be removed
    /// </summary>
    public class RemovePolicy : BaseBRETask
    {
        /// <summary>
        /// The name of the policy
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
            if (MajorVersion >= 0 && MinorVersion >= 0)
            {
                Log.LogMessage("Remove specific policy", null);
                mgr.Remove(RulesEngineObjectType.RuleSetInfo, PolicyName, MajorVersion, MinorVersion);
            }
            else
            {
                Log.LogMessage("Remove all versions of policy", null);
                mgr.Remove(RulesEngineObjectType.RuleSetInfo, PolicyName);
            }
            return true;
        }
    }
}
