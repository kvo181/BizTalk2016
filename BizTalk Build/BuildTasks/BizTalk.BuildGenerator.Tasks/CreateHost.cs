using System;
using Microsoft.Build.Utilities;
using System.Management;
using System.Diagnostics;
using Microsoft.Build.Framework;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// Task to create hosts
    /// </summary>    
    public class CreateHost : Task
    {
        public CreateHost()
        {
            IsDefault = false;
            HostTracking = false;
            Trusted = true;
        }

        #region Properties

        /// <summary>
        /// Host Type
        /// </summary>
        [Required]
        public string HostType { get; set; }

        /// <summary>
        /// GroupName
        /// </summary>
        [Required]
        public string GroupName { get; set; }

        /// <summary>
        /// Trusted
        /// </summary>
        public bool Trusted { get; set; }

        /// <summary>
        /// Host Tracking
        /// </summary>
        public bool HostTracking { get; set; }

        /// <summary>
        /// IsDefault
        /// </summary>
        public bool IsDefault { get; set; }

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
            var hostType = (HostType)Enum.Parse(typeof(HostType), HostType);

            try
            {
                var options = new PutOptions {Type = PutType.CreateOnly};

                var btsHostSetting = HostsHelper.GetHostSettingClass().CreateInstance();
                if (btsHostSetting != null)
                {
                    btsHostSetting["Name"] = HostName;
                    btsHostSetting["HostType"] = (int)hostType;
                    btsHostSetting["NTGroupName"] = GroupName;
                    btsHostSetting["AuthTrusted"] = Trusted;
                    if (hostType == Tasks.HostType.InProcess)
                    {
                        btsHostSetting.SetPropertyValue("HostTracking", HostTracking);
                        btsHostSetting.SetPropertyValue("IsDefault", IsDefault);
                    }
                    else
                    {
                        if (IsDefault || HostTracking)
                            Trace.WriteLine("Host Type is inprocess so Host Tracking and Is Default will be defaulted to false");
                    }
                    btsHostSetting.Put(options);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("CreateHost - " + HostName + " - failed: " + ex.Message);
                throw;
            }
            return true;
        }


    }
    /// <summary>
    /// Host types
    /// </summary>
    public enum HostType
    {
        /// <summary>
        /// Invalid host type
        /// </summary>
        Invalid,
        /// <summary>
        /// Inprocess host
        /// </summary>
        InProcess,
        /// <summary>
        /// Isolated host lie for example iis
        /// </summary>
        Isolated
    }
}
