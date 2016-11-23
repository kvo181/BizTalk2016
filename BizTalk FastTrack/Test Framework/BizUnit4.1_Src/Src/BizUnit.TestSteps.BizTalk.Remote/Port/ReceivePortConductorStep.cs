//---------------------------------------------------------------------
// File: ReceivePortConductorStep.cs
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
	/// The ReceivePortConductorStep enables/dissables a recieve location.
	/// </summary>
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	/// <TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.ReceivePortConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<DelayForCompletion>5</DelayForCompletion> <!-- Optional, seconds to delay for this step to complete -->
	///		<ReceivePortName></ReceivePortName>
	///		<ReceiveLocationName></ReceiveLocationName>
	///		<Action>Enable</Action>
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
	///			<term>ReceivePortName</term>
	///			<description>The name of the receive port to containing the receive location to enable/dissable</description>
	///		</item>
	///		<item>
	///			<term>ReceiveLocationName</term>
	///			<description>The name of the receive location to enable/dissable</description>
	///		</item>
	///		<item>
	///			<term>Action</term>
	///			<description>Enable|Disable</description>
	///		</item>
	///	</list>
	///	</remarks>
	public class ReceivePortConductorStep : TestStepBase
	{
		///<summary>
		/// The name of the receive port to containing the receive location to enable/dissable
		///</summary>
		public string ReceivePortName { get; set; }
		///<summary>
		/// The name of the receive location to enable/dissable
		///</summary>
		public string ReceiveLocationName { get; set; }
		///<summary>
		/// The action to perform on the receive location: Enable|Disable
		///</summary>
		public BizUnitWcfServiceLibrary.ReceivePortAction Action { get; set; }
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
			var step = new BizUnitWcfServiceLibrary.ReceivePortConductorStep
			{
				Action = Action,
				DelayForCompletion = DelayForCompletion,
				ReceiveLocationName = ReceiveLocationName,
				ReceivePortName = ReceivePortName,
			};
			context.LogInfo(step.ToString(), new object[] { });
			client.ReceivePortConductorStep(step);
		}

		///<summary>
		/// Validation method called prior to executing the test step
		///</summary>
		///<param name="context"></param>
		public override void Validate(Context context)
		{
			ArgumentValidation.CheckForEmptyString(ReceivePortName, "ReceivePortName");
			ArgumentValidation.CheckForEmptyString(ReceiveLocationName, "ReceiveLocationName");
		}
	}
}
