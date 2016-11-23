using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// Deploys a BAM tracking profile
    /// </summary>
    public class RemoveTrackingProfile : BaseBttDeployTask 
    {
        /// <summary>
        /// The path to the BAM tracking profile .btt file
        /// </summary>
        [Required]
        public string TrackingProfilePath { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            const string argsFormat = "/remove {0}";
            return ExecuteBttDeploy(string.Format(argsFormat, TrackingProfilePath));            
        }
    }
}
