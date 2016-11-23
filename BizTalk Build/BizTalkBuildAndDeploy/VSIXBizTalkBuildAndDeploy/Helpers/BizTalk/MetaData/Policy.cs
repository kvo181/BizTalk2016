using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    /// <summary>
    /// <bizilante.VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.Tasks.Policies.DeployPolicy 
    ///     PolicyName="OBK.Acceptatie.Aanvraag" 
    ///     PolicyFileName="D:\BizTalk\Projects\OBKDemo\Rules\Policy__OBK.Acceptatie.Aanvraag__1.1.xml" 
    ///     Deploy="False" />
    ///     
    /// <bizilante.VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.Tasks.Policies.RemovePolicy 
    ///     PolicyName="OBK.Acceptatie.Aanvraag" />
    ///     
    /// <Exec Command='BTSTask AddResource /ApplicationName:"ArcelorMittalLogistics" /Type:System.BizTalk:Rules  /Name="OBK.Acceptatie.Aanvraag" /Version="1.1"' />
    /// </summary>
    [Serializable]
    public class Policy
    {
        /// <summary>
        /// Name of the Policy
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// FullPath of the Policy filename
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Major version of the Policy
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// Minor version of the Policy
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// Policy version
        /// </summary>
        public string Version 
        {
            get
            {
                return string.Format("{0}.{1}", Major, Minor);
            }
        }
        /// <summary>
        /// Return the filename of the policy.
        /// </summary>
        public string FullName
        {
            get
            {
                return "Policy__" + Name + "__" + Version + ".xml";
            }
        }
        /// <summary>
        /// Policy filename
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!Directory.Exists(Path)) return string.Empty;
            return new DirectoryInfo(Path).FullName + "\\Policy__" + Name + "__" + Version + ".xml";
        }

        /// <summary>
        /// Create a Policy object out of a given Policy filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Policy</returns>
        public static Policy Create(string fileName)
        {
            string[] parts = fileName.Split(new string[] {"__"}, StringSplitOptions.None);
            if (parts.Length < 3) return null;

            string[] parts1 = parts[2].Split(new string[] { "." }, StringSplitOptions.None);
            if (parts1.Length < 3) return null;

            Policy policy = new Policy();

            policy.Path = new FileInfo(fileName).DirectoryName;
            policy.Name = parts[1];
            policy.Major = int.Parse(parts1[0]);
            policy.Minor = int.Parse(parts1[1]);

            return policy;
        }
    }
}
