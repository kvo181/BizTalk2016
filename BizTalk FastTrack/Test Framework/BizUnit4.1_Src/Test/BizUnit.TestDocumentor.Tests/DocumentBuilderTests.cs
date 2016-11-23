
using System;

namespace BizUnit.TestDocumentor.Tests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class DocumentBuilderTests
    {
        public DocumentBuilderTests() {}

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
        public void DocumentBizUnitTests()
        {
            var documentor = new DocumentBuilder(new Logger());

            var foo = testContextInstance.TestDir;

            documentor.GenerateDocumentation(
                @"..\..\..\Test\BizUnit.TestDocumentor.Tests\Templates\TestReportTemplate0.2.xml",
                @"..\..\..\Test\BizUnit.TestDocumentor.Tests\Templates\CategoryTemplate.xml",
                @"..\..\..\Test\BizUnit.TestDocumentor.Tests\Templates\TestCaseTemplate.xml",
                @"..\..\..\Test\BizUnit.TestDocumentor.Tests\Tests",
                @"..\..\..\Test\BizUnit.TestDocumentor.Tests\Output\BizUnitTestReport_v1.0.xml",
                @"..\..\..\Test\BizUnit.TestDocumentor.Tests\Bins",
                true);
        }
    }
}
