using System;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters
{
    public class BamResourceAdapter : BaseResourceAdapter
    {
        private const string SourceLocationPropertyName = "SourceLocation";
        private const string DestinationLocationPropertyName = "DestinationLocation";

        public static BamResourceAdapter Create(ApplicationResource resource)
        {
            if (resource.Type != ResourceTypes.Bam)
                throw new ApplicationException("Invalid resource type");
            return new BamResourceAdapter(resource);
        }

        private BamResourceAdapter(ApplicationResource resource)
            : base(resource)
        {
        }

        /// <summary>
        /// Gets the name of the activity by parsing the source location
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
        /// Gets the name without the extension, eg if name = SomeActivity.xml then this will be Some
        /// </summary>
        public string ActivityName
        {
            get { return Name.Substring(0, Name.Length - 12); }
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