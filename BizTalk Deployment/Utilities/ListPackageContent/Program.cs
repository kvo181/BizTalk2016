using System;
using System.Collections.Generic;
using Microsoft.BizTalk.ApplicationDeployment;
using System.IO;
using System.Globalization;
using System.Diagnostics;

namespace bizilante.Deployment.Apps
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            if (args.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Please, provide the MSI path");
                Console.ResetColor();
                Environment.ExitCode = 1;
            }
            else
            {
                string msiPath = args[0];
                if (!File.Exists(msiPath))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid MSI Patch: {0}", msiPath);
                    Console.ResetColor();
                    Environment.ExitCode = 2;
                }
                else
                {
                    Dictionary<string, string> props;
                    bool waitForDelete = args.Length > 1;
                    string path = bizilante.Helpers.ListPackageHelper.Helper.ExtractFiles(msiPath, out props);
                    DirectoryInfo di = new DirectoryInfo(path);
                    ListPackageContent(msiPath, di);
                    if (waitForDelete)
                    {
                        Console.WriteLine("Files extracted to: '{0}'", di.FullName);
                        Console.WriteLine("Press <enter> to continue");
                        Console.ReadLine();
                    }
                    di.Delete(true);
                    Environment.ExitCode = 0;
                }
            }
        }

        public static void ListPackageContent(string msiPath, DirectoryInfo di)
        {
            IInstallPackage package = null;
            try
            {
                package = DeploymentUnit.ScanPackage(msiPath);
                if (package != null)
                {
                    Console.WriteLine("Title: " + package.Title);
                    Console.WriteLine("Author: " + package.Author);
                    Console.WriteLine("Subject: " + package.Subject);
                    Console.WriteLine("Comments: " + package.Comments);
                    Console.WriteLine("Keywords: " + package.Keywords);
                    Console.WriteLine("Create Time: " + package.CreateTime.ToString("u", CultureInfo.InvariantCulture));
                    Console.WriteLine("Package Code: " + package.RevisionNumber);
                    Console.WriteLine("Resource type;Luid;Filename;Version");
                    if ((package.Resources != null) && (package.Resources.Length != 0))
                    {
                        foreach (IDeploymentResource resource in package.Resources)
                        {
                            Console.Write(string.Format("{0};{1};", new object[] { resource.ResourceType, resource.Luid }));
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
                                        Console.WriteLine(string.Format("{0};{1}", filename, versionInfo.FileVersion));
                                    }
                                    else
                                        Console.WriteLine(";");
                                }
                                else
                                    Console.WriteLine(";");
                            }
                            else
                                Console.WriteLine(";");
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error occured: {0}", exception.Message);
                Console.ResetColor();
            }
        }
    }
}
