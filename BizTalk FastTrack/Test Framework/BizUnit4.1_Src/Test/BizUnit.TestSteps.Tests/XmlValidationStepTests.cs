
using System;
using System.Xml.Schema;
using BizUnit.TestSteps.ValidationSteps.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.TestSteps.Common;

namespace BizUnit.TestSteps.Tests
{
    /// <summary>
    /// Summary description for XmlValidationStepTests
    /// </summary>
    [TestClass]
    public class XmlValidationStepTests
    {
        public XmlValidationStepTests()
        {
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
        public void XmlValidationStepTest()
        {
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

            Context ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml");
            validation.Execute(data, ctx);
        }

        [ExpectedException(typeof(ApplicationException))]
        [TestMethod]
        public void XmlValidationStepTest_InvalidXPath()
        {
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

            Context ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder002_BadXPath.xml");
            validation.Execute(data, ctx);
        }

        [ExpectedException(typeof(XmlSchemaValidationException))]
        [TestMethod]
        public void XmlValidationStepTest_SchemaValidationFail()
        {
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

            Context ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder003_SchemaValidationFail.xml");
            validation.Execute(data, ctx);
        }

        [ExpectedException(typeof(XmlSchemaValidationException))]
        [TestMethod]
        public void XmlValidationStepTest_SchemaValidationFailMissingElem()
        {
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

            Context ctx = new Context();
            var data = StreamHelper.LoadFileToStream(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder004_SchemaValidationFailMissingElem.xml");
            validation.Execute(data, ctx);
        }
    }
}
