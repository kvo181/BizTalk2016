using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    public class ApplicationDeployment
    {
        private string _PublishMsiPath;

        public string PublishMsiPath
        {
            get { return _PublishMsiPath; }
            set { _PublishMsiPath = value; }
        }

    }
}