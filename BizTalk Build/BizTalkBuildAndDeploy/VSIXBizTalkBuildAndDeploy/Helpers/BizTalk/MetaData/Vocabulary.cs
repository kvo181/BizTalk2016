using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    /// <summary>
    /// <bizilante.VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.Tasks.Policies.DeployVocabulary 
    ///     VocabularyName="OBK.Beslissing.Aanvraag" 
    ///     VocabularyFileName="D:\BizTalk\Projects\OBKDemo\Rules\Vocabulary__OBK.Beslissing.Aanvraag__1.1.xml" />
    ///     
    /// <bizilante.VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.Tasks.Policies.RemoveVocabulary
    ///     VocabularyName="OBK.Beslissing.Aanvraag" />
    /// </summary>
    [Serializable]
    public class Vocabulary
    {
        /// <summary>
        /// Name of the Vocabulary
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// FullPath of the Vocabulary filename
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// Major version of the Vocabulary
        /// </summary>
        public int Major { get; set; }
        /// <summary>
        /// Minor version of the Vocabulary
        /// </summary>
        public int Minor { get; set; }
        /// <summary>
        /// Vocabulary version
        /// </summary>
        public string Version 
        {
            get
            {
                return string.Format("{0}.{1}", Major, Minor);
            }
        }
        /// <summary>
        /// Vocabulary 
        /// </summary>
        public string FullName
        {
            get
            {
                return "Vocabulary__" + Name + "__" + Version + ".xml";
            }
        }
        /// <summary>
        /// Policy filename
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (!Directory.Exists(Path)) return string.Empty;
            return new DirectoryInfo(Path).FullName + "\\Vocabulary__" + Name + "__" + Version + ".xml";
        }

        /// <summary>
        /// Create a Vocabulary object out of a given Policy filename.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>Policy</returns>
        public static Vocabulary Create(string fileName)
        {
            string[] parts = fileName.Split(new string[] {"__"}, StringSplitOptions.None);
            if (parts.Length < 3) return null;

            string[] parts1 = parts[2].Split(new string[] { "." }, StringSplitOptions.None);
            if (parts1.Length < 3) return null;

            Vocabulary vocabulary = new Vocabulary();

            vocabulary.Path = new FileInfo(fileName).DirectoryName;
            vocabulary.Name = parts[1];
            vocabulary.Major = int.Parse(parts1[0]);
            vocabulary.Minor = int.Parse(parts1[1]);

            return vocabulary;
        }
    }
}
