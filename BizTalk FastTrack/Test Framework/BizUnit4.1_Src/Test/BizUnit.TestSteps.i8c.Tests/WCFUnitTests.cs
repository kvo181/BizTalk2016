using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BizUnit.TestSteps.i8c.WCF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using b = BizUnit;

namespace BizUnit.TestSteps.i8c.Tests
{
    [TestClass]
    public class WCFUnitTests
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

        [TestMethod]
        public void TestMethod_11()
        {
            var wcfTestCase = new b.Xaml.TestCase();
            var wcfTestStep = new WcfTestStep();

            wcfTestStep.EndpointName = "BasicHttpBinding_IService1";
            wcfTestStep.InputMessageTypeName = "BizUnit.TestSteps.i8c.Tests.ExampleServiceReference.CompositeType, BizUnit.TestSteps.i8c.Tests";
            wcfTestStep.InterfaceTypeName = "BizUnit.TestSteps.i8c.Tests.ExampleServiceReference.IService1, BizUnit.TestSteps.i8c.Tests";
            wcfTestStep.MethodName = "GetDataUsingDataContract";

            var xdl = new b.TestSteps.DataLoaders.Xml.XmlDataLoader();
            xdl.FilePath = testContextInstance.TestDir + @"\..\..\Test\BizUnit.TestSteps.i8c.Tests\TestData\Input.xml";
            xdl.UpdateXml = new System.Collections.ObjectModel.Collection<b.TestSteps.Common.XPathDefinition>();
            xdl.UpdateXml.Add(new b.TestSteps.Common.XPathDefinition { XPath = "/*[local-name()='CompositeType' and namespace-uri()='']/*[local-name()='FirstValue' and namespace-uri()='']", Value = "4" });
            xdl.UpdateXml.Add(new b.TestSteps.Common.XPathDefinition { XPath = "/*[local-name()='CompositeType' and namespace-uri()='']/*[local-name()='SecondValue' and namespace-uri()='']", Value = "7" });
            wcfTestStep.DataLoader = xdl;

            var xmlvalidationstep = new b.TestSteps.ValidationSteps.Xml.XmlValidationStep();
            xmlvalidationstep.XmlSchemas = new System.Collections.ObjectModel.Collection<b.TestSteps.ValidationSteps.Xml.SchemaDefinition>();
            xmlvalidationstep.XPathValidations = new System.Collections.ObjectModel.Collection<b.TestSteps.Common.XPathDefinition>();

            xmlvalidationstep.XmlSchemas.Add(new b.TestSteps.ValidationSteps.Xml.SchemaDefinition
            {
                XmlSchemaPath = testContextInstance.TestDir + @"\..\..\Test\BizUnit.TestSteps.i8c.Tests\TestData\CompositeTypeSchema.xsd",
                XmlSchemaNameSpace = ""
            });

            var xpathDef = new b.TestSteps.Common.XPathDefinition();
            xpathDef.XPath = "/*[local-name()='CompositeType' and namespace-uri()='']/*[local-name()='Result' and namespace-uri()='']";
            xpathDef.Value = "11";

            xmlvalidationstep.XPathValidations.Add(xpathDef);
            wcfTestStep.SubSteps.Add(xmlvalidationstep);
            wcfTestCase.ExecutionSteps.Add(wcfTestStep);

            var bizunit = new b.BizUnit(wcfTestCase);
            bizunit.RunTest();
        }

        [TestMethod]
        public void TestMethod_97()
        {
            var wcfTestCase = new b.Xaml.TestCase();

            var wcfTestStep = new WcfTestStep();

            var xdl = new b.TestSteps.DataLoaders.Xml.XmlDataLoader();
            xdl.FilePath = testContextInstance.TestDir + @"\..\..\Test\BizUnit.TestSteps.i8c.Tests\TestData\Input.xml";
            xdl.UpdateXml = new System.Collections.ObjectModel.Collection<b.TestSteps.Common.XPathDefinition>();

            xdl.UpdateXml.Add(new b.TestSteps.Common.XPathDefinition { XPath = "/*[local-name()='CompositeType' and namespace-uri()='']/*[local-name()='FirstValue' and namespace-uri()='']", Value = "36" });
            xdl.UpdateXml.Add(new b.TestSteps.Common.XPathDefinition { XPath = "/*[local-name()='CompositeType' and namespace-uri()='']/*[local-name()='SecondValue' and namespace-uri()='']", Value = "61" });

            wcfTestStep.DataLoader = xdl;

            wcfTestStep.EndpointName = "BasicHttpBinding_IService1";
            wcfTestStep.InputMessageTypeName = "BizUnit.TestSteps.i8c.Tests.ExampleServiceReference.CompositeType, BizUnit.TestSteps.i8c.Tests";
            wcfTestStep.InterfaceTypeName = "BizUnit.TestSteps.i8c.Tests.ExampleServiceReference.IService1, BizUnit.TestSteps.i8c.Tests";
            wcfTestStep.MethodName = "GetDataUsingDataContract";

            var xmlvalidationstep = new b.TestSteps.ValidationSteps.Xml.XmlValidationStep();
            xmlvalidationstep.XmlSchemas = new System.Collections.ObjectModel.Collection<b.TestSteps.ValidationSteps.Xml.SchemaDefinition>();
            xmlvalidationstep.XPathValidations = new System.Collections.ObjectModel.Collection<b.TestSteps.Common.XPathDefinition>();

            xmlvalidationstep.XmlSchemas.Add(new b.TestSteps.ValidationSteps.Xml.SchemaDefinition
            {
                XmlSchemaPath = testContextInstance.TestDir + @"\..\..\Test\BizUnit.TestSteps.i8c.Tests\TestData\CompositeTypeSchema.xsd",
                XmlSchemaNameSpace = ""
            });

            var xpathDef = new b.TestSteps.Common.XPathDefinition();
            xpathDef.XPath = "/*[local-name()='CompositeType' and namespace-uri()='']/*[local-name()='Result' and namespace-uri()='']";
            xpathDef.Value = "97";

            xmlvalidationstep.XPathValidations.Add(xpathDef);

            wcfTestStep.SubSteps.Add(xmlvalidationstep);

            wcfTestCase.ExecutionSteps.Add(wcfTestStep);

            var bizunit = new b.BizUnit(wcfTestCase);
            bizunit.RunTest();
        }
    }
}
