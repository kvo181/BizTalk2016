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

namespace BizUnit.TestSteps.BizTalk.Remote
{
	/// <summary>
	/// The GetData step is used to verify the web connection.
	/// </summary>

	public class GetDataStep : TestStepBase
	{
		///<summary>
		/// The value to pass to the GetData webmethod
		///</summary>
		public int Value { get; set; }

		/// <summary>
		/// Execute() implementation
		/// </summary>
		/// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
		public override void Execute(Context context)
		{
			var client = ServiceHelper.GetBizUnitService(context);
			context.LogInfo("GetData result = {0}", client.GetData(Value));
		}

		///<summary>
		/// Validation method called prior to executing the test step
		///</summary>
		///<param name="context"></param>
		public override void Validate(Context context)
		{
		}
	}
}
