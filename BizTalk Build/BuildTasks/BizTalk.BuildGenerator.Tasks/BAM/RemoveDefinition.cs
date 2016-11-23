
namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Removes a BAM definition
    /// </summary>
    public class RemoveDefinition : BaseBmTask 
    {
        /// <summary>
        /// The path to the definition file, expected to be xml or xls
        /// </summary>
        public string DefinitionFilePath { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            const string argsFormat = "remove-all -DefinitionFile:{0}";
            return ExecuteBm(string.Format(argsFormat, DefinitionFilePath));            
        }
    }
}
