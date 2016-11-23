//---------------------------------------------------------------------
// File: SendPortConductorStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Management;
using BizUnit.Common;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Remote.Port
{
	/// <summary>
	/// The SendPortGroupConductorStep starts/stops a send port group.
	/// </summary>
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.SendPortGroupConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<DelayForCompletion>5</DelayForCompletion> <!-- Optional, seconds to delay for this step to complete -->
	///		<SendPortName></SendPortName>
	///		<Action>Start</Action>
	/// </TestStep>
	/// </code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>DelayForCompletion</term>
	///			<description>The number of seconds to deplay for this step to complete</description>
	///		</item>
	///		<item>
	///			<term>SendPortGroupName</term>
	///			<description>The name of the send port group to start/stop</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
	///			<description>Start|Stop</description>
	///		</item>
	///	</list>
	///	</remarks>
	public class SendPortGroupConductorStep : TestStepBase
	{
		///<summary>
		/// The name of the send port group to start/stop
		///</summary>
		public string SendPortGroupName { get; set; }
		///<summary>
		/// The action to perform on the send port group: Start|Stop
		///</summary>
		public BizUnitWcfServiceLibrary.SendPortGroupAction Action { get; set; }
		///<summary>
		/// The number of seconds to deplay for this step to complete
		///</summary>
		public int DelayForCompletion { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			var client = ServiceHelper.GetBizUnitService(context);
			var step = new BizUnitWcfServiceLibrary.SendPortGroupConductorStep
			{
				Action = Action,
				DelayForCompletion = DelayForCompletion,
				SendPortGroupName = SendPortGroupName
			};
			context.LogInfo(step.ToString(), new object[] { });
			client.SendPortGroupConductorStep(step);
		}

		///<summary>
		/// Validation method called prior to executing the test step
		///</summary>
		///<param name="context"></param>
		public override void Validate(Context context)
		{
			ArgumentValidation.CheckForEmptyString(SendPortGroupName, "SendPortGroupName");
		}
	}
}
