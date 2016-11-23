using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.i8c.DataLoaders.Sql;
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
    public class MqSeriesStepTests
    {
        public MqSeriesStepTests()
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
        public void MqSeriesInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Send and Read operations on a MQSeries queue";
            btc.Description = "Check/Validate the MQSeries related steps";
            btc.BizUnitVersion = "4.0.0.1";

            const string queueManager = "QM_ACV_LOC";
            const string queueName = "QL_LOC_ACV_CTW_CONS_OUT";

            // Put a message onto the queue
            var dataLoader = new FileDataLoader();
            dataLoader.FilePath = TestContext.TestDir + @"\..\..\Test\BizUnit.TestSteps.i8c.Tests\TestData\BizTalk2006.ToBeProcessed.A820.Request.xml";
            var wsPut = new MQSeriesPutStep();
            wsPut.QueueManager = queueManager;
            wsPut.Queue = queueName;
            wsPut.MessageBody = dataLoader;
            // Add step
            btc.ExecutionSteps.Add(wsPut);

            // Read and validate the message
            var wsRead = new MQSeriesGetStep();
            wsRead.QueueManager = queueManager;
            wsRead.Queue = queueName;
            wsRead.SubSteps.Add(new BinaryValidationStep
                                    {
                                        ComparisonDataPath = TestContext.TestDir + @"\..\..\Test\BizUnit.TestSteps.i8c.Tests\TestData\BizTalk2006.ToBeProcessed.A820.Request.xml",
                                        ReadAsString = true
                                    });
            // Add step
            btc.ExecutionSteps.Add(wsRead);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "MQSeriesInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void MqSeriesInvoke_LoadFromXaml()
        {
            var tc = TestCase.LoadFromFile(@"..\..\..\BizUnit.TestSteps.i8c.Tests\TestCases\MQSeriesInvoke.xml");
            BizUnit bu = new BizUnit(tc);
            bu.RunTest();
        }

        [TestMethod]
        public void MqSeriesPurge()
        {
            // Create the test case
            var btc = new TestCase();
            btc.Name = "Purge operations on a MQSeries queue";
            btc.Description = "Check/Validate the MQSeries related steps";
            btc.BizUnitVersion = "4.0.0.1";

            var purgeStep = new MQSeriesClearQueueStep();
            purgeStep.QueueManager = "QM_ACV_LOC";
            purgeStep.Queues = new Collection<object>
                                    {
                                      "QX_LOC_ACV"  // Transmission queue for QR_LOC_ACV_XML_OUT
                                    };

            btc.ExecutionSteps.Add(purgeStep);

            // Execute the test
            TestCase.SaveToFile(btc, "MQSeriesPurge.xaml");
            var bizUnit = new BizUnit(btc);
            bizUnit.RunTest();
        }
    }
}
