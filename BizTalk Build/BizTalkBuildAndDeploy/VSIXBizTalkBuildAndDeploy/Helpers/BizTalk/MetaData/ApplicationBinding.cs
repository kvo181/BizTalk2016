using System;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <Exec Command="BTSTask ImportBindings -Source:$(DevelopmentBizTalkBindingFile) -ApplicationName:$(ProductName)" />
    /// </remarks>
    [Serializable]
    public class ApplicationBinding
    {
        private string _BindingFilePath;


        [Editor(typeof (FileNameEditor), typeof (UITypeEditor))]
        [Description("The path to the binding file")]
        public string BindingFilePath
        {
            get { return _BindingFilePath; }
            set { _BindingFilePath = value; }
        }
        /// <summary>
        /// Overrides tostring for the designer
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                if (!string.IsNullOrEmpty(this._BindingFilePath))
                    return System.IO.Path.GetFileName(this._BindingFilePath);
                else
                    return base.ToString();
            }
            catch
            {
                return base.ToString();
            }

        }
    }
}