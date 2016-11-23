using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public class ReleaseCmdFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Build.Release.cmd";

        private string _name = "CustomPropertiesFileBuilder";
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
            fileBuilder.Append(Resources.App_Build_Release);
            GenerationTags.ReplaceTags(fileBuilder, args);

            fileBuilder.Replace("@GenerateBindings@", BizTalk.BizTalkHelper.GenerateBindings.ToString());

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}