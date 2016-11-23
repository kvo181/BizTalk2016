using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// <Exec Command='BTSTask AddResource /Source:$(DevelopmentBizTalkBindingFile) /Property:TargetEnvironment="Development" /ApplicationName:"$(ProductName)" /Type:System.BizTalk:BizTalkBinding ' />
    ///	<Exec Command='BTSTask AddResource /Source:$(UtilitiesAssemblyPath) /ApplicationName:"$(ProductName)" /Type:System.BizTalk:Assembly /Overwrite /Destination:"$(DeploymentDirectory)\Ukm.Integration.CaseManagement.Utilities.dll" /Options:GacOnAdd,GacOnInstall,GacOnImport' />
    ///	<Exec Command='BTSTask AddResource /Source:$(GeneratedTypesAssemblyPath) /ApplicationName:"$(ProductName)" /Type:System.BizTalk:Assembly /Overwrite /Destination:"$(DeploymentDirectory)\Ukm.Integration.CaseManagement.GeneratedTypes.dll" /Options:GacOnAdd,GacOnInstall,GacOnImport' />
    /// <Exec Command='BTSTask AddResource /Source:$(BizTalkAssemblyPath) /ApplicationName:"$(ProductName)" /Type:System.BizTalk:BizTalkAssembly /Overwrite /Destination:"$(DeploymentDirectory)\Ukm.Integration.CaseManagement.VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDatadll" /Options:GacOnAdd,GacOnInstall,GacOnImport' />
    /// <Exec Command='BTSTask AddResource /Source:$(BizTalkBamPath) /ApplicationName:"$(ProductName)" /Type:System.BizTalk:Bam /Overwrite /Destination:"$(DeploymentDirectory)\BAM\SomeActivity.xml" ' />
    /// <Exec Command='BTSTask AddResource /Source:$(BizTalkResourcesPath) /ApplicationName:"$(ProductName)" /Type:System.BizTalk:File /Overwrite /Destination:"$(DeploymentDirectory)\Resources\SomeFile.txt" ' />
    /// 
    /// <!-- UnGac Everything -->
    ///	<Exec Command='"$(GacUtilPath)gacutil" /u Ukm.Integration.CaseManagement.GeneratedType' ContinueOnError="true"/>		
    ///	<Exec Command='"$(GacUtilPath)gacutil" /u Ukm.Integration.CaseManagement.BizTalk' ContinueOnError="true"/>
    ///	<Exec Command='"$(GacUtilPath)gacutil" /u Ukm.Integration.CaseManagement.Utilities' ContinueOnError="true"/>
    /// </remarks>
    [Serializable]
    public class ApplicationResource  : IComparable<ApplicationResource>
    {
        private string _FullName;
        private string _Type;
        private List<ResourceProperty> _Properties = new List<ResourceProperty>();
        private List<string> _References = new List<string>();
        private List<ApplicationResource> _DependantResources = new List<ApplicationResource>();

        public List<ApplicationResource> DependantResources
        {
            get { return _DependantResources; }
            set { _DependantResources = value; }
        }

        public string FullName
        {
            get { return _FullName; }
            set { _FullName = value; }
        }

        public List<string> References
        {
            get { return _References; }
            set { _References = value; }
        }

        public List<ResourceProperty> Properties
        {
            get { return _Properties; }
            set { _Properties = value; }
        }

        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }

        #region IComparable<ApplicationResource> Members
        /// <summary>
        /// Compares this object to another resource
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ApplicationResource other)
        {
            if (DependsOn(this, other))
                return 1;
            else if (DependsOn(other, this))
                return -1;
            else
                return 0;
        }

        #endregion
        /// <summary>
        /// To String
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.FullName;
        }
        /// <summary>
        /// Checks if this object depends on another
        /// </summary>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool DependsOn(ApplicationResource y)
        {
            return DependsOn(this, y);
        }
        /// <summary>
        /// Recurses through the dependancies to find if one appembly depends on another in its reference tree
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool DependsOn(ApplicationResource x, ApplicationResource y)
        {
            foreach (ApplicationResource dependant in x.DependantResources)
            {
                if (x.DependantResources.Contains(y))
                    return true;
                else
                    return DependsOn(dependant, y);
            }

            return false;
        }
    }
}