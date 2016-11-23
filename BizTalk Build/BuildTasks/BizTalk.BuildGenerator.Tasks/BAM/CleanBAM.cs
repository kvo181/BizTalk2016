using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// This will back up all BAM definitions to a temporary location and then use the definition temp file to remove all artefacts from BAM
    /// The bam instance can be restored from this temp location
    /// </summary>
    public class CleanBam : BaseBmTask
    {
        /// <summary>
        /// The path to the temp definition file
        /// </summary>
        [Required]
        public string TempDefinitionFilePath { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            const string backUpArgsFormat = "get-defxml -FileName:{0}";
            ExecuteBm(string.Format(backUpArgsFormat, TempDefinitionFilePath));

            Logger.LogMessage(this, "All current definitions have been backed up to the following location: " + TempDefinitionFilePath);

            const string removeArgsFormat = "remove-all -DefinitionFile:{0}";
            return ExecuteBm(string.Format(removeArgsFormat, TempDefinitionFilePath));            
        }
    }
}
