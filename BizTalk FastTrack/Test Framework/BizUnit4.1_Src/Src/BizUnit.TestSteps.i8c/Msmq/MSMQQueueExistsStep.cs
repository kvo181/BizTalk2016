//---------------------------------------------------------------------
// File: MSMQQueueExistsStep.cs
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
	/// The MSMQQueueExistsStep checks if a MSMQ queue exists
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.MSMQQueueExistsStep">
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
	///			<description>The MSMQ queues to check, multiple entries maybe specified</description>
	///		</item>
	///	</list>
	///	</remarks>	
	public class MsmqQueueExistsStep : TestStepBase
	{
		///<summary>
		/// List of queues to purge
		///</summary>
		public Collection<QueuePathDefinition> QueuePaths { get; set; }
		/// <summary>
		/// Throw error or not?
		/// </summary>
		public bool ThrowError { get; set; }
		/// <summary>
		/// Delete the queue when it exists.
		/// (only when ShouldExist = false)
		/// </summary>
		public bool DeleteWhenExists { get; set; }

		/// <summary>
		/// ITestStep.Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			foreach (QueuePathDefinition queuePathDef in QueuePaths)
			{
				string queuePath = queuePathDef.QueuePath;
			    string machineName = MSMQHelper.GetMachineNameFromQueueName(queuePath);
			    var exists = MSMQHelper.QueueExists(queuePath, machineName);
				context.LogInfo("MSMQQueueExistsStep - queue: '{0}' {1}", queuePath, exists ? "exists" : "does not exist");
				context.Add(string.Format("MSMQ queue: {0}", queuePath), exists);
				if (ThrowError)
					if (queuePathDef.ShouldExist != exists)
						throw new Exception(string.Format("The queue '{0}' {1} but {2}", queuePath, queuePathDef.ShouldExist ? "should exist" : "should not exist", exists ? "exists" : "does not exist"));
				if (!queuePathDef.ShouldExist && exists && DeleteWhenExists)
					MSMQHelper.DeleteMsmqQueue(queuePath, machineName);
			}
		}

		public override void Validate(Context context)
		{
			if (null == QueuePaths)
				throw new ArgumentNullException("QueuePaths is null");
		}
	}
}
