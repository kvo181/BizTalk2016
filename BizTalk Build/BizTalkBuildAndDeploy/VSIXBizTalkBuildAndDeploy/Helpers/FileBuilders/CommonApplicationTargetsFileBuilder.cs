using System;
using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    /// <summary>
    /// Creates the common application targets file
    /// </summary>
    public class CommonApplicationTargetsFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".targets";

        private string _name = "CommonApplicationTargetsFileBuilder";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        #region IFileBuilder Members
        /// <summary>
        /// Implements file builder
        /// </summary>
        /// <param name="args"></param>
        public void Build(GenerationArgs args)
        {
            //Setup Template
            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(Resources.App_targets);
            GenerationTags.ReplaceTags(fileBuilder, args);

            //Add Commands
            ICommandBuilder commandBuilder = new RemoveFromGacCommandBuilder();
            commandBuilder.Build(args, fileBuilder);
            commandBuilder = new InstallInGacCommandBuilder();
            commandBuilder.Build(args, fileBuilder);

            //Write file
            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.ConfigureForSourceCodeRoot(fileBuilder, args);
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}