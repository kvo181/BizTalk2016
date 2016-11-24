using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace bizilante.Deployment.Apps.SSO
{
    public class DeploySSO
    {
        // any method that takes a string as the parameter and returns no value
        public delegate void LogHandler(string message);

        // Define an Event based on the above Delegate
        public event LogHandler Log;

        // By Default, create an OnXXXX Method, to call the Event
        protected void OnLog(string message)
        {
            if (Log != null)
            {
                Log(message);
            }
        }

        public bool Execute(out string title)
        {
            title = string.Format("Deploy SSO file '{0}'", this.NonEncryptedFile);

            // Validate the file
            FileInfo info = new FileInfo(this.NonEncryptedFile);
            if (!info.Exists)
                throw new Exception(string.Format("Could not find the specified input file: '{0}'", new object[] { this.NonEncryptedFile }));

            bool newSSOApp = true;
            XmlDocument document = new XmlDocument();
            string appName = string.Empty;

            // Create the SSO object
            bizilante.SSO.Helper.SSO sso;
            if (!string.IsNullOrEmpty(Timeout))
                sso = new bizilante.SSO.Helper.SSO(int.Parse(Timeout));
            else
                sso = new bizilante.SSO.Helper.SSO();

            // Register the Event handler
            sso.SsoEvent += new EventHandler<bizilante.SSO.Helper.SSOEventArgs>(SSO_Update);

            // Get the name of the SSO application
            appName = Path.GetFileNameWithoutExtension(this.NonEncryptedFile);

            // Check to see if the application is already deployed
            string[] applications = sso.GetApplications();
            for (int i = 0; i < applications.Length; i++)
            {
                if (applications[i].ToUpper() == appName.ToUpper())
                {
                    newSSOApp = false;
                }
            }

            // Load into XmlDOM
            document.Load(this.NonEncryptedFile);

            XmlNodeList list = document.DocumentElement.SelectNodes("applicationData/add");
            List<string> list2 = new List<string>();
            List<string> list3 = new List<string>();

            if (!Overwrite)
            {
                // Check to see if we are deploying an existing application (flag = false)
                if (!newSSOApp)
                {
                    list2.AddRange(sso.GetKeys(appName));
                    list3.AddRange(sso.GetValues(appName));
                }
                foreach (XmlNode node in list)
                {
                    string str3 = node.SelectSingleNode("@key").Value;
                    string str4 = node.SelectSingleNode("@value").Value;
                    if ((!string.IsNullOrEmpty(str3) && !string.IsNullOrEmpty(str4)))
                    {
                        // New key value ?
                        if (!list2.Contains(str3))
                        {
                            list2.Add(str3);
                            list3.Add(str4);
                        }
                        else
                        {
                            list3[list2.IndexOf(str3)] = str4;
                        }
                    }
                }
            }
            else
            {
                foreach (XmlNode node in list)
                {
                    string str3 = node.SelectSingleNode("@key").Value;
                    string str4 = node.SelectSingleNode("@value").Value;
                    list2.Add(str3);
                    list3.Add(str4);
                }
            }
            sso.CreateApplicationFieldsValues(appName, list2.ToArray(), list3.ToArray());

            title = appName;
            return true;
        }

        private void SSO_Update(object sender, bizilante.SSO.Helper.SSOEventArgs e)
        {
            OnLog(e.Message);
        }

        public string NonEncryptedFile { get; set; }
        public string CompanyName { get; set; }
        public string Timeout { get; set; }
        /// <summary>
        /// The application values are overwritten
        /// </summary>
        public bool Overwrite { get; set; }
    }
}

