//---------------------------------------------------------------------
// File: MQSeriesPeekStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.IO;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;

using IBM.WMQ; // $:\Program Files\IBM\WebSphere MQ\bin\amqmdnet.dll - Requires Patch Level > CSD07

namespace BizUnit.TestSteps.i8c.MQSeries
{
    /// <summary>
    /// The MQSeriesPeekStep test step browses the specified MQ Series queue.
    /// </summary>
    /// 
    /// <remarks>
    /// Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
    /// <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
    /// <para>The following shows an example of the Xml representation of this test step.</para>
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.MQSeriesSteps.MQSeriesPeekStep, BizUnit.MQSeriesSteps", Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    /// 	<QueueManager>QM_amachine</QueueManager>
    /// 	<Queue>QUEUE_007</Queue>
    /// 	<WaitTimeout>30</WaitTimeout>		<!-- in seconds, -1 = wait forever -->
    /// 	
    ///		<!-- Note: Validation step could be any generic validation step -->	
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.XmlValidationStep">
    ///			<XmlSchemaPath>.\TestData\PurchaseOrder.xsd</XmlSchemaPath>
    ///			<XmlSchemaNameSpace>http://SendMail.PurchaseOrder</XmlSchemaNameSpace>
    ///			<XPathList>
    ///				<XPathValidation query="/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']">PONumber_0</XPathValidation>
    ///			</XPathList>
    ///		</ValidationStep>			
    /// </TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>QueueManager</term>
    ///			<description>The name of the MQ Series queue manager</description>
    ///		</item>
    ///		<item>
    ///			<term>Queue</term>
    ///			<description>The name of the MQ Series queue to read from.</description>
    ///		</item>
    ///		<item>
    ///			<term>WaitTimeout</term>
    ///			<description>The time to wait for the message, after which if the message is not found the test step will fail, in seconds. Use -1 to wait forever.</description>
    ///		</item>
    ///		<item>
    ///			<term>ValidationStep</term>
    ///			<description>The validation step to used to validate the data read from the queue. (optional)</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class MQSeriesPeekStep : TestStepBase
    {
        ///<summary>
        /// Constructor override
        ///</summary>
        public MQSeriesPeekStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }
        /// <summary>
        /// The name of the MQ Series queue manager
        /// </summary>
        public string QueueManager { get; set; }
        /// <summary>
        /// The name of the Remote MQ Series queue to browse.
        /// </summary>
        public string RemoteQueue { get; set; }
        /// <summary>
        /// The name of the MQ Series queue to browse.
        /// </summary>
        public string Queue { get; set; }
        /// <summary>
        /// The time to wait for the message, after which if the message is not found the test step will fail, in seconds. 
        /// Use -1 to wait forever.
        /// </summary>
        public int WaitTimeout { get; set; }
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

        /// <summary>
        /// When peeking into a MQSeries queue hosted on a remote server,
        /// we need to provide the host to connect to.
        /// </summary>
        public string HostName { get; set; }
        /// <summary>
        /// When peeking into a MQSeries queue hosted on a remote server,
        /// we need to provide the channel name of the SVRCONN channel on the queue manager.
        /// </summary>
        public string Channel { get; set; }
        /// <summary>
        /// When peeking into a MQSeries queue hosted on a remote server,
        /// we need to provide the port to connect to.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            MQQueueManager queueManager = null;
            MQQueue receiveQueue = null;
            string message = null;
            var bLookForMessage = true;
            var bFound = false;
            var cnt = 0;

            try
            {
                // Remote or not?
                if (!string.IsNullOrEmpty(HostName))
                {
                    context.LogInfo("Host to connect to: \"{0}\" on port \"{1}\" with channel \"{2}\"", HostName, Port, Channel);
                    MQEnvironment.Hostname = HostName;
                    MQEnvironment.Port = Port;
                    MQEnvironment.Channel = Channel;
                }

                context.LogInfo("Opening queue manager: \"{0}\"", QueueManager);
                queueManager = new MQQueueManager(QueueManager);

                context.LogInfo("Opening queue: \"{0}\" - remote queue: \"{1}\"", Queue, RemoteQueue);
                receiveQueue = queueManager.AccessQueue(Queue, MQC.MQOO_INPUT_SHARED + MQC.MQOO_FAIL_IF_QUIESCING + MQC.MQOO_BROWSE + MQC.MQOO_SET);

                MQMessage mqMsg = new MQMessage();
                MQGetMessageOptions mqMsgOpts = new MQGetMessageOptions();
                mqMsgOpts.WaitInterval = WaitTimeout * 1000;
                mqMsgOpts.Options = MQC.MQGMO_WAIT + MQC.MQGMO_BROWSE_FIRST;
                mqMsgOpts.MatchOptions = MQC.MQMO_NONE;

                context.LogInfo("Browsing queue '{0}'.", Queue);

                // Loop until the required message is found
                while (!bFound)
                {
                    if (cnt > 0)
                        mqMsgOpts.Options = MQC.MQGMO_WAIT + MQC.MQGMO_BROWSE_NEXT;
                    try
                    {
                        receiveQueue.InhibitGet = MQC.MQQA_GET_ALLOWED;
                        receiveQueue.Get(mqMsg, mqMsgOpts);
                        cnt++;
                    }
                    catch (MQException mqe)
                    {
                        if (mqe.ReasonCode == MQC.MQRC_NO_MSG_AVAILABLE)
                            break;
                        throw;
                    }

                    if ((mqMsg.Format.CompareTo(MQC.MQFMT_STRING) != 0) &&
                        (mqMsg.Format.CompareTo(MQC.MQFMT_XMIT_Q_HEADER) != 0))
                    {
                        throw new NotSupportedException(
                            string.Format("Unsupported message format: '{0}' read from queue: {1}.", mqMsg.Format,
                                          Queue));
                    }
                    else
                    {
                        mqMsg.Seek(0);
                        if (mqMsg.Format.CompareTo(MQC.MQFMT_XMIT_Q_HEADER) == 0)
                        {
                            var strucId = mqMsg.ReadString(4);
                            var version = mqMsg.ReadInt();
                            var remoteQName = mqMsg.ReadString(48);
                            var remoteQMgrName = mqMsg.ReadString(48);
                            /*
                            struct tagMQMD {
   		                            MQCHAR4   StrucId;           // Structure identifier
   		                            MQLONG    Version;           // Structure version number
   		                            MQLONG    Report;            // Options for report messages
   		                            MQLONG    MsgType;           // Message type
   		                            MQLONG    Expiry;            // Message lifetime
   		                            MQLONG    Feedback;          // Feedback or reason code
   		                            MQLONG    Encoding;          // Numeric encoding of message data
   		                            MQLONG    CodedCharSetId;    // Character set identifier of message data
   		                            MQCHAR8   Format;            // Format name of message data
   		                            MQLONG    Priority;          // Message priority
   		                            MQLONG    Persistence;       // Message persistence
   		                            MQBYTE24  MsgId;             // Message identifier
   		                            MQBYTE24  CorrelId;          // Correlation identifier
   		                            MQLONG    BackoutCount;      // Backout counter
   		                            MQCHAR48  ReplyToQ;          // Name of reply queue
   		                            MQCHAR48  ReplyToQMgr;       // Name of reply queue manager
   		                            MQCHAR12  UserIdentifier;    // User identifier
   		                            MQBYTE32  AccountingToken;   // Accounting token
   		                            MQCHAR32  ApplIdentityData;  // Application data relating to identity
   		                            MQLONG    PutApplType;       // Type of application that put the message
   		                            MQCHAR28  PutApplName;       // Name of application that put the message
   		                            MQCHAR8   PutDate;           // Date when message was put
   		                            MQCHAR8   PutTime;           // Time when message was put
   		                            MQCHAR4   ApplOriginData;    // Application data relating to origin
                               }
                             */
                            var bytesMqmd = mqMsg.ReadBytes(324);
                        }
                        message = mqMsg.ReadLine();
                        //message = System.Text.UTF8Encoding.UTF8.GetString(mqMsg.ReadBytes(mqMsg.MessageLength));
                        context.LogData("MQSeries output message:", message);

                        if ((null == SubSteps) || (SubSteps.Count == 0))
                            bLookForMessage = false;
                        else
                        {
                            // Validate data...
                            var msgData = StreamHelper.LoadMemoryStream(message);
                            msgData.Seek(0, SeekOrigin.Begin);
                            // Check it against the validate steps to see if it matches one of them
                            foreach (var subStep in SubSteps)
                            {
                                try
                                {
                                    // Try the validation and catch the exception
                                    var strm = subStep.Execute(msgData, context);
                                    bFound = true;
                                }
                                catch
                                {
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
                if (receiveQueue != null)
                {
                    receiveQueue.Close();
                }

                if (queueManager != null)
                {
                    queueManager.Close();
                }
            }

            context.LogInfo("Number of messages found: {0}, in queue '{1}'", cnt, Queue);

            switch (ExpectedNumberOfMessages)
            {
                case -1:
                    break;
                default:
                    if (ExpectedNumberOfMessages != cnt)
                        throw new Exception(String.Format("Queue '{2}' contained: {0} messages, but the step expected: {1} messages", cnt, ExpectedNumberOfMessages, Queue));
                    break;
            }

            if (!bFound && bLookForMessage && (ExpectedNumberOfMessages > 0))
                throw new Exception(string.Format("Message not found in Queue '{0}', found {1} messages", Queue, cnt));

        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(QueueManager, "QueueManager");
            ArgumentValidation.CheckForEmptyString(Queue, "Queue");
            if (string.IsNullOrEmpty(HostName)) return;
            // Remote connection parameters
            ArgumentValidation.CheckForEmptyString(HostName, "HostName");
            ArgumentValidation.CheckForNullReference(Port, "Port");
            ArgumentValidation.CheckForEmptyString(Channel, "Channel");
        }
    }
}
