using System;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to create hosts
    /// </summary>
    public class CreateHostInstance : Task
    {
        public CreateHostInstance()
        {
            ServerName = Environment.MachineName;
        }

        #region Properties        

        /// <summary>
        /// ServerName
        /// </summary>
        /// <remarks>If not supplied will default to the local machine</remarks>
        public string ServerName { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// HostName
        /// </summary>
        [Required]
        public string HostName { get; set; }

        #endregion

        /// <summary>
        /// Execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {            
            try
            {               
                var serverHostObject = HostsHelper.GetServerHostClass().CreateInstance();
                if (serverHostObject != null)
                {
                    serverHostObject["ServerName"] = ServerName;
                    serverHostObject["HostName"] = HostName;
                    serverHostObject.InvokeMethod("Map", null);
                }
                var hostInstanceObject = HostsHelper.GetHostInstanceClass().CreateInstance();
                if (hostInstanceObject != null)
                    hostInstanceObject["Name"] = string.Format("Microsoft BizTalk Server {0} {1}", HostName, ServerName);
                var args = new object[] { Username, Password };
                if (hostInstanceObject != null) hostInstanceObject.InvokeMethod("Install", args);
            }
            catch (Exception ex)
            {                
                Trace.WriteLine("CreateHostInstance - " + HostName + " - failed: " + ex.Message);
                throw;
            }
            return true;
        }
    }
}
