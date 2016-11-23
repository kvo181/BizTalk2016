using System;
using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders;
using VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders.BizTalkHost;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    /// <summary>
    /// Builds the App.Build.proj file
    /// </summary>
    public class BuildProjectFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Build.proj";

        private string _name = "BuildProjectFileBuilder";
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
            fileBuilder.Append(Resources.App_Build);
            GenerationTags.ReplaceTags(fileBuilder, args);

            //Do Commands
            ExecuteCommandBuilder(new ApplicationResourceCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new BindingFilesCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new CleanHostCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new CreateHostBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new CreateHostInstanceBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new CreateReceiveHandlerBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new CreateSendHandlerBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new DeleteHostCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new DeleteSendHandlerBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new DeleteReceiveHandlerBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new StartHostCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new StopHostCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new UnitTestsCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new AddApplicationReferencesCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new ImportSSOCommandBuilder(), args, fileBuilder);
            ExecuteCommandBuilder(new AssemblyInfoCommandBuilder(), args, fileBuilder);

            // $(SourceCodeRootFolder)\Setup\Resources\ReadMe.htm

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.ConfigureForBizTalkInstallVariable(fileBuilder);
            FileHelper.ConfigureForSourceCodeRoot(fileBuilder, args);
            FileHelper.ConfigureForReadMe(fileBuilder);
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