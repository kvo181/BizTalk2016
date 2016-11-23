
using System;
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.Soap;
using BizUnit.TestSteps.ValidationSteps.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Tests
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
        public void WebServiceInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Serialization Test";
            btc.Description = "Test to blah blah blah, yeah really!";
            btc.BizUnitVersion = "4.0.0.1";

            var ws = new WebServiceStep();
            ws.Action = "http://schemas.virgin-atlantic.com/AncillarySales/Book/Services/2009/IAncillarySalesBook/GetProductTermsAndConditions";
            FileDataLoader dataLoader;
            dataLoader = new FileDataLoader();
            dataLoader.FilePath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\GetProductTermsAndConditions_RQ.xml";
            ws.RequestBody = dataLoader;
            ws.ServiceUrl = "http://localhost/AncillarySalesBook/AncillarySalesBook.svc";
            ws.Username = @"newkydog001\kevinsmi";
            var header = new SoapHeader();
            header.HeaderName = "ServiceCallingContext";
            header.HeaderNameSpace = "http://schemas.virgin-atlantic.com/Services/ServiceCallingContext/2009";

            var ctx = new ServiceCallingContext();
            ctx.ApplicationName = "BVT Tests";
            ctx.GUid = "{1705141E-F530-4657-BA2F-23F0F4A8BCB0}";
            ctx.RequestId = "{59ACDBB4-3FAF-4056-9459-49D43C4128F9}";
            ctx.UserId = "kevin";
            ctx.UTCTransactionStartDate = DateTime.UtcNow;
            ctx.UTCTransactionStartTime = DateTime.UtcNow.ToString("HH:mm:ss.fff");
            header.HeaderInstance = ctx;
            ws.SoapHeaders.Add(header);

            // Validation....
            var validation = new XmlValidationStep();
            var schemaResultType = new SchemaDefinition
            {
                XmlSchemaPath =
                    @"C:\Affinus\Depot\ASS\Main\Dev\Src\VAA.ASS.Schemas\VAACommon\Result_Type.xsd",
                XmlSchemaNameSpace =
                    "http://schemas.virgin-atlantic.com/Common/2009"
            };
            validation.XmlSchemas.Add(schemaResultType);
            var schema = new SchemaDefinition
                             {
                                 XmlSchemaPath =
                                     @"C:\Affinus\Depot\ASS\Main\Dev\Src\VAA.ASS.Schemas\Book\GetProductTermsAndConditions_RS.xsd",
                                 XmlSchemaNameSpace =
                                     "http://schemas.virgin-atlantic.com/AncillarySales/Book/Services/GetProductTermsAndConditions/2009"
                             };
            validation.XmlSchemas.Add(schema);

            var xpathProductId = new XPathDefinition();
            xpathProductId.XPath = "/*[local-name()='GetProductTermsAndConditions_RS' and namespace-uri()='http://schemas.virgin-atlantic.com/AncillarySales/Book/Services/GetProductTermsAndConditions/2009']/*[local-name()='Message' and namespace-uri()='']/*[local-name()='TermsAndConditions' and namespace-uri()='']/@*[local-name()='productId' and namespace-uri()='']";
            xpathProductId.Value = "1";
            validation.XPathValidations.Add(xpathProductId);

            var xpathContent = new XPathDefinition();
            xpathContent.XPath = "/*[local-name()='GetProductTermsAndConditions_RS' and namespace-uri()='http://schemas.virgin-atlantic.com/AncillarySales/Book/Services/GetProductTermsAndConditions/2009']/*[local-name()='Message' and namespace-uri()='']/*[local-name()='TermsAndConditions' and namespace-uri()='']/*[local-name()='Content' and namespace-uri()='']";
            xpathContent.Value = "Terms and Conditions: this product is non-refundable....";
            validation.XPathValidations.Add(xpathContent);

            ws.SubSteps.Add(validation);
            btc.ExecutionSteps.Add(ws);

            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "WebServiceInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void WebServiceInvoke_LoadFromXaml()
        {
            var tc = TestCase.LoadFromFile(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestCases\WebServiceInvokeTest.xml");
            BizUnit bu = new BizUnit(tc);
            bu.RunTest();
        }
    }
}
