using System;
using System.Collections.Generic;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData
{
    /// <summary>
    /// This class is to contain properties for the build
    /// </summary>
    [Serializable]
    public class BuildProperties
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public BuildProperties()
        {
            Properties = new List<BuildProperty>();
        }
        /// <summary>
        /// Properties for the build
        /// </summary>
        public List<BuildProperty> Properties { get; set; }
    }
    /// <summary>
    /// A build property
    /// </summary>
    public class BuildProperty
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// The value of the property
        /// </summary>
        public string PropertyValue { get; set; }
    }
}
