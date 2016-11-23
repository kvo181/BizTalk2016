using Microsoft.Build.Framework;
using System.IO;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Backs up all definitions
    /// </summary>
    public class BackupAllDefinitions : BaseBmTask 
    {
        public BackupAllDefinitions()
        {
            OverwriteExistingFile = true;
        }

        /// <summary>
        /// Default true, but determines if an existing file will be replaced
        /// </summary>
        public bool OverwriteExistingFile { get; set; }

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
            if (OverwriteExistingFile && File.Exists(OutputFilePath))
                File.Delete(OutputFilePath);

            const string argsFormat = "get-defxml -FileName:{0}";
            return ExecuteBm(string.Format(argsFormat, OutputFilePath));            
        }
    }
}
