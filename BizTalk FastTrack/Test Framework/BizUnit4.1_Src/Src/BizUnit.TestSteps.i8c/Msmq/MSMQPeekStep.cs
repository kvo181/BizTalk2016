//---------------------------------------------------------------------
// File: MSMQPeekStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Messaging;
using BizUnit.Xaml;
using StreamHelper = BizUnit.TestSteps.Common.StreamHelper;

namespace BizUnit.TestSteps.i8c.Msmq
{
    /// <summary>
    /// The MSMQPeekStep reads a message from an MSMQ queue - without removing it -.
    /// We count the number of messages available in the queue.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.MSMQReadStep">
    ///		<QueuePath>.\Private$\Test01</QueuePath>
    ///		<Timeout>2000</Timeout>
    ///		<ExpectedNumberOfMessages>2</ExpectedNumberOfMessages>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>QueuePath</term>
    ///			<description>The MSMQ queue to read a message from</description>
    ///		</item>
    ///		<item>
    ///			<term>Timeout</term>
    ///			<description>The timeout to wait for the message to appear in the queue, in milisecs</description>
    ///		</item>
    ///		<item>
    ///			<term>ExpectedNumberOfMessages</term>
    ///			<description>The expected number of messages in the queue.</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class MsmqPeekStep : TestStepBase
    {
        ///<summary>
        /// Queue Path
        ///</summary>
        public string QueuePath { get; set; }
        ///<summary>
        /// Timeout value (milliseconds)
        ///</summary>
        public double TimeOut { get; set; }

        private int _expectedNumberOfMessages = -1;
        /// <summary>
        /// The expected number of messages in the queue.
        /// <remarks>A value -1 indicates we just want to count the number of messages</remarks>
        /// </summary>
        public int ExpectedNumberOfMessages
        {
            get { return _expectedNumberOfMessages; }
            set { _expectedNumberOfMessages = value; }
        }

        ///<summary>
        /// Constructor override
        ///</summary>
        public MsmqPeekStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }

        public override void Execute(Context context)
        {
            var queuePath = QueuePath;
            var timeout = TimeOut;

            var queue = new MessageQueue(MSMQHelper.DeNormalizeQueueName(queuePath));
            var c = queue.CreateCursor();

            // Count msgs in queue...
            var numberOfMessages = 0;
            var msg = PeekWithTimeout(queue, timeout, c, PeekAction.Current);
            if (null != msg)
            {
                numberOfMessages = 1;
                while ((msg = PeekWithoutTimeout(queue, c, PeekAction.Next)) != null)
                {
                    numberOfMessages++;
                    // Dump msg content to console...
                    var msgData = StreamHelper.LoadMemoryStream(msg.BodyStream);
                    StreamHelper.WriteStreamToConsole("MSMQ message data", msgData, context);
                    msgData.Close();
                }
            }

            context.LogInfo("Number of messages found: {0}, in queue '{1}'", numberOfMessages, QueuePath);

            switch (ExpectedNumberOfMessages)
            {
                case -1:
                    break;
                default:
                    if (ExpectedNumberOfMessages != numberOfMessages)
                        throw new Exception(String.Format("Queue contained: {0} messages, but the step expected: {1} messages", numberOfMessages, ExpectedNumberOfMessages));
                    break;
            }

        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(QueuePath))
                throw new ArgumentNullException("QueuePath is null or empty");
        }

        private static Message PeekWithTimeout(MessageQueue q, double timeout, Cursor cursor, PeekAction action)
        {
            Message ret = null;
            try
            {
                ret = q.Peek(TimeSpan.FromMilliseconds(timeout), cursor, action);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                    throw;
            }
            return ret;
        }
        private static Message PeekWithoutTimeout(MessageQueue q, Cursor cursor, PeekAction action)
        {
            Message ret = null;
            try
            {
                ret = q.Peek(new TimeSpan(1), cursor, action);
            }
            catch (MessageQueueException mqe)
            {
                if (mqe.MessageQueueErrorCode != MessageQueueErrorCode.IOTimeout)
                    throw;
            }
            return ret;
        }

    }
}
