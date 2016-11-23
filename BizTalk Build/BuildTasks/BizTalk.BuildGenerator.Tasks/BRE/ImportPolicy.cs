using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Task to import a policy
    /// </summary>
    public class ImportPolicy : BaseBRETask
    {
        /// <summary>
        /// The filepath to the vocabulary
        /// </summary>
        [Required]
        public string FilePath { get; set; }

        /// <summary>
        /// Task execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var mgr = CreateManager();
            mgr.Import(FilePath);
            return true;
        }
    }
}
