using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers
{
    /// <summary>
    /// Replaces a set of preconfigured tags with their meta data values
    /// </summary>
    public class GenerationTags
    {
        public const string ApplicationName = "@ApplicationName@";
        public const string ApplicationDescription = "@ApplicationDescription@";
        public const string PackageDirectory = "@PackageDirectory@";
        public const string MicrosoftSdcTasksTag = "@MicrosoftSdcTasksPath@";
        public const string TasksTag = "@BuildGeneratorTasksPath@";
        public const string SourceCodeRootTag = "@SourceCodeRootFolder@";

        /// <summary>
        /// Replaces all of the defined tags in the file
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="args"></param>
        public static void ReplaceTags(StringBuilder sb, GenerationArgs args)
        {
            sb.Replace(ApplicationName, args.ApplicationDescription.Name);
            sb.Replace(ApplicationDescription, args.ApplicationDescription.Description);
            sb.Replace(PackageDirectory, args.ApplicationDeployment.PublishMsiPath);
            sb.Replace(TasksTag, args.BuildReferences.TasksPath);
        }
    }
}