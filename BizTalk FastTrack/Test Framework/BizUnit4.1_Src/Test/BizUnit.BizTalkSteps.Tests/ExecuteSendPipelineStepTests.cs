
using BizUnit.TestSteps.BizTalk.Pipeline;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.BizTalkSteps.Tests
{
    [TestClass]
    public class ExecuteSendPipelineStepTests
    {
        [TestMethod]
        public void ExecuteSendPipelineWithDefaultXmlAsmWithSimpleSchemaTest()
        {
            // Create test case...
            var tc = new TestCase();
            tc.Name = "ExecuteSendPipelineWithDefaultXmlAsmWithSimpleSchemaTest";

            var pipeStep = new ExecuteSendPipelineStep();
            pipeStep.PipelineAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            pipeStep.PipelineTypeName = "BizUnit.BizTalkTestArtifacts.SendPipeline1";
            var ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema2"
            };
            pipeStep.DocSpecs.Add(ds);
            pipeStep.SourceDir = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\";
            pipeStep.SearchPattern = "Schema2.xml";
            pipeStep.Destination = "Output.020.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Output.020*.xml";
            exists.ExpectedNoOfFiles = 1;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep();
            fv.DirectoryPath = ".";
            fv.SearchPattern = "Output.020.0.xml";
            fv.DeleteFiles = false;

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2";
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteSendPipelineWithDefaultXmlAsmWithSimpleSchemaTest.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteSendPipelineWithDefaultXmlAsmWithSimpleSchemaTest.xaml"));
            bu.RunTest();
        }

        [TestMethod]
        [ExpectedException(typeof(TestStepExecutionException))]
        public void ExecuteSendPipelineWithDefaultXmlAsmWithImportedSchemaTest()
        {
            // Create test case...
            var tc = new TestCase();
            tc.Name = "ExecuteSendPipelineWithDefaultXmlAsmWithImportedSchemaTest";

            var pipeStep = new ExecuteSendPipelineStep();
            pipeStep.PipelineAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            pipeStep.PipelineTypeName = "BizUnit.BizTalkTestArtifacts.SendPipeline1";
            var ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema0"
            };
            pipeStep.DocSpecs.Add(ds);
            ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema3Env"
            };
            pipeStep.DocSpecs.Add(ds); 
            pipeStep.SourceDir = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\";
            pipeStep.SearchPattern = "Child*.xml";
            pipeStep.Destination = "Output.021.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Output.021*.xml";
            exists.ExpectedNoOfFiles = 1;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep();
            fv.DirectoryPath = ".";
            fv.SearchPattern = "Output.021.xml";
            fv.DeleteFiles = false;

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema0.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema0";
            validation.XmlSchemas.Add(sd);
            sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema3Env.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema3Env";
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteSendPipelineWithDefaultXmlAsmWithImportedSchemaTest.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteSendPipelineWithDefaultXmlAsmWithImportedSchemaTest.xaml"));
            bu.RunTest();
        }

        [TestMethod]
        public void ExecuteSendPipelineConfiguredDocSpecXmlAsmWithSimpleSchema()
        {
            // Create test case...
            var tc = new TestCase();
            tc.Name = "ExecuteSendPipelineConfiguredDocSpecXmlAsmWithSimpleSchema";

            var pipeStep = new ExecuteSendPipelineStep();
            pipeStep.PipelineAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            pipeStep.PipelineTypeName = "BizUnit.BizTalkTestArtifacts.SendPipeline2";
            var ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema2"
            };
            pipeStep.DocSpecs.Add(ds);
            pipeStep.SourceDir = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\";
            pipeStep.SearchPattern = "Schema2.xml";
            pipeStep.Destination = "Output.022.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Output.022*.xml";
            exists.ExpectedNoOfFiles = 1;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep();
            fv.DirectoryPath = ".";
            fv.SearchPattern = "Output.022.xml";
            fv.DeleteFiles = false;

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema2.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema2";
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteSendPipelineConfiguredDocSpecXmlAsmWithSimpleSchema.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteSendPipelineConfiguredDocSpecXmlAsmWithSimpleSchema.xaml"));
            bu.RunTest();
        }
    }
}