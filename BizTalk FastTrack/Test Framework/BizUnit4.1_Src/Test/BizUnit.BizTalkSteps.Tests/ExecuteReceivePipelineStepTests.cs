
using BizUnit.TestSteps.BizTalk.Pipeline;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.ValidationSteps.Xml;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.BizTalkSteps.Tests
{
    [TestClass]
    public class ExecuteReceivePipelineStepTests
    {
        [TestMethod]
        public void ExecuteReceivePiplineWithXmlDisAsmTest()
        {
            // Create test case...
            var tc = new TestCase();
            tc.Name = "ExecuteReceivePiplineWithXmlDisAsmTest";

            var pipeStep = new ExecuteReceivePipelineStep();
            pipeStep.PipelineAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            pipeStep.PipelineTypeName = "BizUnit.BizTalkTestArtifacts.ReceivePipeline1";
            var ds = new DocSpecDefinition
                         {
                             AssemblyPath =
                                 @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                             TypeName = "BizUnit.BizTalkTestArtifacts.Schema2"
                         };
            pipeStep.DocSpecs.Add(ds);
            pipeStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema2.xml";
            pipeStep.DestinationFileFormat = "Output010.{0}.xml";
            pipeStep.DestinationFileFormat = "Output010.{0}.xml";
            pipeStep.OutputContextFileFormat = "Context010.{0}.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout= 2000;
            exists.SearchPattern = "Output010*.xml";
            exists.ExpectedNoOfFiles=1;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Context010*.xml";
            exists.ExpectedNoOfFiles = 1;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep();
            fv.DirectoryPath = ".";
            fv.SearchPattern = "Output010.0.xml";
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

            TestCase.SaveToFile(tc, "ExecuteReceivePiplineWithXmlDisAsmTest.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteReceivePiplineWithXmlDisAsmTest.xaml"));
            bu.RunTest();
        }

        [TestMethod]
        public void ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree()
        {
            // Create test case...
            var tc = new TestCase();
            tc.Name = "ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree";

            var pipeStep = new ExecuteReceivePipelineStep();
            pipeStep.PipelineAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            pipeStep.PipelineTypeName = "BizUnit.BizTalkTestArtifacts.ReceivePipeline1";
            var ds = new DocSpecDefinition
            {
                AssemblyPath =
                    @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll",
                TypeName = "BizUnit.BizTalkTestArtifacts.Schema3Env"
            };
            pipeStep.DocSpecs.Add(ds);
            pipeStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema3Env.xml";
            pipeStep.DestinationFileFormat = "Output011.{0}.xml";
            pipeStep.DestinationFileFormat = "Output011.{0}.xml";
            pipeStep.OutputContextFileFormat = "Context011.{0}.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Output011*.xml";
            exists.ExpectedNoOfFiles = 3;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Context011*.xml";
            exists.ExpectedNoOfFiles = 3;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep();
            fv.DirectoryPath = ".";
            fv.SearchPattern = "Output011.0.xml";
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

            TestCase.SaveToFile(tc, "ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteReceivePiplineWithXmlDisAsmTestInterchangeOfThree.xaml"));
            bu.RunTest();
        }

        [TestMethod]
        public void ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest()
        {
            // Create test case...
            var tc = new TestCase();
            tc.Name = "ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest";

            var pipeStep = new ExecuteReceivePipelineStep();
            pipeStep.PipelineAssemblyPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\bin\Debug\BizUnit.BizTalkTestArtifacts.dll";
            pipeStep.PipelineTypeName = "BizUnit.BizTalkTestArtifacts.ReceivePipeline3";
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

            pipeStep.Source = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Instances\Schema3Env.xml";
            pipeStep.DestinationFileFormat = "Output013.{0}.xml";
            pipeStep.DestinationFileFormat = "Output013.{0}.xml";
            pipeStep.OutputContextFileFormat = "Context013.{0}.xml";
            // Add ExecuteReceivePipelineStep to test case
            tc.ExecutionSteps.Add(pipeStep);

            var exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Output013*.xml";
            exists.ExpectedNoOfFiles = 3;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            exists = new ExistsStep();
            exists.DirectoryPath = ".";
            exists.Timeout = 2000;
            exists.SearchPattern = "Context013*.xml";
            exists.ExpectedNoOfFiles = 3;
            // Add ExistsStep to test case
            tc.ExecutionSteps.Add(exists);

            var fv = new FileReadMultipleStep();
            fv.DirectoryPath = ".";
            fv.SearchPattern = "Output013.0.xml";
            fv.DeleteFiles = false;

            var validation = new XmlValidationStep();
            var sd = new SchemaDefinition();
            sd.XmlSchemaPath = @"..\..\..\Test\BizUnit.BizTalkTestArtifacts\Schema0.xsd";
            sd.XmlSchemaNameSpace = "http://BizUnit.BizTalkTestArtifacts.Schema0";
            validation.XmlSchemas.Add(sd);
            // Add validation to FileReadMultipleStep
            fv.SubSteps.Add(validation);
            // Add FileReadMultipleStep to test case
            tc.ExecutionSteps.Add(exists);

            TestCase.SaveToFile(tc, "ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest.xaml");

            // Execute test csse using serialised test case to test round tripping of serialisation...
            var bu = new BizUnit(TestCase.LoadFromFile("ExecuteReceivePipeDocSpecEnvSpecXmlDisAsmWithImportedSchemaTest.xaml"));
            bu.RunTest();
        }
    }
}