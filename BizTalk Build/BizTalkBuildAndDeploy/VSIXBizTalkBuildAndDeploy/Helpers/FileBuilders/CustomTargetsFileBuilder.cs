using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    /// <summary>
    /// Builds the App.Custom.targets file
    /// </summary>
    public class CustomTargetsFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Custom.targets";

        private string _name = "CustomTargetsFileBuilder";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region IFileBuilder Members

        public void Build(GenerationArgs args)
        {
            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(Resources.App_Custom_targets);
            GenerationTags.ReplaceTags(fileBuilder, args);

            ExecuteCommandBuilder(new CreateBindingFilesCommandBuilder(), args, fileBuilder);

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
        private void ExecuteCommandBuilder(ICommandBuilder cmd, GenerationArgs args, StringBuilder sb)
        {
            cmd.Build(args, sb);
        }
    }
}