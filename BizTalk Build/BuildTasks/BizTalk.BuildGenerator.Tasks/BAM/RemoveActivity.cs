
namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Removes a BAM Activity
    /// </summary>
    public class RemoveActivity : BaseBmTask 
    {
        /// <summary>
        /// The name of the activity to remove
        /// </summary>
        public string ActivityName { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            const string argsFormat = "remove-activity -Name:{0}";
            return ExecuteBm(string.Format(argsFormat, ActivityName));
        }
    }
}
