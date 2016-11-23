using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Task to import a vocabulary
    /// </summary>
    public class ImportVocabulary : BaseBRETask
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
            CreateManager().Import(FilePath);            
            return true;
        }
    }
}
