using System;
using System.Collections.Generic;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    public class ApplicationDescription
    {
        private string _Name;
        private string _Description;
        private List<ApplicationResource> _Resources = new List<ApplicationResource>();
        private List<string> _ReferencedApplications = new List<string>();

        public List<string> ReferencedApplications
        {
            get { return _ReferencedApplications; }
            set { _ReferencedApplications = value; }
        }

        public List<ApplicationResource> Resources
        {
            get { return _Resources; }
            set { _Resources = value; }
        }

        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}