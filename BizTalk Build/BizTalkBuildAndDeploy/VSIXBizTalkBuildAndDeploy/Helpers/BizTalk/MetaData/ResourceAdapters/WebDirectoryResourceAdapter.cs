using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters
{
    public class WebDirectoryResourceAdapter : BaseResourceAdapter
    {
        private const string SourceLocationPropertyName = "SourceLocation";
        private const string DestinationLocationPropertyName = "DestinationLocation";
        private const string PhysicalPathPropertyName = "PhysicalPath";
        private const string AspNetVersionPropertyName = "AspNetVersion";
        private const string IISProcessModePropertyName = "IISProcessMode";

        public static WebDirectoryResourceAdapter Create(ApplicationResource resource)
        {
            if (resource.Type != ResourceTypes.WebDirectory)
                throw new ApplicationException("Invalid resource type");

            return new WebDirectoryResourceAdapter(resource);
        }

        private WebDirectoryResourceAdapter(ApplicationResource resource)
            : base(resource)
        {
        }

        public string DestinationLocation
        {
            get { return (string) base.FindPropertyValue(DestinationLocationPropertyName); }
        }

        public string SourceLocation
        {
            get { return (string) base.FindPropertyValue(SourceLocationPropertyName); }
        }

        public string PhysicalPath
        {
            get { return (string) base.FindPropertyValue(PhysicalPathPropertyName); }
        }

        public string AspNetVersion
        {
            get { return (string) base.FindPropertyValue(AspNetVersionPropertyName); }
        }

        public string IISProcessMode
        {
            get { return (string) base.FindPropertyValue(IISProcessModePropertyName); }
        }
    }
}