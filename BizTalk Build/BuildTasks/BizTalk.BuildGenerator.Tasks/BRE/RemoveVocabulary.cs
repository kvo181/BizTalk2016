using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Task to remove a vocabulary, if the version is supplied then it will remove the specific version
    /// if not then all versions will be removed
    /// </summary>
    public class RemoveVocabulary : BaseBRETask
    {
        /// <summary>
        /// The name of the policy
        /// </summary>
        [Required]
        public string VocabularyName { get; set; }

        /// <summary>
        /// Task execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var mgr = CreateManager();
            if (MajorVersion >= 0 && MinorVersion >= 0)
            {
                Log.LogMessage("Remove specific vocabulary", null);
                mgr.Remove(RulesEngineObjectType.VocabularyInfo, VocabularyName, MajorVersion, MinorVersion);
            }
            else
            {
                Log.LogMessage("Remove all versions of vocabulary", null);
                mgr.Remove(RulesEngineObjectType.VocabularyInfo, VocabularyName);
            }
            return true;
        }
    }
}
