using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class PublishedWebServices
    {
        private List<PublishedWebService> _WebServices = new List<PublishedWebService>();

        public List<PublishedWebService> WebServices
        {
            get { return _WebServices; }
            set { _WebServices = value; }
        }
    }
}