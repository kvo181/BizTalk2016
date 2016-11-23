using System.Globalization;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Builds the commands to use create the binding files used by this application
    /// We use the BizTalk Binding Tool for this.
    /// </summary>
    public class CreateBindingFilesCommandBuilder : ICommandBuilder
    {
        private const string CreateBindingCommand =
            "\t\t<bizilante.BizTalkFastTrackBindingTool.BuildTasks.GenerateBindingFiles ContinueOnError=\"true\" ApplicationName=\"$(ProductName)\" ProjectRootPath=\"$(SourceCodeRootFolder)\" IncludeOriginal=\"False\" ConnectionString=\"metadata=res://*/BindingToolEntities.csdl|res://*/BindingToolEntities.ssdl|res://*/BindingToolEntities.msl;provider=System.Data.SqlClient;provider connection string=&quot;Data Source={0};Initial Catalog={1};Integrated Security=True;multipleactiveresultsets=True;App=EntityFramework&quot;\" />";

        private const string CommandTag = "<!-- @CreateBindingsCommand@ -->";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        void ICommandBuilder.Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            if (!BizTalk.BizTalkHelper.GenerateBindings)
            {
                fileBuilder.Replace(CommandTag, string.Empty);
                return;
            }

            string server = string.Empty;
            string database = string.Empty;
            BizTalk.BizTalkHelper.GetBizTalkBindingToolDb(out server, out database);

            string command =
                string.Format(CultureInfo.InvariantCulture, CreateBindingCommand, server, database);

            StringBuilder commandBuilder = new StringBuilder();
            commandBuilder.Append(command);

            fileBuilder.Replace(CommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}