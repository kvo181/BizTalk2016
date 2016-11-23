using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.Xaml;
using BizUnit.TestSteps.BizTalk.Port;
using System.IO;
using BizUnit.TestSteps.Time;

namespace BizUnit.TestSteps.i8c.Tests
{
    [TestClass]
    public class ReceiveLocationTests
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        [TestMethod]
        public void CreateLocation()
        {
            TestCase btc = new TestCase();

            string address = Path.GetTempPath();

            ReceiveLocationCreateStep createStep = new ReceiveLocationCreateStep
            {
                PortName = "BizUnitTestPort",
                LocationName = "BizUnitTestLocation",
                Address = address,
                Pipeline = ReceiveLocationCreateStep.PipelineType.Xml
            };
            btc.ExecutionSteps.Add(createStep);

            DelayStep delayStep = new DelayStep
            {
                DelayMilliSeconds = 60000
            };
            btc.ExecutionSteps.Add(delayStep);

            ReceiveLocationEnabledStep validateStep = new ReceiveLocationEnabledStep
            {
                ReceiveLocationName = "BizUnitTestLocation",
                IsDisabled = true
            };
            btc.ExecutionSteps.Add(validateStep);

            ReceivePortConductorStep enableStep = new ReceivePortConductorStep
            {
                ReceivePortName = "BizUnitTestPort",
                ReceiveLocationName = "BizUnitTestLocation",
                Action = ReceivePortConductorStep.ReceivePortAction.Enable
            };
            btc.ExecutionSteps.Add(enableStep);

            ReceivePortConductorStep disableStep = new ReceivePortConductorStep
            {
                ReceivePortName = "BizUnitTestPort",
                ReceiveLocationName = "BizUnitTestLocation",
                Action = ReceivePortConductorStep.ReceivePortAction.Disable
            };
            btc.CleanupSteps.Add(disableStep);

            ReceiveLocationDeleteStep deleteStep = new ReceiveLocationDeleteStep
            {
                LocationName = "BizUnitTestLocation"
            };
            btc.CleanupSteps.Add(deleteStep);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();

        }
    }
}
