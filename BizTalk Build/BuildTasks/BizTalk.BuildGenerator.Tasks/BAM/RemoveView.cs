
namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Removes a BAM Activity
    /// </summary>
    public class RemoveView : BaseBmTask 
    {
        /// <summary>
        /// The name of the View to remove
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            const string argsFormat = "remove-view -Name:{0}";
            return ExecuteBm(string.Format(argsFormat, ViewName));
        }
    }
}
