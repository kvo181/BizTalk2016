using System;
using System.Collections.Specialized;
using Microsoft.BizTalk.SSOClient.Interop;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Config PropertyBag for config coming from SSO
    /// </summary>
    internal class ConfigurationPropertyBag : IPropertyBag
    {
        private readonly HybridDictionary _properties;
        internal ConfigurationPropertyBag()
        {
            _properties = new HybridDictionary();
        }
        public void Read(string propName, out object ptrVar, int errLog)
        {
            ptrVar = _properties[propName];
        }
        public void Write(string propName, ref object ptrVar)
        {
            _properties.Add(propName, ptrVar);
        }
        public bool Contains(string key)
        {
            return _properties.Contains(key);
        }
        public void Remove(string key)
        {
            _properties.Remove(key);
        }
    }

    /// <summary>
    /// Contains access to SSO Configuration
    /// </summary>
    public static class SSOConfiguration
    {
        public static string IdenifierGuid = "ConfigProperties";
        /// <summary>
        /// Read method helps get configuration data
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to read</param>
        /// <returns>
        ///  The value of the property stored in the given affiliate application of this component.
        /// </returns>
        public static string Read(string appName, string propName)
        {
            try
            {
                var ssoStore = new SSOConfigStore();
                var appMgmtBag = new ConfigurationPropertyBag();
                ((ISSOConfigStore)ssoStore).GetConfigInfo(appName, IdenifierGuid, SSOFlag.SSO_FLAG_RUNTIME, appMgmtBag);
                object propertyValue;
                appMgmtBag.Read(propName, out propertyValue, 0);
                return (string)propertyValue;
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }
        /// <summary>
        /// Write method helps write configuration data
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to write</param>
        /// <param name="propValue">The property value to write</param>
        public static void Write(string appName, string propName, string propValue)
        {
            try
            {
                var ssoStore = new SSOConfigStore();
                var appMgmtBag = new ConfigurationPropertyBag();
                ((ISSOConfigStore)ssoStore).GetConfigInfo(appName, IdenifierGuid, SSOFlag.SSO_FLAG_RUNTIME, appMgmtBag);
                object tempProp = propValue;               
                appMgmtBag.Remove(propName);
                                
                appMgmtBag.Write(propName, ref tempProp);
                ((ISSOConfigStore)ssoStore).SetConfigInfo(appName, IdenifierGuid, appMgmtBag);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }
        /// <summary>
        /// Write method helps write configuration data
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="properties"></param>
        internal static void Write(string appName, ConfigurationPropertyBag properties)
        {
            try
            {
                var ssoStore = new SSOConfigStore();                                                                
                ((ISSOConfigStore)ssoStore).SetConfigInfo(appName, IdenifierGuid, properties);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message);
                throw;
            }
        }
    }
}

