using Microsoft.Build.Framework;
using System.IO;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Backs up the definition file so if something has been incorrectly configured then
    /// the original is still there to fix it with
    /// </summary>
    public class BackupDefinitionFile : BaseBmTask
    {
        /// <summary>
        /// The path to the definition file
        /// </summary>
        [Required]
        public string DefinitionFilePath { get; set; }

        /// <summary>
        /// The path to the file where this will be placed
        /// </summary>
        [Required]
        public string OutputFilePath { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            File.Copy(DefinitionFilePath, OutputFilePath, true);
            return true;
        }
    }
}
