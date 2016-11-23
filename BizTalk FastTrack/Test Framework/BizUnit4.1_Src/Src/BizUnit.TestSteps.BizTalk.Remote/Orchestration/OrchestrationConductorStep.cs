//---------------------------------------------------------------------
// File: OrchestrationConductorStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Management;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.BizTalk.Remote.Orchestration
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
        public BizUnitWcfServiceLibrary.OrchestrationAction Action { get; set; }
        ///<summary>
        /// Time to wait in seconds after having stopped/started the orchestration
        ///</summary>
        public int DelayForCompletion { get; set; }

        /// <summary>
        /// ITestStep.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            var client = ServiceHelper.GetBizUnitService(context);
            var step = new BizUnitWcfServiceLibrary.OrchestrationConductorStep
            {
                Action = Action,
                AssemblyName = AssemblyName,
                DelayForCompletion = DelayForCompletion,
                OrchestrationName = OrchestrationName
            };
            context.LogInfo(step.ToString(), new object[] { });
            client.OrchestrationConductorStep(step);
        }

        /// <summary>
        /// Validate the class instance
        /// </summary>
        /// <param name="context"></param>
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
    }
}