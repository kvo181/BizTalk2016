using System.Text;
using System.IO;
using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    /// <summary>
    /// Builds the App.Setup.targets file
    /// </summary>
    public class ApplicationPropertiesFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".properties";

        private string _Name = "ApplicationPropertiesFileBuilder";
        public string Name
        {
            get
            {
                return _Name;
            }
        }

        #region IFileBuilder Members
        /// <summary>
        /// implements file builder
        /// </summary>
        /// <param name="args"></param>
        public void Build(GenerationArgs args)
        {
            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(Resources.App_properties);
            GenerationTags.ReplaceTags(fileBuilder, args);

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.ConfigureForSourceCodeRoot(fileBuilder, args);

            //The root tag is only replaced here so it is not overwritten when configuring the root path everywhere else
            if (!string.IsNullOrEmpty(args.SolutionPath))
            {
                fileBuilder.Replace(GenerationTags.SourceCodeRootTag,
                                    new DirectoryInfo(Path.GetDirectoryName(args.SolutionPath)).FullName.TrimEnd(
                                        new char[] { '\\' }));
                fileBuilder.Replace("@SolutionName@", Path.GetFileName(args.SolutionPath));
            }

            // We need to replace the version info
            fileBuilder.Replace("@MajorVersion@", args.MajorVersion);
            fileBuilder.Replace("@MinorVersion@", args.MinorVersion);
            fileBuilder.Replace("@AssemblyInfoFiles@", BizTalk.BizTalkHelper.GetAssemblyInfoFiles(BizTalk.BizTalkHelper.ProjectStructureType));

            FileHelper.WriteFile(filePath, fileBuilder);

            _Name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}