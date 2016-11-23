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
    public class CreateReceiveHandler: Task
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
        public override bool  Execute()
        {
            try
            {
                var options = new PutOptions {Type = PutType.CreateOnly};

                using (var recieveHandlerManagementClass = new ManagementClass("root\\MicrosoftBizTalkServer", "MSBTS_ReceiveHandler", null))
                {
                    foreach (var recieveHandler in
                        recieveHandlerManagementClass.GetInstances().Cast<ManagementObject>().Where(recieveHandler => (string) recieveHandler["AdapterName"] == AdapterName && (string) recieveHandler["HostName"] == HostName))
                    {
                        recieveHandler.Delete();
                    }

                    using (var recieveHandlerManager = recieveHandlerManagementClass.CreateInstance())
                    {
                        if (recieveHandlerManager != null)
                        {
                            recieveHandlerManager["AdapterName"] = AdapterName;
                            recieveHandlerManager["HostName"] = HostName;
                            recieveHandlerManager.Put(options);
                        }
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
