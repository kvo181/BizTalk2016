using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    /// <summary>
    /// <ImportSSOConfigurationApplicationTask CompnayName="test" NonEncryptedFile="C:\Users\Administrator\Documents\SSO App Export\TestApp.xml" />
    /// </summary>
    [Serializable]
    public class SSOApplication
    {
        /// <summary>
        /// Company name to use when importing SSO applications
        /// </summary>
        public string CompanyName { get; set; }
        /// <summary>
        /// Name of the SSO Application script
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Path of the SSO Application script
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Destination in the BizTalk resources
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// SSO Application script
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!Directory.Exists(Path)) return string.Empty;
            return new DirectoryInfo(Path).FullName + "\\" + Name + ".xml";
        }
    }
}
