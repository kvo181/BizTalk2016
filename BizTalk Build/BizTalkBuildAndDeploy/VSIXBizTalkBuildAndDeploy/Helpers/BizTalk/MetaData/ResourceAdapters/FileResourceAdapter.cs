using System;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters
{
    public class FileResourceAdapter : BaseResourceAdapter
    {
        private const string SourceLocationPropertyName = "SourceLocation";
        private const string DestinationLocationPropertyName = "DestinationLocation";

        public static FileResourceAdapter Create(ApplicationResource resource)
        {
            if (resource.Type != ResourceTypes.File)
                throw new ApplicationException("Invalid resource type");
            return new FileResourceAdapter(resource);
        }

        private FileResourceAdapter(ApplicationResource resource)
            : base(resource)
        {
        }

        /// <summary>
        /// Gets the name of the resource by parsing the source location
        /// </summary>
        public string Name
        {
            get
            {
                string[] sourceLocationParts = SourceLocation.Split(char.Parse(@"\"));
                return sourceLocationParts[sourceLocationParts.GetUpperBound(0)];
            }
        }

        /// <summary>
        /// Gets the name without the extension, eg if name = SomeFile.xml then this will be SomeFile
        /// </summary>
        public string ActivityName
        {
            get { return Name.Substring(0, Name.Length - 4); }
        }

        public string SourceLocation
        {
            get { return (string) base.FindPropertyValue(SourceLocationPropertyName); }
        }

        public string DestinationLocation
        {
            get { return (string) base.FindPropertyValue(DestinationLocationPropertyName); }
        }
    }
}