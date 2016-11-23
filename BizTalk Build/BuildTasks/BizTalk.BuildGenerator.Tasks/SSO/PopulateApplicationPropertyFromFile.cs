using System;
using Microsoft.Build.Framework;
using System.IO;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Populates a custom property in an SSO application with data read from a file
    /// </summary>
    public class PopulateApplicationPropertyFromFile : BaseSSOTask
    {
        /// <summary>
        /// The name of the application property
        /// </summary>
        [Required]
        public string PropertyName { get; set; }

        /// <summary>
        /// The file path for data to populate in the field
        /// </summary>
        [Required]
        public string FilePath { get; set; }

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
            if (!File.Exists(FilePath))
                throw new ApplicationException("The file with the property data does not exist");
            
            var data = File.ReadAllText(FilePath);
            SSOConfiguration.Write(ApplicationName, PropertyName, data);
            return true;
        }
    }
}
