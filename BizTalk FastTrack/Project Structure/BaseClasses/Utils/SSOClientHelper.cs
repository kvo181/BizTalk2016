using System;
using System.Collections.Specialized;
using Microsoft.BizTalk.CAT.BestPractices.Framework.Instrumentation;
using Microsoft.BizTalk.SSOClient.Interop;

namespace bizilante.BaseClasses
{
    /// <summary>
    /// Configuration PropertyBag
    /// </summary>
    public class ConfigurationPropertyBag : IPropertyBag
    {
        private HybridDictionary properties;
        internal ConfigurationPropertyBag()
        {
            properties = new HybridDictionary();
        }

        /// <summary>
        /// Read properties
        /// </summary>
        /// <param name="propName">Properie Name</param>
        /// <param name="ptrVar">PrtVar</param>
        /// <param name="errLog">ErrLog</param>
        public void Read(string propName, out object ptrVar,int errLog)
        {
            ptrVar = properties[propName];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propName"></param>
        /// <param name="ptrVar"></param>
        public void Write(string propName, ref object ptrVar)
        {
            properties.Add(propName, ptrVar);
        }

        /// <summary>
        /// Check if key exists in properties
        /// </summary>
        /// <param name="key">Key to search</param>
        /// <returns>True OR False</returns>
        public bool Contains(string key)
        {
            return properties.Contains(key);
        }

        /// <summary>
        /// Remove Key from properties
        /// </summary>
        /// <param name="key">Key to remove</param>
        public void Remove(string key)
        {
            properties.Remove(key);
        }
    }

    /// <summary>
    /// Helper to read out the SSO Configuration
    /// </summary>
    public static class SSOClientHelper
    {
        private const string IdenifierGuid = "ConfigProperties";

        private const string EnvAppName = "BizTalk.Environment";
        private const string EnvPropName = "Environment";

        /// <summary>
        /// Read method helps get configuration data (based on the current environment value)
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to read</param>
        /// <returns>
        ///  The value of the property stored in the given affiliate application of this component.
        /// </returns>
        public static string Read(string appName, string propName)
        {
            Guid callToken = TraceManager.CustomComponent.TraceIn(appName, propName);
            object propertyValue = null;
            string propNameToRead = string.Format("{0}_{1}", Environment, propName);
            try
            {
                SSOConfigStore ssoStore = new SSOConfigStore();
                ConfigurationPropertyBag appMgmtBag = new ConfigurationPropertyBag();
                ((ISSOConfigStore) ssoStore).GetConfigInfo(appName, IdenifierGuid, SSOFlag.SSO_FLAG_RUNTIME,
                                                           (IPropertyBag) appMgmtBag);
                appMgmtBag.Read(propNameToRead, out propertyValue, 0);
                // In case the result is null, we try to read the property without pre-pending the environment
                if (null == propertyValue)
                    return ReadInternal(appName, propName);
            }
            catch (Exception e)
            {
                TraceManager.CustomComponent.TraceInfo("Failed to read '{0}' for application '{1}'", propNameToRead,
                                                       appName);
                TraceManager.CustomComponent.TraceError(e);
                throw;
            }
            finally
            {
                TraceManager.CustomComponent.TraceOut(callToken, propertyValue);
            }
            return (string)propertyValue;
        }


        /// <summary>
        /// Read method helps get configuration data (wihtout taking environment into account)
        /// </summary>        
        /// <param name="appName">The name of the affiliate application to represent the configuration container to access</param>
        /// <param name="propName">The property name to read</param>
        /// <returns>
        ///  The value of the property stored in the given affiliate application of this component.
        /// </returns>
        private static string ReadInternal(string appName, string propName)
        {
            try
            {
                SSOConfigStore ssoStore = new SSOConfigStore();
                ConfigurationPropertyBag appMgmtBag = new ConfigurationPropertyBag();
                ((ISSOConfigStore)ssoStore).GetConfigInfo(appName, IdenifierGuid, SSOFlag.SSO_FLAG_RUNTIME, (IPropertyBag)appMgmtBag);
                object propertyValue = null;
                appMgmtBag.Read(propName, out propertyValue, 0);
                return (string)propertyValue;
            }
            catch (Exception e)
            {
                TraceManager.CustomComponent.TraceInfo("Failed to read '{0}' for application '{1}'", propName, appName);
                TraceManager.CustomComponent.TraceError(e);
                throw;
            }
        }

        private static string _environment;
        /// <summary>
        /// Environment value to use when reading SSO properties
        /// </summary>
        public static string Environment
        {
            get
            {
                if (string.IsNullOrEmpty(_environment))
                    _environment = ReadInternal(EnvAppName, EnvPropName);
                return _environment;
            }
        }
    }
}
