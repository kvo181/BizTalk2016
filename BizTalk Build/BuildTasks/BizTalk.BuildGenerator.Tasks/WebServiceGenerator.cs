using System;
using Microsoft.BizTalk.WebServices;
using Microsoft.BizTalk.WebServices.Description;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// This will generate a wse web service for biztalk based on a supplied web service description
    /// </summary>
    /// <example>
    /// <WebServiceGenerator WebServiceDescriptionPath="C:\PathtoDescription" />
    /// </example>
    public class WebServiceGenerator : Task
    {
        /// <summary>
        /// The path of the web service description
        /// </summary>
        [Required]
        public string WebServiceDescriptionPath { get; set; }

        /// <summary>
        /// The execute of the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Console.WriteLine(@"Generating Standard Web Services");
            var desc = WebServiceDescription.LoadXml(WebServiceDescriptionPath);
            var builder = new WebServiceBuilder {WebServiceDescription = desc};
            builder.ProgressCallback += OnProgress;
            builder.BuildWebService();

            Console.WriteLine(@"Completed publishing web services");
            return true;
        }

        /// <summary>
        /// Handler for progress messages
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnProgress(object sender, ProgressEventArgs e)
        {            
            Console.WriteLine(String.Format(@"OnProgress: {0} {1}", e.Value, e.Message));
        }
    }
}