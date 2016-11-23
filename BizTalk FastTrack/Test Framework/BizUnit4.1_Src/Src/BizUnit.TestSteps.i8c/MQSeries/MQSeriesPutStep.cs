//---------------------------------------------------------------------
// File: MQSeriesPutStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.IO;
using BizUnit.Common;
using BizUnit.Xaml;
using IBM.WMQ;

namespace BizUnit.TestSteps.i8c.MQSeries
{
	/// <summary>
	/// The MQSeriesPutStep test step writes data to an MQ Series queue.
	/// </summary>
	/// 
	/// <remarks>
	/// Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
	/// <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
	/// <para>The following shows an example of the Xml representation of this test step.</para>
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MQSeriesSteps.MQSeriesPutStep, BizUnit.MQSeriesSteps", Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<QueueManager>QM_server</QueueManager>
	///		<Queue>QueueName</Queue>
	///		<SourcePath>.\TestData\InDoc1.txt</SourcePath>
	///	</TestStep>
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
	///			<term>SourcePath</term>
	///			<description>The location of the FILE containing the data that will be written to the queue.</description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class MQSeriesPutStep : TestStepBase
	{
		/// <summary>
		/// The name of the MQ Series queue manager
		/// </summary>
		public string QueueManager { get; set; }
		/// <summary>
		/// The name of the MQ Series queue to read from.
		/// </summary>
		public string Queue { get; set; }
		///<summary>
		/// Message to load onto queue.
		///</summary>
		public DataLoaderBase MessageBody { get; set; }
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
		    var reader = new StreamReader(MessageBody.Load(context));
			string testData = reader.ReadToEnd();

			context.LogData("MSMQ input message:", testData);

            // Remote or not?
            if (!string.IsNullOrEmpty(HostName))
            {
                context.LogInfo("Host to connect to: \"{0}\" on port \"{1}\" with channel \"{2}\"", HostName, Port, Channel);
                MQEnvironment.Hostname = HostName;
                MQEnvironment.Port = Port;
                MQEnvironment.Channel = Channel;
            }
			MQSeriesHelper.WriteMessage(QueueManager, Queue, testData, context);
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
