using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BizUnit.TestSteps.BizTalk.Remote.Common;
using BizUnit.TestSteps.BizTalk.Remote.Host;
using BizUnit.TestSteps.BizTalk.Remote.Port;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnitWcfServiceLibrary;

namespace BizUnit.BizTalkServices.Tests
{
    /// <summary>
    /// Summary description for UnitTestRemoteStep
    /// </summary>
    [TestClass]
    public class UnitTestRemoteStep
    {
        public UnitTestRemoteStep()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void TestStopRemoteSendPort()
        {
            // Create the test case
            var testCase = new TestCase();
            testCase.Name = "Stop a send port in a remote group";
            testCase.ExpectedResults = "Test succeeds";

            var remote = new RemoteServerHostStep
                             {
                                 RemoteServer = "99-001-097-v058"
                             };
            testCase.ExecutionSteps.Add(remote);

            var sendPort = new TestSteps.BizTalk.Remote.Port.SendPortConductorStep
            {
                                   Action = SendPortAction.Stop,
                                   SendPortName = "HS_FileVerif_FILE",
                                   DelayForCompletion = 5
                               };
            testCase.ExecutionSteps.Add(sendPort);

            TestCase.SaveToFile(testCase, "TestStopRemoteSendPort.xml");
            // Execute the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

        }
        [TestMethod]
        public void TestStartRemoteSendPort()
        {
            // Create the test case
            var testCase = new TestCase();
            testCase.Name = "Start a send port in a remote group";
            testCase.ExpectedResults = "Test succeeds";

            var remote = new RemoteServerHostStep
            {
                RemoteServer = "99-001-097-v058"
            };
            testCase.ExecutionSteps.Add(remote);

            var sendPort = new TestSteps.BizTalk.Remote.Port.SendPortConductorStep
            {
                Action = SendPortAction.Start,
                SendPortName = "HS_FileVerif_FILE",
                DelayForCompletion = 5
            };
            testCase.ExecutionSteps.Add(sendPort);

            TestCase.SaveToFile(testCase, "TestStartRemoteSendPort.xml");

            // Execute the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

        }
        [TestMethod]
        public void TestStartRemoteHost()
        {
            // Create the test case
            var testCase = new TestCase();
            testCase.Name = "Start a host instance in a remote group";
            testCase.ExpectedResults = "Test succeeds";

            var remote = new RemoteServerHostStep
            {
                RemoteServer = "99-001-097-v058"
            };
            testCase.ExecutionSteps.Add(remote);

            var sendPort = new TestSteps.BizTalk.Remote.Host.HostConductorStep
            {
                Action = "start",
                HostInstanceName = "OnlineHost_YYY",
                Servers = "99-001-097-V058"
            };
            testCase.ExecutionSteps.Add(sendPort);

            TestCase.SaveToFile(testCase, "TestStartRemoteHost.xml");

            // Execute the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

        }
        [TestMethod]
        public void TestStopRemoteHost()
        {
            // Create the test case
            var testCase = new TestCase();
            testCase.Name = "Stop a host instance in a remote group";
            testCase.ExpectedResults = "Test succeeds";

            var remote = new RemoteServerHostStep
            {
                RemoteServer = "99-001-097-v058"
            };
            testCase.ExecutionSteps.Add(remote);

            var sendPort = new TestSteps.BizTalk.Remote.Host.HostConductorStep
            {
                Action = "stop",
                HostInstanceName = "OnlineHost_YYY",
                Servers = "99-001-097-V058"
            };
            testCase.ExecutionSteps.Add(sendPort);

            TestCase.SaveToFile(testCase, "TestStartRemoteHost.xml");

            // Execute the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

        }
    }
}
