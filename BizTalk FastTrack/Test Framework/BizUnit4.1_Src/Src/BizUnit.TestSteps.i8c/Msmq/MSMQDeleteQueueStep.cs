//---------------------------------------------------------------------
// File: MSMQDeleteQueueStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.ObjectModel;
using System.Xml;
using System.Messaging;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Msmq
{
	/// <summary>
	/// The MSMQDeleteQueueStep deletes one or more MSMQ queues
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQDeleteQueueStep">
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
	///			<term>QueueName</term>
	///			<description>The name of the MSMQ queue to delete, e.g. .\Private$\Test02 <para>(one or more)</para></description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class MsmqDeleteQueueStep : TestStepBase
	{
		///<summary>
		/// List of queues to delete
		///</summary>
		public Collection<QueuePathDefinition> QueuePaths { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed between tests</param>
		public override void Execute(Context context)
		{
			foreach (QueuePathDefinition queuePathDef in QueuePaths)
			{
				string queuePath = queuePathDef.QueuePath;
                string machineName = MSMQHelper.GetMachineNameFromQueueName(queuePath);
                MSMQHelper.DeleteMsmqQueue(queuePath, machineName);

				context.LogInfo( "The queue: \"{0}\" was deleted successfully.", queuePath );
			}
		}

		public override void Validate(Context context)
		{
			if (null == QueuePaths)
				throw new ArgumentNullException("QueuePaths is null");
		}
	}
}
