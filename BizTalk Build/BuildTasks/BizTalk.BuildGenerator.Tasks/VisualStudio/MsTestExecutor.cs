using System;
using System.Linq;
using System.Text;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;
using System.Diagnostics;

namespace BizTalk.BuildGenerator.Tasks.VisualStudio
{
    /// <summary>
    /// Executes MsTest to run tests for your BizTalk solution
    /// </summary>
    public class MsTestExecutor : Task
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public MsTestExecutor()
        {
            var basePath = Environment.GetEnvironmentVariable("VS100COMNTOOLS");
            MsTestPath = string.Format("\"{0}..\\IDE\\mstest.exe\" ", basePath);
            Trace.WriteLine("MsTest Path: " + MsTestPath);
        }

        /// <summary>
        /// The paths to the assemblies to test
        /// </summary>
        public ITaskItem[] TestAssemblyPaths { get; set; }

        /// <summary>
        /// The path to the test run config
        /// </summary>
        public string TestRunConfigPath { get; set; }

        /// <summary>
        /// Optionally specify a path for MsTest
        /// </summary>
        public string MsTestPath { get; set; }

        /// <summary>
        /// Executes the task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {            
            var sb = new StringBuilder();
            sb.Append(MsTestPath);

            //Add test containers
            if (TestAssemblyPaths != null)
            {
                foreach (var assemblyPath in TestAssemblyPaths.Select(taskItem => taskItem.ItemSpec))
                {
                    sb.AppendFormat("/testcontainer:\"{0}\" ", assemblyPath);
                }
            }
            
            //Add test run config
            if (!string.IsNullOrEmpty(TestRunConfigPath))
                sb.AppendFormat("/runconfig:\"{0}\"", TestRunConfigPath);

            Trace.WriteLine("Executing command: " + sb);
            var exec = new Microsoft.Build.Tasks.Exec {BuildEngine = BuildEngine, Command = sb.ToString()};

            return exec.Execute();
        }
    }
}
