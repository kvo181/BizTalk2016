using BizUnit.TestSteps.BizTalk.Port;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BizUnit;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Tests
{
    
    
    /// <summary>
    ///This is a test class for ReceiveLocationOperatingWindowEnabledStepTest and is intended
    ///to contain all ReceiveLocationOperatingWindowEnabledStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReceiveLocationOperatingWindowEnabledStepTest
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

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void OperatingWindowEnableInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Enabel a service window on a receive location";
            btc.Description = "Check/Validate the related steps";
            btc.BizUnitVersion = "4.0.0.1";
           
            var step = new ReceiveLocationOperatingWindowEnabledStep();
            step.Enable = true;
            step.ReceiveLocationName = "Receive Location1";

            btc.ExecutionSteps.Add(step);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "OperatingWindowEanbleInvoke.xaml");
            bu.RunTest();
        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void OperatingWindowDisableInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Enabel a service window on a receive location";
            btc.Description = "Check/Validate the related steps";
            btc.BizUnitVersion = "4.0.0.1";

            var step = new ReceiveLocationOperatingWindowEnabledStep();
            step.Enable = false;
            step.ReceiveLocationName = "Receive Location1";

            btc.ExecutionSteps.Add(step);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "OperatingWindowDisableInvoke.xaml");
            bu.RunTest();
        }
    }
}
