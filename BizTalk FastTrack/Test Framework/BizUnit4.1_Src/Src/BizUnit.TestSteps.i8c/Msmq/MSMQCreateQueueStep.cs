//---------------------------------------------------------------------
// File: MSMQCreateQueueStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Messaging;
using BizUnit.Xaml;
using System.Xml;

namespace BizUnit.TestSteps.i8c.Msmq
{
	/// <summary>
	/// The MSMQCreateQueueStep creates one or more new MSMQ queues
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQCreateQueueStep">
	///		<QueuePath transactional="true">.\Private$\Test01</QueuePath>
	///		<QueuePath transactional="true">.\Private$\Test02</QueuePath>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>QueueName</term>
	///			<description>The name of the MSMQ queue to create, e.g. .\Private$\Test02 <para>(one or more)</para></description>
	///		</item>
	///		<item>
	///			<term>QueueName/@transactional</term>
	///			<description>If true, the queue created will be transactional</description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class MsmqCreateQueueStep : TestStepBase
	{
		///<summary>
		/// List of queues to create
		///</summary>
		public Collection<QueuePathDefinition> QueuePaths { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			foreach(QueuePathDefinition queuePathDef in QueuePaths)
			{
				string queuePath = queuePathDef.QueuePath;
				string machineName = MSMQHelper.GetMachineNameFromQueueName(queuePath);
				bool transactional = queuePathDef.Transactional;

				MSMQHelper.QueueCreate(machineName, queuePath, string.Format("Test Queue : {0}", Guid.NewGuid()), transactional);

				context.LogInfo( "The queue: \"{0}\" was created successfully.", queuePath );
			}
		}

		public override void Validate(Context context)
		{
			if (null == QueuePaths)
				throw new ArgumentNullException("QueuePaths is null");
		}
	}
}
