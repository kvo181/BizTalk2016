using System;
using System.Collections.Generic;
using System.Linq;
using EnvDTE100;
using EnvDTE;
using System.Collections;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using System.IO;
using Microsoft.Win32;
using System.Text;
using BizTalkBuildAndDeployLauncherLibrary;

namespace VSIXBizTalkBuildAndDeploy.Helpers
{
    class BizTalkBuildAndDeployHelper
    {
        static OutputWindowPane _owp;
        public static OutputWindowPane Owp
        {
            get
            {
                if (null == _owp)
                    throw new Exception("OutputWindowPane not yet initialized!");
                return _owp;
            }
            set
            {
                if (null == _owp)
                    _owp = value;
            }
        }

        static bool _isBizTalkSolution = false;
        static Solution4 _currentSolution;

        public static void CreateBizTalkBuildAndDeployScript(Package package, Solution4 soln, OutputWindowPane owp)
        {
            Owp = owp;
            Stack projects = new Stack();

            Options.ProjectStructureTypeEnum projectStructureType = BizTalk.BizTalkHelper.ProjectStructureType;
            owp.OutputString(string.Format("Project Structure used {0}", projectStructureType));
            owp.OutputString(Environment.NewLine);
            owp.OutputString(Environment.NewLine);

            // Now you can do what you like with it.
            owp.OutputString(string.Format("{0} - Start listing build dependencies solution {1}", DateTime.Now, soln.FullName));
            owp.OutputString(Environment.NewLine);
            foreach (BuildDependency dep in soln.SolutionBuild.BuildDependencies)
                projects.Push(dep.Project.Name);
            owp.OutputString("List in AddResource order:");
            owp.OutputString(Environment.NewLine);
            foreach (string s in projects)
            {
                owp.OutputString(s);
                owp.OutputString(Environment.NewLine);
            }
            owp.OutputString("Solution count: " + soln.Count);
            owp.OutputString(Environment.NewLine);
            owp.OutputString(string.Format("{0} - End listing build dependencies solution", DateTime.Now));
            owp.OutputString(Environment.NewLine);
            owp.OutputString(Environment.NewLine);

            // Get the BizTalk application name
            string sName = GetBizTalkApplicationName0(soln.Projects);
            if (string.IsNullOrEmpty(sName))
            {
                owp.OutputString(string.Format("Nothing was done since this is no BizTalk solution!!"));
                owp.OutputString(Environment.NewLine);
                owp.OutputString(Environment.NewLine);
                return;
            }

            // The Build folder must exist!
            FileInfo fi = new FileInfo(soln.FullName);
            string path = fi.Directory.FullName + "\\Build";
            if (!Directory.Exists(path))
            {
                owp.OutputString(string.Format("Nothing was done since this folder '{0}' does not exist!!", path));
                owp.OutputString(Environment.NewLine);
                owp.OutputString(Environment.NewLine);
                return;
            }

            GenerationArgs args = new GenerationArgs();

            args.ProjectStructureType = projectStructureType;
            args.AssemblyVersionType = BizTalk.BizTalkHelper.AssemblyVersionType;

            args.BuildReferences = new BizTalk.MetaDataBuildGenerator.MetaData.BuildReferences();
            string tasksPath = BizTalk.BizTalkHelper.TasksPath;
            owp.OutputString(string.Format("TasksPath = '{0}'", tasksPath));
            if (!string.IsNullOrEmpty(tasksPath) && Directory.Exists(tasksPath))
                args.BuildReferences.TasksPath = new DirectoryInfo(tasksPath).FullName.TrimEnd(new char[] { '\\' });
            else
                args.BuildReferences.TasksPath = string.Empty;
            owp.OutputString(string.Format("-> TasksPath = '{0}'", args.BuildReferences.TasksPath));
            owp.OutputString(Environment.NewLine);
            owp.OutputString(Environment.NewLine);

            args.BuildProperties.Properties.AddRange(GetBuildProperties());

            args.SolutionPath = fi.FullName;
            args.OutputFolder = fi.Directory.FullName + "\\Build";

            try
            {
                // Get the application version info
                string majorVersion;
                string minorVersion;
                string release;
                string build;
                BizTalk.BizTalkHelper.GetApplicationVersion(projectStructureType, args.SolutionPath, out majorVersion, out minorVersion, out release, out build);
                args.MajorVersion = majorVersion;
                args.MinorVersion = minorVersion;
                args.Release = release;
                args.Build = build;

                args.ApplicationDescription = new BizTalk.MetaDataBuildGenerator.ApplicationDescription();
                args.ApplicationDescription.Description = string.Format("{0}.{1}.{2}.{3}", majorVersion, minorVersion, release, build);
                args.ApplicationDescription.Name = sName;
                args.ApplicationDescription.Resources = GetBizTalkApplicationResources(soln.Projects);
                args.ApplicationDescription.Resources.AddRange(GetBizTalkExternalAssemblies(args.SolutionPath));
                args.ApplicationDescription.Resources.AddRange(GetBizTalkApplicationBindings(args.SolutionPath));
                args.ApplicationDescription.Resources.AddRange(GetBizTalkApplicationBam(args.SolutionPath));
                args.ApplicationDescription.Resources.AddRange(GetBizTalkApplicationFiles(args.SolutionPath));
                args.ApplicationDescription.ReferencedApplications = GetBizTalkApplicationReferences(args.ApplicationDescription.Name, args.SolutionPath);

                args.ApplicationSetup = new BizTalk.MetaDataBuildGenerator.ApplicationSetup();
                args.ApplicationSetup.IncludeSetup = true;
                args.ApplicationSetup.SetupBuildFilePath = fi.Directory.FullName;

                args.ApplicationBindings = new BizTalk.MetaDataBuildGenerator.ApplicationBindings();
                args.ApplicationBindings.BindingFiles =
                    new List<BizTalk.MetaDataBuildGenerator.ApplicationBinding>();
                BizTalk.MetaDataBuildGenerator.ApplicationBinding appBinding =
                    new BizTalk.MetaDataBuildGenerator.ApplicationBinding();
                appBinding.BindingFilePath = BizTalk.BizTalkHelper.GetBindingFilePath(projectStructureType);
                args.ApplicationBindings.BindingFiles.Add(appBinding);

                args.ApplicationDeployment = new BizTalk.MetaDataBuildGenerator.ApplicationDeployment();
                args.ApplicationDeployment.PublishMsiPath = BizTalk.BizTalkHelper.GetPublishMsiPath(projectStructureType);

                args.BizTalkHosts = new BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHosts();
                args.BizTalkHosts.Hosts = GetBizTalkHosts(args);

                args.SsoApplications = new BizTalk.MetaDataBuildGenerator.SSOApplications();
                args.SsoApplications.SSOApps = GetSSOApps(args);

                args.UnitTesting = new BizTalk.MetaDataBuildGenerator.UnitTesting();
                args.UnitTesting.TestRunConfigPath = "$(SourceCodeRootFolder)\\$(ProductName).testsettings";
                args.UnitTesting.TestContainers = GetUnitTests(soln.Projects);

                args.Rules = GetRules(args);

                IBuildProvider buildProvider = new DefaultBuildProcessController();
                buildProvider.Update += new BuildProcessUpdate(UpdateProgress);
                buildProvider.CreateBuild(owp, args);
            }
            catch (Exception ex)
            {
                owp.OutputString("ERROR!");
                owp.OutputString(Environment.NewLine);
                owp.OutputString(ex.Message);
                owp.OutputString(Environment.NewLine);
            }
        }
        public static void ExecuteBizTalkBuildAndDeployScript(Solution4 soln, OutputWindowPane owp)
        {
            Owp = owp;
            FileInfo fi = new FileInfo(soln.FullName);
            List<string> scripts = GetBizTalkBuildScripts(fi.FullName);

            BizTalkBuildAndDeployLauncher form = new BizTalkBuildAndDeployLauncher(scripts);
            form.UpdateEvent += new EventHandler<UpdateEventArgs>(BizTalkBuildAndDeployLauncher_UpdateEvent);
            form.ShowDialog();

            return;
        }
        /// <summary>
        /// Is this a solution that contains BizTalk projects
        /// </summary>
        /// <param name="soln"></param>
        /// <returns>true or false</returns>
        public static bool IsBizTalkSolution(Solution4 soln)
        {
            if (!soln.IsOpen)
            {
                _currentSolution = null;
                _isBizTalkSolution = false;
            }
            else
            {
                if ((null == _currentSolution) ||
                    (!_currentSolution.FileName.Equals(soln.FileName)) ||
                    (_currentSolution.Projects.Count != soln.Projects.Count))
                {
                    _currentSolution = soln;
                    // Get the BizTalk application name
                    string sName = GetBizTalkApplicationName0(soln.Projects);
                    if (string.IsNullOrEmpty(sName))
                        _isBizTalkSolution = false;
                    else
                        _isBizTalkSolution = true;
                }
            }
            return _isBizTalkSolution;
        }
        static void BizTalkBuildAndDeployLauncher_UpdateEvent(object sender, UpdateEventArgs e)
        {
            _owp.OutputString(e.Message);
            _owp.OutputString(Environment.NewLine);
        }

        private static void UpdateProgress(OutputWindowPane owp, int percentageComplete, string buildFile)
        {
            try
            {
                owp.OutputString(string.Format("{0}: {1:d2} % - {2}", DateTime.Now, percentageComplete, buildFile));
                owp.OutputString(string.Format("\r"));
            }
            catch
            { }
        }

        private static string GetBizTalkApplicationName(Projects projects)
        {
            if (null == projects)
                return null;

            foreach (Project prj in projects)
            {
                _owp.OutputString(string.Format("Project name:{0} - kind:{1} - {2}", prj.Name, prj.Kind, EnvironmentDTE.EnvDTEHelper.GetProjectTypeGuids(prj)));
                _owp.OutputString(Environment.NewLine);
                if (prj.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                    return "Test";
                else
                {
                    foreach (ProjectItem item in prj.ProjectItems)
                    {
                        if (null != item.SubProject)
                        {
                            _owp.OutputString(string.Format("Project name:{0} - kind:{1} - {2}", item.SubProject.Name, item.SubProject.Kind, EnvironmentDTE.EnvDTEHelper.GetProjectTypeGuids(item.SubProject)));
                            _owp.OutputString(Environment.NewLine);
                            if (item.SubProject.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                            {
                                _owp.OutputString(string.Format("Project Properties"));
                                _owp.OutputString(Environment.NewLine);
                                foreach (Property p in item.SubProject.Properties)
                                {
                                    try
                                    {
                                        _owp.OutputString(string.Format("{0} = {1}", p.Name, p.Value));
                                        _owp.OutputString(Environment.NewLine);
                                    }
                                    catch { }
                                }
                                _owp.OutputString(string.Format("Configuration Properties"));
                                _owp.OutputString(Environment.NewLine);
                                foreach (Property p in item.SubProject.ConfigurationManager.ActiveConfiguration.Properties)
                                {
                                    try
                                    {
                                        _owp.OutputString(string.Format("{0}", p.Name));
                                        _owp.OutputString(string.Format(" = {0}", p.Value));
                                        _owp.OutputString(Environment.NewLine);
                                        if (p.Name == "ConfigProperties")
                                        {
                                            try
                                            {
                                                _owp.OutputString(string.Format("Config Properties"));
                                                IDictionary dicConfigProps = p.Value as IDictionary;
                                                if (dicConfigProps != null)
                                                    foreach (KeyValuePair<string, string> kvp in dicConfigProps)
                                                    {
                                                        _owp.OutputString(string.Format("{0} = {1}", kvp.Key, kvp.Value));
                                                        _owp.OutputString(Environment.NewLine);
                                                    }
                                            }
                                            catch { }
                                        }
                                    }
                                    catch { }
                                }

                                return "Test";
                            }
                        }
                    }
                }
            }
            return string.Empty;
        }
        private static string GetBizTalkApplicationName(Project prj)
        {
            foreach (ProjectItem item in prj.ProjectItems)
            {
                if (null != item.SubProject)
                {
                    _owp.OutputString(string.Format("Project name:{0} - kind:{1} - {2}", item.SubProject.Name, item.SubProject.Kind, EnvironmentDTE.EnvDTEHelper.GetProjectTypeGuids(item.SubProject)));
                    _owp.OutputString(Environment.NewLine);
                    if (item.SubProject.Kind == "{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}")
                        return "Test";
                    else
                        return GetBizTalkApplicationName(item.SubProject);
                }
            }
            return string.Empty;
        }

        private static string GetBizTalkApplicationName0(Project prj)
        {
            if (prj.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in prj.ProjectItems)
                {
                    if (null != item.SubProject)
                        return GetBizTalkApplicationName0(item.SubProject);
                }
            }
            else
            {
                IVsSolution globalService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (globalService != null)
                {
                    IVsHierarchy ppHierarchy = null;
                    globalService.GetProjectOfUniqueName(prj.UniqueName, out ppHierarchy);
                    // TODO
                    if (ppHierarchy is Microsoft.VisualStudio.BizTalkProject.IBizTalkProject)
                    {
                        Microsoft.VisualStudio.BizTalkProject.IBizTalkProject btprj =
                            ppHierarchy as Microsoft.VisualStudio.BizTalkProject.IBizTalkProject;
                        IVsBuildPropertyStorage buildPropertyStorage = ppHierarchy as IVsBuildPropertyStorage;
                        string sName = null;
                        buildPropertyStorage.GetPropertyValue("ApplicationName", "Debug", (uint)_PersistStorageType.PST_USER_FILE, out sName);
                        if (null != _owp)
                        {
                            _owp.OutputString(string.Format("Found BizTalk application name '{0}' in project {1}", sName, prj.UniqueName));
                            _owp.OutputString(Environment.NewLine);
                            _owp.OutputString(Environment.NewLine);
                        }
                        return sName;
                    }
                }
            }
            return null;
        }
        private static string GetBizTalkApplicationName0(Projects projects)
        {
            string sApplicationName = null;
            foreach (Project prj in projects)
            {
                // This will fail when the solution contains no BizTalk project
                try
                {
                    sApplicationName = GetBizTalkApplicationName0(prj);
                    if (!string.IsNullOrEmpty(sApplicationName))
                        return sApplicationName;
                }
                catch { }
            }
            return null;
        }

        private static List<BizTalk.MetaDataBuildGenerator.ApplicationResource> GetBizTalkApplicationResources(Projects projects)
        {
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources =
                new List<BizTalk.MetaDataBuildGenerator.ApplicationResource>();
            foreach (Project prj in projects)
            {
                applicationResources.AddRange(GetBizTalkApplicationResources(prj));
            }
            SetDependantResources(applicationResources);

            // 1. Get the non-BizTalk assemblies first
            // 2. Get the BizTalk assemblies
            BizTalk.ApplicationResourceSorter.Sort(applicationResources);

            // 3. Add the binding files

            _owp.OutputString(string.Format("{0} - Start listing Application Resources", DateTime.Now));
            _owp.OutputString("List in AddResource order");
            _owp.OutputString(Environment.NewLine);
            foreach (BizTalk.MetaDataBuildGenerator.ApplicationResource res in applicationResources)
            {
                var name = (from a in res.Properties
                            where a.Name == "OutputFileName"
                            select a.Value).FirstOrDefault();
                if (string.IsNullOrEmpty((string)name)) continue;
                _owp.OutputString(res.FullName + ": " + (string)name);
                _owp.OutputString(Environment.NewLine);
            }
            _owp.OutputString("Resources count: " + applicationResources.Count);
            _owp.OutputString(Environment.NewLine);
            _owp.OutputString(string.Format("{0} - End listing Application Resources", DateTime.Now));
            _owp.OutputString(Environment.NewLine);
            _owp.OutputString(Environment.NewLine);

            return applicationResources;
        }
        private static List<BizTalk.MetaDataBuildGenerator.ApplicationResource> GetBizTalkApplicationResources(Project prj)
        {
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources =
                new List<BizTalk.MetaDataBuildGenerator.ApplicationResource>();
            if (prj.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in prj.ProjectItems)
                {
                    if (null != item.SubProject)
                        applicationResources.AddRange(GetBizTalkApplicationResources(item.SubProject));
                }
            }
            else if (prj.Kind == EnvDTE.Constants.vsProjectKindMisc)
            {
                foreach (ProjectItem item in prj.ProjectItems)
                {
                    if (null != item.SubProject)
                        applicationResources.AddRange(GetBizTalkApplicationResources(item.SubProject));
                }
            }
            else if (!SkipProjectType(prj))
            {
                IVsSolution globalService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (globalService != null)
                {
                    IVsHierarchy ppHierarchy = null;
                    globalService.GetProjectOfUniqueName(prj.UniqueName, out ppHierarchy);

                    BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource =
                        new BizTalk.MetaDataBuildGenerator.ApplicationResource();
                    if (ppHierarchy is Microsoft.VisualStudio.BizTalkProject.IBizTalkProject)
                    {
                        applicationResource.FullName = "System.BizTalk:BizTalkAssembly";
                        applicationResource.Type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.BizTalkAssembly;
                    }
                    else
                    {
                        applicationResource.FullName = "System.BizTalk:Assembly";
                        applicationResource.Type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.Assembly;
                    }

                    BizTalk.MetaDataBuildGenerator.ResourceProperty property = null;
                    bool bSkip = false;
                    foreach (Property p in prj.Properties)
                    {
                        property = null;
                        switch (p.Name)
                        {
                            case "FullPath":
                                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                                property.Name = "FullPath";
                                property.Value = p.Value;
                                break;
                            case "OutputFileName":
                                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                                property.Name = "OutputFileName";
                                property.Value = p.Value;
                                break;
                            case "DefaultNamespace":
                                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                                property.Name = "DefaultNamespace";
                                property.Value = p.Value;
                                break;
                            case "OutputType":
                                if ((int)p.Value != 2)
                                    bSkip = true;
                                break;
                        }
                        if (null != property)
                            applicationResource.Properties.Add(property);
                    }

                    IVsBuildPropertyStorage buildPropertyStorage = ppHierarchy as IVsBuildPropertyStorage;
                    string sName = null;
                    buildPropertyStorage.GetPropertyValue("OutputPath", "Debug", (uint)_PersistStorageType.PST_PROJECT_FILE, out sName);
                    if (null != sName)
                    {
                        property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                        property.Name = "OutputPath";
                        property.Value = sName;
                        applicationResource.Properties.Add(property);
                    }

                    // Get the Project References
                    VSLangProj80.VSProject2 csPrj = prj.Object as VSLangProj80.VSProject2;
                    if (null != csPrj)
                        foreach (VSLangProj80.Reference3 prjRef in csPrj.References)
                            applicationResource.References.Add(prjRef.Name);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "SourceLocation";
                    property.Value = GetSourceLocation(applicationResource.Properties);
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "DestinationLocation";
                    property.Value = GetDestinationLocation(applicationResource.Properties);
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "UpdateGacOnImport";
                    property.Value = "true";
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "Gacutil";
                    property.Value = "true";
                    applicationResource.Properties.Add(property);

                    // Add Resource
                    if (!bSkip) applicationResources.Add(applicationResource);

                }
            }
            return applicationResources;
        }
        private static List<BizTalk.MetaDataBuildGenerator.UnitTestContainer> GetUnitTests(Projects projects)
        {
            List<BizTalk.MetaDataBuildGenerator.UnitTestContainer> unitTests =
                new List<BizTalk.MetaDataBuildGenerator.UnitTestContainer>();
            foreach (Project prj in projects)
            {
                unitTests.AddRange(GetUnitTests(prj));
            }
            foreach (var unitTest in unitTests)
            {
                _owp.OutputString(string.Format("Found UnitTest {0}", unitTest.Location));
                _owp.OutputString(Environment.NewLine);
            }
            if (unitTests.Count > 0)
                _owp.OutputString(Environment.NewLine);
            return unitTests;
        }
        private static List<BizTalk.MetaDataBuildGenerator.UnitTestContainer> GetUnitTests(Project prj)
        {
            List<BizTalk.MetaDataBuildGenerator.UnitTestContainer> unitTests =
                new List<BizTalk.MetaDataBuildGenerator.UnitTestContainer>();
            if (prj.Kind == EnvDTE80.ProjectKinds.vsProjectKindSolutionFolder)
            {
                foreach (ProjectItem item in prj.ProjectItems)
                {
                    if (null != item.SubProject)
                        unitTests.AddRange(GetUnitTests(item.SubProject));
                }
            }
            else if (prj.Kind == EnvDTE.Constants.vsProjectKindMisc)
            {
                foreach (ProjectItem item in prj.ProjectItems)
                {
                    if (null != item.SubProject)
                        unitTests.AddRange(GetUnitTests(item.SubProject));
                }
            }
            else if (UnitTestProjectType(prj))
            {
                IVsSolution globalService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (globalService != null)
                {
                    IVsHierarchy ppHierarchy = null;
                    globalService.GetProjectOfUniqueName(prj.UniqueName, out ppHierarchy);

                    BizTalk.MetaDataBuildGenerator.UnitTestContainer unitTest =
                        new BizTalk.MetaDataBuildGenerator.UnitTestContainer();
                    List<BizTalk.MetaDataBuildGenerator.ResourceProperty> props =
                        new List<BizTalk.MetaDataBuildGenerator.ResourceProperty>();

                    BizTalk.MetaDataBuildGenerator.ResourceProperty property = null;
                    // TODO - what is the purpose of bSkip?
                    bool bSkip = false;
                    foreach (Property p in prj.Properties)
                    {
                        property = null;
                        switch (p.Name)
                        {
                            case "FullPath":
                                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                                property.Name = "FullPath";
                                property.Value = p.Value;
                                break;
                            case "OutputFileName":
                                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                                property.Name = "OutputFileName";
                                property.Value = p.Value;
                                break;
                            case "DefaultNamespace":
                                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                                property.Name = "DefaultNamespace";
                                property.Value = p.Value;
                                break;
                            case "OutputType":
                                if ((int)p.Value != 2)
                                    bSkip = true;
                                break;
                        }
                        if (null != property)
                            props.Add(property);
                    }

                    IVsBuildPropertyStorage buildPropertyStorage = ppHierarchy as IVsBuildPropertyStorage;
                    string sName = null;
                    buildPropertyStorage.GetPropertyValue("OutputPath", "Debug", (uint)_PersistStorageType.PST_PROJECT_FILE, out sName);
                    if (null != sName)
                    {
                        property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                        property.Name = "OutputPath";
                        property.Value = sName;
                        props.Add(property);
                    }

                    unitTest.Location = GetSourceLocation(props);

                    // Add Test
                    unitTests.Add(unitTest);

                }
            }
            return unitTests;
        }

        private static List<BizTalk.MetaDataBuildGenerator.ApplicationResource> GetBizTalkApplicationBindings(string sourceCodeRootFolder)
        {
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources =
                new List<BizTalk.MetaDataBuildGenerator.ApplicationResource>();
            string bindingsPath = BizTalk.BizTalkHelper.GetBindingsPath(BizTalk.BizTalkHelper.ProjectStructureType, sourceCodeRootFolder);
            DirectoryInfo di = new DirectoryInfo(bindingsPath);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, bindings were skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return applicationResources;
            }

            DirectoryInfo[] diArray = di.GetDirectories();
            foreach (DirectoryInfo diItem in diArray)
            {
                FileInfo[] fiArray = diItem.GetFiles("*.xml");
                if (fiArray.Length == 0) continue;

                BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource =
                    new BizTalk.MetaDataBuildGenerator.ApplicationResource();
                applicationResource.FullName = "System.BizTalk:BizTalkBinding";
                applicationResource.Type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.BizTalkBinding;

                BizTalk.MetaDataBuildGenerator.ResourceProperty property =
                    new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                property.Name = "TargetEnvironment";
                property.Value = diItem.Name;
                applicationResource.Properties.Add(property);

                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                property.Name = "SourceLocation";
                property.Value = fiArray[0].FullName;
                applicationResource.Properties.Add(property);

                _owp.OutputString(string.Format("Found binding for environment {0}, {1}", applicationResource.Properties[0].Value, applicationResource.Properties[1].Value));
                _owp.OutputString(Environment.NewLine);

                applicationResources.Add(applicationResource);
            }

            _owp.OutputString(Environment.NewLine);
            return applicationResources;
        }
        private static List<BizTalk.MetaDataBuildGenerator.ApplicationResource> GetBizTalkApplicationBam(string sourceCodeRootFolder)
        {
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources =
                new List<BizTalk.MetaDataBuildGenerator.ApplicationResource>();
            string path = BizTalk.BizTalkHelper.GetBamPath(BizTalk.BizTalkHelper.ProjectStructureType, sourceCodeRootFolder);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, BAM is skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return applicationResources;
            }

            FileInfo[] fiArray = di.GetFiles("*Activity.xml");
            if (fiArray.Length == 0) return applicationResources;

            foreach (FileInfo fi in fiArray)
            {
                BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource =
                    new BizTalk.MetaDataBuildGenerator.ApplicationResource();
                applicationResource.FullName = "System.BizTalk:Bam";
                applicationResource.Type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.Bam;

                BizTalk.MetaDataBuildGenerator.ResourceProperty property =
                    new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                property.Name = "SourceLocation";
                property.Value = fi.FullName;
                applicationResource.Properties.Add(property);

                property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                property.Name = "DestinationLocation";
                property.Value = "%BTAD_InstallDir%\\BAM\\" + fi.Name;
                applicationResource.Properties.Add(property);

                _owp.OutputString(string.Format("Found BAM activity {0}", applicationResource.Properties[0].Value));
                _owp.OutputString(Environment.NewLine);

                applicationResources.Add(applicationResource);
            }

            _owp.OutputString(Environment.NewLine);
            return applicationResources;
        }
        private static List<BizTalk.MetaDataBuildGenerator.ApplicationResource> GetBizTalkApplicationFiles(string sourceCodeRootFolder)
        {
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources =
                new List<BizTalk.MetaDataBuildGenerator.ApplicationResource>();
            string path = BizTalk.BizTalkHelper.GetFilesPath(BizTalk.BizTalkHelper.ProjectStructureType, sourceCodeRootFolder);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, resources are skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return applicationResources;
            }

            FileInfo[] fiArray = di.GetFiles();
            if (fiArray.Length > 0)
            {
                foreach (FileInfo fi in fiArray)
                {
                    BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource =
                        new BizTalk.MetaDataBuildGenerator.ApplicationResource();
                    applicationResource.FullName = "System.BizTalk:File";
                    applicationResource.Type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.File;

                    BizTalk.MetaDataBuildGenerator.ResourceProperty property =
                        new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "SourceLocation";
                    property.Value = fi.FullName;
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "DestinationLocation";
                    property.Value = "%BTAD_InstallDir%\\Resources\\" + fi.Name;
                    applicationResource.Properties.Add(property);

                    _owp.OutputString(string.Format("Found resource {0}", applicationResource.Properties[0].Value));
                    _owp.OutputString(Environment.NewLine);

                    applicationResources.Add(applicationResource);
                }
            }

            // We can have a Post-Processing and Pre-Processing sub folder for the parent of the Resources folder
            DirectoryInfo[] diScripts = di.Parent.GetDirectories("*Processing");
            if (diScripts.Length > 0)
            {
                foreach (DirectoryInfo diScript in diScripts)
                {
                    string fullName;
                    string type;
                    switch (diScript.Name)
                    {
                        case "Pre-Processing":
                            fullName = "System.BizTalk:PreProcessingScript";
                            type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.PreProcessingScript;
                            break;
                        case "Post-Processing":
                            fullName = "System.BizTalk:PostProcessingScript";
                            type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.PostProcessingScript;
                            break;
                        default:
                            continue;
                    }
                    FileInfo[] fiScriptsArray1 = diScript.GetFiles("*.bat");
                    FileInfo[] fiScriptsArray2 = diScript.GetFiles("*.cmd");
                    List<FileInfo> fiList = fiScriptsArray1.ToList();
                    fiList.AddRange(fiScriptsArray2.ToList());
                    FileInfo[] fiScriptsArray = fiList.ToArray();
                    if (fiScriptsArray.Length == 0) continue;

                    foreach (FileInfo fi in fiScriptsArray)
                    {
                        BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource =
                            new BizTalk.MetaDataBuildGenerator.ApplicationResource();
                        applicationResource.FullName = fullName;
                        applicationResource.Type = type;

                        BizTalk.MetaDataBuildGenerator.ResourceProperty property =
                            new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                        property.Name = "SourceLocation";
                        property.Value = fi.FullName;
                        applicationResource.Properties.Add(property);

                        property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                        property.Name = "DestinationLocation";
                        property.Value = "%BTAD_InstallDir%\\Resources\\Scripts\\" + fi.Name;
                        applicationResource.Properties.Add(property);

                        _owp.OutputString(string.Format("Found {0} script {1}", applicationResource.Type, applicationResource.Properties[0].Value));
                        _owp.OutputString(Environment.NewLine);

                        applicationResources.Add(applicationResource);
                    }
                }
            }

            _owp.OutputString(Environment.NewLine);
            return applicationResources;
        }
        private static List<BizTalk.MetaDataBuildGenerator.SSOApplication> GetSSOApps(GenerationArgs args)
        {
            List<BizTalk.MetaDataBuildGenerator.SSOApplication> ssoApps =
                new List<BizTalk.MetaDataBuildGenerator.SSOApplication>();
            if (null == args.SsoApplications)
                return ssoApps;

            string path = BizTalk.BizTalkHelper.GetSsoPath(BizTalk.BizTalkHelper.ProjectStructureType, args.SolutionPath);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, SSO is skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return ssoApps;
            }

            FileInfo[] fiArray = di.GetFiles("*.xml");
            if (fiArray.Length == 0) return ssoApps;

            foreach (FileInfo fi in fiArray)
            {
                BizTalk.MetaDataBuildGenerator.SSOApplication ssoApp =
                    new BizTalk.MetaDataBuildGenerator.SSOApplication();
                ssoApp.CompanyName = BizTalk.BizTalkHelper.CompanyName;
                ssoApp.Path = fi.DirectoryName;
                ssoApp.Name = fi.Name.Replace(fi.Extension, string.Empty);
                ssoApp.Destination = "%BTAD_InstallDir%\\Resources\\SSO\\" + fi.Name;
                ssoApps.Add(ssoApp);
                _owp.OutputString(string.Format("Found SSO {0}", fi.FullName));
                _owp.OutputString(Environment.NewLine);
            }
            if (ssoApps.Count > 0)
                _owp.OutputString(Environment.NewLine);
            return ssoApps;
        }
        private static BizTalk.MetaDataBuildGenerator.Rules GetRules(GenerationArgs args)
        {
            BizTalk.MetaDataBuildGenerator.Rules rules = new BizTalk.MetaDataBuildGenerator.Rules();

            string path = BizTalk.BizTalkHelper.GetRulesPath(BizTalk.BizTalkHelper.ProjectStructureType, args.SolutionPath);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, policies are skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return rules;
            }

            // Get the policies to deploy
            FileInfo[] fiArray = di.GetFiles("Policy__*.xml");
            if (fiArray.Length > 0)
            {
                rules.Policies = new List<BizTalk.MetaDataBuildGenerator.Policy>();
                foreach (FileInfo fi in fiArray)
                {
                    _owp.OutputString(string.Format("Found policy {0}", fi.FullName));
                    _owp.OutputString(Environment.NewLine);
                    rules.Policies.Add(BizTalk.MetaDataBuildGenerator.Policy.Create(fi.FullName));
                }
            }

            // Get the vocabularies to deploy
            fiArray = di.GetFiles("Vocabulary__*.xml");
            if (fiArray.Length == 0) return rules;

            rules.Vocabularies = new List<BizTalk.MetaDataBuildGenerator.Vocabulary>();
            foreach (FileInfo fi in fiArray)
            {
                _owp.OutputString(string.Format("Found vocabulary {0}", fi.FullName));
                _owp.OutputString(Environment.NewLine);
                rules.Vocabularies.Add(BizTalk.MetaDataBuildGenerator.Vocabulary.Create(fi.FullName));
            }
            if (rules.Policies.Count > 0 || rules.Vocabularies.Count > 0)
                _owp.OutputString(Environment.NewLine);
            return rules;
        }
        private static List<BizTalk.MetaDataBuildGenerator.ApplicationResource> GetBizTalkExternalAssemblies(string sourceCodeRootFolder)
        {
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources =
                new List<BizTalk.MetaDataBuildGenerator.ApplicationResource>();
            string path = BizTalk.BizTalkHelper.GetExternalAssembliesPath(BizTalk.BizTalkHelper.ProjectStructureType, sourceCodeRootFolder);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, external assemblies were skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return applicationResources;
            }

            FileInfo[] fiArray = di.GetFiles("*.dll");
            if (fiArray.Length > 0)
            {
                foreach (FileInfo fi in fiArray)
                {
                    BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource =
                        new BizTalk.MetaDataBuildGenerator.ApplicationResource();
                    applicationResource.FullName = "System.BizTalk:Assembly";
                    applicationResource.Type = BizTalk.MetaDataBuildGenerator.MetaData.ResourceTypes.Assembly;

                    BizTalk.MetaDataBuildGenerator.ResourceProperty property =
                        new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "SourceLocation";
                    property.Value = fi.FullName;
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "DestinationLocation";
                    property.Value = "%BTAD_InstallDir%\\" + fi.Name;
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "UpdateGacOnImport";
                    property.Value = "true";
                    applicationResource.Properties.Add(property);

                    property = new BizTalk.MetaDataBuildGenerator.ResourceProperty();
                    property.Name = "Gacutil";
                    property.Value = "true";
                    applicationResource.Properties.Add(property);

                    _owp.OutputString(string.Format("Found external assembly for environment {0}, {1}", applicationResource.Properties[0].Value, applicationResource.Properties[1].Value));
                    _owp.OutputString(Environment.NewLine);

                    applicationResources.Add(applicationResource);
                }
            }

            _owp.OutputString(Environment.NewLine);
            return applicationResources;
        }
        private static List<string> GetBizTalkBuildScripts(string sourceCodeRootFolder)
        {
            List<string> scripts = new List<string>();
            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(sourceCodeRootFolder) + "\\Build");
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, build scripts cannot be executed!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return scripts;
            }

            FileInfo[] fiArray = di.GetFiles("*.cmd");
            if (fiArray.Length == 0) return scripts;

            foreach (FileInfo fi in fiArray)
            {
                _owp.OutputString(string.Format("Found build script {0}", fi.FullName));
                _owp.OutputString(Environment.NewLine);
                scripts.Add(fi.FullName);
            }

            _owp.OutputString(Environment.NewLine);
            return scripts;
        }

        private static string GetSourceLocation(List<BizTalk.MetaDataBuildGenerator.ResourceProperty> props)
        {
            string sPath = null;
            string sOutputPath = null;
            string sOutputFilename = null;
            foreach (BizTalk.MetaDataBuildGenerator.ResourceProperty p in props)
            {
                switch (p.Name)
                {
                    case "FullPath":
                        sPath = (string)p.Value;
                        break;
                    case "OutputPath":
                        sOutputPath = (string)p.Value;
                        break;
                    case "OutputFileName":
                        sOutputFilename = (string)p.Value;
                        break;
                }
            }
            return sPath + sOutputPath + sOutputFilename;
        }
        private static string GetDestinationLocation(List<BizTalk.MetaDataBuildGenerator.ResourceProperty> props)
        {
            string sOutputFilename = null;
            bool isPipelineComponent = false;
            foreach (BizTalk.MetaDataBuildGenerator.ResourceProperty p in props)
            {
                switch (p.Name)
                {
                    case "OutputFileName":
                        sOutputFilename = (string)p.Value;
                        break;
                    case "DefaultNamespace":
                        isPipelineComponent = ((string)p.Value).Contains("PipelineComponent");
                        break;
                }
            }
            if (!isPipelineComponent)
                return "%BTAD_InstallDir%\\" + sOutputFilename;
            else
                return BizTalkInstallationPath + "Pipeline Components\\" + sOutputFilename;
        }

        /// <summary>
        /// Some project types are not included in the MSBUILD script
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        private static bool SkipProjectType(Project prj)
        {
            // {A7AD58EA-4E85-42B2-A802-6540251CA5EF} = Console App
            // {978C614F-708E-4E1A-B201-565925725DBA} = Deployment setup
            // {54435603-DBB4-11D2-8724-00A0C9A8B90C} = vdproj
            // {c8d11400-126e-41cd-887f-60bd40844f9e} = dbproj
            // {3AC096D0-A1C2-E12C-1390-A8335801FDAB} = test
            // Web Application {349C5851-65DF-11DA-9384-00065B846F21} 
            // Windows Communication Foundation (WCF) {3D9AD99F-2412-4246-B90B-4EAA41C64699} 
            // Web Site {E24C65DC-7377-472B-9ABA-BC803B73C61A}
            // See http://www.mztools.com/articles/2008/mz2008017.aspx
            string[] projectTypesToSkip = new string[] { "{A7AD58EA-4E85-42B2-A802-6540251CA5EF}"
                ,"{978C614F-708E-4E1A-B201-565925725DBA}"
                ,"{54435603-DBB4-11D2-8724-00A0C9A8B90C}"
                ,"{C8D11400-126E-41CD-887F-60BD40844F9E}"
                ,"{3AC096D0-A1C2-E12C-1390-A8335801FDAB}"
                ,"{349C5851-65DF-11DA-9384-00065B846F21}"
                ,"{3D9AD99F-2412-4246-B90B-4EAA41C64699}"
                ,"{E24C65DC-7377-472B-9ABA-BC803B73C61A}"
            };

            string projectTypeGuids = EnvironmentDTE.EnvDTEHelper.GetProjectTypeGuids(prj);
            if (string.IsNullOrEmpty(projectTypeGuids)) projectTypeGuids = prj.Kind;
            string[] guids = projectTypeGuids.Split(new string[] { ";" }, StringSplitOptions.None);
            foreach (string kind in guids.ToList())
            {
                if (projectTypesToSkip.ToList().Contains(kind.ToUpper()))
                {
                    Owp.OutputString(string.Format("Project name: {0} - Type:{1} ==> Skipped", prj.UniqueName, kind));
                    Owp.OutputString(Environment.NewLine);
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Do we have an UnitTest project type?
        /// </summary>
        /// <param name="prj"></param>
        /// <returns></returns>
        private static bool UnitTestProjectType(Project prj)
        {
            // {3AC096D0-A1C2-E12C-1390-A8335801FDAB} = test
            string[] projectTypes = new string[] { "{3AC096D0-A1C2-E12C-1390-A8335801FDAB}" };

            string projectTypeGuids = EnvironmentDTE.EnvDTEHelper.GetProjectTypeGuids(prj);
            if (string.IsNullOrEmpty(projectTypeGuids)) projectTypeGuids = prj.Kind;
            string[] guids = projectTypeGuids.Split(new string[] { ";" }, StringSplitOptions.None);
            foreach (string kind in guids.ToList())
            {
                if (projectTypes.ToList().Contains(kind))
                {
                    Owp.OutputString(string.Format("Project name: {0} - Type:{1} ==> Test project", prj.UniqueName, kind));
                    Owp.OutputString(Environment.NewLine);
                    return true;
                }
            }
            return false;
        }

        private static void SetDependantResources(
            List<BizTalk.MetaDataBuildGenerator.ApplicationResource> applicationResources)
        {
            foreach (BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource in applicationResources)
            {
                var name = (from a in applicationResource.Properties
                            where a.Name == "OutputFileName"
                            select a.Value).FirstOrDefault();
                if (string.IsNullOrEmpty((string)name)) continue;
                StringBuilder sb = new StringBuilder((string)name);
                sb.Replace(".dll", "#");
                // A dependant resource = resource which has this one included it it's references
                foreach (BizTalk.MetaDataBuildGenerator.ApplicationResource applicationResource2 in applicationResources)
                {
                    if (applicationResource2 != applicationResource)
                    {
                        foreach (string sReference in applicationResource2.References)
                        {
                            if (sb.ToString().Contains(sReference + "#"))
                            {
                                applicationResource.DependantResources.Add(applicationResource2);
                                break;
                            }
                        }
                        //if (applicationResource2.References.Contains(sb.ToString()))
                        //    applicationResource.DependantResources.Add(applicationResource2);
                    }
                }
            }
        }

        /// <summary>
        /// Get the BizTalk installation path
        /// </summary>
        private static string BizTalkInstallationPath
        {
            get
            {
                string installPath = null;
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"Software\Microsoft\BizTalk Server\3.0"))
                    {
                        installPath = (string)key.GetValue("InstallPath");
                    }
                }
                catch { }
                return installPath;
            }
        }

        /// <summary>
        /// Get the list of BizTalk application's referenced by this application
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sourceCodeRootFolder"></param>
        /// <returns></returns>
        private static List<string> GetBizTalkApplicationReferences(string name, string sourceCodeRootFolder)
        {
            string path = BizTalk.BizTalkHelper.GetApplicationReferencesPath(BizTalk.BizTalkHelper.ProjectStructureType, sourceCodeRootFolder);
            DirectoryInfo di = new DirectoryInfo(path);
            if (!di.Exists)
            {
                _owp.OutputString(string.Format("Project structure does not contain the folder {0}, ApplicationReferences are skipped!", di.FullName));
                _owp.OutputString(Environment.NewLine);
                return null;
            }
            return ReadBizTalkApplicationReferences(di.FullName);
        }
        private static void SaveBizTalkApplicationReferences(string sourceCodeRootFolder, List<string> applicationReferences)
        {
            FileInfo fi = new FileInfo(sourceCodeRootFolder + "\\ApplicationReferences.txt");
            StreamWriter w = fi.CreateText();
            foreach (string line in applicationReferences)
                w.WriteLine(line);
            w.Flush();
            w.Close();

            return;
        }
        private static List<string> ReadBizTalkApplicationReferences(string sourceCodeRootFolder)
        {
            List<string> applicationReferences = new List<string>();

            FileInfo fi = new FileInfo(sourceCodeRootFolder + "\\ApplicationReferences.txt");
            if (!fi.Exists) return applicationReferences;

            string line;
            StreamReader r = fi.OpenText();
            while ((line = r.ReadLine()) != null)
            {
                _owp.OutputString(string.Format("Found ApplicationReference {0}", line));
                _owp.OutputString(Environment.NewLine);
                applicationReferences.Add(line);
            }
            r.Close();

            _owp.OutputString(Environment.NewLine);
            return applicationReferences;
        }

        private static List<BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty> GetBuildProperties()
        {
            List<BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty> buildProps =
                new List<BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty>();
            buildProps.Add(new BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty() { PropertyName = "BizTalkDatabaseServerName", PropertyValue = BizTalk.BizTalkHelper.BizTalkDatabaseServerName });
            buildProps.Add(new BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty() { PropertyName = "BizTalkManagementDatabaseName", PropertyValue = BizTalk.BizTalkHelper.BizTalkManagementDatabaseName });
            buildProps.Add(new BizTalk.MetaDataBuildGenerator.MetaData.BuildProperty() { PropertyName = "PipelineComponentsFolderPath", PropertyValue = BizTalk.BizTalkHelper.PipelineComponentsPath });
            return buildProps;
        }
        private static List<BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHost> GetBizTalkHosts(GenerationArgs args)
        {
            List<BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHost> biztalkHosts =
                new List<BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHost>();
            if (null == args.ApplicationBindings)
                return biztalkHosts;

            foreach (BizTalk.MetaDataBuildGenerator.ApplicationBinding binding in args.ApplicationBindings.BindingFiles)
            {
                StringBuilder sb = new StringBuilder(binding.BindingFilePath);
                sb.Replace("$(SourceCodeRootFolder)", new DirectoryInfo(Path.GetDirectoryName(args.SolutionPath)).FullName.TrimEnd(new char[] { '\\' }));
                sb.Replace("$(ProductName)", args.ApplicationDescription.Name);
                BizTalk.BindingInfoHelper helper = new BizTalk.BindingInfoHelper(sb.ToString());
                foreach (string hostName in helper.BizTalkHosts)
                {
                    BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHost host =
                        new BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHost();
                    host.CanCreate = false;
                    host.HostType = BizTalk.MetaDataBuildGenerator.MetaData.BizTalkHostType.InProcess;
                    host.Name = hostName;
                    biztalkHosts.Add(host);
                    _owp.OutputString(string.Format("Found BizTalkHost {0}", hostName));
                    _owp.OutputString(Environment.NewLine);
                }
            }
            if (biztalkHosts.Count > 0)
                _owp.OutputString(Environment.NewLine);
            return biztalkHosts;
        }

    }
}
