using System;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace BizTalk.BuildGenerator.Tasks.BAM
{
    /// <summary>
    /// This is a base class for tasks which will use BM.exe
    /// </summary>
    public class BaseBmTask : Task
    {
        public BaseBmTask()
        {
            BmPath = @"c:\Program Files\Microsoft BizTalk Server 2009\Tracking\BM";
        }

        /// <summary>
        /// The name of the database to pass to BM.exe, this is optional
        /// </summary>
        public string Database { get; set; }

        /// <summary>
        /// The name of the database server to pass to BM.exe, this is optional
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// The path to BM.exe, this is optional
        /// </summary>
        public string BmPath { get; set; }

        /// <summary>
        /// Executes BM.exe with a set of arguments
        /// </summary>
        /// <param name="arguments"></param>
        protected bool ExecuteBm(string arguments)
        {
            var exec = new Microsoft.Build.Tasks.Exec
                           {
                               BuildEngine = BuildEngine,
                               Command = "\"" + BmPath + "\" " + arguments
                           };

            if (!string.IsNullOrEmpty(Database))
                exec.Command += string.Format(" -Database:{0}", Database);
            if(!string.IsNullOrEmpty(Server))
                exec.Command += string.Format(" -Server:{0}", Server);

            Trace.WriteLine("Executing command: " + exec.Command);
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
