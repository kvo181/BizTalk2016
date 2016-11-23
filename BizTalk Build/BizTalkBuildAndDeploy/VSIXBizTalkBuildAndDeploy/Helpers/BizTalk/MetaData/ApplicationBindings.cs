using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class ApplicationBindings
    {
        private List<ApplicationBinding> _BindingFiles = new List<ApplicationBinding>();

        [Description("The binding files to be applied to the application during the build")]
        public List<ApplicationBinding> BindingFiles
        {
            get { return _BindingFiles; }
            set { _BindingFiles = value; }
        }
    }
}