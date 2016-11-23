using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.Diagnostics;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to start hosts
    /// </summary>
    /// <example>
    /// <BizTalk.BuildGenerator.Tasks.StartHost HostName="$(InProcessHostName)"/>
    /// </example>
    public class StartHost : Task
    {
        #region Properties

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
                var hostObject = HostsHelper.GetHostObject(HostName);
                hostObject.InvokeMethod("Start", null, null);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Start host failed: " + ex);
                throw;
            }
        }
    }
}
