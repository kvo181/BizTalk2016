//---------------------------------------------------------------------
// File: MQSeriesGetStep.cs
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
using IBM.WMQ;

namespace BizUnit.TestSteps.i8c.MQSeries
{
	/// <summary>
	/// The MQSeriesGetStep test step reads data from a specified MQ Series queue.
	/// </summary>
	/// 
	/// <remarks>
	/// Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
	/// <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
	/// <para>The following shows an example of the Xml representation of this test step.</para>
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MQSeriesSteps.MQSeriesGetStep, BizUnit.MQSeriesSteps", Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
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
	public class MQSeriesGetStep : TestStepBase
	{
		///<summary>
		/// Constructor override
		///</summary>
		public MQSeriesGetStep()
		{
			SubSteps = new Collection<SubStepBase>();
		}
		/// <summary>
		/// The name of the MQ Series queue manager
		/// </summary>
		public string QueueManager { get; set; }
		/// <summary>
		/// The name of the MQ Series queue to read from.
		/// </summary>
		public string Queue { get; set; }
		/// <summary>
		/// The time to wait for the message, after which if the message is not found the test step will fail, in seconds. 
		/// Use -1 to wait forever.
		/// </summary>
		public int WaitTimeout { get; set; }
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
            // Remote or not?
            if (!string.IsNullOrEmpty(HostName))
            {
                context.LogInfo("Host to connect to: \"{0}\" on port \"{1}\" with channel \"{2}\"", HostName, Port, Channel);
                MQEnvironment.Hostname = HostName;
                MQEnvironment.Port = Port;
                MQEnvironment.Channel = Channel;
            }
            var message = MQSeriesHelper.ReadMessage(QueueManager, Queue, WaitTimeout, context);
			context.LogData("MQSeries output message:", message);

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
				}
				catch (Exception ex)
				{
					context.LogException(ex);
					throw;
				}
			}
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
