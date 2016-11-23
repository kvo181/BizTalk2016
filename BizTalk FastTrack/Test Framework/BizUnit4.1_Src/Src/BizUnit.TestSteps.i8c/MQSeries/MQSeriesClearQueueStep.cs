//---------------------------------------------------------------------
// File: MQSeriesClearQueueStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using BizUnit.Common;
using BizUnit.Xaml;
using IBM.WMQ;

namespace BizUnit.TestSteps.i8c.MQSeries
{ // $:\Program Files\IBM\WebSphere MQ\bin\amqmdnet.dll - Requires Patch Level > CSD07

	/// <summary>
	/// The MQSeriesClearQueueStep test step clears one or more MQ Series queues, this test step is typically used to cleanup a test case.
	/// </summary>
	/// 
	/// <remarks>
	/// Note: this test step requires amqmdnet.dll, this is present in MQ Patch Level > CSD07
	/// <para>Also, this test step is packaged in the assembly: BizUnit.MQSeriesSteps.dll</para>
	/// <para>The following shows an example of the Xml representation of this test step.</para>
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MQSeriesSteps.MQSeriesClearQueueStep, BizUnit.MQSeriesSteps", Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	/// 	<QueueManager>QM_server</QueueManager>
	/// 	<Queue>QUEUE_1</Queue>
	/// 	<Queue>QUEUE_2</Queue>
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
	///			<description>The name of the MQ Series queue to clear. (one or more)</description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class MQSeriesClearQueueStep : TestStepBase
	{
	    ///<summary>
	    /// Default constructor
	    ///</summary>
	    public MQSeriesClearQueueStep()
	    {
	        Queues = new Collection<object>();
	    }
		/// <summary>
		/// The name of the MQ Series queue manager
		/// </summary>
		public string QueueManager { get; set; }
		/// <summary>
		/// The name of the MQ Series queues to purge.
		/// </summary>
		public Collection<object> Queues { get; set; }
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
			MQQueueManager queueManager;

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
                context.LogInfo("Opening queue manager '{0}'.", QueueManager);
				queueManager = new MQQueueManager(QueueManager);
			}
			catch (Exception e)
			{
				throw new Exception(string.Format("Failed to open queue manager {0}.", QueueManager), e);
			}

			var errors = false;

			try
			{
				foreach (var q in Queues)
					errors = MQSeriesHelper.Purge(queueManager, q.ToString(), context);
			}
			finally
			{
				if (queueManager != null)
					queueManager.Close();
				if (errors)
					throw new Exception("Failed to clear at least one queue.");
			}
		}

		public override void Validate(Context context)
		{
			ArgumentValidation.CheckForEmptyString(QueueManager, "QueueManager");
			ArgumentValidation.CheckForNullReference(Queues, "Queues");
            if (string.IsNullOrEmpty(HostName)) return;
            // Remote connection parameters
            ArgumentValidation.CheckForEmptyString(HostName, "HostName");
            ArgumentValidation.CheckForNullReference(Port, "Port");
            ArgumentValidation.CheckForEmptyString(Channel, "Channel");
        }
	}
}
