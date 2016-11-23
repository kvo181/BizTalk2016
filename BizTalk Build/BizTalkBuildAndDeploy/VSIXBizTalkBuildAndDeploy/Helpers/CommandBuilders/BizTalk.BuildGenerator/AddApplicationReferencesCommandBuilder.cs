using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Creates the add references commands
    /// </summary>
    public class AddApplicationReferencesCommandBuilder : ICommandBuilder
    {
        private const string Tag = "<!-- @@AddReferencedApplications@@ -->";
        private const string CommandTemplate = "\t\t<BizTalk.BuildGenerator.Tasks.AddReference ReferencedApplicationName=\"{0}\" MessageBoxConnection=\"$(BizTalkManagementDatabaseConnectionString)\" ApplicationName=\"$(ProductName)\"/>";

        #region ICommandBuilder Members
        /// <summary>
        /// Implements the command builder execution method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        public void Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            if (null == args.ApplicationDescription.ReferencedApplications) return;
            StringBuilder sb = new StringBuilder();
            foreach (string referencedApplication in args.ApplicationDescription.ReferencedApplications)
            {
                sb.AppendFormat(CommandTemplate, referencedApplication);
                sb.AppendLine();
            }
            fileBuilder.Replace(Tag, sb.ToString());
        }

        #endregion
    }
}
