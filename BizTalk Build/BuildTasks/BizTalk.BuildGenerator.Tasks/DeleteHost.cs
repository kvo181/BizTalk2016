using System;
using Microsoft.Build.Utilities;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to delete hosts
    /// </summary>
    public class DeleteHost : Task
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
                HostsHelper.GetHostSettingsObject(HostName).Delete();
                return true;
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Delete host failed: " + ex);
                throw;
            }
        }        
    }
}
