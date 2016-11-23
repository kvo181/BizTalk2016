using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public class DebugCmdFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Build.Debug.cmd";

        private string _name = "DebugCmdFileBuilder";
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
            fileBuilder.Append(Resources.App_Build_Debug);
            GenerationTags.ReplaceTags(fileBuilder, args);

            fileBuilder.Replace("@GenerateBindings@", BizTalk.BizTalkHelper.GenerateBindings.ToString());

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}