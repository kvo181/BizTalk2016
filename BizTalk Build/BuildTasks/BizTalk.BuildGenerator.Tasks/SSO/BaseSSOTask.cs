using Microsoft.Build.Utilities;

namespace BizTalk.BuildGenerator.Tasks.SSO
{
    /// <summary>
    /// Base task for Business Rules Engine Tasks
    /// </summary>
    public class BaseSSOTask : Task
    {
        public BaseSSOTask()
        {
            SSOManagePath = @"C:\Program Files\Common Files\Enterprise Single Sign-On\ssomanage";
        }

        /// <summary>
        /// Executes SSOManage with a set of arguments
        /// </summary>
        /// <param name="arguments"></param>
        protected void ExecuteSsoManage(string arguments)
        {
            var exec = new Microsoft.Build.Tasks.Exec
                           {
                               BuildEngine = BuildEngine,
                               Command = "\"" + SSOManagePath + "\" " + arguments
                           };
            exec.Execute();            
        }

        /// <summary>
        /// The ppath to SSOManage
        /// </summary>
        public string SSOManagePath { get; set; }

        /// <summary>
        /// Execute
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            return true;
        }
    }
}
