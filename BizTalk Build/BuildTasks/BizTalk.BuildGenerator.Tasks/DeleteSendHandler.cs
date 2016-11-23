using System;
using System.Linq;
using System.Management;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to create a send handler
    /// </summary>
    public class DeleteSendHandler : Task
    {
        /// <summary>
        /// Host name
        /// </summary>
        [Required]
        public string HostName { get; set; }

        /// <summary>
        /// Adapter name
        /// </summary>
        [Required]
        public string AdapterName { get; set; }

        /// <summary>
        /// The execute method
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            try
            {
                using (var handlerManagementClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_SendHandler2", null))
                {
                    foreach (var handler in
                        handlerManagementClass.GetInstances().Cast<ManagementObject>().Where(handler => (string) handler["AdapterName"] == AdapterName && (string) handler["HostName"] == HostName))
                    {
                        System.Diagnostics.Trace.WriteLine("Found send handler for host " + HostName + " for the adapter " + AdapterName);
                        handler.Delete();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry(GetType().FullName, ex.ToString());
                throw;
            }
        }
    }
}
