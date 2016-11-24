using Microsoft.EnterpriseSingleSignOn.Interop;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Xml;

namespace bizilante.SSO.Helper
{
    public class SSO
    {
        private static string CONFIG_NAME = "ConfigProperties";
        private int _timeout = 60; // seconds

        private string _secrectServer = "";
        private string _ssoDBServer = "";
        private string _ssoDB = "";
        private string _affiliateAppMgrGroup = "";
        private string _ssoAdminGroup = "";
        private readonly string _bizTalkEmailAddress = "BizTalkAdmin@{0}.com";

        public EventHandler<SSOEventArgs> SsoEvent;

        public string SSOAdminGroup
        {
            get
            {
                return _ssoAdminGroup;
            }
            set
            {
                _ssoAdminGroup = value;
            }
        }

        public string AffiliateAppMgrGroup
        {
            get
            {
                return _affiliateAppMgrGroup;
            }
            set
            {
                _affiliateAppMgrGroup = value;
            }
        }

        public string DBServer
        {
            get { return _ssoDBServer; }
            set { _ssoDBServer = value; }
        }
        public string DB
        {
            get { return _ssoDB; }
            set { _ssoDB = value; }
        }

        public SSO()
        {
            _bizTalkEmailAddress = 
                string.Format(_bizTalkEmailAddress, ConfigurationManager.AppSettings["CompanyName"]);
        }
        public SSO(string companyName)
        {
            _bizTalkEmailAddress =
                string.Format(_bizTalkEmailAddress, companyName);
        }
        public SSO(int timeout)
        {
            _timeout = timeout;
            _bizTalkEmailAddress =
                string.Format(_bizTalkEmailAddress, ConfigurationManager.AppSettings["CompanyName"]);
        }

        public static string Encrypt(string toEncrypt, string key)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(toEncrypt);
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key2 = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            ICryptoTransform cryptoTransform = new TripleDESCryptoServiceProvider
            {
                Key = key2,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            }.CreateEncryptor();
            byte[] array = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
            return Convert.ToBase64String(array, 0, array.Length);
        }

        public static string Decrypt(string toDecrypt, string key)
        {
            byte[] array = Convert.FromBase64String(toDecrypt);
            MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
            byte[] key2 = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
            ICryptoTransform cryptoTransform = new TripleDESCryptoServiceProvider
            {
                Key = key2,
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            }.CreateDecryptor();
            byte[] bytes = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        private void CreateApplicationFields(string appName, ISSOAdmin admin, string[] arrKeys)
        {
            try
            {
                int flags = 536870912;
                int num = arrKeys.Length;
                admin.CreateFieldInfo(appName, _bizTalkEmailAddress, flags);
                for (int i = 0; i < num; i++)
                {
                    admin.CreateFieldInfo(appName, arrKeys[i].ToString(), flags);
                    DoSsoEvent("CreateApplicationFields", string.Format("{0}", arrKeys[i]), false);
                }
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - CreateApplicationFields", ex.Message, true);
            }
        }

        private void Enlist(object obj, Transaction tx)
        {
            try
            {
                IPropertyBag propertyBag = (IPropertyBag)obj;
                object dtcTransaction = TransactionInterop.GetDtcTransaction(tx);
                ISSOAdmin2 iSSOAdmin = (ISSOAdmin2)new SSOAdmin();
                int flags;
                int auditAppDeleteMax;
                int auditMappingDeleteMax;
                int auditNtpLookupMax;
                int auditXpLookupMax;
                int ticketTimeout;
                int credCacheTimeout;
                iSSOAdmin.GetGlobalInfo(out flags, out auditAppDeleteMax, out auditMappingDeleteMax, out auditNtpLookupMax, out auditXpLookupMax, out ticketTimeout, out credCacheTimeout, out _secrectServer, out _ssoAdminGroup, out _affiliateAppMgrGroup);
                object secrectServer = _secrectServer;
                propertyBag.Write("CurrentSSOServer", ref secrectServer);
                propertyBag.Write("Transaction", ref dtcTransaction);
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - Enlist", ex.Message, true);
            }
        }

        private void EnableApplication(string appName, ISSOAdmin admin)
        {
            try
            {
                int num = 2;
                admin.UpdateApplication(appName, null, null, null, null, num, num);
                DoSsoEvent("EnableApplication", string.Format("{0} enabled", appName), false);
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - EnableApplication", ex.Message, true);
            }
        }

        public string[] GetApplications()
        {
            string[] array = new string[10];
            if (_secrectServer == null || _secrectServer == "")
            {
                GetSecretServerName();
            }
            string[] result;
            try
            {
                string commandText = string.Format("Select ai_app_name from SSOX_ApplicationInfo where ai_contact_info='{0}'", _bizTalkEmailAddress);
                SqlConnection sqlConnection = new SqlConnection();
                sqlConnection.ConnectionString = string.Concat(new string[]
                {
                    "Data Source=",
                    _ssoDBServer,
                    "; Initial Catalog=",
                    _ssoDB,
                    "; Integrated Security=SSPI"
                });
                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(new SqlCommand
                {
                    Connection = sqlConnection,
                    CommandText = commandText
                });
                DataSet dataSet = new DataSet();
                sqlDataAdapter.Fill(dataSet, "Result");
                int count = dataSet.Tables[0].Rows.Count;
                array = new string[count];
                for (int i = 0; i < count; i++)
                {
                    array[i] = dataSet.Tables[0].Rows[i][0].ToString();
                }
                sqlConnection.Close();
                result = array;
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - GetApplications", ex.Message, true);
                string[] array2 = new string[10];
                result = array2;
            }
            return result;
        }

        public void GetSecretServerName()
        {
            try
            {
                ISSOAdmin2 iSSOAdmin = (ISSOAdmin2)new SSOAdmin();
                int flags;
                int auditAppDeleteMax;
                int auditMappingDeleteMax;
                int auditNtpLookupMax;
                int auditXpLookupMax;
                int ticketTimeout;
                int credCacheTimeout;
                iSSOAdmin.GetGlobalInfo(out flags, out auditAppDeleteMax, out auditMappingDeleteMax, out auditNtpLookupMax, out auditXpLookupMax, out ticketTimeout, out credCacheTimeout, out _secrectServer, out _ssoAdminGroup, out _affiliateAppMgrGroup);
                _ssoDBServer = (Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ENTSSO\\SQL", "Server", "") as string);
                _ssoDB = (Registry.GetValue("HKEY_LOCAL_MACHINE\\Software\\Microsoft\\ENTSSO\\SQL", "Database", "") as string);
                DoSsoEvent("GetSecretServerName", string.Format("Server={0}, Database={1}", _ssoDBServer, _ssoDB), false);
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - GetSecretServerName", ex.Message, true);
            }
        }

        public void CreateApplication(string name, string[] arrKeys)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    int numFields = arrKeys.Length;
                    int flags = 1310720;
                    ISSOAdmin iSSOAdmin = (ISSOAdmin)new SSOAdmin();
                    Enlist(iSSOAdmin, Transaction.Current);
                    iSSOAdmin.CreateApplication(name, name + " Configuration Data", _bizTalkEmailAddress, _affiliateAppMgrGroup, _ssoAdminGroup, flags, numFields);
                    DoSsoEvent("CreateApplication", string.Format("{0}", name), false);
                    CreateApplicationFields(name, iSSOAdmin, arrKeys);
                    EnableApplication(name, iSSOAdmin);
                    transactionScope.Complete();
                }
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - CreateApplication", ex.Message, true);
            }
        }

        public void CreateApplicationFieldsValues(string name, string[] arrKeys, string[] arrValues)
        {
            try
            {
                DeleteApplication(name);
                CreateApplication(name, arrKeys);
                SaveApplicationData(name, arrKeys, arrValues);
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - CreateApplicationFieldsValues", ex.Message, true);
            }
        }

        public void SaveApplicationData(string appName, string[] arrKeys, string[] arrValues)
        {
            try
            {
                int num = arrKeys.Length;
                SSOPropertyBag sSOPropertyBag = new SSOPropertyBag();
                for (int i = 0; i < num; i++)
                {
                    sSOPropertyBag.SetValue<string>(arrKeys[i], arrValues[i]);
                    DoSsoEvent("SaveApplicationData", string.Format("Key={0}, Value={1}", arrKeys[i], arrValues[i]), false);
                }
                ISSOConfigStore iSSOConfigStore = (ISSOConfigStore)new SSOConfigStore();
                iSSOConfigStore.SetConfigInfo(appName, SSO.CONFIG_NAME, sSOPropertyBag);
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - SaveApplicationData", ex.Message, true);
            }
        }

        public void DeleteApplication(string appName)
        {
            try
            {
                using (TransactionScope transactionScope = new TransactionScope())
                {
                    ISSOAdmin iSSOAdmin = (ISSOAdmin)new SSOAdmin();
                    Enlist(iSSOAdmin, Transaction.Current);
                    iSSOAdmin.DeleteApplication(appName);
                    transactionScope.Complete();
                    DoSsoEvent("DeleteApplication", string.Format("{0}", appName), false);
                }
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - DeleteApplication", ex.Message, true);
            }
        }

        public string[] GetKeys(string appName)
        {
            string[] result;
            try
            {
                SSOPropertyBag sSOPropertyBag = new SSOPropertyBag();
                ISSOConfigStore iSSOConfigStore = (ISSOConfigStore)new SSOConfigStore();
                iSSOConfigStore.GetConfigInfo(appName, SSO.CONFIG_NAME, 4, sSOPropertyBag);
                string[] array = new string[sSOPropertyBag.Dictionary.Count];
                int num = 0;
                foreach (KeyValuePair<string, object> current in sSOPropertyBag.Dictionary)
                {
                    array[num] = current.Key.ToString();
                    num++;
                }
                result = array;
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - GetKeys", ex.Message, true);
                result = new string[]
                {
                    "ERROR: " + ex.Message
                };
            }
            return result;
        }

        public string[] GetValues(string appName)
        {
            string[] result;
            try
            {
                SSOPropertyBag sSOPropertyBag = new SSOPropertyBag();
                ISSOConfigStore iSSOConfigStore = (ISSOConfigStore)new SSOConfigStore();
                iSSOConfigStore.GetConfigInfo(appName, SSO.CONFIG_NAME, 4, sSOPropertyBag);
                string[] array = new string[sSOPropertyBag.Dictionary.Count];
                int num = 0;
                foreach (KeyValuePair<string, object> current in sSOPropertyBag.Dictionary)
                {
                    array[num] = current.Value.ToString();
                    num++;
                }
                result = array;
            }
            catch (Exception ex)
            {
                DoSsoEvent("SSO Helper - GetValues", ex.Message, true);
                result = new string[]
                {
                    ""
                };
            }
            return result;
        }

        public bool ImportSSOApplication(string encryptionKey, string appName, string encryptedText)
        {
            bool flag = true;
            XmlDocument xmlDocument = new XmlDocument();
            string[] applications = GetApplications();
            for (int i = 0; i < applications.Length; i++)
            {
                if (applications[i].ToUpper() == appName.ToUpper())
                {
                    flag = false;
                }
            }
            byte[] bytes;
            try
            {
                bytes = Encoding.ASCII.GetBytes(SSO.Decrypt(encryptedText, encryptionKey));
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry("SSO Helper - ImportSSOApplication", "Error decrypting sso extract: \r\n" + ex.Message);
                bool result = false;
                return result;
            }
            MemoryStream memoryStream = new MemoryStream(bytes);
            try
            {
                xmlDocument.Load(memoryStream);
            }
            catch (Exception ex2)
            {
                EventLog.WriteEntry("SSO Helper - ImportSSOApplication", "Error loading decrypted memorystream: \r\n" + ex2.Message);
                bool result = false;
                return result;
            }
            finally
            {
                memoryStream.Dispose();
            }
            XmlElement documentElement = xmlDocument.DocumentElement;
            XmlNodeList xmlNodeList = documentElement.SelectNodes("applicationData/add");
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            if (!flag)
            {
                list.AddRange(GetKeys(appName));
                list2.AddRange(GetValues(appName));
            }
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                string value = xmlNode.SelectSingleNode("@key").Value;
                string value2 = xmlNode.SelectSingleNode("@value").Value;
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && !list.Contains(value))
                {
                    list.Add(value);
                    list2.Add(value2);
                }
            }
            DoSsoEvent("ImportSSOApplication", string.Format("{0}", appName), false);
            CreateApplicationFieldsValues(appName, list.ToArray(), list2.ToArray());
            return true;
        }
        public bool ImportSSOApplication(string appName, string filename)
        {
            bool flag = true;
            string[] applications = GetApplications();
            for (int i = 0; i < applications.Length; i++)
            {
                if (applications[i].ToUpper() == appName.ToUpper())
                {
                    flag = false;
                }
            }
            XmlDocument xmlDocument = new XmlDocument();
            try
            {
                xmlDocument.Load(filename);
            }
            catch (Exception ex2)
            {
                EventLog.WriteEntry("SSO Helper - ImportSSOApplication", string.Format("Error loading file: {0}\r\n", filename) + ex2.Message);
                bool result = false;
                return result;
            }
            finally
            {
            }
            XmlElement documentElement = xmlDocument.DocumentElement;
            XmlNodeList xmlNodeList = documentElement.SelectNodes("applicationData/add");
            List<string> list = new List<string>();
            List<string> list2 = new List<string>();
            if (!flag)
            {
                list.AddRange(GetKeys(appName));
                list2.AddRange(GetValues(appName));
            }
            foreach (XmlNode xmlNode in xmlNodeList)
            {
                string value = xmlNode.SelectSingleNode("@key").Value;
                string value2 = xmlNode.SelectSingleNode("@value").Value;
                if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2) && !list.Contains(value))
                {
                    list.Add(value);
                    list2.Add(value2);
                }
            }
            DoSsoEvent("ImportSSOApplication", string.Format("{0}", appName), false);
            CreateApplicationFieldsValues(appName, list.ToArray(), list2.ToArray());
            return true;
        }

        private void DoSsoEvent(string source, string message, bool isError)
        {
            if (null == SsoEvent)
            {
                EventLog.WriteEntry(source, message);
                return;
            }

            SSOEventArgs args = new SSOEventArgs(source, message, isError);
            SsoEvent(this, args);
        }
    }
}
