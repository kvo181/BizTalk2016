using System;
using System.Linq;
using Microsoft.BizTalk.ExplorerOM;
using System.Management;
using System.Diagnostics;
using System.Reflection;
using System.Collections.Generic;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// This is a helper class for working with the BtsCatalogExplorer
    /// </summary>
    public static class BtsCatalogExplorerHelper
    {
        /// <summary>
        /// thios will check for the existance of a  biztalk application
        /// </summary>
        /// <param name="explorer"></param>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static bool ApplicationExists(BtsCatalogExplorer explorer, string applicationName)
        {
            if (explorer == null)
                throw new NullReferenceException("The explorer has not been provided");

            return explorer.Applications.Cast<Application>().Any(btsApp => btsApp.Name == applicationName);
        }

        /// <summary>
        /// This will create an application
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="newApplicationName"></param>
        /// <param name="description"></param>
        public static void CreateApplication(BtsCatalogExplorer catalog, string newApplicationName, string description)
        {
            if (catalog == null)
                throw new NullReferenceException("The catalog is null");

            var newApplication = catalog.AddNewApplication();
            newApplication.Name = newApplicationName;
            newApplication.Description = description;
            catalog.SaveChanges();
            catalog.Refresh();
        }

        /// <summary>
        /// This will delete an application
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="applicationName"></param>
        public static void DeleteApplication(BtsCatalogExplorer catalog, string applicationName)
        {
            if (catalog == null)
                throw new NullReferenceException("The catalog is null");

            var appToRemove = catalog.Applications[applicationName];
            catalog.RemoveApplication(appToRemove);
            catalog.SaveChanges();
            catalog.Refresh();
        }

        /// <summary>
        /// This will start an application
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="applicationName"></param>
        public static void StartApplication(BtsCatalogExplorer catalog, string applicationName)
        {
            if (catalog == null)
                throw new NullReferenceException("The catalog is null");

            try
            {
                catalog.Applications[applicationName].Start(ApplicationStartOption.StartAll);
                catalog.SaveChanges();
                catalog.Refresh();
            }
            catch (Exception)
            {
                AlternativeStart(catalog, applicationName);
            }
        }
        /// <summary>
        /// Tries to start the app one piece at a time as there was a problem doing it in bulk
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="applicationName"></param>
        private static void AlternativeStart(BtsCatalogExplorer catalog, string applicationName)
        {
            Trace.WriteLine("Attempting alternative method to start application");
            var app = catalog.Applications[applicationName];
         
            foreach (SendPort sendPort in app.SendPorts)
            {
                Trace.WriteLine(string.Format("Starting send port: {0}", sendPort.Name));
                StartSendPort(sendPort.Name);
            }

            foreach (ReceivePort recievePort in app.ReceivePorts)
            {
                foreach (ReceiveLocation recieveLocation in recievePort.ReceiveLocations)
                {
                    Trace.WriteLine(string.Format("Starting recieve location: {0}", recieveLocation.Name));
                    EnableRecieveLocation(recieveLocation.Name, recievePort.Name);
                }
            }

            foreach (BtsOrchestration orchestration in app.Orchestrations)
            {
                Trace.WriteLine(string.Format("Starting orchestration: {0}", orchestration.FullName));
                EnlistOrchestration(orchestration.FullName, orchestration.BtsAssembly.DisplayName, orchestration.Host.Name);
                StartOrchestration(orchestration.FullName, orchestration.BtsAssembly.DisplayName);
            }            
        }
        /// <summary>
        /// Enables a recieve loation
        /// </summary>
        /// <param name="name"></param>
        /// <param name="portName"></param>
        private static void EnableRecieveLocation(string name, string portName)
        {
            const string scopeFormat = "root\\MicrosoftBizTalkServer";
            const string managementPathFormat = "MSBTS_ReceiveLocation.Name='{0}',ReceivePortName='{1}'";
            Trace.WriteLine("Starting recieve location: " + name);

            var managementPath = string.Format(managementPathFormat, name, portName);
            var classInstance = new ManagementObject(scopeFormat, managementPath, null);

            classInstance.InvokeMethod("Enable", null, null);
        }
        /// <summary>
        /// Starts a send port
        /// </summary>
        /// <param name="name"></param>
        private static void StartSendPort(string name)
        {
            const string scopeFormat = "root\\MicrosoftBizTalkServer";
            const string managementPathFormat = "MSBTS_SendPort.Name='{0}'";
            Trace.WriteLine("Starting send port: " + name);

            var managementPath = string.Format(managementPathFormat, name);
            var classInstance = new ManagementObject(scopeFormat, managementPath, null);

            classInstance.InvokeMethod("Start", null, null);            
        }
        /// <summary>
        /// Starts the orchestration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assemblyFullName"></param>
        private static void StartOrchestration(string name, string assemblyFullName)
        {
            const string scopeFormat = "root\\MicrosoftBizTalkServer";
            const string managementPathFormat = "SELECT * FROM MSBTS_Orchestration WHERE AssemblyName = '{0}' And Name = '{1}'";

            var assemblyName = new AssemblyName(assemblyFullName);

            Trace.WriteLine("Starting - Orchestration Name: " + name);
            Trace.WriteLine("Orchestration Assembly Name: " + assemblyFullName);

            var args = new List<string> {assemblyName.Name, name, assemblyName.Version.ToString()};
            var managementPath = string.Format(managementPathFormat, args.ToArray());
            Trace.WriteLine("Management Path: " + managementPath);

            var query = string.Format(managementPathFormat, args.ToArray());
            var searcher = new ManagementObjectSearcher(scopeFormat, query);

            foreach (ManagementObject orchestrationManagementObject in searcher.Get())
            {
                Trace.WriteLine("Found orchestration: " + (string)orchestrationManagementObject["Name"]);

                var inParams = orchestrationManagementObject.GetMethodParameters("Start");
                inParams["AutoEnableReceiveLocationFlag"] = 1;
                inParams["AutoResumeOrchestrationInstanceFlag"] = 1;
                inParams["AutoStartSendPortsFlag"] = 1;

                orchestrationManagementObject.InvokeMethod("Start", inParams, null);                
            }
        }
        /// <summary>
        /// Starts the orchestration
        /// </summary>
        /// <param name="name"></param>
        /// <param name="assemblyFullName"></param>
        /// <param name="hostName"></param>
        private static void EnlistOrchestration(string name, string assemblyFullName, string hostName)
        {
            const string scopeFormat = "root\\MicrosoftBizTalkServer";
            const string managementPathFormat = "SELECT * FROM MSBTS_Orchestration WHERE AssemblyName = '{0}' And Name = '{1}'";

            var assemblyName = new AssemblyName(assemblyFullName);

            Trace.WriteLine("Enlisting - Orchestration Name: " + name);
            Trace.WriteLine("Orchestration Assembly Name: " + assemblyFullName);

            var args = new List<string> {assemblyName.Name, name, assemblyName.Version.ToString()};
            var managementPath = string.Format(managementPathFormat, args.ToArray());
            Trace.WriteLine("Management Path: " + managementPath);

            var query = string.Format(managementPathFormat, args.ToArray());
            var searcher = new ManagementObjectSearcher(scopeFormat, query);

            foreach (ManagementObject orchestrationManagementObject in searcher.Get())
            {
                Trace.WriteLine("Found orchestration: " + (string)orchestrationManagementObject["Name"]);

                var inParams = orchestrationManagementObject.GetMethodParameters("Enlist");
                inParams["HostName"] = hostName;
                orchestrationManagementObject.InvokeMethod("Enlist", inParams, null);
            }
        }
        

        /// <summary>
        /// This will stop an application
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="applicationName"></param>
        public static void StopApplication(BtsCatalogExplorer catalog, string applicationName)
        {
            if (catalog == null)
                throw new NullReferenceException("The catalog is null");

            var app = catalog.Applications[applicationName];
            app.Stop(ApplicationStopOption.StopAll);
            catalog.SaveChanges();
            catalog.Refresh();
        }
        /// <summary>
        /// This will add a reference from one BTS app to another
        /// </summary>
        /// <param name="catalog"></param>
        /// <param name="applicationName"></param>
        /// <param name="referencedApplicationName"></param>
        public static void AddReference(BtsCatalogExplorer catalog, string applicationName, string referencedApplicationName)
        {
            if (catalog == null)
                throw new NullReferenceException("The catalog is null");

            var app = catalog.Applications[applicationName];
            var referencedApp = catalog.Applications[referencedApplicationName];
            app.AddReference(referencedApp);
            catalog.SaveChanges();
            catalog.Refresh();
        }
    }
}