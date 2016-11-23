namespace VSIXBizTalkBuildAndDeploy.Helpers
{
    public sealed class PathHelper
    {
        /// <summary>
        /// Formats a path so it is confgurable based on build parameters
        /// </summary>
        /// <param name="path"></param>
        public static string MakeConfigurable(string path)
        {
            const string Debug = @"\bin\Debug";
            const string Release = @"\bin\Release";
            const string Development = @"\bin\Development";
            const string Deployment = @"\bin\Deployment";
            const string BizTalkPath = @"\bin\$(DeploymentMode)";
            const string DotNetPath = @"\bin\$(ConfigurationName)";

            path = path.Replace(Debug, DotNetPath);
            path = path.Replace(Release, DotNetPath);
            path = path.Replace(Development, BizTalkPath);
            path = path.Replace(Deployment, BizTalkPath);

            return path;
        }
    }
}