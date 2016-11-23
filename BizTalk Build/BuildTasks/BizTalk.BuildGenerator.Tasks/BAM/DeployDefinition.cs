using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Deploys a BAM definition
    /// </summary>
    public class DeployDefinition : BaseBmTask 
    {
        /// <summary>
        /// The path to the BAM definition, expected to be xml or xls
        /// </summary>
        [Required]
        public string DefinitionFilePath { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            const string argsFormat = "deploy-all -DefinitionFile:{0}";
            return ExecuteBm(string.Format(argsFormat, DefinitionFilePath));            
        }
    }
}
