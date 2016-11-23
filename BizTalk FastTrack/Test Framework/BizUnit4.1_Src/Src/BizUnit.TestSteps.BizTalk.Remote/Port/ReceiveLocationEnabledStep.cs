//---------------------------------------------------------------------
// File: ReceiveLocationEnabledStep.cs
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
	/// The ReceiveLocationEnabledStep test step checks to determine whether a specific receive location is enabled or disabled.
	/// </summary>
	/// 
	/// <remarks>
	/// The following shows an example of the Xml representation of this test step.
	/// 
	/// <code escaped="true">
	///	<TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.ReceiveLocationEnabledStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
	///		<ReceiveLocationName>GovenorIn</ReceiveLocationName>
	///		<IsDisabled>true</IsDisabled>
	///	</TestStep>
	///	</code>
	///	
	///	<list type="table">
	///		<listheader>
	///			<term>Tag</term>
	///			<description>Description</description>
	///		</listheader>
	///		<item>
	///			<term>ReceiveLocationName</term>
	///			<description>The name of the receive location to check</description>
	///		</item>
	///		<item>
	///			<term>IsDisabled</term>
	///			<description>If true is specified then the test step will check to see that the receive location is disabled, if false, the step will check it is enabled</description>
	///		</item>
	///	</list>
	///	</remarks>

	public class ReceiveLocationEnabledStep : TestStepBase
	{
		///<summary>
		/// The name of the receive location to check
		///</summary>
		public string ReceiveLocationName { get; set; }
		///<summary>
		/// If true is specified then the test step will check to see that the receive location is disabled, if false, the step will check it is enabled
		///</summary>
		public bool IsDisabled { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			var client = ServiceHelper.GetBizUnitService(context);
			var step = new BizUnitWcfServiceLibrary.ReceiveLocationEnabledStep
			{
				IsDisabled = IsDisabled,
				ReceiveLocationName = ReceiveLocationName
			};
			context.LogInfo(step.ToString(), new object[] { });
			client.ReceiveLocationEnabledStep(step);
		}

        /// <summary>
        /// Validate this class instance
        /// </summary>
        /// <param name="context"></param>
		public override void Validate(Context context)
		{
			ArgumentValidation.CheckForEmptyString(ReceiveLocationName, "ReceiveLocationName");
		}
	}
}
