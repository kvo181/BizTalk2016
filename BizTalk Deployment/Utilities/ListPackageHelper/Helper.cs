using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.BizTalk.ApplicationDeployment.MSIManager.WindowsInstaller;
using Microsoft.BizTalk.ApplicationDeployment.MSIManager.Cab;
using Microsoft.BizTalk.ApplicationDeployment;
using System.Globalization;
using System.Diagnostics;

using WindowsInstaller = Microsoft.Deployment.WindowsInstaller;

namespace bizilante.Helpers.ListPackageHelper
{
    public class Helper
    {
        public static string ExtractFiles(string msiPath, out Dictionary<string, string> properties)
        {
            string str = null;
            using (Database database = Installer.OpenDatabase(msiPath, DatabaseOpenMode.ReadOnly))
            {
                str = ExtractMediaStream(database);
                if (str == null)
                    throw new ApplicationDeploymentException(string.Format("UnableToExtractMSI", new object[] { msiPath }));

                properties = ExtractProperties(database);

                using (View view = database.OpenView("SELECT `File`, `FileName` FROM `File`", new object[0]))
                {
                    view.Execute();
                    Record record = null;
                    while ((record = view.Fetch()) != null)
                    {
                        using (record)
                        {
                            string str3 = (string)record[2];
                            string sourceFileName = Path.Combine(str, (string)record[1]);
                            string destFileName = Path.Combine(str, str3.Substring(str3.IndexOf('|') + 1));
                            File.Move(sourceFileName, destFileName);
                            continue;
                        }
                    }
                }
                foreach (string str6 in Directory.GetFiles(str, "*.cab"))
                {
                    string fileName = Path.GetFileName(str6);
                    new CabinetInfo(str6).ExtractAll(str, false, null, false);
                }
                database.Close();
            }
            return str;
        }

        public static Dictionary<string, string> ExtractProperties(Database msiDb)
        {
            Dictionary<string, string> properties = new Dictionary<string, string>();
            string str1 = (string)msiDb.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductCode'", new object[0]);
            properties.Add("ProductCode", str1);
            string str2 = (string)msiDb.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductVersion'", new object[0]);
            properties.Add("ProductVersion", str2);
            string str3 = (string)msiDb.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = 'UpgradeCode'", new object[0]);
            properties.Add("UpgradeCode", str3);
            string str4 = (string)msiDb.ExecuteScalar("SELECT `Value` FROM `Property` WHERE `Property` = 'ProductLanguage'", new object[0]);
            properties.Add("ProductLanguage", str4);

            return properties;
        }

        public static string ExtractMediaStream(Database msiDb)
        {
            string tempFileName = Path.GetTempFileName();
            string tempFolder = GetTempFolder(tempFileName);
            string filePath = Path.ChangeExtension(tempFileName, ".cab");
            string str4 = (string)msiDb.ExecuteScalar("SELECT `Cabinet` FROM `Media` WHERE `DiskId` = {0}", new object[] { 1 });
            using (View view = msiDb.OpenView("SELECT `Name`, `Data` FROM `_Streams` WHERE `Name` = '{0}'", new object[] { str4.Substring(1) }))
            {
                view.Execute();
                Record record = view.Fetch();
                if (record == null)
                {
                    throw new InstallerException("Stream not found: " + str4);
                }
                using (record)
                {
                    record.GetStream("Data", filePath);
                }
            }
            CabinetInfo info = new CabinetInfo(filePath);
            info.ExtractAll(tempFolder);
            info.Delete();
            return tempFolder;
        }

        public static string[] ListPackageContent(string msiPath)
        {
            return ListPackageContentAsList(msiPath).ToArray();
        }
        public static List<string> ListPackageContentAsList(string msiPath)
        {
            IInstallPackage package = null;
            List<string> packageInfo = new List<string>();
            if (string.IsNullOrWhiteSpace(msiPath)) return packageInfo;
            try
            {
                Dictionary<string, string> properties;
                string path = Helper.ExtractFiles(msiPath, out properties);
                DirectoryInfo di = new DirectoryInfo(path);

                package = DeploymentUnit.ScanPackage(msiPath);
                if (package != null)
                {
                    packageInfo.Add("Title: " + package.Title);
                    packageInfo.Add("Author: " + package.Author);
                    packageInfo.Add("Subject: " + package.Subject);
                    packageInfo.Add("Comments: " + package.Comments);
                    packageInfo.Add("Keywords: " + package.Keywords);
                    packageInfo.Add("Create Time: " + package.CreateTime.ToString("u", CultureInfo.InvariantCulture));
                    packageInfo.Add("Package Code: " + package.RevisionNumber);
                    if ((null != properties) && properties.ContainsKey("ProductCode"))
                        packageInfo.Add("Product Code: " + properties["ProductCode"]);
                    else
                        packageInfo.Add("Product Code: ");
                    packageInfo.Add("Resource type;Luid;Filename;Version");
                    if ((package.Resources != null) && (package.Resources.Length != 0))
                    {
                        foreach (IDeploymentResource resource in package.Resources)
                        {
                            string resourceItem = string.Format("{0};{1};", new object[] { resource.ResourceType, resource.Luid });
                            // Get the corresponding file
                            if (resource.Properties.ContainsKey("DestinationLocation"))
                            {
                                string filename = (string)resource.Properties["DestinationLocation"];
                                if (!string.IsNullOrEmpty(filename))
                                {
                                    FileInfo fi = new FileInfo(filename);
                                    FileInfo[] fis = di.GetFiles(fi.Name, SearchOption.AllDirectories);
                                    if (fis.Length > 0)
                                    {
                                        FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fis[0].FullName);
                                        resourceItem += string.Format("{0};{1}", filename, versionInfo.FileVersion);
                                    }
                                    else
                                        resourceItem += ";";
                                }
                                else
                                    resourceItem += ";";
                            }
                            else
                                resourceItem += ";";

                            packageInfo.Add(resourceItem);
                        }
                    }
                }
                di.Delete(true);
            }
            catch (Exception exception)
            {
                throw new Exception(string.Format("ListPackageContent: Error occured: {0}", exception.Message), exception);
            }
            return packageInfo;
        }

        public static string[] GetFilesFromMSI(string msiPath)
        {
            List<string> properties = new List<string>();
            using (WindowsInstaller.Session session = WindowsInstaller.Installer.OpenPackage(msiPath, true))
            {
                properties.Add(session.GetProductProperty("ProductVersion"));
                properties.Add(session.GetProductProperty("ProductCode"));
                properties.Add(session.GetProductProperty("UpgradeCode"));
            }
            return properties.ToArray();
        }

        private static string GetTempFolder(string tmpFile)
        {
            if (File.Exists(tmpFile))
            {
                File.Delete(tmpFile);
            }
            string path = Path.ChangeExtension(tmpFile, null);
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            return path;
        }
    }
}
