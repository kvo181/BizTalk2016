using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters
{
    public class BindingResourceAdapter : BaseResourceAdapter
    {
        private const string TargetEnvironmentPropertyName = "TargetEnvironment";
        private const string SourceLocationPropertyName = "SourceLocation";
        private const string DestinationLocationPropertyName = "DestinationLocation";
        private const string FullNamePropertyName = "FullName";

        public static BindingResourceAdapter Create(ApplicationResource resource)
        {
            if (resource.Type != ResourceTypes.BizTalkBinding)
                throw new ApplicationException("Invalid resource type");

            return new BindingResourceAdapter(resource);
        }

        private BindingResourceAdapter(ApplicationResource resource) : base(resource)
        {
        }

        public string TargetEnvironment
        {
            get { return (string) base.FindPropertyValue(TargetEnvironmentPropertyName); }
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