using System;
using System.Management;
using System.Globalization;

namespace BizTalk.BuildGenerator.Tasks
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
            const string classPath = "MSBTS_ServerHost";
            return GetManagementClass(classPath);
        }
        /// <summary>
        /// Gets a host settings management class
        /// </summary>
        /// <returns></returns>
        public static ManagementClass GetHostSettingClass()
        {
            const string classPath = "MSBTS_HostSetting";
            return GetManagementClass(classPath);
        }
        /// <summary>
        /// Gets a host instance management class
        /// </summary>
        /// <returns></returns>
        public static ManagementClass GetHostInstanceClass()
        {
            const string classPath = "MSBTS_HostInstance";
            return GetManagementClass(classPath);
        }
        /// <summary>
        /// Gets a management class for working with BizTalk management objects
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ManagementClass GetManagementClass(string path)
        {
            var managementScope = GetManagementScope();
            var managementPath = new ManagementPath(path);
            var managementClass = new ManagementClass(managementScope, managementPath, null);
            managementClass.Get();

            return managementClass;
        }
        /// <summary>
        /// Gets a management scope for BizTalk
        /// </summary>
        /// <returns></returns>
        private static ManagementScope GetManagementScope()
        {
            const string scopePath = @"root\MicrosoftBizTalkServer";

            var managementScope = new ManagementScope(scopePath)
                                      {
                                          Options =
                                              {
                                                  EnablePrivileges = true,
                                                  Impersonation = ImpersonationLevel.Impersonate
                                              }
                                      };
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
            var scope = GetManagementScope();
            var query = new ObjectQuery(string.Format(CultureInfo.InvariantCulture, "select * from {0} where name = '{1}'", new object[] { "MSBTS_HostSetting", hostSettingName }));
            var objects = new ManagementObjectSearcher(scope, query, null).Get();
            if (objects.Count == 0)
                throw new ArgumentException("The host setting does not exist or could not be found.", "hostSettingName");

            using (var enumerator = objects.GetEnumerator())
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
            var scope = GetManagementScope();
            var query = new ObjectQuery(string.Format(CultureInfo.InvariantCulture, "select * from {0} where name = '{1}'", new object[] { "MSBTS_Host", hostName }));
            var objects = new ManagementObjectSearcher(scope, query, null).Get();
            if (objects.Count == 0)
            {
                throw new ArgumentException("The host does not exist or could not be found.", "hostName");
            }
            using (var enumerator = objects.GetEnumerator())
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
            var scope = GetManagementScope();
            var query = new ObjectQuery(string.Format(CultureInfo.InvariantCulture, "select * from {0} where name = '{1}'", new object[] { "MSBTS_Host", hostName }));
            var objects = new ManagementObjectSearcher(scope, query, null).Get();
            return objects.Count != 0;
        }


    }
}
