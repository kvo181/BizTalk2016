using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Enable an application from SSO
    /// </summary>
    public class EnableApplication : BaseSSOTask
    {
        /// <summary>
        /// Name of the sso application
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// execute task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {           
            var arguments = string.Format("-enableapp {0}", ApplicationName);
            ExecuteSsoManage(arguments);
            return true;
        }
    }
}
