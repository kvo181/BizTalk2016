//using System;
//using Microsoft.BizTalk.WseWebServices;
//using Microsoft.BizTalk.WseWebServices.Description;
//using Microsoft.Build.Framework;
//using Microsoft.Build.Utilities;

//namespace BizTalk.BuildGenerator.Tasks
//{
//    /// <summary>
//    /// This will generate a wse web service for biztalk based on a supplied web service description
//    /// </summary>
//    /// <example>
//    /// <WseWebServiceGenerator WebServiceDescriptionPath="C:\PathtoDescription" />
//    /// </example>
//    public class WseWebServiceGenerator : Task
//    {
//        private string _Path;

//        /// <summary>
//        /// The path of the web service description
//        /// </summary>
//        [Required]
//        public string WebServiceDescriptionPath
//        {
//            get { return _Path; }
//            set { _Path = value; }
//        }

//        /// <summary>
//        /// The execute of the task
//        /// </summary>
//        /// <returns></returns>
//        public override bool Execute()
//        {
//            Console.WriteLine("Generating WSE Web Services");
//            WebServiceDescription desc = WebServiceDescription.LoadXml(_Path);
//            WebServiceBuilder builder = new WebServiceBuilder();
//            builder.WebServiceDescription = desc;
//            builder.ProgressCallback += new ProgressEventHandler(OnProgress);
//            builder.BuildWebService();

//            Console.WriteLine("Completed publishing web services");
//            return true;
//        }

//        /// <summary>
//        /// Handler for progress messages
//        /// </summary>
//        /// <param name="sender"></param>
//        /// <param name="e"></param>
//        private static void OnProgress(object sender, ProgressEventArgs e)
//        {
//            Console.WriteLine(String.Format("OnProgress: {0} {1}", e.Value, e.Message));
//        }
//    }
//}