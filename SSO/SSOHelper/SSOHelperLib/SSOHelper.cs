using bizilante.SSO.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Xml;

namespace bizilante.SSO.Tools
{
    public class SSOHelper
    {
        public static string Decrypt(string encryptedFile, string encryptionKey, out string appName)
        {
            appName = string.Empty;
            XmlDocument document = new XmlDocument();
            string toDecrypt = string.Empty;
            FileInfo info = new FileInfo(encryptedFile);
            if (!info.Exists)
            {
                throw new Exception(string.Format("Could not find the specified input file: '{0}'", new object[] { encryptedFile }));
            }

            try
            {
                byte[] bytes;
                StreamReader reader = new StreamReader(encryptedFile);
                toDecrypt = reader.ReadToEnd();
                reader.Dispose();
                appName = Path.GetFileNameWithoutExtension(encryptedFile);

                try
                {
                    bytes = Encoding.ASCII.GetBytes(Helper.SSO.Decrypt(toDecrypt, encryptionKey));
                }
                catch (Exception exception1)
                {
                    throw new Exception(string.Format("Failed to decrypt. {0}", exception1.Message), exception1);
                }

                MemoryStream inStream = new MemoryStream(bytes);
                try
                {
                    document.Load(inStream);
                    document.Normalize();
                }
                catch (Exception exception2)
                {
                    throw new Exception(string.Format("Failed to load xml. {0}", exception2.Message), exception2);
                }
                finally
                {
                    inStream.Dispose();
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                // Save the document to a string and auto-indent the output.
                StringBuilder sb = new StringBuilder();
                XmlWriter writer = XmlWriter.Create(sb, settings);
                document.Save(writer);
                return sb.ToString();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string Decrypt(string encryptedFile, string encryptionKey, out string appName, out string filename)
        {
            appName = string.Empty;
            filename = string.Empty;
            XmlDocument document = new XmlDocument();
            string toDecrypt = string.Empty;
            FileInfo info = new FileInfo(encryptedFile);
            if (!info.Exists)
            {
                throw new Exception(string.Format("Could not find the specified input file: '{0}'", new object[] { encryptedFile }));
            }

            try
            {
                byte[] bytes;
                StreamReader reader = new StreamReader(encryptedFile);
                toDecrypt = reader.ReadToEnd();
                reader.Dispose();
                appName = Path.GetFileNameWithoutExtension(encryptedFile);

                try
                {
                    bytes = Encoding.ASCII.GetBytes(Helper.SSO.Decrypt(toDecrypt, encryptionKey));
                }
                catch (Exception exception1)
                {
                    throw new Exception(string.Format("Failed to decrypt. {0}", exception1.Message), exception1);
                }

                MemoryStream inStream = new MemoryStream(bytes);
                try
                {
                    document.Load(inStream);
                    document.Normalize();
                }
                catch (Exception exception2)
                {
                    throw new Exception(string.Format("Failed to load xml. {0}", exception2.Message), exception2);
                }
                finally
                {
                    inStream.Dispose();
                }

                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                // Save the document to a file and auto-indent the output.
                filename = Path.Combine(Path.GetDirectoryName(encryptedFile), appName + ".xml");
                XmlWriter writer = XmlWriter.Create(filename, settings);
                document.Save(writer);
                // Save the document to a string and auto-indent the output.
                StringBuilder sb = new StringBuilder();
                writer = XmlWriter.Create(sb, settings);
                document.Save(writer);
                return sb.ToString();

            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Import(string decryptedFile, out string appName)
        {
            bool flag = true;
            XmlDocument document = new XmlDocument();
            appName = string.Empty;
            string toDecrypt = string.Empty;
            FileInfo info = new FileInfo(decryptedFile);
            if (!info.Exists)
            {
                throw new Exception(string.Format("Could not find the specified input file: '{0}'", new object[] { decryptedFile }));
            }

            try
            {
                Helper.SSO sso = new Helper.SSO();
                appName = Path.GetFileNameWithoutExtension(decryptedFile);
                string[] applications = sso.GetApplications();
                for (int i = 0; i < applications.Length; i++)
                {
                    if (applications[i].ToUpper() == appName.ToUpper())
                    {
                        flag = false;
                        break;
                    }
                }

                try
                {
                    document.Load(decryptedFile);
                }
                catch (Exception exception2)
                {
                    throw new Exception(string.Format("Failed to load xml. {0}", exception2.Message), exception2);
                }
                finally
                {
                }

                XmlNodeList list = document.DocumentElement.SelectNodes("applicationData/add");
                List<string> list2 = new List<string>();
                List<string> list3 = new List<string>();
                Dictionary<string, string> dict = new Dictionary<string, string>();
                if (!flag)
                    sso.GetKeyValues(appName, dict);

                // Create or update fields
                bool update = false;
                foreach (XmlNode node in list)
                {
                    string str3 = node.SelectSingleNode("@key").Value;
                    string str4 = node.SelectSingleNode("@value").Value;
                    if (!string.IsNullOrEmpty(str3) && !string.IsNullOrEmpty(str4))
                    {
                        list2.Add(str3);
                        list3.Add(str4);
                        if (!dict.ContainsKey(str3))
                            update = true;
                        else
                        {
                            if (string.CompareOrdinal(dict[str3], str4) != 0)
                            {
                                int idx = list2.IndexOf(str3);
                                if (idx >= 0)
                                {
                                    update = true;
                                    list3[idx] = str4;
                                }
                            }
                        }
                    }
                }
                if (update)
                    sso.CreateApplicationFieldsValues(appName, list2.ToArray(), list3.ToArray());
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void Export(string filename, string encryptionKey, out string appName, out string exportedFilename)
        {
            try
            {
                Helper.SSO sso = new Helper.SSO();
                if (filename.ToLower().EndsWith(".xml"))
                    exportedFilename = filename.Replace(filename.Substring(filename.Length - 3), "sso");
                else
                    exportedFilename = filename;
                appName = Path.GetFileNameWithoutExtension(exportedFilename);
                string[] applications = sso.GetApplications();
                for (int j = 0; j < applications.Length; j++)
                {
                    if (applications[j].ToUpper() == appName.ToUpper())
                    {
                        // Export this application
                        string[] keys = sso.GetKeys(appName);
                        string[] values = sso.GetValues(appName);
                        StringBuilder builder = new StringBuilder();
                        builder.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?><SSOApplicationExport><applicationData>");
                        for (int i = 0; i < keys.Length; i++)
                        {
                            if ((keys[i] != null) && !(keys[i] == ""))
                            {
                                builder.Append("<add key=\"" + keys[i] + "\" value=\"" + HttpUtility.HtmlEncode(values[i]) + "\" />");
                            }
                        }
                        builder.Append("</applicationData></SSOApplicationExport>");
                        StreamWriter writer = new StreamWriter(exportedFilename, false);
                        try
                        {
                            writer.Write(Helper.SSO.Encrypt(builder.ToString(), encryptionKey));
                            writer.Flush();
                        }
                        catch (Exception exception)
                        {
                            throw new Exception(string.Format("Failed to encrypt {0}: {1}", appName, exception.Message), exception);
                        }
                        finally
                        {
                            writer.Close();
                            writer.Dispose();
                        }
                        break;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}
