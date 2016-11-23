using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public class QuickBuildDebugCmdFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Build.Quick.Debug.cmd";

        private string _name = "QuickBuildDebugCmdFileBuilder";
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
            fileBuilder.Append(Resources.App_QuickBuild_Debug);
            GenerationTags.ReplaceTags(fileBuilder, args);

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}