using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace bizilante.BuildGenerator.SSO.Tasks
{
    public class Import : Task
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string NonEncryptedFile { get; set; }

        public override bool Execute()
        {
            Exception exception;
            bool flag = true;
            XmlDocument document = new XmlDocument();
            string appName = string.Empty;
            FileInfo info = new FileInfo(this.NonEncryptedFile);
            if (!info.Exists)
            {
                base.Log.LogError("Could not find the specified input file", new object[] { this.NonEncryptedFile });
                return false;
            }
            try
            {
                appName = Path.GetFileNameWithoutExtension(this.NonEncryptedFile);
                Helpers.SSO sso = new Helpers.SSO(Log, this.CompanyName);
                string[] applications = sso.GetApplications();
                for (int i = 0; i < applications.Length; i++)
                {
                    if (applications[i].ToUpper() == appName.ToUpper())
                    {
                        flag = false;
                    }
                }
                try
                {
                    document.Load(info.FullName);
                }
                catch (Exception exception2)
                {
                    exception = exception2;
                    base.Log.LogErrorFromException(exception, true);
                    return false;
                }
                finally
                {
                }
                XmlNodeList list = document.DocumentElement.SelectNodes("applicationData/add");
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                if (!flag)
                {
                    list2.AddRange(sso.GetKeys(appName));
                    list3.AddRange(sso.GetValues(appName));
                }
                foreach (XmlNode node in list)
                {
                    string str3 = node.SelectSingleNode("@key").Value;
                    string str4 = node.SelectSingleNode("@value").Value;
                    if ((!string.IsNullOrEmpty(str3) && !string.IsNullOrEmpty(str4)) && !list2.Contains(str3))
                    {
                        list2.Add(str3);
                        list3.Add(str4);
                    }
                }
                sso.CreateApplicationFieldsValues(appName, list2.ToArray(), list3.ToArray());
                Log.LogMessage(MessageImportance.Normal, "Import SSO application with name: '{0}'", appName);
                for (int i = 0; i < list2.Count; i++)
                {
                    Log.LogMessage(MessageImportance.Normal, "key:'{0}', value:'{1}'", list2[i], list3[i]);
                }
            }
            catch (Exception exception3)
            {
                exception = exception3;
                base.Log.LogErrorFromException(exception, true);
                return false;
            }
            return true;
        }
    }
}

