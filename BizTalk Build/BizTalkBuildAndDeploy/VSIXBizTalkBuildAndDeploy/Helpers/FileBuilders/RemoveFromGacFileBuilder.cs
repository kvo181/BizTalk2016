using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    public class RemoveFromGacFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".RemoveFromGac.cmd";

        #region IFileBuilder Members

        private string _name = "RemoveFromGacFileBuilder";
        public string Name
        {
            get
            {
                return _name;
            }
        }

        public void Build(GenerationArgs args)
        {
            StringBuilder fileBuilder = new StringBuilder();
            fileBuilder.Append(Resources.App_RemoveFromGac);
            GenerationTags.ReplaceTags(fileBuilder, args);

            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}