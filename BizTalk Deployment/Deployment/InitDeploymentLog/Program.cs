using System;
using Microsoft.BizTalk.ApplicationDeployment;
using System.Data.SqlClient;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using Microsoft.BizTalk.ExplorerOM;

namespace InitDeploymentLog
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Launch();
#endif
            if (args.Length < 3)
            {
                ShowHelp();
                Environment.ExitCode = 1;
                return;
            }
            string server = args[0];
            string database = args[1];
            string env = args[2];
            string groupName = GetGroupName(env);
            if (string.IsNullOrEmpty(groupName))
            {
                ShowHelp();
                Environment.ExitCode = 1;
                return;
            }
            Group grp = new Group();
            grp.DBServer = server;
            grp.DBName = database;
            try
            {
                Console.WriteLine("Initial upload to the DeploymentDb: '{0}'", bizilante.Helpers.LogDeployment.Utils.GetDeploymentDb());
                BtsCatalogExplorer btsExplorer = (BtsCatalogExplorer)grp.CatalogExplorer;
                Console.WriteLine(string.Format("Retrieving applications from {0}...", btsExplorer.ConnectionString));
                foreach (Microsoft.BizTalk.ApplicationDeployment.Application application in grp.Applications)
                {
                    Microsoft.BizTalk.ExplorerOM.Application app = btsExplorer.Applications[application.Name];
                    if (!app.IsSystem)
                    {
                        Console.WriteLine(string.Format("Initializing application {0}...", application.Name));
                        long id = bizilante.Helpers.LogDeployment.Utils.InsertIntoDeployment(
                            groupName,
                            env,
                            Environment.UserName,
                            DateTime.Now,
                            app.Name,
                            app.Description != null ? app.Description : "0.0.0.0",
                            "Initialisation",
                            string.Empty);
                        long id_package = bizilante.Helpers.LogDeployment.Utils.InsertIntoPackage(
                            id,
                            "Initial upload",
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            string.Empty,
                            application.Name);
                        string maxversion = "0.0.0.0";
                        int resourceCount = 0;
                        foreach (IResource resource in application.ResourceCollection)
                        {
                            if (resource.Properties.ContainsKey("IsSystem") && (bool)resource.Properties["IsSystem"])
                                break;
                            resourceCount++;
                            string str7 = GetResourceDisplayName(resource.ResourceType, resource.Luid, application.Name);
                            Console.WriteLine(string.Format("Resource: {0}", str7));
                            // We need to extract the resource CAB file
                            string path = Path.Combine(Path.GetTempPath(), application.Name);
                            DirectoryInfo di = new DirectoryInfo(path);
                            string filename = string.Empty;
                            string version = string.Empty;
                            if (((Resource)resource).Unpack(path))
                            {
                                // Get the corresponding file
                                if (resource.Properties.ContainsKey("DestinationLocation"))
                                {
                                    filename = (string)resource.Properties["DestinationLocation"];
                                    if (!string.IsNullOrEmpty(filename))
                                    {
                                        FileInfo fi = new FileInfo(filename);
                                        Console.WriteLine(string.Format("file: {0}", filename));
                                        FileInfo[] fis = di.GetFiles(fi.Name, SearchOption.AllDirectories);
                                        if (fis.Length > 0)
                                        {
                                            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo(fis[0].FullName);
                                            Console.WriteLine(string.Format("version: {0}", versionInfo.FileVersion));
                                            if (null != versionInfo.FileVersion)
                                            {
                                                version = versionInfo.FileVersion;
                                                if (IsVersionGreater(maxversion, version))
                                                    maxversion = version;
                                            }
                                        }
                                    }
                                }
                            }
                            bizilante.Helpers.LogDeployment.Utils.InsertIntoFiles(
                                id_package,
                                resource.ResourceType,
                                resource.Luid,
                                filename,
                                version);
                            try
                            {
                                di.Delete(true);
                            }
                            catch { }
                        }
                        bizilante.Helpers.LogDeployment.Utils.SetEndDeployment(id, DateTime.Now, false, string.Empty);
                        bizilante.Helpers.LogDeployment.Utils.SetApplicationVersion(application.Name, maxversion);
                        Console.WriteLine(string.Format("Finished Initializing application {0}, version={1}", application.Name, maxversion));
                    }
                }
                Console.WriteLine("Finished Initializing");
                Environment.ExitCode = 0;
            }
            catch (Exception exception)
            {
                WriteError(string.Format("Error occured: {0}", exception.Message));
                Environment.ExitCode = 2;
            }
            finally
            {
                if (grp != null)
                    grp.Dispose();
            }
            Console.ReadLine();
        }

        public static void ShowHelp()
        {
            Console.WriteLine("InitDeploymentLog <server> <databasename> <environment>");
            Console.WriteLine("<server>: BizTalk database servername");
            Console.WriteLine("<databasename>: BizTalk management database e.g. BizTalkMgmtDb");
            Console.WriteLine("<environment>: loc, dev, tst, edu, hfx or prd");
            Console.WriteLine();
        }
        private static string GetGroupName(string env)
        {
            string groupname = string.Empty;
            switch (env.Trim().ToLower())
            {
                case "loc":
                case "dev":
                case "tst":
                case "edu":
                case "hfx":
                case "prd":
                    groupname = "BizTalk " + env.Trim();
                    break;
            }
            return groupname;
        }
        private static string GetResourceDisplayName(string resourceType, string luid, string applicationName)
        {
            if ((!(resourceType == "System.BizTalk:File") && !(resourceType == "System.BizTalk:PreProcessingScript")) && !(resourceType == "System.BizTalk:PostProcessingScript"))
                return luid;
            else
                return (applicationName + ":" + luid.Substring(luid.IndexOf(":", StringComparison.Ordinal) + 1));
        }
        private static void WriteError(string message)
        {
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        }
        public static string GetConnectionString(string server, string database)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
            builder.DataSource = server;
            builder.InitialCatalog = database;
            builder.IntegratedSecurity = true;
            builder.ApplicationName = Assembly.GetExecutingAssembly().GetName().Name;
            string str = builder.ToString();
            return str;
        }
        /// <summary>
        /// Compare two file versions
        /// </summary>
        /// <param name="version1"></param>
        /// <param name="version2"></param>
        /// <returns>true when version2 > version1</returns>
        private static bool IsVersionGreater(string version1, string version2)
        {
            bool bResult = false;
            string[] parts1 = version1.Split(new string[] { "." }, StringSplitOptions.None);
            string[] parts2 = version2.Split(new string[] { "." }, StringSplitOptions.None);

            for (int i = 0; i < 4; i++)
            {
                if (int.Parse(parts2[i]) > int.Parse(parts1[i]))
                {
                    bResult = true;
                    break;
                }
                else if (int.Parse(parts2[i]) < int.Parse(parts1[i]))
                    break;
            }
            return bResult;
        }
    }
}
