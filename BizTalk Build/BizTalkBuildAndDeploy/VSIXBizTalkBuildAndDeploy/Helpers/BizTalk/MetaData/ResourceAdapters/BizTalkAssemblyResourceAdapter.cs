using System;
using System.Text;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk.MetaDataBuildGenerator.MetaData.ResourceAdapters
{
    public class BizTalkAssemblyResourceAdapter : BaseResourceAdapter
    {
        private const string UpdateGacPropertyName = "UpdateGac";
        private const string GacutilPropertyName = "Gacutil";
        private const string UpdateGacOnImportPropertyName = "UpdateGacOnImport";
        private const string SourceLocationPropertyName = "SourceLocation";
        private const string DestinationLocationPropertyName = "DestinationLocation";
        private const string FullNamePropertyName = "FullName";

        public static BizTalkAssemblyResourceAdapter Create(ApplicationResource resource)
        {
            if (resource.Type != ResourceTypes.BizTalkAssembly)
                throw new ApplicationException("Invalid resource type");

            return new BizTalkAssemblyResourceAdapter(resource);
        }

        private BizTalkAssemblyResourceAdapter(ApplicationResource resource)
            : base(resource)
        {
        }

        /// <summary>
        /// Gets the name of the assembly by parsing the source location
        /// </summary>
        public string Name
        {
            get
            {
                string[] sourceLocationParts = SourceLocation.Split(char.Parse(@"\"));
                return sourceLocationParts[sourceLocationParts.GetUpperBound(0)];
            }
        }

        /// <summary>
        /// Gets the name without the extension, eg if name = MyAssembly.dll then this will be MyAssembly
        /// </summary>
        public string AssemblyNameWithoutExtension
        {
            get { return Name.Substring(0, Name.Length - 4); }
        }

        public string Options
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                if (UpdateGac)
                    sb.Append("GacOnAdd,");
                if (UpdateGacOnImport)
                    sb.Append("GacOnImport,");
                if (GacUtil)
                    sb.Append("GacOnInstall,");

                if (sb.Length > 0)
                    sb.Remove(sb.Length - 1, 1); //Remove the last comma

                return sb.ToString();
            }
        }

        public bool UpdateGac
        {
            get { return Convert.ToBoolean(base.FindPropertyValue(UpdateGacPropertyName)); }
        }

        public bool GacUtil
        {
            get { return Convert.ToBoolean(base.FindPropertyValue(GacutilPropertyName)); }
        }

        public bool UpdateGacOnImport
        {
            get { return Convert.ToBoolean(base.FindPropertyValue(UpdateGacOnImportPropertyName)); }
        }

        public string SourceLocation
        {
            get { return (string) base.FindPropertyValue(SourceLocationPropertyName); }
        }

        public string DestinationLocation
        {
            get { return (string) base.FindPropertyValue(DestinationLocationPropertyName); }
        }
    }
}