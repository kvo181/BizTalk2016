using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Xml;
using System.IO;

namespace bizilante.BuildGenerator.Tasks
{
    public class PrepareArtifacts : Task
    {
        /// <summary>
        /// Path to the Artefacts.xml file
        /// </summary>
        [Required]
        public string FilePath { get; set; }
        /// <summary>
        /// Name of the Biztalk application
        /// </summary>
        [Required]
        public string ApplicationName { get; set; }
        public override bool Execute()
        {
            if (string.IsNullOrEmpty(ApplicationName))
                throw new ArgumentException("ApplicationName argument cannot be empty");
            if (string.IsNullOrEmpty(FilePath))
                throw new ArgumentException("FilePath argument cannot be empty");

            FileInfo fi = new FileInfo(FilePath);
            if (!fi.Exists)
                throw new Exception(string.Format("'{0}' does not exist!", fi.FullName));

            // We need to remove the BindingInfo out of the Artefacts.xml file
            XmlDocument d = new XmlDocument();
            try
            {
                Logger.LogMessage(this, string.Format("Load '{0}'", this.FilePath));
                d.Load(fi.FullName);

                // Remove System.BizTalk:BizTalkBinding node
                string xpath = string.Format("//*[local-name()='Resource'][@Type='System.BizTalk:BizTalkBinding'][@Luid='Application/{0}']", ApplicationName);
                Logger.LogMessage(this, string.Format("Remove node '{0}'", xpath));
                XmlNode t = d.SelectSingleNode(xpath);
                t.ParentNode.RemoveChild(t);

                // Remove System.BizTalk:WebDirectory node
                xpath = string.Format("//*[local-name()='Resource'][@Type='System.BizTalk:WebDirectory']");
                Logger.LogMessage(this, string.Format("Remove node '{0}'", xpath));
                XmlNodeList l = d.SelectNodes(xpath);
                foreach (XmlNode n in l)   
                   n.ParentNode.RemoveChild(n);

                Logger.LogMessage(this, string.Format("Save updated '{0}'", this.FilePath));
                d.Save(this.FilePath);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(this, string.Format("Prepare on '{0}' failed (Reason={1})", this.FilePath, ex.Message));
                throw;
            }
            return true;
        }
    }
}
