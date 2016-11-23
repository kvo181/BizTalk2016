using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.Soap;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Tests
{
    /// <summary>
    /// Summary description for WebServiceStep
    /// </summary>
    [TestClass]
    public class WebServiceStepTests
    {
        public WebServiceStepTests()
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
        public void WebServiceInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Send SOAP message to BizTalk";
            btc.Description = "Send a SOAP message to BizTalk as would have done Horizon";
            btc.BizUnitVersion = "4.0.0.1";

            var ws = new WebServiceStep();
            ws.Action = "http://bts.online.bizilante.intranet/ACVCSC.BizTalk.WebServiceInterface/SubmitRequest";
            FileDataLoader dataLoader;
            dataLoader = new FileDataLoader();
            dataLoader.FilePath = @"..\..\..\BizUnit.TestSteps.i8c.Tests\TestData\SubmitRequest.xml";
            ws.RequestBody = dataLoader;
            ws.ServiceUrl = "http://localhost:8888/ACVCSC.BizTalk.Common.WebServiceInterface/Common.asmx";
            ws.UseDefaultCredentials = true;
            ws.HasResponse = false;

            // Validation....

            // Add steps
            btc.ExecutionSteps.Add(ws);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "WebServiceInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void WebServiceInvoke_LoadFromXaml()
        {
            var tc = TestCase.LoadFromFile(@"..\..\..\BizUnit.TestSteps.i8c.Tests\TestCases\WebServiceInvokeTest.xml");
            BizUnit bu = new BizUnit(tc);
            bu.RunTest();
        }
    }
}
