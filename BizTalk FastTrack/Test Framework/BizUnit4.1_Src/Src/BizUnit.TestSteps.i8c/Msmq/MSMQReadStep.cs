//---------------------------------------------------------------------
// File: MSMQReadStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Messaging;
using System.Runtime.InteropServices;
using BizUnit.Xaml;
using StreamHelper = BizUnit.TestSteps.Common.StreamHelper;

namespace BizUnit.TestSteps.i8c.Msmq
{
    /// <summary>
    /// The MSMQReadStep reads a message from an MSMQ queue and optionally validates the contents of it
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<TestStep assemblyPath="" typeName="BizUnit.MSMQReadStep">
    ///		<QueuePath>.\Private$\Test01</QueuePath>
    ///		<Timeout>2000</Timeout>
    ///		
    ///		<ContextProperties>
    ///			<ContextProperty MSMQProp="CorrelationId" CtxPropName="MSMQ_CorrelationId" />
    ///			<ContextProperty MSMQProp="AppSpecific" CtxPropName="MSMQ_AppSpecific" />
    ///			<ContextProperty MSMQProp="Label" CtxPropName="MSMQ_Label" />
    ///		</ContextProperties>
    ///		
    ///		<ValidationStep assemblyPath="" typeName="BizUnit.BinaryValidation">
    ///			<ComparisonDataPath>.\TestData\ResultDoc1.xml</ComparisonDataPath>
    ///		</ValidationStep>
    ///
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
    ///			<term>ValidationStep</term>
    ///			<description>The validation step that will be used to validate the contents of the message read from the queue (optional).</description>
    ///		</item>
    ///		<item>
    ///			<term>ContextProperties/ContextProperty</term>
    ///			<description>Allows properties from the MSMQ message to be written to the BizUnit context. 
    ///			The MSMQProp attribute specifies the property on the MSMQ message, e.g. "CorrelationId", the 
    ///			CtxPropName attribute specifies the name of the property to write the value of the MSMQ property to.
    ///			<para>Note: All properties on System.Messaging.Message are supported.</para>
    ///			<para>(Optional)(One or more)</para></description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class MsmqReadStep : TestStepBase
    {
        ///<summary>
        /// Queue Path
        ///</summary>
        public string QueuePath { get; set; }
        ///<summary>
        /// Timeout value (milliseconds)
        ///</summary>
        public double TimeOut { get; set; }
        ///<summary>
        /// Context Properties
        ///</summary>
        public Collection<ContextProperty> ContextProperties { get; set; }

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

        ///<summary>
        /// Constructor override
        ///</summary>
        public MsmqReadStep()
        {
            SubSteps = new Collection<SubStepBase>();
        }

        public override void Execute(Context context)
        {
            MemoryStream msgData = null;

            var queuePath = QueuePath;
            var timeout = TimeOut;

            try
            {
                var queue = new MessageQueue(MSMQHelper.DeNormalizeQueueName(queuePath));

                // Receive msg from queue...
                if (BodyType != VarEnum.VT_EMPTY)
                    queue.Formatter = new ActiveXMessageFormatter();
                var msg = queue.Receive(TimeSpan.FromMilliseconds(timeout), MessageQueueTransactionType.Single);
                if (msg == null)
                    throw new Exception("No message read!");

                // Dump msg content to console...
                msgData = StreamHelper.LoadMemoryStream(msg.BodyStream);
                StreamHelper.WriteStreamToConsole("MSMQ message data", msgData, context);

                // Validate data...
                msgData.Seek(0, SeekOrigin.Begin);
                // Check it against the validate steps to see if it matches one of them
                foreach (var subStep in SubSteps)
                {
                    try
                    {
                        // Try the validation and catch the exception
                        var strm = subStep.Execute(msgData, context);
                    }
                    catch (Exception ex)
                    {
                        context.LogException(ex);
                        throw;
                    }
                }

                ProcessContextProperties(context, ContextProperties, msg);

            }
            finally
            {
                if (null != msgData)
                {
                    msgData.Close();
                }
            }

        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(QueuePath))
                throw new ArgumentNullException("QueuePath is null or empty");
        }

        private static void ProcessContextProperties(Context context, Collection<ContextProperty> props, Message msg)
        {
            if (null == props) return;
            if (props.Count == 0) return;

            foreach (var prop in props)
            {
                // <ContextProperty MSMQProp="CorrelationId" CtxPropName="MSMQ_CorrelationId" />
                string ctxPropName = prop.CtxPropName;
                string msmqPropName = prop.MsmqProp;

                var pi = msg.GetType().GetProperty(msmqPropName);
                object val;
                try
                {
                    val = pi.GetValue(msg, null);
                }
                catch (Exception)
                {
                    context.LogInfo("The property: \"{0}\" did not have a value set", msmqPropName);
                    continue;
                }

                context.LogInfo("Property: \"{0}\", Value: \"{1}\" written to context", msmqPropName, val);
                context.Add(ctxPropName, val);
            }
        }
    }
}
