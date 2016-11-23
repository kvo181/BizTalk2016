//---------------------------------------------------------------------
// File: BizUnitTests.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.Xaml;
using BizUnit.CoreSteps.TestSteps;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.DataLoaders.File;

namespace BizUnit.Extensions.Tests
{
    /// <summary>
    /// The BizUnit Extensions Tests is a test library for some of the extensions.
    /// It is provided here as a indicative sample of what may be achieved but 
    /// actually unlike NUnit tests which test the assembly in question and can be 
    /// assisted by mock libraries, the BizUnit assemblies actually control real 
    /// orchestrations and other Biztalk artifacts which it is impossible to mock
    /// and provide. Therefore please use these only as a guide.
    /// 
    /// In the future versions of the test library we will try and provide a sample
    /// application which can be deployed for the tests to be run against and which
    /// will allow the tests to be completely repeatable and reusable. It may also be 
    /// more prudent to do this against the SDK samples since they are freely available 
    /// in any case
    /// 
    /// The method numbers are out of sequence in some places because we have refactored 
    /// the code base to move some steps into other libraries and created separate test 
    /// projects for them. However, the sequence number is only for keeping track of tests
    /// and does not represent any flow.
    /// </summary>
    [TestClass]
    public class BizUnitExtensionsObjectTests
    {
        /// <summary>
        /// The common setup for all tests. Runs the Test_Setup.xml file which copies the InDoc1.xml file
        /// from the source folder to the Rec_Setup folder. This is the same as the CoreTests
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            //			BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_Setup.xml", BizUnit.TestGroupPhase.TestGroupSetup );
            //			bizUnit.RunTest();
        }

        /// <summary>
        /// Common teardown step to delete the xml file from the Rec_Setup folder
        /// </summary>
        [TestCleanup]
        public void TearDown()
        {

            //BizUnit.
            //BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_TearDown.xml",BizUnit.TestGroupPhase.TestGroupTearDown);
            //bizUnit.RunTest();
        }

        /// <summary>
        /// This test loads directly into the context using the ContextPopulate Step and then uses
        /// the context data to copy a file from the specified source to the target location
        /// </summary>
        [TestMethod]
        public void Test_01_ContextPopulateStep()
        {
            //Setup the steps we need
            ContextPopulateStep cpStep = new ContextPopulateStep();
            cpStep.ContextKey = "Test";
            cpStep.KeyValue = "Test123";
            cpStep.DataType = "String";
            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "test_01_contextpopulatestep";
            //add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(cpStep);

            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            //Now verify that the context has got the data
            Assert.AreEqual(cpStep.KeyValue, bizUnit.Ctx.GetValue("Test"));

        }


        /// <summary>
        /// This test uses the dotnetobjectinvokerex step 
        /// </summary>
        [TestMethod]
        public void Test_02_DotNetObjectInvokerEx()
        {
            //Setup the steps you need
            DotNetObjectInvokerExStep dnstep = new DotNetObjectInvokerExStep();
            dnstep.TypeName = "BizUnit.Extensions.Utilities.DatabaseHelperEx";
            dnstep.AssemblyPath = "";
            dnstep.MethodToInvoke = "ExecuteSqlCommand";
            dnstep.Parameters.Add(ConfigurationManager.AppSettings["DBConnectionString"]);
            dnstep.Parameters.Add(ConfigurationManager.AppSettings["DBSelectCommand"]);

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_02_DotNetObjectInvokerEx";
            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(dnstep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();
        }
        /// <summary>
        /// This step calls the XmlPoke to change the value of a node within a file
        /// </summary>
        [TestMethod]
        public void Test_03_XmlPokeStep()
        {
            //setup the steps required
            //this is the setup stage
            CreateStep fcStep = new CreateStep();
            fcStep.DataSource = new FileDataLoader { FilePath = @".\..\..\TestData\XmlPokeData.xml" };
            fcStep.CreationPath = @".\..\..\TestData\XmlPokeDataOut.xml";

            //this is the actual execution
            XmlPokeStep xpStep = new XmlPokeStep();
            xpStep.FileName = @".\..\..\TestData\XmlPokeDataOut.xml";
            xpStep.Expressions.Add("/PersonObject/partyid||1000");

            //Instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_03_XmlPokeStep";
            //Add the test steps into the container at the required stages
            tc.SetupSteps.Add(fcStep);
            tc.ExecutionSteps.Add(xpStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            //Do an ordinary XPath evaluation of the result as part of the assertion
            //because there is no strong typing of the validation steps
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@".\..\..\TestData\XmlPokeDataOut.xml");
            XmlNode testNode = xDoc.SelectSingleNode("/PersonObject/partyid");
            string actualValue = testNode.InnerText;

            //Do an assertion on the XpAth value
            Assert.AreEqual("1000", actualValue);
            //now delete the file 
            File.Delete(@".\..\..\TestData\XmlPokeDataOut.xml");

        }

        /// <summary>
        /// This test creates a couple of files and then calls the filedeleteex to delete the files.
        /// The example is rather trivial as the step is better used in a large use case where the context
        /// is populated with several file names (perhaps temporary file names) over the course of several steps
        /// and then the step is used to delete them all. 
        /// It could also be used with a GUI front end where we add this step in after a number of other steps
        /// have been put into the flow. For a simple test harness it is not expected that a developer will go
        /// through the process of adding files into a list just to delete them. System.IO provides enough
        /// functionality for that
        /// </summary>
        [TestMethod]
        public void Test_04_FileDeleteEx()
        {
            // First we create two sample files to be deleted
            string path = @".\..\..\TestData\BizUnitTestFile.txt";
            string path2 = @".\..\..\TestData\BizUnitTestFile2.txt";
            TextWriter ts = File.CreateText(path);
            ts.WriteLine("This file is used to test the BizUnit fileDeleteExStep.");
            ts.Close();
            ts = File.CreateText(path2);
            ts.WriteLine("This file is also used to test the BiUnit FileDeleteExStep.");
            ts.Close();

            //Now we setup the BizUnit steps
            FileDeleteExStep fdStep = new FileDeleteExStep();
            //put the files to delete into a list and pass it to the step
            List<string> files = new List<string>();
            files.Add(path);
            files.Add(path2);
            fdStep.FilesToDelete = files;
            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_04_FileDeleteEx";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(fdStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();
        }
        /// <summary>
        /// checks that the system returns immediately on finding a file and that it can 
        /// also put that file name into the context
        /// </summary>
        [TestMethod]
        public void Test_05_WaitOnFileEx()
        {
            //setup the required steps
            WaitOnFileExStep wStep = new WaitOnFileExStep();
            wStep.Path = @".\..\..\TestData\out\";
            wStep.FileFilter = "*.xml";
            wStep.TimeOut = 5000;
            wStep.IncludeOldFiles = "Y";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_05_WaitOnFileEx";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(wStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            Assert.AreEqual("file.xml", bizUnit.Ctx.GetValue("DetectedFileName"));
        }

        /// <summary>
        /// This test checks the string formatter step
        /// </summary>
        [TestMethod]
        public void Test_06_StringFormatterStep()
        {
            //Setup the required step
            StringFormatterStep sfStep = new StringFormatterStep();
            sfStep.BaseString = "Update table set fld = '{0}' where fld1 = '{1}'";
            sfStep.Delimiter = "|";
            sfStep.NumParams = 2;
            sfStep.ParamString = "Smith|10192929239";
            sfStep.ContextKeyWithResult = "OutputString";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_06_StringFormatterStep";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(sfStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            Assert.AreEqual("Update table set fld = 'Smith' where fld1 = '10192929239'",
                bizUnit.Ctx.GetValue("OutputString"));

        }

        /// <summary>
        /// This test checks the Stringformatter multiple step
        /// </summary>
        [TestMethod]
        public void Test_07_StringFormatterMultipleStep()
        {
            //set up the required steps
            StringFormatterMultipleStep sfStep = new StringFormatterMultipleStep();
            sfStep.BaseString = "exec NotifyNewCRM '{0}','1'";
            string[] tokens = new string[1];
            tokens[0] = "101000215026";
            sfStep.Tokens = tokens;
            sfStep.ContextKeyWithResult = "OutputString";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_07_StringFormatterMultipleStep";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(sfStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            Assert.AreEqual("exec NotifyNewCRM '101000215026','1'",
                bizUnit.Ctx.GetValue("OutputString"));

        }

        /// <summary>
        /// This test checks the XmlValidate step
        /// </summary> 
        [TestMethod]
        public void Test_08_XmlValidate()
        {
            // Create the test case
            var testCase = TestCase.LoadFromFile(@".\..\..\TestCases\Test_08_XmlValidate.xml");
            // Execute the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();
        }

        [TestMethod]
        public void Test_09_XmlValidateFail()
        {
            // Create the test case
            var testCase = TestCase.LoadFromFile(@".\..\..\TestCases\Test_09_XmlValidate_Fail.xml");
            // Execute the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();
        }

        [TestMethod]
        public void Test_10_SoapHttpRequestResponseStepEx()
        {
            //setup the required steps
            SoapHttpRequestResponseStepEx soapStep = new SoapHttpRequestResponseStepEx();
            soapStep.Url = "http://localhost/POWebService/SubmitPOService.asmx";
            soapStep.SoapAction = "http://Microsoft.Samples.BizTalk.ConsumeWebService/webservices/submitPO";
            soapStep.InputFile = @".\..\..\TestData\PORequest.xml";
            soapStep.OutputFile = @".\..\..\TestData\out\POResponse.xml";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_10_SoapHttpRequestResponseStepEx";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(soapStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@".\..\..\TestData\out\POResponse.xml");
            XmlNode testNode = xDoc.SelectSingleNode("/*[local-name()='submitPOResponse']/*[local-name()='submitPOResult' ]/*[local-name()='InvoiceNumber' ]");
            string actualValue = testNode.InnerText;

            //Do an assertion on the XpAth value
            Assert.AreEqual("string", actualValue);
            //now delete the file 
            File.Delete(@".\..\..\TestData\out\POResponse.xml");

        }

        [TestMethod]
        public void Test_11_RestHttpRequestResponseStepEx_GET()
        {
            //setup the required steps
            RestHttpRequestResponseStepEx soapStep = new RestHttpRequestResponseStepEx();
            soapStep.Url = "http://localhost//resttestwebservice/api/books";
            soapStep.Method = "GET";
            soapStep.ContentType = "application/xml";
            soapStep.InputFile = @".\..\..\TestData\GetBooks.json";
            soapStep.OutputFile = @".\..\..\TestData\out\GetBooksResponse.xml";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_11_RestHttpRequestResponseStepEx_GET";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(soapStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@".\..\..\TestData\out\GetBooksResponse.xml");
            XmlNode testNode = xDoc.SelectSingleNode("/*[local-name()='ArrayOfBookDTO']/*[local-name()='BookDTO' ]/*[local-name()='Id' ]");
            Assert.IsNotNull(testNode, "Xml Id node not found");
            //Do an assertion on the XpAth value
            string actualValue = testNode.InnerText;
            Assert.AreEqual("1", actualValue);
            //now delete the file 
            File.Delete(@".\..\..\TestData\out\GetBooksResponse.xml");

        }
        [TestMethod]
        public void Test_11_RestHttpRequestResponseStepEx_DEL()
        {
            //setup the required steps
            RestHttpRequestResponseStepEx soapStep = new RestHttpRequestResponseStepEx();
            soapStep.Url = "http://localhost//resttestwebservice/api/books/1";
            soapStep.Method = "DELETE";
            soapStep.ContentType = "application/xml";
            soapStep.InputFile = @".\..\..\TestData\GetBooks.json";
            soapStep.OutputFile = @".\..\..\TestData\out\DelBooksResponse.xml";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_11_RestHttpRequestResponseStepEx_DEL";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(soapStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            Assert.IsTrue(File.Exists(@".\..\..\TestData\out\DelBooksResponse.xml"), "DELETE failed! See Output log.");
            File.Delete(@".\..\..\TestData\out\DelBooksResponse.xml");

        }
        [TestMethod]
        public void Test_11_RestHttpRequestResponseStepEx_POST()
        {
            //setup the required steps
            RestHttpRequestResponseStepEx soapStep = new RestHttpRequestResponseStepEx();
            soapStep.Url = "http://localhost//resttestwebservice/api/books";
            soapStep.Method = "POST";
            soapStep.ContentType = "application/xml";
            soapStep.InputFile = @".\..\..\TestData\PostBooks.xml";
            soapStep.OutputFile = @".\..\..\TestData\out\PostBooksResponse.xml";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_11_RestHttpRequestResponseStepEx_POST";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(soapStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            Assert.IsTrue(File.Exists(@".\..\..\TestData\out\PostBooksResponse.xml"), "POST failed! See Output log.");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@".\..\..\TestData\out\PostBooksResponse.xml");
            XmlNode testNode = xDoc.SelectSingleNode("/*[local-name()='BookDTO' ]/*[local-name()='Id' ]");
            Assert.IsNotNull(testNode, "Xml Id node not found");
            //Do an assertion on the XpAth value
            string actualValue = testNode.InnerText;
            Assert.AreEqual("123", actualValue);
            File.Delete(@".\..\..\TestData\out\PostBooksResponse.xml");

        }
        [TestMethod]
        public void Test_11_RestHttpRequestResponseStepEx_PUT()
        {
            //setup the required steps
            RestHttpRequestResponseStepEx soapStep = new RestHttpRequestResponseStepEx();
            soapStep.Url = "http://localhost//resttestwebservice/api/books/1";
            soapStep.Method = "PUT";
            soapStep.ContentType = "application/xml";
            soapStep.InputFile = @".\..\..\TestData\PutBooks.xml";
            soapStep.OutputFile = @".\..\..\TestData\out\PutBooksResponse.xml";

            //Now instantiate the test case container
            TestCase tc = new TestCase();
            tc.Name = "Test_11_RestHttpRequestResponseStepEx_PUT";

            //Add the test steps into the container at the required stages
            tc.ExecutionSteps.Add(soapStep);
            //Initialise BizUnit runner with the test case container
            BizUnit bizUnit = new BizUnit(tc);
            //run the test
            bizUnit.RunTest();

            Assert.IsTrue(File.Exists(@".\..\..\TestData\out\PutBooksResponse.xml"), "PUT failed! See Output log.");
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@".\..\..\TestData\out\PutBooksResponse.xml");
            XmlNode testNode = xDoc.SelectSingleNode("/*[local-name()='BookDTO' ]/*[local-name()='Title' ]");
            Assert.IsNotNull(testNode, "Xml Title node not found");
            //Do an assertion on the XpAth value
            string actualValue = testNode.InnerText;
            Assert.AreEqual("Updated title", actualValue);
            File.Delete(@".\..\..\TestData\out\PutBooksResponse.xml");

        }

    }

}
