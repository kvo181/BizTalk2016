using System;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Populates a custom property in an SSO application with data
    /// </summary>
    public class PopulateApplicationProperty : BaseSSOTask 
    {
        /// <summary>
        /// The name of the application property
        /// </summary>
        [Required]
        public string PropertyName { get; set; }

        /// <summary>
        /// The data to populate in the field
        /// </summary>
        [Required]
        public string Data { get; set; }

        /// <summary>
        /// Name of the sso application
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Console.WriteLine("Executing Populate Application Property");
            
            SSOConfiguration.Write(ApplicationName, PropertyName, Data);
            return true;
        }
    }
}
