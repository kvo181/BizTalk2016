using System;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to stop hosts
    /// </summary>
    public class StopHost : Task
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
                hostObject.InvokeMethod(@"Stop", null, null);

                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine(@"Stop host failed: " + ex);
                throw;
            }
        }
    }
}
