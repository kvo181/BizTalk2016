using System;
using System.Diagnostics;
using System.Linq;
using System.Management;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to create a send handler
    /// </summary>
    public class CreateSendHandler : Task
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
                var options = new PutOptions {Type = PutType.CreateOnly};

                var handlerManagementClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_SendHandler2", null);

                foreach (var handler in
                    handlerManagementClass.GetInstances().Cast<ManagementObject>().Where(handler => (string) handler["AdapterName"] == AdapterName && (string) handler["HostName"] == HostName))
                {
                    handler.Delete();
                }

                var handlerInstance = handlerManagementClass.CreateInstance();
                if (handlerInstance != null)
                {
                    handlerInstance["AdapterName"] = AdapterName;
                    handlerInstance["HostName"] = HostName;
                    handlerInstance.Put(options);
                }

                return true;
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(GetType().FullName, ex.ToString());
                throw;
            }
        }
    }
}
