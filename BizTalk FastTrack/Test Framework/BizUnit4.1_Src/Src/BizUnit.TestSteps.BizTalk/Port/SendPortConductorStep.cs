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
        /// The action to perform on the send port
        ///</summary>
        public enum SendPortAction
        {
            ///<summary>
            /// Start the orchestration
            ///</summary>
            Start,
            ///<summary>
            /// Stop the orchestration
            ///</summary>
            Stop,
            ///<summary>
            /// Unenlist the orchestration
            ///</summary>
            Unenlist
        }

        private enum SendPortStatus
        {
            Bound = 1,
            Stopped,
            Started
        }

        ///<summary>
        /// The name of the send port to containing the receive location to enable/dissable
        ///</summary>
        public string SendPortName { get; set; }
        ///<summary>
        /// The action to perform on the send port: Start|Stop
        ///</summary>
        public SendPortAction Action { get; set; }
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
            switch (Action)
            {
                case SendPortAction.Start:
                    Start(SendPortName, context);
                    break;
                case SendPortAction.Stop:
                    Stop(SendPortName, context);
                    break;
                case SendPortAction.Unenlist:
                    Unenlist(SendPortName, context);
                    break;
            }

            // Delay if necessary to allow the orchestration to start/stop
            if (DelayForCompletion > 0)
            {
                context.LogInfo("Waiting for {0} seconds before recommencing testing.", DelayForCompletion);
                System.Threading.Thread.Sleep(DelayForCompletion * 1000);
            }
        }

        ///<summary>
        /// Validation method called prior to executing the test step
        ///</summary>
        ///<param name="context"></param>
        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(SendPortName, "SendPortName");
        }

        private static void Start(string sendPortName, Context context)
        {
            var started = false;
            var wmiQuery =
                String.Format("Select * from MSBTS_SendPort where Name=\"{0}\"", sendPortName);
            context.LogInfo("Query for Send Port: {0}", wmiQuery);

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
                    context.LogInfo("current send port status: {0}", status);
                    switch (status)
                    {
                        case (UInt32)SendPortStatus.Bound:
                            // We need to Enlist the send port first
                            context.LogInfo("Enlisting Send Port: {0}", sendPortName);
                            portInstance.InvokeMethod("Enlist", null);
                            break;
                        case (UInt32)SendPortStatus.Stopped:
                            context.LogInfo("Starting Send Port: {0}", sendPortName);
                            portInstance.InvokeMethod("Start", null);
                            context.LogInfo("Send Port: {0} was successfully started", sendPortName);
                            started = true;
                            break;
                        case (UInt32)SendPortStatus.Started:
                            context.LogInfo("Send Port: {0} was already started", sendPortName);
                            started = true;
                            break;
                        default:
                            var e = new Exception(string.Format("Unhandled send port status: {0}", status));
                            context.LogException(e);
                            throw e;
                    }
                }
            }

            if (!started)
            {
                throw new Exception(string.Format("Failed to start the send port: \"{0}\" after {1} retries: status={2}", sendPortName, retryCnt, status));
            }
        }

        private static void Stop(string sendPortName, Context context)
        {
            var stopped = false;
            var wmiQuery =
                String.Format("Select * from MSBTS_SendPort where Name=\"{0}\"", sendPortName);
            context.LogInfo("Query for Send Port: {0}", wmiQuery);

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
                    context.LogInfo("current send port status: {0}", status);
                    switch (status)
                    {
                        case (UInt32)SendPortStatus.Bound:
                            // We need to Enlist the orchestration first
                            context.LogInfo("Enlisting Send Port: {0}", sendPortName);
                            portInstance.InvokeMethod("Enlist", null);
                            break;
                        case (UInt32)SendPortStatus.Started:
                            context.LogInfo("Stopping Send Port: {0}", sendPortName);
                            portInstance.InvokeMethod("Stop", null);
                            context.LogInfo("Send Port: {0} was successfully stopped", sendPortName);
                            stopped = true;
                            break;
                        case (UInt32)SendPortStatus.Stopped:
                            context.LogInfo("Send Port: {0} was already stopped", sendPortName);
                            stopped = true;
                            break;
                        default:
                            var e = new Exception(string.Format("Unhandled send port status: {0}", status));
                            context.LogException(e);
                            throw e;
                    }
                }
            }

            if (!stopped)
            {
                throw new Exception(string.Format("Failed to stop the send port: \"{0}\" after {1} retries: status={2}", sendPortName, retryCnt, status));
            }
        }

        private static void Unenlist(string sendPortName, Context context)
        {
            var bound = false;
            var wmiQuery =
                String.Format("Select * from MSBTS_SendPort where Name=\"{0}\"", sendPortName);
            context.LogInfo("Query for Send Port: {0}", wmiQuery);

            var enumOptions = new EnumerationOptions { ReturnImmediately = false };
            var portSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer", wmiQuery, enumOptions);

            var retryCnt = -1;
            var status = (UInt32)999;
            while (!bound && (retryCnt < 10))
            {
                retryCnt++;
                foreach (ManagementObject portInstance in portSearcher.Get())
                {
                    status = (UInt32)portInstance.GetPropertyValue("Status");
                    context.LogInfo("current send port status: {0}", status);
                    switch (status)
                    {
                        case (UInt32)SendPortStatus.Stopped:
                        case (UInt32)SendPortStatus.Started:
                            context.LogInfo("UnEnlisting Send Port: {0}", sendPortName);
                            portInstance.InvokeMethod("UnEnlist", null);
                            context.LogInfo("Send Port: {0} was successfully unenlisted", sendPortName);
                            bound = true;
                            break;
                        case (UInt32)SendPortStatus.Bound:
                            context.LogInfo("Send Port: {0} was already unenlisted", sendPortName);
                            bound = true;
                            break;
                        default:
                            var e = new Exception(string.Format("Unhandled send port status: {0}", status));
                            context.LogException(e);
                            throw e;
                    }
                }
            }

            if (!bound)
            {
                throw new Exception(string.Format("Failed to unenlist the send port: \"{0}\" after {1} retries: status={2}", sendPortName, retryCnt, status));
            }
        }
    }
}
