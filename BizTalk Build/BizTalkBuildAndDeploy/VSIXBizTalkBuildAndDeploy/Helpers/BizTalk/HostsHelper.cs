using System;
using System.Globalization;
using System.Management;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk
{
    /// <summary>
    /// Provides helper classes for managing hosts
    /// </summary>
    public class HostsHelper
    {
        /// <summary>
        /// Gets a server host management class
        /// </summary>
        /// <returns></returns>
        public static ManagementClass GetServerHostClass()
        {
            const string ClassPath = "MSBTS_ServerHost";
            return GetManagementClass(ClassPath);
        }
        /// <summary>
        /// Gets a host settings management class
        /// </summary>
        /// <returns></returns>
        public static ManagementClass GetHostSettingClass()
        {
            const string ClassPath = "MSBTS_HostSetting";
            return GetManagementClass(ClassPath);
        }
        /// <summary>
        /// Gets a host instance management class
        /// </summary>
        /// <returns></returns>
        public static ManagementClass GetHostInstanceClass()
        {
            const string ClassPath = "MSBTS_HostInstance";
            return GetManagementClass(ClassPath);
        }
        /// <summary>
        /// Gets a management class for working with BizTalk management objects
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ManagementClass GetManagementClass(string path)
        {
            ManagementScope managementScope = GetManagementScope();
            ManagementPath managementPath = new ManagementPath(path);
            ManagementClass managementClass = new ManagementClass(managementScope, managementPath, null);
            managementClass.Get();

            return managementClass;
        }
        /// <summary>
        /// Gets a management scope for BizTalk
        /// </summary>
        /// <returns></returns>
        private static ManagementScope GetManagementScope()
        {
            const string ScopePath = @"root\MicrosoftBizTalkServer";

            ManagementScope managementScope = new ManagementScope(ScopePath);
            managementScope.Options.EnablePrivileges = true;
            managementScope.Options.Impersonation = ImpersonationLevel.Impersonate;
            managementScope.Connect();

            return managementScope;
        }
        /// <summary>
        /// Gets the management object for managing the host settings
        /// </summary>
        /// <param name="hostSettingName"></param>
        /// <returns></returns>
        public static ManagementObject GetHostSettingsObject(string hostSettingName)
        {
            ManagementScope scope = GetManagementScope();
            ObjectQuery query = new ObjectQuery(string.Format(CultureInfo.InvariantCulture, "select * from {0} where name = '{1}'", new object[] { "MSBTS_HostSetting", hostSettingName }));
            ManagementObjectCollection objects = new ManagementObjectSearcher(scope, query, null).Get();
            if (objects.Count == 0)
                throw new ArgumentException("The host setting does not exist or could not be found.", "hostName");

            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objects.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    return (ManagementObject)enumerator.Current;
                }
            }
            return null;
        }
        /// <summary>
        /// Returns a management object for the host
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static ManagementObject GetHostObject(string hostName)
        {
            ManagementScope scope = GetManagementScope();
            ObjectQuery query = new ObjectQuery(string.Format(CultureInfo.InvariantCulture, "select * from {0} where name = '{1}'", new object[] { "MSBTS_Host", hostName }));
            ManagementObjectCollection objects = new ManagementObjectSearcher(scope, query, null).Get();
            if (objects.Count == 0)
            {
                throw new ArgumentException("The host does not exist or could not be found.", "hostName");
            }
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = objects.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    return (ManagementObject)enumerator.Current;
                }
            }
            return null;
        }
        /// <summary>
        /// Checks if the host exists
        /// </summary>
        /// <param name="hostName"></param>
        /// <returns></returns>
        public static bool Exists(string hostName)
        {
            ManagementScope scope = GetManagementScope();
            ObjectQuery query = new ObjectQuery(string.Format(CultureInfo.InvariantCulture, "select * from {0} where name = '{1}'", new object[] { "MSBTS_Host", hostName }));
            ManagementObjectCollection objects = new ManagementObjectSearcher(scope, query, null).Get();
            if (objects.Count == 0)
                return false;
            else
                return true;
        }


    }
}
