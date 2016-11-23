using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.ServiceModel;
using System.Text;
using System.Threading;

namespace BizUnit.TestSteps.i8c.Msmq
{
    public class MSMQHelper
    {
        /// <summary>
        /// We cache the list of private queues per machine.
        /// </summary>
        private static Dictionary<string, List<string>> _privateQueuesByMachine;

        ///<summary>
        /// Does the given queue exist?
        /// When the machine is the local machine we use the MessageQueue API
        /// Otherwise we retrieve all queueus from the machine and check if the queue is between them
        ///</summary>
        ///<param name="queueName"></param>
        ///<param name="machineName"></param>
        ///<returns></returns>
        public static bool QueueExists(string queueName, string machineName)
        {
            return QueueExists(queueName, machineName, false);
        }
        ///<summary>
        /// Does the given queue exist?
        /// When the machine is the local machine we use the MessageQueue API
        /// Otherwise we retrieve all queueus from the machine and check if the queue is between them
        ///</summary>
        ///<param name="queueName"></param>
        ///<param name="machineName"></param>
        /// <param name="bVerifyLocal"></param>
        ///<returns></returns>
        public static bool QueueExists(string queueName, string machineName, bool bVerifyLocal)
        {
            /*
             * MessageQueue.Exists cannot be called to verify the existence of a remote private queue.
             * In order to verify the existance of a queue, we use GetPrivateQueuesByMachine method and iterate the results to find a match.
             */
            bool bRemoteQueue = true;
            if (Environment.MachineName.ToLower().Equals(machineName.ToLower()) ||
                machineName.Equals("."))
                bRemoteQueue = false;

            bool bExists;
            if (!bRemoteQueue)
            {
                string localQueueName = NormalizeQueueName(queueName);
                int iRetries = 0;
                bExists = MessageQueue.Exists(localQueueName);
                if (bVerifyLocal)
                {
                    while (!bExists && iRetries < 3)
                    {
                        iRetries++;
                        Thread.Sleep(100);
                        bExists = MessageQueue.Exists(localQueueName);
                    }
                }
                return bExists;
            }
            
            RefreshPrivateQueues(machineName);
            // The queue name has to be mentioned as "FormatName:Direct=OS:machinename\\private$\\queuename".
            string queuePathToCheck = queueName;
            if (!queueName.ToLower().StartsWith("formatname:direct=os:"))
                queuePathToCheck = "FormatName:Direct=OS:" + queueName;
            bExists = _privateQueuesByMachine[machineName].Contains(queuePathToCheck.ToLower());
            return bExists;
        }
        /// <summary>
        /// Create a remote or local MSMQ queue
        /// </summary>
        /// <param name="machineName"></param>
        /// <param name="queueName"></param>
        /// <param name="label"></param>
        /// <param name="transactional"></param>
        /// <remarks>
        /// In case a remote queue needs to be created, we do this via the MsmqService proxy.
        /// <para>For local queues we use the MessageQueue API</para>
        /// </remarks>
        public static void QueueCreate(string machineName, string queueName, string label, bool transactional)
        {
            bool bRemoteQueue = true;
            if (Environment.MachineName.ToLower().Equals(machineName.ToLower()) ||
                machineName.Equals("."))
                bRemoteQueue = false;

            if (!bRemoteQueue)
            {
                MessageQueue adminQueue = MessageQueue.Create(queueName, transactional);
                adminQueue.Label = label;
                adminQueue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);
            }
            else
            {
                MSMQWcfServiceLibrary.IMsmqService proxy = MsmqService(machineName);
                using (proxy as IDisposable)
                {
                    proxy.CreateQueue(queueName, label, transactional);
                }
            }
        }

        /// <summary>
        /// Delete a private transactional MSMQ queue used for batching.
        /// </summary>
        /// <param name="queuePath">Name (path) of the MSMQ queue</param>
        /// <param name="machineName"></param>
        public static void DeleteMsmqQueue(string queuePath, string machineName)
        {
            if (string.IsNullOrEmpty(queuePath))
                throw new ArgumentNullException("queuePath");

            if (!QueueExists(queuePath, machineName))
                throw new Exception(string.Format("Queue {0} does not exist", queuePath));

            var bRemoteQueue = true;
            if (Environment.MachineName.ToLower().Equals(machineName.ToLower()) ||
                machineName.Equals("."))
                bRemoteQueue = false;

            // Delete the queue
            // A MSMQ queue can only be deleted on the machine where it is hosted!
            if (!bRemoteQueue)
                MessageQueue.Delete(queuePath);
            else
            {
                var proxy = MsmqService(machineName);
                using (proxy as IDisposable)
                {
                    proxy.DeleteQueue(queuePath);
                }
            }

            // Refresh the list of queues on this machine.
            RefreshPrivateQueues(machineName);
        }
        /// <summary>
        /// Queue is hosted by which machine.
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>Returns the machine name of a Normalized QueueName</returns>
        public static string GetMachineNameFromQueueName(string queueName)
        {
            string normalizedQueueName = NormalizeQueueName(queueName);
            return normalizedQueueName.Substring(0, normalizedQueueName.IndexOf("\\"));
        }
        /// <summary>
        /// A de-normalized queue name starts with "formatname:direct=os:"
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>Queue name with "formatname:direct=os:"</returns>
        public static string DeNormalizeQueueName(string queueName)
        {
            if (string.IsNullOrEmpty(queueName)) return string.Empty;
            return queueName.ToLower().StartsWith("formatname:direct=os:") ? queueName : "FormatName:DIRECT=OS:" + queueName;
        }
        /// <summary>
        /// A normalized queue name does not include "formatname:direct=os:"
        /// </summary>
        /// <param name="queueName"></param>
        /// <returns>Queue name without "formatname:direct=os:"</returns>
        private static string NormalizeQueueName(string queueName)
        {
            if (string.IsNullOrEmpty(queueName)) return string.Empty;
            return !queueName.ToLower().StartsWith("formatname:direct=os:") ? queueName : queueName.Substring(21);
        }
        /// <summary>
        /// Since we cache the list of private queues per machine, we sometimes may need to refresh the list.
        /// </summary>
        /// <param name="machineName"></param>
        private static void RefreshPrivateQueues(string machineName)
        {
            if (null == _privateQueuesByMachine)
                _privateQueuesByMachine = new Dictionary<string, List<string>>();
            if (!_privateQueuesByMachine.ContainsKey(machineName))
                _privateQueuesByMachine.Add(machineName, GetPrivateQueues(machineName));
            else
                _privateQueuesByMachine[machineName] = GetPrivateQueues(machineName);
        }
        /// <summary>
        /// Get the list of private queues on a given machine.
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns>List of queue path</returns>
        /// <remarks>We use MessageQueue.GetPrivateQueuesByMachine to get the list of private queues on a machine</remarks>
        private static List<string> GetPrivateQueues(string machineName)
        {
            MessageQueue[] queues = MessageQueue.GetPrivateQueuesByMachine(machineName);
            return queues.Select(queue => queue.Path.ToLower()).ToList();
        }
        /// <summary>
        /// Get the private MSMQ queue on a given machine, with a given path.
        /// </summary>
        /// <param name="queuePath"></param>
        /// <param name="machineName"></param>
        /// <returns>MessageQueue</returns>
        /// <remarks>We use MessageQueue.GetPrivateQueuesByMachine to get the list of private queues on a machine and return the queue when found in the list</remarks>
        private static MessageQueue GetPrivateQueue(string queuePath, string machineName)
        {
            string queuePathToCheck = queuePath;
            /*
             * MessageQueue.Exists cannot be called to verify the existence of a remote private queue.
             * In order to verify the existance of a queue, we use GetPrivateQueuesByMachine method and iterate the results to find a match.
             */
            /*
             * MessageQueue.Exists cannot be called to verify the existence of a remote private queue.
             * In order to verify the existance of a queue, we use GetPrivateQueuesByMachine method and iterate the results to find a match.
             */
            bool bRemoteQueue = true;
            if (Environment.MachineName.ToLower().Equals(machineName.ToLower()) ||
                machineName.Equals("."))
                bRemoteQueue = false;
            if (!bRemoteQueue)
            {
                // The queue name has to be mentioned as "FormatName:Direct=OS:machinename\\private$\\queuename".
                queuePathToCheck = queuePathToCheck.Replace(".", Environment.MachineName);
                if (!queuePathToCheck.ToLower().StartsWith("formatname:direct=os:"))
                    queuePathToCheck = "FormatName:Direct=OS:" + queuePathToCheck;
            }
            MessageQueue[] queues = MessageQueue.GetPrivateQueuesByMachine(machineName);
            return queues.FirstOrDefault(queue => queue.Path.ToLower().Equals(queuePathToCheck.ToLower(), StringComparison.OrdinalIgnoreCase));
        }
        /// <summary>
        /// Proxy used to communicate with the WCF MsmqService
        /// (binding used is NetTcpBinding)
        /// </summary>
        /// <param name="machineName"></param>
        /// <returns>MSMQWcfServiceLibrary.IMsmqService</returns>
        /// <remarks>
        /// We need to call a Wcf service in order to be able to create a private queue on
        /// a remote computer. The service has to be running on the remote computer, since a
        /// private queue can only by created locally.
        /// <para>
        /// The running service has to be available as:
        /// net.tcp://{machineName}/MsmqWcfService/MsmqService.svc
        /// </para>
        /// In order to get the security correct we had to use an empty string for the SPN.
        /// <para>
        /// - If you set the SPN or UPN equal to an empty string, a number of different things happen, depending on the security level and authentication mode being used:
        /// </para>
        /// <para>
        /// - If you are using transport level security, NT LanMan (NTLM) authentication is chosen. 
        /// </para>
        /// <para>
        /// - If you are using message level security, authentication may fail, depending on the authentication mode: 
        /// </para>
        /// <para>
        /// - If you are using spnego mode and the AllowNtlm attribute is set to false, authentication fail. 
        /// </para>
        /// <para>
        /// - If you are using spnego mode and the AllowNtlm attribute is set to true, authentication fails if the UPN is empty, but succeeds if the SPN is empty. 
        /// </para>
        /// - If you are using Kerberos direct (also known as "one-shot"), authentication fails. 
        /// </remarks>
        private static MSMQWcfServiceLibrary.IMsmqService MsmqService(string machineName)
        {
            // We need to call a Wcf service in order to be able to create a private queue on
            // a remote computer. The service has to be running on the remote computer, since a
            // private queue can only by created locally.
            // The running service has to be available as:
            // net.tcp://<servername>/MsmqWcfService/MsmqService.svc
            NetTcpBinding binding = new NetTcpBinding();
            Uri uri = new Uri(string.Format("net.tcp://{0}/MsmqWcfService/MsmqService.svc", machineName));
            EndpointAddress address = new EndpointAddress(uri, EndpointIdentity.CreateSpnIdentity(""));
            return ChannelFactory<MSMQWcfServiceLibrary.IMsmqService>.CreateChannel(binding, address);
        }
    }
}
