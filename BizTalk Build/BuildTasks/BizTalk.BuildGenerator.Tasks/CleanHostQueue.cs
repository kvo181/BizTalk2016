using System.Collections.Generic;
using Microsoft.Build.Utilities;
using System.Globalization;
using System.Management;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace BizTalk.BuildGenerator.Tasks
{
    /// <summary>
    /// The service status codes
    /// </summary>
    internal class ServiceStatus
    {
        internal const string ReadyToRun = "1";
        internal const string Active = "2";
        internal const string SuspendedResumable = "4";
        internal const string Dehydrated = "8";
        internal const string CompletedWithDiscardedMessages = "16";
        internal const string SuspendedNotResumable = "32";
        internal const string InBreakpoint = "64";
    }
    /// <summary>
    /// Cleans all messages and service instances from a host queue
    /// </summary>
    public class CleanHostQueue : Task
    {
        /// <summary>
        /// The name of the host to clean
        /// </summary>
        public string HostName { get; set; }

        /// <summary>
        /// Executes the build task
        /// </summary>
        /// <returns></returns>
        public override bool Execute()
        {
            Logger.LogMessage(this, @"Executing task: " + GetType().FullName);
            Logger.LogMessage(this, @"Host: " + HostName);
            Clean(HostName);
            Logger.LogMessage(this, @"Finished Executing task: " + GetType().FullName);
            return true;
        }
        /// <summary>
        /// Overrides the execute
        /// </summary>
        /// <returns></returns>
        public void Clean(string hostName)
        {
            var noMessageInstances = CountMessageInstances(hostName);
            if (noMessageInstances > 0)
            {
                SuspendMessageInstances(hostName);
                TerminateMessageInstances(hostName);
                noMessageInstances = CountMessageInstances(hostName);
                if (noMessageInstances > 0)
                    Trace.WriteLine(@"There are still message instances on the queue");
            }

            var noServiceInstances = CountServiceInstances(hostName);
            if (noServiceInstances <= 0) return;
            SuspendServiceInstances(hostName);
            TerminateServiceInstances(hostName);
            noServiceInstances = CountServiceInstances(hostName);
            if (noServiceInstances > 0)
                Trace.WriteLine(@"There are still service instances on the queue");
        }
        /// <summary>
        /// Determines if the instance can be terminated
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static bool CanTerminate(string status)
        {
            return status == ServiceStatus.CompletedWithDiscardedMessages
                   || status == ServiceStatus.SuspendedNotResumable
                   || status == ServiceStatus.SuspendedResumable;
        }

        /// <summary>
        /// Determines if the instance can be terminated
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        private static bool CanSuspend(string status)
        {
            return status != ServiceStatus.CompletedWithDiscardedMessages
                   && status != ServiceStatus.SuspendedNotResumable
                   && status != ServiceStatus.SuspendedResumable;
        }

        /// <summary>
        /// Records to the trace the action
        /// </summary>
        /// <param name="instanceId"></param>
        /// <param name="typeId"></param>
        /// <param name="classId"></param>
        /// <param name="action"></param>
        private void TraceAction(string instanceId, string typeId, string classId, string action)
        {
            var args = new List<string> {instanceId, typeId, classId, action};
            var message = string.Format("Instance ID: {0}, Action: {3}, Type: {1}, Class: {2}", args.ToArray());
            Logger.LogMessage(this, message);
        }
        /// <summary>
        /// Displays the message instances currently on this queue
        /// </summary>
        /// <param name="hostName"></param>
        private int CountMessageInstances(string hostName)
        {
            var count = 0;

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, @"Display Message Instances");
            Logger.LogMessage(this, string.Empty);

            var query = string.Format(CultureInfo.CurrentCulture, @"SELECT * FROM MSBTS_MessageInstance");
            var searcher = new ManagementObjectSearcher(new ManagementScope(@"root\MicrosoftBizTalkServer"), new WqlObjectQuery(query), null);

            foreach (ManagementObject messageInstanceManager in searcher.Get())
            {
                var host = messageInstanceManager[@"HostName"].ToString();
                var status = messageInstanceManager[@"ServiceInstanceStatus"].ToString();

                if (host != hostName) continue;
                var serviceInstanceId = messageInstanceManager[@"ServiceInstanceID"].ToString();
                Logger.LogMessage(this, @"Service Instance: " + serviceInstanceId + @" is " + status);
                count++;
            }

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, count + @" Message Instances in queue");
            Logger.LogMessage(this, string.Empty);
            return count;
        }
        /// <summary>
        /// This will terminate all message instances belonging to a host
        /// </summary>
        /// <param name="hostName"></param>
        private void TerminateMessageInstances(string hostName)
        {
            var count = 0;

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, @"Terminate Message Instances");
            Logger.LogMessage(this, string.Empty);

            var query = string.Format(CultureInfo.CurrentCulture, @"SELECT * FROM MSBTS_MessageInstance");
            var searcher = new ManagementObjectSearcher(new ManagementScope(@"root\MicrosoftBizTalkServer"), new WqlObjectQuery(query), null);

            foreach (ManagementObject messageInstanceManager in searcher.Get())
            {
                var host = messageInstanceManager[@"HostName"].ToString();
                var status = messageInstanceManager[@"ServiceInstanceStatus"].ToString();

                if (host != hostName) continue;
                if (!CanTerminate(status)) continue;
                var serviceInstanceId = messageInstanceManager[@"ServiceInstanceID"].ToString();
                var serviceTypeId = messageInstanceManager[@"ServiceTypeId"].ToString();
                var serviceClassId = messageInstanceManager[@"ServiceTypeId"].ToString();  //For some reason the service class id always seems to return nullref exception and  and the service type seems to work if used instead, dont know why?

                TerminateServiceInstancesById(hostName, serviceInstanceId, serviceClassId, serviceTypeId);
                TraceAction(serviceInstanceId, serviceTypeId, serviceClassId, @"Terminate Message Instance");
                count++;
            }

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, count + @" Message Instances have been terminated");
            Logger.LogMessage(this, string.Empty);
        }
        /// <summary>
        /// This will suspend all message instances belonging to a host
        /// </summary>
        /// <param name="hostName"></param>
        private void SuspendMessageInstances(string hostName)
        {
            var count = 0;

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, @"Suspend Message Instances");
            Logger.LogMessage(this, string.Empty);

            var query = string.Format(CultureInfo.CurrentCulture, @"SELECT * FROM MSBTS_MessageInstance");
            var searcher = new ManagementObjectSearcher(new ManagementScope(@"root\MicrosoftBizTalkServer"), new WqlObjectQuery(query), null);

            foreach (ManagementObject messageInstanceManager in searcher.Get())
            {
                var host = messageInstanceManager[@"HostName"].ToString();
                var status = messageInstanceManager[@"ServiceInstanceStatus"].ToString();

                if (host != hostName) continue;
                if (!CanSuspend(status)) continue;
                var serviceInstanceId = messageInstanceManager[@"ServiceInstanceID"].ToString();
                var serviceTypeId = messageInstanceManager[@"ServiceTypeId"].ToString();
                var serviceClassId = messageInstanceManager[@"ServiceTypeId"].ToString();        //For some reason the service class id always seems to return nullref exception and      and the service type seems to work if used instead, dont know why?             

                SuspendServiceInstancesById(hostName, serviceInstanceId, serviceClassId, serviceTypeId);
                TraceAction(serviceInstanceId, serviceTypeId, serviceClassId, @"Suspend Message Instance");
                count++;
            }

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, count + @" Message Instances have been suspended");
            Logger.LogMessage(this, string.Empty);
        }
        /// <summary>
        /// Suspends all service instances for a host
        /// </summary>
        /// <param name="hostName"></param>
        private int CountServiceInstances(string hostName)
        {
            var count = 0;

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, @"Display Service Instances");
            Logger.LogMessage(this, string.Empty);

            var query = string.Format(CultureInfo.CurrentCulture, @"SELECT * FROM MSBTS_ServiceInstance");
            var searcher = new ManagementObjectSearcher(new ManagementScope(@"root\MicrosoftBizTalkServer"), new WqlObjectQuery(query), null);

            foreach (ManagementObject serviceInstanceManager in searcher.Get())
            {
                var host = serviceInstanceManager[@"HostName"].ToString();
                var status = serviceInstanceManager[@"ServiceStatus"].ToString();

                if (host != hostName) continue;
                var serviceInstanceId = serviceInstanceManager[@"InstanceID"].ToString();
                Logger.LogMessage(this, @"Service Instance: " + serviceInstanceId + @" is " + status);
                count++;
            }

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, count + @" Service Instances currently in queue");
            Logger.LogMessage(this, string.Empty);
            return count;
        }
        /// <summary>
        /// Suspends all service instances for a host
        /// </summary>
        /// <param name="hostName"></param>
        private void SuspendServiceInstances(string hostName)
        {
            var count = 0;

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, @"Suspend Service Instances");
            Logger.LogMessage(this, string.Empty);

            var query = string.Format(CultureInfo.CurrentCulture, @"SELECT * FROM MSBTS_ServiceInstance");
            var searcher = new ManagementObjectSearcher(new ManagementScope(@"root\MicrosoftBizTalkServer"), new WqlObjectQuery(query), null);

            foreach (ManagementObject serviceInstanceManager in searcher.Get())
            {
                var host = serviceInstanceManager[@"HostName"].ToString();
                var status = serviceInstanceManager[@"ServiceStatus"].ToString();

                if (host != hostName) continue;
                if (!CanSuspend(status)) continue;
                var serviceInstanceId = serviceInstanceManager[@"InstanceID"].ToString();
                var serviceTypeId = serviceInstanceManager[@"ServiceTypeId"].ToString();
                var serviceClassId = serviceInstanceManager[@"ServiceTypeId"].ToString();  //For some reason the service class id always seems to return nullref exception and  and the service type seems to work if used instead, dont know why?

                SuspendServiceInstancesById(hostName, serviceInstanceId, serviceClassId, serviceTypeId);
                TraceAction(serviceInstanceId, serviceTypeId, serviceClassId, @"Terminate Message Instance");
                count++;
            }

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, count + @" Service Instances have been suspended");
            Logger.LogMessage(this, string.Empty);
        }
        /// <summary>
        /// Terminates all service instances for a host
        /// </summary>
        /// <param name="hostName"></param>
        private void TerminateServiceInstances(string hostName)
        {
            var count = 0;

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, @"Terminate Service Instances");
            Logger.LogMessage(this, string.Empty);

            var query = string.Format(CultureInfo.CurrentCulture, @"SELECT * FROM MSBTS_ServiceInstance");
            var searcher = new ManagementObjectSearcher(new ManagementScope(@"root\MicrosoftBizTalkServer"), new WqlObjectQuery(query), null);

            foreach (ManagementObject serviceInstanceManager in searcher.Get())
            {
                var host = serviceInstanceManager[@"HostName"].ToString();
                var status = serviceInstanceManager[@"ServiceStatus"].ToString();

                if (host != hostName) continue;
                if (!CanTerminate(status)) continue;
                var serviceInstanceId = serviceInstanceManager[@"InstanceID"].ToString();
                var serviceTypeId = serviceInstanceManager[@"ServiceTypeId"].ToString();
                var serviceClassId = serviceInstanceManager[@"ServiceTypeId"].ToString();  //For some reason the service class id always seems to return nullref exception and the service type seems to work if used instead, dont know why?

                TerminateServiceInstancesById(hostName, serviceInstanceId, serviceClassId, serviceTypeId);
                TraceAction(serviceInstanceId, serviceTypeId, serviceClassId, @"Terminate Service Instance");
                count++;
            }

            Logger.LogMessage(this, string.Empty);
            Logger.LogMessage(this, count + @" Service Instances have been terminated");
            Logger.LogMessage(this, string.Empty);
        }
        /// <summary>
        /// Suspends one service instance
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="serviceInstanceId"></param>
        /// <param name="serviceClassId"></param>
        /// <param name="serviceTypeId"></param>
        private void SuspendServiceInstancesById(string hostName, string serviceInstanceId, string serviceClassId, string serviceTypeId)
        {
            var serviceInstanceIdList = new List<string>();
            var serviceClassIdList = new List<string>();
            var serviceTypeIdList = new List<string>();

            serviceInstanceIdList.Add(serviceInstanceId);
            serviceClassIdList.Add(serviceClassId);
            serviceTypeIdList.Add(serviceTypeId);


            var managementObjectPath = string.Format(CultureInfo.CurrentCulture, "root\\MicrosoftBizTalkServer:MSBTS_HostQueue.HostName=\"{0}\"", hostName);
            var managementObject = new ManagementObject(managementObjectPath);

            try
            {
                managementObject.InvokeMethod(@"SuspendServiceInstancesByID", new object[] { serviceClassIdList.ToArray(), serviceTypeIdList.ToArray(), serviceInstanceIdList.ToArray() });
            }
            catch (COMException comEx)
            {
                var args = new List<string>
                               {
                                   hostName,
                                   serviceInstanceId,
                                   serviceClassId,
                                   serviceTypeId,
                                   comEx.ToString()
                               };
                var message = string.Format(@"Com Error -- Host: {0}, Instance: {1}, Class: {2}, Type: {3}, Error: {4}", args.ToArray());
                Logger.LogMessage(this, message);
                if (comEx.Message != @"Suspend request could not be sent for the specified instance.  The instance might not exist in any of the MessageBoxes, or database failure might have been encountered.")
                    throw;
            }
        }
        /// <summary>
        /// Terminates one service instance
        /// </summary>
        /// <param name="hostName"></param>
        /// <param name="serviceInstanceId"></param>
        /// <param name="serviceClassId"></param>
        /// <param name="serviceTypeId"></param>
        private void TerminateServiceInstancesById(string hostName, string serviceInstanceId, string serviceClassId, string serviceTypeId)
        {
            var serviceInstanceIdList = new List<string>();
            var serviceClassIdList = new List<string>();
            var serviceTypeIdList = new List<string>();

            serviceInstanceIdList.Add(serviceInstanceId);
            serviceClassIdList.Add(serviceClassId);
            serviceTypeIdList.Add(serviceTypeId);


            var managementObjectPath = string.Format(CultureInfo.CurrentCulture, "root\\MicrosoftBizTalkServer:MSBTS_HostQueue.HostName=\"{0}\"", hostName);
            var managementObject = new ManagementObject(managementObjectPath);

            try
            {
                managementObject.InvokeMethod(@"TerminateServiceInstancesByID", new object[] { serviceClassIdList.ToArray(), serviceTypeIdList.ToArray(), serviceInstanceIdList.ToArray() });
            }
            catch (COMException comEx)
            {
                var args = new List<string>
                               {
                                   hostName,
                                   serviceInstanceId,
                                   serviceClassId,
                                   serviceTypeId,
                                   comEx.ToString()
                               };
                var message = string.Format(@"Com Error -- Host: {0}, Instance: {1}, Class: {2}, Type: {3}, Error: {4}", args.ToArray());
                Logger.LogMessage(this, message);
                if (comEx.Message != @"Terminate request could not be sent for the specified instance.  The instance might not exist in any of the MessageBoxes, or database failure might have been encountered.")
                    throw;
            }
        }
    }
}



