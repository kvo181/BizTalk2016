using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;
using System.Diagnostics;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <remarks />
    public class GenerateTasksFile : Task
    {
        private string _targetsFilePath;
        private const string MsbuildNamespace = "http://schemas.microsoft.com/developer/msbuild/2003";

        /// <summary>
        /// Any prefix before the assembly name
        /// </summary>
        public string AssemblyPathPrefix { get; set; }

        /// <summary>
        /// The list of assemblies to inspect
        /// </summary>
        [Required]
        public ITaskItem[] AssemblyPaths { get; set; }

        /// <remarks />
        [Required]
        public string TargetsFilePath
        {
            get { return _targetsFilePath; }
            set { _targetsFilePath = value; }
        }
        
        /// <summary>
        /// Implements the execute method
        /// </summary>
        /// <returns></returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "Microsoft.Build.Utilities.TaskLoggingHelper.LogWarning(System.String,System.Object[])")]
        public override bool Execute()
        {
            try
            {
                //System.Diagnostics.Debugger.Launch();
                var tasksFilePath = Path.GetFullPath(_targetsFilePath);

                var targetsFile = new XmlDocument();
                var xmlDeclaration = targetsFile.CreateXmlDeclaration("1.0", "utf-8", null);
                var rootElement = targetsFile.CreateElement("Project");
                rootElement.SetAttribute("xmlns", MsbuildNamespace);
                targetsFile.AppendChild(xmlDeclaration);
                targetsFile.AppendChild(rootElement);
                foreach (var assemblyPathItem in AssemblyPaths)
                {
                    var assemblyPath = assemblyPathItem.ItemSpec;
                    var assemblyAbsolutePath = Path.GetFullPath(assemblyPath);
                    var assembly = Assembly.Load(File.ReadAllBytes(assemblyAbsolutePath));                    
                    var taskNames = GetMsBuildTaskNamesFromAssembly(assembly, Log);
                    foreach (var taskName in taskNames)
                    {
                        var usingTaskElement = targetsFile.CreateElement("UsingTask");
                        var taskNameAttrib = targetsFile.CreateAttribute("TaskName");
                        taskNameAttrib.InnerText = taskName;
                        var assemblyFileAttrib = targetsFile.CreateAttribute("AssemblyFile");
                        if (string.IsNullOrEmpty(AssemblyPathPrefix))
                            assemblyFileAttrib.InnerText = assembly.GetName().Name + ".dll";
                        else
                            assemblyFileAttrib.InnerText = string.Format(@"{0}\{1}.dll", AssemblyPathPrefix, assembly.GetName().Name);

                        usingTaskElement.Attributes.Append(taskNameAttrib);
                        usingTaskElement.Attributes.Append(assemblyFileAttrib);
                        rootElement.AppendChild(usingTaskElement);
                    }                    
                }
                using (var fs = new FileStream(tasksFilePath, FileMode.Create, FileAccess.Write))
                {
                    targetsFile.Save(fs);
                    fs.Flush();
                }

                if (rootElement.ChildNodes.Count < 1) // No MSBuild Tasks have been found
                {
                    if (BuildEngine != null)
                        Log.LogWarning("No MSBuild Tasks were found in the supplied assembly list.");                    
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.LogError(ex.ToString());
                return false;
            }
        }
        /// <summary>
        /// Gets a list of the typenames which derive from Task
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private static IEnumerable<string> GetMsBuildTaskNamesFromAssembly(Assembly assembly, TaskLoggingHelper log)
        {
            try
            {                
                var types = assembly.GetTypes();
                var taskTypeList = new ArrayList();
                foreach (var type in types.Where(type => type.IsSubclassOf(typeof (Task))))
                {
                    taskTypeList.Add(type);
                }
                var taskTypes = (Type[])taskTypeList.ToArray(typeof(Type));
                var taskNames = new string[taskTypes.Length];
                for (var i = 0; i < taskTypes.Length; i++)
                    taskNames[i] = taskTypes[i].FullName;
                return taskNames;
            }
            catch (ReflectionTypeLoadException ex)
            {
                Trace.WriteLine(ex.ToString());
                log.LogError(ex.ToString());
                foreach (var e in ex.LoaderExceptions)
                {
                    Trace.WriteLine(e.ToString());
                    log.LogError(e.ToString());
                }
                return null;
            }
        }       
    }
}