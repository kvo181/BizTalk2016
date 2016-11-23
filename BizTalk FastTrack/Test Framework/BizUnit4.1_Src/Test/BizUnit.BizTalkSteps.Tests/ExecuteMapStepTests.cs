
using BizUnit.TestSteps.BizTalk.Map;
using BizUnit.TestSteps.Common;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.BizTalkSteps.Tests
{
    [TestClass]
    public class ExecuteMapStepTests
    {
        [TestMethod]
        public void MapDocumentInstanceTest()
        {
            var mapStep = new ExecuteMapStep();
            mapStep.MapAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            mapStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.xml";
            mapStep.MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2";
            mapStep.Destination = "Schema2.001.xml";

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase();
            tc.Name = "MapDocumentInstanceTest";
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTest.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);
        }

        [TestMethod]
        public void MapDocumentInstanceTestAndValidate()
        {
            var mapStep = new ExecuteMapStep();
            mapStep.MapAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            mapStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.xml";
            mapStep.MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2";
            mapStep.Destination = "Schema2.002.xml";

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2";
            validation.XmlSchemas.Add(sd);
            var xpd = new XPathDefinition();
            xpd.XPath = "/*[local-name()='Schema2Root' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema2']/*[local-name()='Child1' and namespace-uri()='']/@*[local-name()='Child1Attribute1' and namespace-uri()='']";
            xpd.Value = "1";
            validation.XPathValidations.Add(xpd);

            // Add validation...
            mapStep.SubSteps.Add(validation);

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase();
            tc.Name = "MapDocumentInstanceTest";
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTestAndValidate.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);
        }

        [TestMethod]
        public void MapDocumentInstanceTestAndValidateAndAddValueToCtx()
        {
            var mapStep = new ExecuteMapStep();
            mapStep.MapAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            mapStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.xml";
            mapStep.MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2";
            mapStep.Destination = "Schema2.003.xml";

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2";
            validation.XmlSchemas.Add(sd);
            var xpd = new XPathDefinition();
            xpd.XPath = "/*[local-name()='Schema2Root' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema2']/*[local-name()='Child1' and namespace-uri()='']/@*[local-name()='Child1Attribute1' and namespace-uri()='']";
            xpd.Value = "1";
            xpd.ContextKey = "Child1.Child1Attribute1";
            validation.XPathValidations.Add(xpd);

            // Add validation...
            mapStep.SubSteps.Add(validation);

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase();
            tc.Name = "MapDocumentInstanceTest";
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTestAndValidateAndAddValueToCtx.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);

            Assert.AreEqual("1", ctx.GetValue("Child1.Child1Attribute1"));
        }

        [TestMethod]
        [ExpectedException(typeof(ValidationStepExecutionException))]
        public void MapDocumentInstanceTestAndValidateInvalidDocument()
        {
            var mapStep = new ExecuteMapStep();
            mapStep.MapAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            mapStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema1.error.xml";
            mapStep.MapTypeName = "BizUnit.BizTalkTestArtifacts.MapSchema1ToSchema2";
            mapStep.Destination = "Schema2.005.xml";

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2";
            validation.XmlSchemas.Add(sd);
            var xpd = new XPathDefinition();
            xpd.XPath = "/*[local-name()='Schema2Root' and namespace-uri()='http://BizUnit.BizTalkTestArtifacts.Schema2']/*[local-name()='Child1' and namespace-uri()='']/@*[local-name()='Child1Attribute1' and namespace-uri()='']";
            xpd.Value = "1";
            validation.XPathValidations.Add(xpd);

            // Add validation...
            mapStep.SubSteps.Add(validation);

            // Save the test case to ensure seralisation works as expected....
            var tc = new TestCase();
            tc.Name = "MapDocumentInstanceTest";
            tc.ExecutionSteps.Add(mapStep);
            TestCase.SaveToFile(tc, "MapDocumentInstanceTestAndValidateInvalidDocument.xaml");

            // Execute test step only
            var ctx = new Context();
            mapStep.Execute(ctx);
        }
    }
}