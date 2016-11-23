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

namespace BizUnit.TestSteps.BizTalk.Port
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
		/// The action to perform on the send port group
		///</summary>
		public enum SendPortGroupAction 
		{
			///<summary>
			/// Start the Send Port Group
			///</summary>
			Start,
			///<summary>
			/// Stop the Send Port Group
			///</summary>
			Stop
		}

		private enum SendPortGroupStatus
		{
			Bound = 1,
			Stopped,
			Started
		}

		///<summary>
		/// The name of the send port group to stop/start
		///</summary>
		public string SendPortGroupName { get; set; }
		///<summary>
		/// The action to perform on the send port group: Start|Stop
		///</summary>
		public SendPortGroupAction Action { get; set; }
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
			if ( Action == SendPortGroupAction.Start )
			{
				Start( SendPortGroupName, context );
			}
			else
			{
				Stop( SendPortGroupName, context );
			}

			// Delay if necessary to allow the orchestration to start/stop
			if (DelayForCompletion > 0)
			{
				context.LogInfo("Waiting for {0} seconds before recommencing testing.", DelayForCompletion);
				System.Threading.Thread.Sleep(DelayForCompletion*1000);
			}
		}

		///<summary>
		/// Validation method called prior to executing the test step
		///</summary>
		///<param name="context"></param>
		public override void Validate(Context context)
		{
			ArgumentValidation.CheckForEmptyString(SendPortGroupName, "SendPortGroupName");
		}

		private static void Start(string sendPortGroupName, Context context)
		{
			var started = false;
			var wmiQuery =
				String.Format("Select * from MSBTS_SendPortGroup where Name=\"{0}\"", sendPortGroupName);
			context.LogInfo("Query for Send Port Group: {0}", wmiQuery);

			var enumOptions = new EnumerationOptions { ReturnImmediately = false };
			var portSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer", wmiQuery, enumOptions);

			var retryCnt = -1;
			var status = (UInt32)999;
			while (!started && (retryCnt < 10))
			{
				retryCnt++;
				foreach (ManagementObject portInstance in portSearcher.Get())
				{
					status = (UInt32)portInstance.GetPropertyValue("Status");
					context.LogInfo("current send port group status: {0}", status);
					switch (status)
					{
						case (UInt32)SendPortGroupStatus.Bound:
							// We need to Enlist the send port first
							context.LogInfo("Enlisting Send Port Group: {0}", sendPortGroupName);
							portInstance.InvokeMethod("Enlist", null);
							break;
						case (UInt32)SendPortGroupStatus.Stopped:
							context.LogInfo("Starting Send Port Group: {0}", sendPortGroupName);
							portInstance.InvokeMethod("Start", null);
							context.LogInfo("Send Port Group: {0} was successfully started", sendPortGroupName);
							started = true;
							break;
						case (UInt32)SendPortGroupStatus.Started:
							context.LogInfo("Send Port Group: {0} was already started", sendPortGroupName);
							started = true;
							break;
						default:
							var e = new Exception(string.Format("Unhandled send port group status: {0}", status));
							context.LogException(e);
							throw e;
					}
				}
			}

			if (!started)
			{
				throw new Exception(string.Format("Failed to start the send port group: \"{0}\" after {1} retries: status={2}", sendPortGroupName, retryCnt, status));
			}
		}

		private static void Stop(string sendPortGroupName, Context context)
		{
			var stopped = false;
			var wmiQuery =
				String.Format("Select * from MSBTS_SendPortGroup where Name=\"{0}\"", sendPortGroupName);
			context.LogInfo("Query for Send Port Group: {0}", wmiQuery);

			var enumOptions = new EnumerationOptions { ReturnImmediately = false };
			var portSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer", wmiQuery, enumOptions);

			var retryCnt = -1;
			var status = (UInt32)999;
			while (!stopped && (retryCnt < 10))
			{
				retryCnt++;
				foreach (ManagementObject portInstance in portSearcher.Get())
				{
					status = (UInt32)portInstance.GetPropertyValue("Status");
					context.LogInfo("current send port group status: {0}", status);
					switch (status)
					{
						case (UInt32)SendPortGroupStatus.Bound:
							// We need to Enlist the group first
							context.LogInfo("Enlisting Send Port Group: {0}", sendPortGroupName);
							portInstance.InvokeMethod("Enlist", null);
							break;
						case (UInt32)SendPortGroupStatus.Started:
							context.LogInfo("Stopping Send Port Group: {0}", sendPortGroupName);
					        portInstance.InvokeMethod("Stop", null);
							context.LogInfo("Send Port Group: {0} was successfully stopped", sendPortGroupName);
							stopped = true;
							break;
						case (UInt32)SendPortGroupStatus.Stopped:
							context.LogInfo("Send Port Group: {0} was already stopped", sendPortGroupName);
							stopped = true;
							break;
						default:
							var e = new Exception(string.Format("Unhandled send port group status: {0}", status));
							context.LogException(e);
							throw e;
					}
				}
			}

			if (!stopped)
			{
				throw new Exception(string.Format("Failed to stop the send port group: \"{0}\" after {1} retries: status={2}", sendPortGroupName, retryCnt, status));
			}
		}
	}
}
