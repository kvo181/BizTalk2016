using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using System.Reflection;
using System.Data.SqlClient;
using OM = Microsoft.BizTalk.ExplorerOM;
using System.Security.Permissions;
using Microsoft.Win32;
using System.Globalization;
using System.IO;
using Microsoft.BizTalk.ApplicationDeployment;
using Microsoft.VisualStudio.Shell;
using VSIXBizTalkBuildAndDeploy.Helpers.Options;

namespace VSIXBizTalkBuildAndDeploy.Helpers.BizTalk
{
    class BizTalkServerRegistry
    {
        public string BizTalkMgmtDbName { get; set; }
        public string BizTalkMgmtDb { get; set; }
        public string InstallPath { get; set; }
    }
    class BizTalkBindingToolDb
    {
        public string Server { get; set; }
        public string Database { get; set; }
    }

    /// <summary>
    /// Helper class for getting applications
    /// </summary>
    public sealed class BizTalkHelper
    {
        private static BizTalkServerRegistry _bizTalkServerRegistry;
        private static BizTalkServerRegistry BizTalkServerRegistry
        {
            get { return _bizTalkServerRegistry ?? (_bizTalkServerRegistry = GetMgmtServerInfo()); }
        }
        private static BizTalkBindingToolDb _bizTalkBindingToolDb;
        private static BizTalkBindingToolDb BizTalkBindingToolDb
        {
            get { return _bizTalkBindingToolDb ?? (_bizTalkBindingToolDb = GetBizTalkBindingToolDb()); }
        }

        /// <summary>
        /// The name of the management db
        /// </summary>
        public static string BizTalkManagementDatabaseName
        {
            get {
                return !string.IsNullOrEmpty(BizTalkServerRegistry.BizTalkMgmtDbName) ? BizTalkServerRegistry.BizTalkMgmtDbName : "BizTalkMgmtDb";
            }
        }
        /// <summary>
        /// The name of the database server
        /// </summary>
        public static string BizTalkDatabaseServerName
        {
            get
            {
                string serverName = !string.IsNullOrEmpty(BizTalkServerRegistry.BizTalkMgmtDb)
                                        ? BizTalkServerRegistry.BizTalkMgmtDb
                                        : ".";
                return serverName.ToUpper().Replace(Environment.MachineName.ToUpper(), ".");
            }
        }

        /// <summary>
        /// The Project Structure to use in the Build scripts
        /// </summary>
        public static Options.ProjectStructureTypeEnum ProjectStructureType
        {
            get
            {
                return GetProjectStructureType();
            }
        }
        public static AssemblyVersionEnum AssemblyVersionType
        {
            get
            {
                return GetAssemblyVersionType();
            }
        }

        /// <summary>
        /// The root path of the MSBuild tasks
        /// </summary>
        public static string TasksPath
        {
            get
            {
                return GetTasksPath();
            }
        }

        /// <summary>
        /// The path for the BizTalk Custom Pipeline Components
        /// </summary>
        public static string PipelineComponentsPath
        {
            get
            {
                if (!string.IsNullOrEmpty(BizTalkServerRegistry.InstallPath))
                    return (BizTalkServerRegistry.InstallPath.EndsWith("\\") ? BizTalkServerRegistry.InstallPath + "Pipeline Components" : BizTalkServerRegistry.InstallPath + "\\Pipeline Components");
                else
                    return (Properties.Settings.Default.BizTalkInstallPath.EndsWith("\\") ? BizTalkServerRegistry.InstallPath + "Pipeline Components" : BizTalkServerRegistry.InstallPath + "\\Pipeline Components");
            }
        }

        /// <summary>
        /// Does the build task need to generate the binding files or not
        /// </summary>
        public static bool GenerateBindings
        {
            get
            {
                return GetGeneratedBindings();
            }
        }

        /// <summary>
        /// The company name of the MSBuild SSO import task
        /// </summary>
        public static string CompanyName
        {
            get
            {
                return GetSSOCompanyName();
            }
        }

        /// <summary>
        /// The visual studio package calling this helper
        /// </summary>
        public static Package Package { get; internal set; }

        /// <summary>
        /// Get the binding used during the deploy after build.
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <returns></returns>
        public static string GetBindingFilePath(Options.ProjectStructureTypeEnum projectStructureType)
        {
            string bindingFilePath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    bindingFilePath = "$(SourceCodeRootFolder)\\..\\Install\\Bindings\\LOC\\$(ProductName).BindingInfo.LOC.xml";
                    break;
                default:
                    bindingFilePath = "$(SourceCodeRootFolder)\\Bindings\\$(ProductName).BindingInfo.xml";
                    break;
            }
            return bindingFilePath;
        }

        /// <summary>
        /// Get the path where we publish the msi
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <returns></returns>
        public static string GetPublishMsiPath(Options.ProjectStructureTypeEnum projectStructureType)
        {
            string publishMsiPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    publishMsiPath = "$(SourceCodeRootFolder)\\..\\Install\\MSI";
                    break;
                default:
                    publishMsiPath = "$(SourceCodeRootFolder)\\Publish";
                    break;
            }
            return publishMsiPath;
        }

        /// <summary>
        /// Get the directory containing the Rules used in the MSBUILD script
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <returns></returns>
        public static string GetRulesTargetsPath(Options.ProjectStructureTypeEnum projectStructureType)
        {
            string rulesPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    rulesPath = "$(SourceCodeRootFolder)\\..\\Install\\Policies";
                    break;
                default:
                    rulesPath = "$(SourceCodeRootFolder)\\Setup\\Rules";
                    break;
            }
            return rulesPath;
        }

        /// <summary>
        /// Get the bindings directory containing the binding files per environment
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetBindingsPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string bindingsPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    bindingsPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install\\Bindings";
                    break;
                default:
                    bindingsPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup\\Bindings";
                    break;
            }
            return bindingsPath;
        }

        /// <summary>
        /// Get the directory containing the BAM activities
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetBamPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string bamPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    bamPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install\\BAM";
                    break;
                default:
                    bamPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup\\BAM";
                    break;
            }
            return bamPath;
        }

        /// <summary>
        /// Get the directory containing the files (added as File resource type)
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetFilesPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string filesPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    filesPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install\\Scripts\\Resources";
                    break;
                default:
                    filesPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup\\Resources";
                    break;
            }
            return filesPath;
        }

        /// <summary>
        /// Get the directory containing the SSO configurations
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetSsoPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string ssoPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    ssoPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install\\SSO";
                    break;
                default:
                    ssoPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup\\SSO";
                    break;
            }
            return ssoPath;
        }

        /// <summary>
        /// Get the directory containing the Rules
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetRulesPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string rulesPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    rulesPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install\\Policies";
                    break;
                default:
                    rulesPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup\\Rules";
                    break;
            }
            return rulesPath;
        }

        /// <summary>
        /// Get the directory containing the external assemblies
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetExternalAssembliesPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string path;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    path = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install\\Assemblies";
                    break;
                default:
                    path = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup\\Assemblies";
                    break;
            }
            return path;
        }

        /// <summary>
        /// Get the directory containing the Application references
        /// </summary>
        /// <param name="projectStructureType"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        public static string GetApplicationReferencesPath(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder)
        {
            string applicationReferencesPath;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    applicationReferencesPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\..\\Install";
                    break;
                default:
                    applicationReferencesPath = Path.GetDirectoryName(sourceCodeRootFolder) + "\\Setup";
                    break;
            }
            return applicationReferencesPath;
        }

        ///<summary>
        /// Get the application major/minor version values.
        ///</summary>
        ///<param name="projectStructureType"></param>
        ///<param name="sourceCodeRootFolder"></param>
        ///<param name="majorVersion"></param>
        ///<param name="minorVersion"></param>
        /// <param name="release"></param>
        /// <param name="build"></param>
        public static void GetApplicationVersion(Options.ProjectStructureTypeEnum projectStructureType, string sourceCodeRootFolder, out string majorVersion, out string minorVersion, out string release, out string build)
        {
            majorVersion = "1";
            minorVersion = "0";
            release = "0";
            build = "0";
            string configFileName;
            string sourceRootFolder = Path.GetDirectoryName(sourceCodeRootFolder);
            if (string.IsNullOrEmpty(sourceRootFolder))
                throw new Exception(string.Format("sourceCodeRootFolder: '{0}', empty folder!!", sourceCodeRootFolder));

            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    configFileName = new DirectoryInfo(sourceRootFolder).FullName.TrimEnd(new char[] { '\\' }) + "\\config.xml";
                    GetVersion2(configFileName, out majorVersion, out minorVersion, out release, out build);
                    break;
                default:
                    configFileName = new DirectoryInfo(sourceRootFolder).FullName.TrimEnd(new char[] { '\\' }) + "\\Setup\\config.xml";
                    GetVersion(configFileName, out majorVersion, out minorVersion);
                    break;
            }

        }

        ///<summary>
        /// Get the list of AssemblyInfo files - used to change the version -
        ///</summary>
        ///<param name="projectStructureType"></param>
        ///<returns></returns>
        public static string GetAssemblyInfoFiles(Options.ProjectStructureTypeEnum projectStructureType)
        {
            string assemblyInfoFiles;
            switch (projectStructureType)
            {
                case Options.ProjectStructureTypeEnum.ACVCSC:
                    assemblyInfoFiles = "$(MSBuildProjectDirectory)\\..\\BizTalkAssemblyInfo.cs";
                    break;
                default:
                    assemblyInfoFiles = "$(MSBuildProjectDirectory)\\..\\Setup\\BizTalkAssemblyInfo.cs";
                    break;
            }
            return assemblyInfoFiles;
        }

        /// <summary>
        /// Gets a list of the adapters in BizTalk
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAdapters()
        {
            List<string> adapters = new List<string>();
            using (Group group = new Group())
            {
                group.DBName = BizTalkManagementDatabaseName;
                group.DBServer = BizTalkDatabaseServerName;
                foreach (OM.ProtocolType protocol in group.CatalogExplorer.ProtocolTypes)
                {
                    adapters.Add(protocol.Name);
                }
            }
            return adapters;
        }
        /// <summary>
        /// Gets all of the applications deployed on the server
        /// </summary>        
        /// <returns></returns>
        public static List<string> GetApplications()
        {
            List<string> applicationNames = new List<string>();
            using (OM.BtsCatalogExplorer explorer = new OM.BtsCatalogExplorer())
            {
                explorer.ConnectionString = GetConnectionString(BizTalkDatabaseServerName, BizTalkManagementDatabaseName);
                foreach (OM.Application app in explorer.Applications)
                {
                    applicationNames.Add(app.Name);
                }
            }
            return applicationNames;
        }
        /// <summary>
        /// Describes the application
        /// </summary>
        /// <param name="applicationName"></param>
        /// <returns></returns>
        public static MetaDataBuildGenerator.ApplicationDescription DescribeApplication(string applicationName)
        {
            MetaDataBuildGenerator.ApplicationDescription applicationDescription = new MetaDataBuildGenerator.ApplicationDescription();

            using (Group group = new Group())
            {
                group.DBName = BizTalkManagementDatabaseName;
                group.DBServer = BizTalkDatabaseServerName;
                Application app = group.Applications[applicationName];

                applicationDescription.Name = app.Name;
                applicationDescription.Description = app.Description;

                //Add application references
                if (app.References != null)
                {
                    foreach (IApplication referencedApp in app.References)
                    {
                        applicationDescription.ReferencedApplications.Add(referencedApp.Name);
                    }
                }

                foreach (Resource resource in app.ResourceCollection)
                {
                    Debug.WriteLine("Resource Type: " + resource.ResourceType);
                    MetaDataBuildGenerator.ApplicationResource applicationResource = new MetaDataBuildGenerator.ApplicationResource();
                    applicationResource.FullName = resource.Luid;
                    applicationResource.Type = GetResourceType(resource.ResourceType);

                    if (IncludeResource(applicationResource))
                    {
                        AddResourceReferences(resource, applicationResource);
                        AddResourceProperties(resource, applicationResource);
                        applicationDescription.Resources.Add(applicationResource);
                    }
                }

                AddDependantResources(applicationDescription);

                SortResources(applicationDescription);
            }
            return applicationDescription;
        }

        ///<summary>
        /// Get the connection string to connect to the BizTalk group.
        ///</summary>
        ///<param name="server"></param>
        ///<param name="database"></param>
        ///<returns></returns>
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
        /// Get the BizTalkBindingTool database server name and database name
        /// </summary>
        /// <param name="server"></param>
        /// <param name="database"></param>
        public static void GetBizTalkBindingToolDb(out string server, out string database)
        {
            server = BizTalkBindingToolDb.Server;
            database = BizTalkBindingToolDb.Database;
        }

        #region Private Methods
        /// <summary>
        /// Returns just the stuff after the : in the type name
        /// </summary>
        /// <param name="typeDescription"></param>
        /// <returns></returns>
        private static string GetResourceType(string typeDescription)
        {
            int splitPosition = typeDescription.IndexOf(":", System.StringComparison.Ordinal);
            return typeDescription.Substring(splitPosition + 1);
        }
        /// <summary>
        /// Sorts the resources based on their dependancies.  
        /// They need to be sorted so when we generate a script to deploy the resources will
        /// be loaded to BizTalk in the correct order
        /// </summary>
        /// <param name="applicationDescription"></param>
        private static void SortResources(MetaDataBuildGenerator.ApplicationDescription applicationDescription)
        {
            ApplicationResourceSorter.Sort(applicationDescription.Resources);
            //applicationDescription.Resources.Sort();
            Trace.WriteLine("The resource order after Sort is:");
            foreach (MetaDataBuildGenerator.ApplicationResource r in applicationDescription.Resources)
                Trace.WriteLine(r.FullName);
        }
        /// <summary>
        /// Loads up the resource properties
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="applicationResource"></param>
        private static void AddResourceProperties(
            Resource resource, 
            MetaDataBuildGenerator.ApplicationResource applicationResource)
        {
            IEnumerator<KeyValuePair<string, object>> enumerator = resource.Properties.GetEnumerator();
            while (enumerator.MoveNext())
            {
                Debug.WriteLine(enumerator.Current.Key + ":" + Convert.ToString(enumerator.Current.Value));
                MetaDataBuildGenerator.ResourceProperty property = new MetaDataBuildGenerator.ResourceProperty();
                property.Name = enumerator.Current.Key;
                property.Value = enumerator.Current.Value;
                applicationResource.Properties.Add(property);
            }
        }
        /// <summary>
        /// Uses reflection to load the references of each application resource
        /// </summary>
        /// <param name="applicationResource"></param>
        /// <param name="resource"></param>
        private static void AddResourceReferences(
            Resource resource, 
            MetaDataBuildGenerator.ApplicationResource applicationResource)
        {
            const string SourceLocationPropertyName = "SourceLocation";

            if (applicationResource.Type == MetaDataBuildGenerator.MetaData.ResourceTypes.Assembly || applicationResource.Type == MetaDataBuildGenerator.MetaData.ResourceTypes.BizTalkAssembly)
            {
                string location = (string)resource.Properties[SourceLocationPropertyName];
                Assembly a = Assembly.LoadFile(location);
                foreach (AssemblyName name in a.GetReferencedAssemblies())
                {
                    Trace.WriteLine(string.Format("Reflected Assembly: {0} depends on {1}", a.FullName, name.FullName));
                    applicationResource.References.Add(name.FullName);
                }
            }

        }
        /// <summary>
        /// Looks at the references for each resource and creates links between resources based on the references.  This then allows the resources to be later sorted based on these dependancies
        /// </summary>
        /// <param name="applicationDescription"></param>
        private static void AddDependantResources(
            MetaDataBuildGenerator.ApplicationDescription applicationDescription)
        {
            foreach (MetaDataBuildGenerator.ApplicationResource resource in applicationDescription.Resources)
            {
                Trace.WriteLine("");
                Trace.WriteLine(resource.FullName);
                Trace.WriteLine("Depends on:");
                foreach (MetaDataBuildGenerator.ApplicationResource dependantResource in applicationDescription.Resources)
                {
                    if (dependantResource.FullName != resource.FullName)
                    {
                        if (resource.References.Contains(dependantResource.FullName))
                        {
                            Trace.WriteLine(dependantResource.FullName);
                            resource.DependantResources.Add(dependantResource);
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Validates the resource to ensure it should be included
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        private static bool IncludeResource(MetaDataBuildGenerator.ApplicationResource resource)
        {
            bool include = true;

            if (resource.Type == MetaDataBuildGenerator.MetaData.ResourceTypes.BizTalkBinding)
            {
                MetaDataBuildGenerator.MetaData.ResourceAdapters.BindingResourceAdapter bindingAdapter = MetaDataBuildGenerator.MetaData.ResourceAdapters.BindingResourceAdapter.Create(resource);
                if (string.IsNullOrEmpty(bindingAdapter.SourceLocation))
                    include = false;
            }

            return include;
        }

        [RegistryPermission(SecurityAction.Demand, Read = @"SOFTWARE\Microsoft\BizTalk Server\3.0\Administration")]
        private static BizTalkServerRegistry GetMgmtServerInfo()
        {
            BizTalkServerRegistry registry = new BizTalkServerRegistry();
            try
            {
                string name = @"SOFTWARE\Microsoft\BizTalk Server\3.0";
                RegistryKey key = Registry.LocalMachine.OpenSubKey(name);
                if (key == null)
                    throw new BizTalkRegistryMissingException(string.Format(CultureInfo.CurrentCulture, "Could not locate the BizTalk Registry key. The VS package must be installed on a BizTalk development Server.  Reg Key Lookup '{0}'.", new object[] { name }));
                registry.InstallPath = (string)key.GetValue("InstallPath");

                RegistryKey subKey = key.OpenSubKey("Administration");
                if (subKey == null)
                    throw new BizTalkRegistryMissingException(string.Format(CultureInfo.CurrentCulture, "Could not locate the BizTalk Registry key. The VS package must be installed on a BizTalk development Server.  Reg Key Lookup '{0}'.", new object[] { name + "\\Administration" }));
                registry.BizTalkMgmtDbName = (string)subKey.GetValue("MgmtDBName");
                registry.BizTalkMgmtDb = (string)subKey.GetValue("MgmtDBServer");

                key.Close();
                key = null;
            }
            catch { }

            return registry;
        }

        private static string GetTasksPath()
        {
            string tasksPath = string.Empty;
            try
            {
                Options.OptionPageGrid page = (Options.OptionPageGrid)Package.GetDialogPage(typeof(Options.OptionPageGrid));
                tasksPath = page.TasksPath;
            }
            catch { }
            return tasksPath;
        }

        private static Options.ProjectStructureTypeEnum GetProjectStructureType()
        {
            Options.ProjectStructureTypeEnum projectStructureType = Options.ProjectStructureTypeEnum.Default;
            try
            {
                Options.OptionPageGrid page = (Options.OptionPageGrid)Package.GetDialogPage(typeof(Options.OptionPageGrid));
                projectStructureType = page.ProjectStructureType;
            }
            catch { }
            return projectStructureType;
        }
        private static Options.AssemblyVersionEnum GetAssemblyVersionType()
        {
            Options.AssemblyVersionEnum assemblyVersionType = Options.AssemblyVersionEnum.Default;
            try
            {
                Options.OptionPageGrid page = (Options.OptionPageGrid)Package.GetDialogPage(typeof(Options.OptionPageGrid));
                assemblyVersionType = page.AssemblyVersionType;
            }
            catch { }
            return assemblyVersionType;
        }

        private static void GetVersion(string filename, out string majorVersion, out string minorVersion)
        {
            if (!File.Exists(filename))
                throw new Exception(string.Format("File does not exist: {0}", filename));

            majorVersion = "1";
            minorVersion = "0";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            XmlNode node = xmlDoc.SelectSingleNode("//Version");
            if (null == node)
                throw new Exception(string.Format("Version node not found in file={0}", filename));
            string versionFormat = node.InnerText;

            string[] format = versionFormat.Split(new string[] { "." }, StringSplitOptions.None);

            if (format.Length > 0) majorVersion = format[0];
            if (format.Length > 1) minorVersion = format[1];

        }

        private static void GetVersion2(string filename, out string majorVersion, out string minorVersion, out string release, out string build)
        {
            if (!File.Exists(filename))
                throw new Exception(string.Format("File does not exist: {0}", filename));

            majorVersion = "1";
            minorVersion = "0";
            release = "0";
            build = "0";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);

            XmlNode node = xmlDoc.SelectSingleNode("//Version");
            if (null == node)
                throw new Exception(string.Format("Version node not found in file={0}", filename));
            string versionFormat = node.InnerText;

            string[] format = versionFormat.Split(new string[] { "." }, StringSplitOptions.None);

            if (format.Length > 0) majorVersion = format[0];
            if (format.Length > 1) minorVersion = format[1];
            if (format.Length > 2) release = format[2];
            if (format.Length > 3) build = format[3];

        }

        private static BizTalkBindingToolDb GetBizTalkBindingToolDb()
        {
            BizTalkBindingToolDb bindingToolDb = new BizTalkBindingToolDb();
            try
            {
                Options.OptionPageGrid page = (Options.OptionPageGrid)Package.GetDialogPage(typeof(Options.OptionPageGrid));
                bindingToolDb.Server = page.BizTalkBindingToolDbServer;
                bindingToolDb.Database = page.BizTalkBindingToolDbDatabase;
            }
            catch { }
            return bindingToolDb;
        }

        private static bool GetGeneratedBindings()
        {
            bool generate = false;
            try
            {
                Options.OptionPageGrid page = (Options.OptionPageGrid)Package.GetDialogPage(typeof(Options.OptionPageGrid));
                generate = page.GenerateBindings;
            }
            catch { }
            return generate;
        }

        private static string GetSSOCompanyName()
        {
            try
            {
                Options.OptionPageGrid page = (Options.OptionPageGrid)Package.GetDialogPage(typeof(Options.OptionPageGrid));
                return page.CompanyName;
            }
            catch { }
            return string.Empty;
        }
        #endregion
    }
}