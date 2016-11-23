//---------------------------------------------------------------------
// File: BizUnitTests.cs
// 
// Summary: 
//
// Author: bizilante (http://blogs.msdn.com/kevinsmi)
//
//---------------------------------------------------------------------
// Copyright (c) 
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.IO;


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
    /// projects for them
    /// </summary>
    [TestClass]
    public class BizUnitExtensionsXmlTests
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
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_01_ContextPopulate.xml");
            bizUnit.RunTest();
        }


        /// <summary>
        /// This test uses the dotnetobjectinvokerex step 
        /// </summary>
        [TestMethod]
        public void Test_02_DotNetObjectInvokerEx()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_02_DotNetObjectInvokerEx.xml");
            bizUnit.RunTest();



        }
        /// <summary>
        /// This step calls the XmlPoke to change the value of a node within a file
        /// </summary>
        [TestMethod]
        public void Test_03_XmlPokeStep()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_03_XmlPokeStep.xml");
            bizUnit.RunTest();
        }

        /// <summary>
        /// This test creates a couple of files and then calls the filedeleteex and makes use of the context
        /// to delete the files
        /// </summary>
        [TestMethod]
        public void Test_04_FileDeleteEx()
        {
            // First make a file for the test to try and delete
            string path = @".\..\..\TestData\BizUnitTestFile.txt";
            string path2 = @".\..\..\TestData\BizUnitTestFile2.txt";
            TextWriter ts = File.CreateText(path);
            ts.WriteLine("This file is used to test the BizUnit fileDeleteExStep.");
            ts.Close();
            ts = File.CreateText(path2);
            ts.WriteLine("This file is also used to test the BiUnit FileDeleteExStep.");
            ts.Close();

            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_04_FileDeleteEx.xml");
            bizUnit.RunTest();
        }
        /// <summary>
        /// checks that the system returns immediately on finding a file and that it can 
        /// also put that file name into the context
        /// </summary>
        [TestMethod]
        public void Test_05_WaitOnFileEx()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_05_WaitOnFileEx.xml");
            bizUnit.RunTest();

        }

        /// <summary>
        /// This test checks the string formatter step
        /// </summary>
        [TestMethod]
        public void Test_06_StringFormatterStep()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_06_StringFormatter.xml");
            bizUnit.RunTest();

        }

        /// <summary>
        /// This test checks the Stringformatter multiple step
        /// </summary>
        [TestMethod]
        public void Test_07_StringFormatterMultipleStep()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_07_StringFormatterMultiple.xml");
            bizUnit.RunTest();

        }



        /// <summary>
        /// This test checks the XmlValidate step
        /// </summary> 
        [TestMethod]
        public void Test_08_XmlValidate()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_08_XmlValidate.xml");
            bizUnit.RunTest();
        }

        [TestMethod]
        public void Test_09_XmlValidateFail()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_09_XmlValidate_Fail.xml");
            bizUnit.RunTest();
        }

        /// <summary>
        /// This test requires that the SDK sample ConsumeWebService is deployed
        /// </summary>
        [TestMethod]
        public void Test_10_SoapHttpRequestResponseStepEx()
        {
            BizUnit bizUnit = new BizUnit(@".\..\..\TestCases\Test_10_SoapHttpRequestResponseStepEx.xml");
            bizUnit.RunTest();

            //TODO: this part should really be part of an XML Peek Step
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(@".\..\..\TestData\out\POResponse.xml");
            XmlNode testNode = xDoc.SelectSingleNode("/*[local-name()='submitPOResponse']/*[local-name()='submitPOResult' ]/*[local-name()='InvoiceNumber' ]");
            string actualValue = testNode.InnerText;

            //Do an assertion on the XpAth value
            Assert.AreEqual("string", actualValue);
        }

    }

}
