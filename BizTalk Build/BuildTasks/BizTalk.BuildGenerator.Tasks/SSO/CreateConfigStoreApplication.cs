using Microsoft.Build.Framework;
using Microsoft.EnterpriseSingleSignOn.Interop;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Creates a set of SSO applications based on the details of a file
    /// </summary>
    public class CreateConfigStoreApplication : BaseSSOTask
    {
        [Required]
        public string CommaDelimitedFieldNames { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string UserGroup { get; set; }

        [Required]
        public string AdministratorGroup { get; set; }

        [Required]
        public string ApplicationName { get; set; }


        /// <summary>
        /// execute task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            var fieldNames = CommaDelimitedFieldNames.Split(",".ToCharArray());
            var noFields = fieldNames.Length + 1;
            var appFlags = 0;

            //bitwise operation for flags
            appFlags |= SSOFlag.SSO_FLAG_APP_CONFIG_STORE;
            appFlags |= SSOFlag.SSO_FLAG_SSO_WINDOWS_TO_EXTERNAL;
            appFlags |= SSOFlag.SSO_FLAG_APP_ALLOW_LOCAL;

            var admin = new ISSOAdmin();            
            admin.CreateApplication(ApplicationName, Description, "", UserGroup, AdministratorGroup, appFlags, noFields);
                        
            const int fieldFlags = 0;            
            admin.CreateFieldInfo(ApplicationName, "Reserved", fieldFlags);
            foreach (var fieldName in fieldNames)
            {
                admin.CreateFieldInfo(ApplicationName, fieldName, fieldFlags);
            }

            admin.UpdateApplication(ApplicationName, null, null, null, null, SSOFlag.SSO_FLAG_ENABLED, SSOFlag.SSO_FLAG_ENABLED);

            var propertiesBag = new ConfigurationPropertyBag();
            foreach (var field in fieldNames)
            {
                object val = "<empty>";
                propertiesBag.Write(field, ref val);                
            }
            SSOConfiguration.Write(ApplicationName, propertiesBag);

            return true;
        }
    }
}
