//---------------------------------------------------------------------
// File: MSMQWriteStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Messaging;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Msmq
{
    /// <summary>
    /// The MSMQWriteStep writes a message to an MSMQ queue and optionally validates the contents of it
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.MSMQWriteStep">
    ///		<SourcePath>.\TestData\InDoc1.xml</SourcePath>
    ///		<QueuePath>.\Private$\Test01</QueuePath>
    ///		<MessageLabel>MSMQ_To_MSMQ_Test_Test_01</MessageLabel>
    ///		<CorrelationId>1234</CorrelationId>
    ///		<AppSpecific>5678</AppSpecific>
    ///		<UseTransactions>true</UseTransactions>
    ///	</TestStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>SourcePath</term>
    ///			<description>The FILE containing the data to be written to an MSMQ queue</description>
    ///		</item>
    ///		<item>
    ///			<term>QueuePath</term>
    ///			<description>The MSMQ queue to write a new message to</description>
    ///		</item>
    ///		<item>
    ///			<term>MessageLabel</term>
    ///			<description>The MSMQ label to associate with the new message</description>
    ///		</item>
    ///		<item>
    ///			<term>CorrelationId</term>
    ///			<description>The MSMQ CorrelationId to associate with the new message</description>
    ///		</item>
    ///		<item>
    ///			<term>AppSpecific</term>
    ///			<description>The MSMQ AppSpecific property to associate with the new message (int32)</description>
    ///		</item>
    ///		<item>
    ///			<term>UseTransactions</term>
    ///			<description>Defaults to true, when using transactions the message will be written to the queue using MessageQueueTransactionType.Single, if set to false MessageQueueTransactionType.None will be used (optional)</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class MsmqWriteStep : TestStepBase
    {
        private MessageQueueTransactionType _transactionType = MessageQueueTransactionType.Single;
        private Stream _request;

        ///<summary>
        /// Message to load onto queue.
        ///</summary>
        public DataLoaderBase MessageBody { get; set; }

        ///<summary>
        /// The MSMQ queue to write a new message to
        ///</summary>
        public string QueuePath { get; set; }
        ///<summary>
        /// The MSMQ label to associate with the new message
        ///</summary>
        public string MessageLabel { get; set; }
        ///<summary>
        /// The MSMQ CorrelationId to associate with the new message
        ///</summary>
        public string CorrelationId { get; set; }
        ///<summary>
        /// The MSMQ AppSpecific property to associate with the new message (int32)
        ///</summary>
        public int AppSpecific { get; set; }

        private bool _useTransactions = true;
        ///<summary>
        /// Defaults to true, when using transactions the message will be written to the queue using MessageQueueTransactionType.Single, 
        /// if set to false MessageQueueTransactionType.None will be used (optional)
        ///</summary>
        public bool UseTransactions
        {
            get { return _useTransactions; }
            set { _useTransactions = value; }
        }

        private VarEnum _bodyType = VarEnum.VT_EMPTY;
        /// <summary>
        /// The message body's true type, such as a string, a date, a currency, or a number.
        /// (Optional)
        /// <remarks>We use VT_LPWSTR for DMFA testing</remarks>
        /// </summary>
        public VarEnum BodyType
        {
            get { return _bodyType; }
            set { _bodyType = value; }
        }

        public override void Execute(Context context)
        {
            _request = MessageBody.Load(context);
            context.LogXmlData("Message", _request, true);

            try
            {
                if (!UseTransactions)
                    _transactionType = MessageQueueTransactionType.None;

                context.LogInfo("MSMQWriteStep about to write data to the queue: {0}", QueuePath);

                var queue = new MessageQueue(MSMQHelper.DeNormalizeQueueName(QueuePath));
                var msg = new Message();

                if (BodyType == VarEnum.VT_EMPTY)
                {
                    msg.BodyStream = _request;
                }
                else
                {
                    msg.BodyType = (int)BodyType;
                    var formatter = new ActiveXMessageFormatter();
                    formatter.Write(msg, Common.StreamHelper.WriteStreamToString(_request));
                }

                msg.UseDeadLetterQueue = true;

                if (!string.IsNullOrEmpty(CorrelationId))
                    msg.CorrelationId = CorrelationId;
                msg.AppSpecific = AppSpecific;

                queue.Send(msg, MessageLabel, _transactionType);
            }
            finally
            {
                if (null != _request)
                    _request.Close();
            }
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(QueuePath))
                throw new ArgumentNullException("QueuePath is null or empty");
            if (string.IsNullOrEmpty(MessageLabel))
                throw new ArgumentNullException("MessageLabel is null or empty");
        }
    }
}
