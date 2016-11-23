
using BizUnit.BizUnitOM;
using BizUnit.TestSteps.Time;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class BizUnit4CoreTests
    {
        public BizUnit4CoreTests()
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
        public void SerializationV4TestStepsOnly()
        {
            TestCase btc = new TestCase();
            btc.Name = "Serialization Test";

            var fm = new DelayStep();
            fm.DelayMilliSeconds = 35;
            btc.SetupSteps.Add(fm);

            string testCase = TestCase.Save(btc);
            var btcNew = TestCase.LoadXaml(testCase);
        }

        [TestMethod]
        public void SerializationV4TestAndLegacySteps()
        {
            //TestCase btc = new TestCase();
            //btc.Name = "Serialization Test";
            //btc.BizUnitVersion = "4.0.0.1";

            //FileDeleteMultipleStep fdm = new FileDeleteMultipleStep();
            //fdm.Directory = @"C:\Tmp";
            //fdm.SearchPattern = "*.xml";
            //btc.SetupSteps.Add(fdm);

            //DelayStep fm = new DelayStep();
            //fm.Delay = 35;
            //btc.SetupSteps.Add(fm);

            //string testCase = BizUnitSerializationHelper.Serialize(btc);

            //BizUnitSerializationHelper btcNew = (TestCase)BizUnitSerializationHelper.Deserialize(testCase);
        }

        [TestMethod]
        public void ExecuteTestCase()
        {
            TestCase btc = new TestCase();
            btc.Name = "Serialization Test";
            btc.Description = "Test to blah blah blah, yeah really!";
            btc.BizUnitVersion = "4.0.0.1";

            var fm = new DelayStep {DelayMilliSeconds = 35};
            btc.SetupSteps.Add(fm);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }
    }
}
