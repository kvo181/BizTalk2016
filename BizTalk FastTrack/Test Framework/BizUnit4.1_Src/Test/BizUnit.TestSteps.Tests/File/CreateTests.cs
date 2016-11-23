
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.ValidationSteps.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.TestSteps.Tests.File
{

    /// <summary>
    /// Summary description for FileCreateTests
    /// </summary>
    [TestClass]
    public class CreateTests
    {
        public CreateTests()
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
        public void CreateFileTest()
        {
            var step = new CreateStep();
            step.CreationPath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\FileCreateStepTest.testdelxml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            var readStep = new FileReadMultipleStep();
            readStep.DirectoryPath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\.";
            readStep.SearchPattern = "*.testdelxml";

            var validation = new XmlValidationStep();
            var schemaPurchaseOrder = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder.xsd",
                XmlSchemaNameSpace =
                    "http://SendMail.PurchaseOrder"
            };
            validation.XmlSchemas.Add(schemaPurchaseOrder);

            var xpathProductId = new XPathDefinition();
            xpathProductId.Description = "PONumber";
            xpathProductId.XPath = "/*[local-name()='PurchaseOrder' and namespace-uri()='http://SendMail.PurchaseOrder']/*[local-name()='PONumber' and namespace-uri()='']";
            xpathProductId.Value = "12323";
            validation.XPathValidations.Add(xpathProductId);

            readStep.SubSteps.Add(validation);

            readStep.Execute(new Context());
        }
    }
}
