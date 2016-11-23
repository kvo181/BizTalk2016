using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders
{
    /// <summary>
    /// Builds the commands to use BTSTask to apply the binding files to an application
    /// </summary>
    public class BindingFilesCommandBuilder : ICommandBuilder
    {
        private const string ApplyBindingFileCommand =
            "\t\t<Exec Command='BTSTask ImportBindings -Source:\"{0}\" -ApplicationName:$(ProductName)' />";

        private const string CommandTag = "<!-- @ApplyBindingsCommand@ -->";

        #region ICommandBuilder Members

        /// <summary>
        /// Implements the build method
        /// </summary>
        /// <param name="args"></param>
        /// <param name="fileBuilder"></param>
        void ICommandBuilder.Build(GenerationArgs args, StringBuilder fileBuilder)
        {
            StringBuilder commandBuilder = new StringBuilder();

            foreach (BizTalk.MetaDataBuildGenerator.ApplicationBinding binding in args.ApplicationBindings.BindingFiles)
            {
                StringBuilder sb = new StringBuilder(binding.BindingFilePath);
                sb.Replace("$(SourceCodeRootFolder)", new DirectoryInfo(Path.GetDirectoryName(args.SolutionPath)).FullName.TrimEnd(new char[] { '\\' }));
                sb.Replace("$(ProductName)", args.ApplicationDescription.Name);
                string bindingFilePath = sb.ToString();
                if (!File.Exists(bindingFilePath)) continue;
                string command =
                    string.Format(CultureInfo.InvariantCulture, ApplyBindingFileCommand, binding.BindingFilePath);
                commandBuilder.Append(command);
                commandBuilder.Append(Environment.NewLine);
            }

            fileBuilder.Replace(CommandTag, commandBuilder.ToString());
        }

        #endregion
    }
}