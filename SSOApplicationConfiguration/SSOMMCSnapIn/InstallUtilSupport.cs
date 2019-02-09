using Microsoft.ManagementConsole;
using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;

namespace bizilante.ManagementConsole.SSO
{
    [RunInstaller(true)]
    public class InstallUtilSupport : SnapInInstaller
    {
        private const string CompanyNameMSIContextParm = "companyname";
        private const string TargetDirectoryMSIContextParm = "targetdir";
        private const string CompanyNameAppSettingKey = "CompanyName";
        private const string SnapInDllBasePath = "{0}bizilante.SSOMMCSnapIn.dll";

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            //System.Diagnostics.Debugger.Launch();
            string targetdir = Context.Parameters[TargetDirectoryMSIContextParm];
            string companyname = Context.Parameters[CompanyNameMSIContextParm];
            if (string.IsNullOrWhiteSpace(companyname))
            {
                throw new InvalidOperationException("Company Name is a required field.");
            }
            string exePath = string.Format(SnapInDllBasePath, targetdir);
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(exePath);
            KeyValueConfigurationElement companyNameConfigurationElement =
                configuration.AppSettings.Settings[CompanyNameAppSettingKey];
            if (null == companyNameConfigurationElement)
                configuration.AppSettings.Settings.Add(CompanyNameAppSettingKey, companyname);
            else
                configuration.AppSettings.Settings[CompanyNameAppSettingKey].Value = companyname;
            configuration.Save();
        }
    }
}
