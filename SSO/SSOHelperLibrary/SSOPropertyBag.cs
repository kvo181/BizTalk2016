using Microsoft.EnterpriseSingleSignOn.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace bizilante.SSO.Helper
{
    internal class SSOPropertyBag : IPropertyBag
    {
        private Dictionary<string, object> _dictionary = new Dictionary<string, object>();

        public Dictionary<string, object> Dictionary
        {
            get
            {
                return this._dictionary;
            }
            set
            {
                this._dictionary = value;
            }
        }

        public T GetValue<T>(string propName)
        {
            T result;
            try
            {
                if (this._dictionary.ContainsKey(propName))
                {
                    result = (T)((object)this._dictionary[propName]);
                }
                else
                {
                    result = default(T);
                }
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SSO MMC Snap-In - PropertyBag - GetValue", ex.Message);
                result = default(T);
            }
            return result;
        }

        public void SetValue<T>(string propName, T value)
        {
            try
            {
                this._dictionary[propName] = value;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SSO MMC Snap-In - PropertyBag - SetValue", ex.Message);
            }
        }

        void IPropertyBag.Read(string propName, out object ptrVar, int errorLog)
        {
            ptrVar = null;
            if (this._dictionary.ContainsKey(propName))
            {
                ptrVar = this._dictionary[propName];
            }
        }

        void IPropertyBag.Write(string propName, ref object ptrVar)
        {
            try
            {
                this._dictionary[propName] = ptrVar;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SSO MMC Snap-In - PropertyBag - Write", ex.Message);
            }
        }
    }
}
