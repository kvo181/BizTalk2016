using System;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public class GenerateBindingsFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".GenerateBindings.cmd";

        private string _name = "GenerateBindingsFileBuilder";
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
            fileBuilder.Append(Resources.App_GenerateBindings);
            GenerationTags.ReplaceTags(fileBuilder, args);

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}