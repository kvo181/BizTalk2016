using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    /// <summary>
    /// Builds the App.Rules.targets file
    /// </summary>
    public class RulesTargetsFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Rules.targets";

        #region IFileBuilder Members
        private string _name = "RulesTargetsFileBuilder";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// Builds the App.Rules.targets file
        /// </summary>
        /// <param name="args"></param>
        public void Build(GenerationArgs args)
        {
            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(Resources.App_Rules_targets);
            GenerationTags.ReplaceTags(fileBuilder, args);

            ExecuteCommandBuilder(new CommandBuilders.DeployRulesCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new CommandBuilders.RemoveRulesCommandBuilder(), args, fileBuilder);

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion

        /// <summary>
        /// Executes the command builder
        /// </summary>
        /// <param name="args"></param>
        /// <param name="sb"></param>
        /// <param name="cmd"></param>
        private void ExecuteCommandBuilder(CommandBuilders.ICommandBuilder cmd, GenerationArgs args, StringBuilder sb)
        {
            cmd.Build(args, sb);
        }
    }
}