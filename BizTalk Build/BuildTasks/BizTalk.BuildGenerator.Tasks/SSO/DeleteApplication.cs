using Microsoft.Build.Framework;
using Microsoft.EnterpriseSingleSignOn.Interop;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Deletes an application from SSO
    /// </summary>
    public class DeleteApplication : BaseSSOTask 
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
            var admin = new ISSOAdmin();
            admin.DeleteApplication(ApplicationName);            
            return true;
        }
    }
}
