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
	/// The SendPortConductorStep starts/stops a send port.
	/// </summary>
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.SendPortConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
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
	///			<term>SendPortName</term>
	///			<description>The name of the send port to start/stop</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
	///			<description>Start|Stop</description>
	///		</item>
	///	</list>
	///	</remarks>
	public class SendPortConductorStep : TestStepBase
	{
		///<summary>
		/// The name of the send port to containing the receive location to enable/dissable
		///</summary>
		public string SendPortName { get; set; }
		///<summary>
		/// The action to perform on the send port: Start|Stop
		///</summary>
		public BizUnitWcfServiceLibrary.SendPortAction Action { get; set; }
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
			var step = new BizUnitWcfServiceLibrary.SendPortConductorStep
			{
				Action = Action,
				DelayForCompletion = DelayForCompletion,
				SendPortName = SendPortName
			};
			context.LogInfo(step.ToString(), new object[] { });
			client.SendPortConductorStep(step);
		}

		///<summary>
		/// Validation method called prior to executing the test step
		///</summary>
		///<param name="context"></param>
		public override void Validate(Context context)
		{
			ArgumentValidation.CheckForEmptyString(SendPortName, "SendPortName");
		}
	}
}
