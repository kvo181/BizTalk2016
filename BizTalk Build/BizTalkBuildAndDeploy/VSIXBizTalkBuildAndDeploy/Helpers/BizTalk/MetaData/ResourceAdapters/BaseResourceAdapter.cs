namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters
{
    public abstract class BaseResourceAdapter
    {
        private ApplicationResource _Resource;

        public ApplicationResource Resource
        {
            get { return _Resource; }
            set { _Resource = value; }
        }

        public BaseResourceAdapter(ApplicationResource resource)
        {
            _Resource = resource;
        }

        protected object FindPropertyValue(string key)
        {
            foreach (ResourceProperty property in _Resource.Properties)
            {
                if (property.Name == key)
                    return property.Value;
            }
            return null;
        }

        /// <summary>
        /// Formats a path so it is confgurable based on build parameters
        /// </summary>
        /// <param name="path"></param>
        public static string FormatResourcePath(string path)
        {
            return PathHelper.MakeConfigurable(path);
        }
    }
}