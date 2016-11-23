using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    [Serializable]
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class UnitTesting
    {
        private List<UnitTestContainer> _TestContainers = new List<UnitTestContainer>();
        private string _TestRunConfigPath;

        [Description("TestRunConfig Path")]
        [Editor(typeof (FileNameEditor), typeof (UITypeEditor))]
        public string TestRunConfigPath
        {
            get { return _TestRunConfigPath; }
            set { _TestRunConfigPath = value; }
        }

        [Description("List of test assemblies")]
        public List<UnitTestContainer> TestContainers
        {
            get { return _TestContainers; }
            set { _TestContainers = value; }
        }
    }
}