using System.Collections.Generic;
using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    public class SSOApplications
    {
        public List<SSOApplication> SSOApps { get; set; }
    }
}