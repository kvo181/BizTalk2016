using System;
using Microsoft.Build.Utilities;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// This is a base class for tasks which will use bttdeploy.exe
    /// </summary>
    public class BaseBttDeployTask : Task
    {
        public BaseBttDeployTask()
        {
            BttDeployPath = @"c:\Program Files\Microsoft BizTalk Server 2009\Tracking\bttdeploy";
        }

        public string ManagementDb { get; set; }

        /// <summary>
        /// The path to BTTDeploy.exe, this is optional
        /// </summary>
        public string BttDeployPath { get; set; }

        /// <summary>
        /// Executes BTTDeploy with a set of arguments
        /// </summary>
        /// <param name="arguments"></param>
        protected bool ExecuteBttDeploy(string arguments)
        {
            var exec = new Microsoft.Build.Tasks.Exec
                           {
                               BuildEngine = BuildEngine,
                               Command = "\"" + BttDeployPath + "\" " + arguments
                           };

            if (!string.IsNullOrEmpty(ManagementDb))
                exec.Command += string.Format(" /mgdb {0}", ManagementDb);            

            return exec.Execute();            
        }
        /// <summary>
        /// Execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
