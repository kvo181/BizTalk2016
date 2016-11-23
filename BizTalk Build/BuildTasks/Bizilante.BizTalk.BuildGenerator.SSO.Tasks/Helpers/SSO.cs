using System;
using Microsoft.Build.Utilities;

namespace bizilante.BuildGenerator.SSO.Tasks.Helpers
{
    internal class SSO
    {
        private readonly TaskLoggingHelper _log;
        private readonly bizilante.SSO.Helper.SSO _sso;
        private readonly string _companyName;
        public SSO(TaskLoggingHelper log, string companyName)
        {
            _log = log;
            _companyName = companyName;
            _sso = new bizilante.SSO.Helper.SSO(companyName);
        }

        public string[] GetApplications()
        {
            try
            {
                return _sso.GetApplications();
            }
            catch (Exception exception)
            {
                _log.LogError("SSOConfigurationImportMSBuildTask - GetApplications failed : {0}", exception.Message);
                throw;
            }
        }
        public string[] GetKeys(string appName)
        {
            try
            {
                return _sso.GetKeys(appName);
            }
            catch (Exception exception)
            {
                _log.LogError("SSOConfigurationImportMSBuildTask - GetKeys {0} failed: {1}", appName, exception.Message);
                throw;
            }
        }

        public string[] GetValues(string appName)
        {
            try
            {
                return _sso.GetValues(appName);
            }
            catch (Exception exception)
            {
                _log.LogError("SSOConfigurationImportMSBuildTask - GetValues {0} failed: {1}", appName, exception.Message);
                throw;
            }
        }
        public void CreateApplicationFieldsValues(string name, string[] arrKeys, string[] arrValues)
        {
            try
            {
                _sso.CreateApplicationFieldsValues(name, arrKeys, arrValues);
            }
            catch (Exception exception)
            {
                _log.LogError("SSOConfigurationImportMSBuildTask - CreateApplicationFieldsValues {0} failed: {1}", name, exception.Message);
                throw;
            }
        }

    }
}

