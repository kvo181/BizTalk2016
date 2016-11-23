using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.i8c.DataLoaders.Sql;
using BizUnit.TestSteps.i8c.IIS;
using BizUnit.TestSteps.i8c.MQSeries;
using BizUnit.TestSteps.i8c.ValidationSteps.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Tests
{
    /// <summary>
    /// Summary description for MQSeries test Steps
    /// </summary>
    [TestClass]
    public class IisRecycleAppPoolStepTests
    {
        public IisRecycleAppPoolStepTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
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
        public void RecycleInvoke()
        {
            var btc = new TestCase();
            btc.Name = "Recycle an application pool";
            btc.Description = "Check/Validate the IIS related steps";
            btc.BizUnitVersion = "4.0.0.1";

            var iis = new IisRecycleAppPoolStep();
            iis.AppPoolName = "BizTalkWebServicesPool";

            // Add step
            btc.ExecutionSteps.Add(iis);

            // Save and Execute test
            var bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "IISRecycleInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void RecycleInvokeLoadFromXaml()
        {
            var tc = TestCase.LoadFromFile(@"..\..\..\BizUnit.TestSteps.i8c.Tests\TestCases\IISRecycleInvoke.xml");
            var bu = new BizUnit(tc);
            bu.RunTest();
        }
    }
}
