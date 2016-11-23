//---------------------------------------------------------------------
// File: OrchestrationConductorStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2016, bizilante. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Management;
using BizUnit.Xaml;
using System.Threading;

namespace BizUnit.TestSteps.BizTalk.Orchestration
{
    /// <summary>
    /// The OrchestrationConductorStep may be used to stop/start an orchestration.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    /// <TestStep assemblyPath="" typeName="BizUnit.BizTalkSteps.OrchestrationConductorStep, BizUnit.BizTalkSteps, Version=3.1.0.0, Culture=neutral, PublicKeyToken=7eb7d82981ae5162">
    ///		<DelayForCompletion>5</DelayForCompletion> <!-- Optional, seconds to delay for this step to complete -->
    ///		<AssemblyName>BizUnitTest.Process</AssemblyName>
    ///		<OrchestrationName>BizUnitTest.Process.SubmitToLedger</OrchestrationName>
    ///		<Action>Start</Action>
    /// </TestStep>
    /// </code>
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>DelayForCompletion</term>
    ///			<description>The delay before executing the step <para>(optional)</para></description>
    ///		</item>
    ///		<item>
    ///			<term>AssemblyName</term>
    ///			<description>The name of the assembly containing the orchestration, e.g. BizUnitTest.Process</description>
    ///		</item>
    ///		<item>
    ///			<term>OrchestrationName</term>
    ///			<description>The name of the orchestration to start/stop</description>
    ///		</item>
    ///		<item>
    ///			<term>Action</term>
    ///			<description>Start|Stop</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	

    public class OrchestrationConductorStep : TestStepBase
    {
        ///<summary>
        /// Name of the BizTalk assembly
        ///</summary>
        public string AssemblyName { get; set; }
        ///<summary>
        /// Name of the Orchestration
        ///</summary>
        public string OrchestrationName { get; set; }
        ///<summary>
        /// We only allow you to stop/start an orchestration
        ///</summary>
        public OrchestrationAction Action { get; set; }
        ///<summary>
        /// Time to wait in seconds after having stopped/started the orchestration
        ///</summary>
        public int DelayForCompletion { get; set; }

        ///<summary>
        /// Possible actions on the orchestration
        ///</summary>
        public enum OrchestrationAction
        {
            ///<summary>
            /// Start the orchestration
            ///</summary>
            Start,
            ///<summary>
            /// Stop the orchestration
            ///</summary>
            Stop
        }

        private enum OrchestrationStatus
        {
            Unbound = 1,
            Bound,
            Stopped,
            Started
        }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            try
            {
                if (Action == OrchestrationAction.Start)
                {
                    Start(AssemblyName, OrchestrationName, context);
                }
                else
                {
                    Stop(AssemblyName, OrchestrationName, context);
                }

                // Delay if necessary to allow the orchestration to start/stop
                if (DelayForCompletion > 0)
                {
                    context.LogInfo("Waiting for {0} seconds before recommencing testing.", DelayForCompletion);
                    System.Threading.Thread.Sleep(DelayForCompletion * 1000);
                }
            }
            catch (System.Runtime.InteropServices.COMException e)
            {
                context.LogException(e);
                throw;
            }
        }

        public override void Validate(Context context)
        {
            if (string.IsNullOrEmpty(AssemblyName))
            {
                throw new ArgumentNullException("AssemblyName");
            }

            if (string.IsNullOrEmpty(OrchestrationName))
            {
                throw new ArgumentNullException("OrchestrationName");
            }
        }

        private static void Start(string assemblyName, string orchestrationName, Context context)
        {
            var started = false;
            var wmiQuery =
                String.Format("Select * from MSBTS_Orchestration where Name=\"{0}\" and AssemblyName=\"{1}\"", orchestrationName, assemblyName);
            context.LogInfo("Query for Orchestration: {0}", wmiQuery);

            var enumOptions = new EnumerationOptions { ReturnImmediately = false };
            var orchestrationSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer", wmiQuery, enumOptions);

            var retryCnt = -1;
            var status = (UInt32)999;
            var found = false;
            while (!started && (retryCnt < 10))
            {
                retryCnt++;
                if (retryCnt > 0) Thread.Sleep(5000);
                foreach (ManagementObject orchestrationInstance in orchestrationSearcher.Get())
                {
                    found = true;
                    status = (UInt32)orchestrationInstance.GetPropertyValue("OrchestrationStatus");
                    context.LogInfo("current orchestration status: {0}", status);
                    if (status == (UInt32)OrchestrationStatus.Unbound)
                    {
                        var e = new Exception("Orchestration in unbound");
                        context.LogException(e);
                        throw e;
                    }
                    switch (status)
                    {
                        case (UInt32)OrchestrationStatus.Bound:
                            // We need to Enlist the orchestration first
                            context.LogInfo("Enlisting Orchestration: {0}", orchestrationName);
                            orchestrationInstance.InvokeMethod("Enlist", new object[] { });
                            break;
                        case (UInt32)OrchestrationStatus.Stopped:
                            context.LogInfo("Starting Orchestration: {0}", orchestrationName);
                            /*
                             * AutoEnableReceiveLocationFlag 
                             * [in] Specifies whether receive locations associated with this orchestration should be automatically enabled. Permissible values for this parameter are (1) "No auto enable of receive locations related to this orchestration", or (2) "Enable all receive locations related to this orchestration". Note that the integer values must be used in code and script. The default value is 1. 
                             * AutoResumeOrchestrationInstanceFlag 
                             * [in] Specifies whether service instances of this orchestration type that were manually suspended previously should be automatically resumed. Permissible values for this parameter are (1) "No auto resume of service instances of this orchestration", or (2) "Automatically resume all suspended service instances of this orchestration" Note that the integer values must be used in code and script. The default value is 2. 
                             */
                            orchestrationInstance.InvokeMethod("Start", new object[] { 2, 2 });
                            context.LogInfo("Orchestration: {0} was successfully started", orchestrationName);
                            started = true;
                            break;
                        case (UInt32)OrchestrationStatus.Started:
                            context.LogInfo("Orchestration: {0} was already started", orchestrationName);
                            started = true;
                            break;
                        default:
                            var e = new Exception(string.Format("Unhandled orchestration status: {0}", status));
                            context.LogException(e);
                            throw e;
                    }
                }
                if (!found) retryCnt = 10;
            }

            if (!found)
                throw new Exception(string.Format("Orchestration: \"{0}\" in \"{1}\" not found", orchestrationName, assemblyName));
            if (!started)
                throw new Exception(string.Format("Failed to start the orchestration: \"{0}\" after {1} retries: status={2}", orchestrationName, retryCnt, status));
        }

        private static void Stop(string assemblyName, string orchestrationName, Context context)
        {
            var stopped = false;
            var wmiQuery =
                String.Format("Select * from MSBTS_Orchestration where Name=\"{0}\" and AssemblyName=\"{1}\"", orchestrationName, assemblyName);
            context.LogInfo("Query for Orchestration: {0}", wmiQuery);

            var enumOptions = new EnumerationOptions { ReturnImmediately = false };
            var orchestrationSearcher = new ManagementObjectSearcher("root\\MicrosoftBizTalkServer", wmiQuery, enumOptions);

            var retryCnt = -1;
            var status = (UInt32)999;
            var found = false;
            while (!stopped && (retryCnt < 10))
            {
                retryCnt++;
                if (retryCnt > 0) Thread.Sleep(5000);
                foreach (ManagementObject orchestrationInstance in orchestrationSearcher.Get())
                {
                    found = true;
                    status = (UInt32)orchestrationInstance.GetPropertyValue("OrchestrationStatus");
                    context.LogInfo("current orchestration status: {0}", status);
                    if (status == (UInt32)OrchestrationStatus.Unbound)
                    {
                        var e = new Exception("Orchestration is unbound");
                        context.LogException(e);
                        throw e;
                    }
                    switch (status)
                    {
                        case (UInt32)OrchestrationStatus.Bound:
                            // We need to Enlist the orchestration first
                            context.LogInfo("Enlisting Orchestration: {0}", orchestrationName);
                            orchestrationInstance.InvokeMethod("Enlist", new object[] { });
                            break;
                        case (UInt32)OrchestrationStatus.Started:
                            context.LogInfo("Stopping Orchestration: {0}", orchestrationName);
                            /*
                             * AutoEnableReceiveLocationFlag 
                             * [in] [in] Permissible values for this property are (1) "No auto disable of receive locations related to this Orchestration", or (2) "Disable all receive locations related to this orchestration that are not shared by other orchestration(s)". Note that the integer values must be used in code and script. The default value is 1. 
                             * AutoResumeOrchestrationInstanceFlag 
                             * [in] Permissible values for this property are (1) "No auto suspend of service instances of this Orchestration", or (2) "Suspend all running service instances of this Orchestration". Note that the integer values must be used in code and script. The default value is 2. 
                             */
                            orchestrationInstance.InvokeMethod("Stop", new object[] { 2, 2 });
                            context.LogInfo("Orchestration: {0} was successfully stopped", orchestrationName);
                            stopped = true;
                            break;
                        case (UInt32)OrchestrationStatus.Stopped:
                            context.LogInfo("Orchestration: {0} was already stopped", orchestrationName);
                            stopped = true;
                            break;
                        default:
                            var e = new Exception(string.Format("Unhandled orchestration status: {0}", status));
                            context.LogException(e);
                            throw e;
                    }
                }
                if (!found) retryCnt = 10;
            }

            if (!found)
                throw new Exception(string.Format("Orchestration: \"{0}\" in \"{1}\" not found", orchestrationName, assemblyName));
            if (!stopped)
                throw new Exception(string.Format("Failed to stop the orchestration: \"{0}\" after {1} retries: status={2}", orchestrationName, retryCnt, status));
        }
    }
}