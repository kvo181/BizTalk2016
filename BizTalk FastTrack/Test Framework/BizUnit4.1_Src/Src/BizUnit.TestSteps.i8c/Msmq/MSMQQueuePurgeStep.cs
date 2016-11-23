//---------------------------------------------------------------------
// File: MSMQQueuePurgeStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Messaging;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Msmq
{
	/// <summary>
	/// The MSMQQueuePurgeStep purges an MSMQ queue
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQQueuePurgeStep">
	///		<QueuePath>.\Private$\Test01</QueuePath>
	///		<QueuePath>.\Private$\Test02</QueuePath>
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
	///			<description>The MSMQ queue to purge, multiple entries maybe specified</description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class MsmqQueuePurgeStep : TestStepBase
	{
	    ///<summary>
	    /// Default constructor
	    ///</summary>
	    public MsmqQueuePurgeStep()
	    {
	        QueuePaths = new Collection<QueuePathDefinition>();
	    }
		///<summary>
		/// List of queues to purge
		///</summary>
		public Collection<QueuePathDefinition> QueuePaths { get; set; }

		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			foreach (QueuePathDefinition queuePathDef in QueuePaths)
			{
				var queuePath = queuePathDef.QueuePath;
				var q = new MessageQueue(MSMQHelper.DeNormalizeQueueName(queuePath));
				q.Purge();
				context.LogInfo("MSMQQueuePurgeStep has purged the queue: {0}", queuePath);
			}
		}

		public override void Validate(Context context)
		{
			if (null == QueuePaths)
                throw new ArgumentNullException("QueuePaths is null");
		}
	}
}
