using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData
{
    /// <summary>
    /// Contains information about the location of references for the build.  For example the location of sdc tasks
    /// </summary>
    [Serializable]
    public class BuildReferences
    {        
        private string _TasksPath;

        /// <summary>
        /// The location of the  tasks
        /// </summary>
        [Editor(typeof (FolderNameEditor), typeof (UITypeEditor))]
        [Description("The path to the  Build Generator MsBuild Tasks")]
        public string TasksPath
        {
            get { return _TasksPath; }
            set { _TasksPath = value; }
        }        
    }
}