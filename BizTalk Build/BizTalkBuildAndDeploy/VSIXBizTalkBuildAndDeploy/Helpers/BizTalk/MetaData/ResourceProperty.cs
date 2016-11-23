using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    public class ResourceProperty
    {
        private string _Name;
        private object _Value;

        public object Value
        {
            get { return _Value; }
            set { _Value = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
    }
}