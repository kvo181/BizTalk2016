using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Creates a set of SSO applications based on the details of a file
    /// </summary>
    public class CreateApplications : BaseSSOTask
    {
        /// <summary>
        /// The location of the file defining the SSO Applications
        /// </summary>
        [Required]
        public string ApplicationDefinitionPath { get; set; }

        /// <summary>
        /// execute task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var arguments = string.Format("-createapps {0}", ApplicationDefinitionPath);
            ExecuteSsoManage(arguments);
            return true;
        }
    }
}
