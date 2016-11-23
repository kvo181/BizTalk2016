using System;
using System.Text;
using VSIXBizTalkBuildAndDeploy.Helpers.CommandBuilders;

namespace VSIXBizTalkBuildAndDeploy.Helpers.FileBuilders
{
    /// <summary>
    /// Builds the App.Custom.targets file
    /// </summary>
    public class CustomPropertiesFileBuilder : IFileBuilder
    {
        private const string FileExtension = ".Custom.properties";

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
            fileBuilder.Append(Resources.App_Custom_Properties);
            GenerationTags.ReplaceTags(fileBuilder, args);

            if (args.BuildProperties != null && args.BuildProperties.Properties != null)
            {
                StringBuilder sb = new StringBuilder();
                foreach (BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty property in args.BuildProperties.Properties)
                {
                    sb.AppendFormat("<{0}>{1}</{0}>", property.PropertyName, property.PropertyValue);
                    sb.AppendLine();
                }
                fileBuilder.Replace("<!-- @CustomProperties@ -->", sb.ToString());
            }
            string filePath = args.OutputFolder + @"\" + args.ApplicationDescription.Name + FileExtension;
            FileHelper.WriteFile(filePath, fileBuilder);

            _name = args.ApplicationDescription.Name + FileExtension;
        }

        #endregion
    }
}