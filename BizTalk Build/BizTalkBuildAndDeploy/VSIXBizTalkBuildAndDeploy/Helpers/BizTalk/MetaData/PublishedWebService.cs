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
    /// <!--<WebServiceGenerator WebServiceDescriptionPath=".Ukm.Integration.CaseManagement.Web.Services.Description2.xml" />-->
    ///	<WseWebServiceGenerator WebServiceDescriptionPath="CaseManagement.Web.Services.Description.Wse.xml" />
    /// </remarks>
    [Serializable]
    public class PublishedWebService
    {
        private string _WebServiceDescriptionPath;
        private WebServiceTypes _Type;

        [Description("The type of web services which will be generated")]
        public WebServiceTypes Type
        {
            get { return _Type; }
            set { _Type = value; }
        }


        [Editor(typeof (FileNameEditor), typeof (UITypeEditor))]
        [Description("The path to the web service description file")]
        public string WebServiceDescriptionPath
        {
            get { return _WebServiceDescriptionPath; }
            set { _WebServiceDescriptionPath = value; }
        }
        /// <summary>
        /// Overrides tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {            
            try
            {
                string comment = Enum.GetName(typeof(WebServiceTypes), this._Type);
                if (!string.IsNullOrEmpty(this._WebServiceDescriptionPath))
                    return comment + ": " + System.IO.Path.GetFileName(this._WebServiceDescriptionPath);
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