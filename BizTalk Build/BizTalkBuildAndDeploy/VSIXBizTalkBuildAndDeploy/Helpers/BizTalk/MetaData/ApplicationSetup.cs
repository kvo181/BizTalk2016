using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    public class ApplicationSetup
    {
        private bool _IncludeSetup;
        private string _SetupBuildFilePath;

        public string SetupBuildFilePath
        {
            get { return _SetupBuildFilePath; }
            set { _SetupBuildFilePath = value; }
        }

        public bool IncludeSetup
        {
            get { return _IncludeSetup; }
            set { _IncludeSetup = value; }
        }
    }
}