using System;
using System.Messaging;
using Microsoft.BizTalk.CAT.BestPractices.Framework.Instrumentation;

namespace MSMQWcfServiceLibrary
{
    public class MsmqService : IMsmqService
    {
        /// <summary>
        /// Create a local MSMQ queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="label"></param>
        /// <param name="transactional"></param>
        /// <returns>Path of the created queue.</returns>
        public string CreateQueue(string queueName, string label, bool transactional)
        {
            Guid callToken = TraceManager.ServiceComponent.TraceIn(queueName, label, transactional);
            /*
             * The syntax for the path parameter depends on the type of queue it references:
             * Public queue MachineName\QueueName
             * Private queue MachineName\Private$\QueueName
             * Use "." for the local computer
             */
            var queue = MessageQueue.Create(NormalizeQueueName(queueName), transactional);
            queue.Label = label;
            queue.SetPermissions("Everyone", MessageQueueAccessRights.FullControl);

            TraceManager.ServiceComponent.TraceOut(callToken, queue.Path);

            return queue.Path;
        }

        public void DeleteQueue(string queueName)
        {
            Guid callToken = TraceManager.ServiceComponent.TraceIn(queueName);
            MessageQueue.Delete(NormalizeQueueName(queueName));
            TraceManager.ServiceComponent.TraceOut(callToken);
        }
        private static string NormalizeQueueName(string queueName)
        {
            if (string.IsNullOrEmpty(queueName)) return string.Empty;
            if (!queueName.ToLower().StartsWith("formatname:direct=os:")) return queueName;
            return queueName.Substring(21);
        }

    }
}
