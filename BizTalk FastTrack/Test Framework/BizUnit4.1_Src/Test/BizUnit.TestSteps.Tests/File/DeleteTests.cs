
using System;
using BizUnit.TestSteps.File;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.Tests.File
{
    using System.IO;
    using DataLoaders.File;
    using File;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Summary description for DeleteTest
    /// </summary>
    [TestClass]
    public class DeleteTests
    {
        public DeleteTests()
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
        public void DeleteFileTest()
        {
            var step = new CreateStep();
            step.CreationPath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\DeleteTest_FileToBeDeleted.xml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            var deleteStep = new DeleteStep();
            deleteStep.FilePathsToDelete.Add(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\DeleteTest_FileToBeDeleted.xml");
            deleteStep.Execute(new Context());

            try
            {
                var deletedFile = System.IO.File.Open(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\DeleteTest_FileToBeDeleted.xml", FileMode.Open,
                                    FileAccess.Read);
            }
            catch (System.IO.FileNotFoundException)
            {
                ; // Expected!                
            }
        }

        [TestMethod]
        public void DeleteFileByWildCardTest()
        {
            var step = new CreateStep();
            step.CreationPath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\DeleteTest_FileToBeDeleted1.wildCardTestxml";
            var dl = new FileDataLoader();
            dl.FilePath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\PurchaseOrder001.xml";
            step.DataSource = dl;
            step.Execute(new Context());

            step.CreationPath = @"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\DeleteTest_FileToBeDeleted2.wildCardTestxml";
            step.Execute(new Context());

            var deleteStep = new DeleteStep();
            deleteStep.FilePathsToDelete.Add(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\*.wildCardTestxml");
            deleteStep.Execute(new Context());

            try
            {
                var deletedFile = System.IO.File.Open(@"..\..\..\Test\BizUnit.TestSteps.Tests\TestData\DeleteTest_FileToBeDeleted.wildCardTestxml", FileMode.Open,
                                    FileAccess.Read);
            }
            catch (System.IO.FileNotFoundException)
            {
                ; // Expected!                
            }
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void TestCaseValidationTest()
        {
            var step = new CreateStep();
            var tc = new TestCase();
            tc.ExecutionSteps.Add(step);
            var bu = new BizUnit(tc);
            bu.RunTest();
        }
    }
}
