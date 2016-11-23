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
    /// <Exec Command='"$(VS80COMNTOOLS)..\IDE\mstest.exe" /testcontainer:\Ukm\Integration\CaseManagement\BizUnitSteps\Tests\bin\debug\Bupa.Ukm.Integration.CaseManagement.BizUnitSteps.Tests.dll /runconfig:$(ProductName).testrunconfig' />
    ///	<Exec Command='"$(VS80COMNTOOLS)..\IDE\mstest.exe" /testcontainer:\Ukm\Integration\CaseManagement\Utilities\Tests\bin\debug\Bupa.Ukm.Integration.CaseManagement.Utilities.Tests.dll /runconfig:$(ProductName).testrunconfig' />
    ///		<Exec Command='"$(VS80COMNTOOLS)..\IDE\mstest.exe" /testcontainer:\Ukm\Integration\CaseManagement\BizUnitTests\bin\debug\Bupa.Ukm.Integration.CaseManagement.BizUnitTests.dll /runconfig:$(ProductName).testrunconfig' />
    /// </remarks>
    [Serializable]
    [TypeConverter(typeof (ExpandableObjectConverter))]
    public class UnitTestContainer
    {
        private string _Location;

        [Editor(typeof (FileNameEditor), typeof (UITypeEditor))]
        [Description("The location of the assembly which will contain unit tests")]
        public string Location
        {
            get { return _Location; }
            set { _Location = value; }
        }

        /// <summary>
        /// Overrides tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            try
            {
                if (!string.IsNullOrEmpty(this._Location))
                    return System.IO.Path.GetFileName(this._Location);
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