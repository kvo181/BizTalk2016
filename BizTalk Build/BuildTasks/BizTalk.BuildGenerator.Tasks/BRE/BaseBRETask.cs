using Microsoft.Build.Utilities;

namespace BizTalk.BuildGenerator.Tasks.BRE
{
    /// <summary>
    /// Base task for Business Rules Engine Tasks
    /// </summary>
    public class BaseBRETask : Task
    {
        public BaseBRETask()
        {
            MajorVersion = -1;
            MinorVersion = -1;
        }

        /// <summary>
        /// The minor version number
        /// </summary>
        public int MinorVersion { get; set; }

        /// <summary>
        /// The major version number
        /// </summary>
        public int MajorVersion { get; set; }

        /// <summary>
        /// Indicates if the task is configured to work for a specific version
        /// </summary>
        public bool IsSpecificVersion
        {
            get
            {
                return MajorVersion >= 0 && MinorVersion >= 0;
            }
        }

        /// <summary>
        /// Creates the bre manager
        /// </summary>
        /// <returns></returns>
        protected BREManager CreateManager()
        {
            if (!string.IsNullOrEmpty(DatabaseName) && !string.IsNullOrEmpty(ServerName))
                return new BREManager(DatabaseName, ServerName);
            return new BREManager();
        }

        /// <summary>
        /// Execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            return true;   
        }

        /// <summary>
        /// The server name for the BRE database
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// The database name for the BRE database
        /// </summary>        
        public string DatabaseName { get; set; }
    }
}
