
using System;
using System.Diagnostics;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.Time;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BizUnit.TestSteps.Tests.Time
{
    /// <summary>
    /// Summary description for DelayTest
    /// </summary>
    [TestClass]
    public class DelayTests
    {
        public DelayTests()
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
        public void DelayTest()
        {
            int stepDelayDuration = 500;
            var step = new DelayStep();
            step.DelayMilliSeconds = stepDelayDuration;

            var sw = new Stopwatch();
            sw.Start();

            step.Execute(new Context());

            var actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(stepDelayDuration, actualDuration, 20);

            stepDelayDuration = 5;
            step.DelayMilliSeconds = stepDelayDuration;

            sw = new Stopwatch();
            sw.Start();

            step.Execute(new Context());

            actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        }

        [TestMethod]
        public void DelayTestCaseTest()
        {
            int stepDelayDuration = 500;
            var step = new DelayStep();
            step.DelayMilliSeconds = stepDelayDuration;

            var sw = new Stopwatch();
            sw.Start();

            step.Execute(new Context());

            var actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(stepDelayDuration, actualDuration, 20);

            stepDelayDuration = 5;
            step.DelayMilliSeconds = stepDelayDuration;

            var tc = new TestCase();
            tc.ExecutionSteps.Add(step);

            TestCase.SaveToFile(tc, "DelayTestCaseTest.xaml");
            var bu = new BizUnit(TestCase.LoadFromFile("DelayTestCaseTest.xaml"));

            sw = new Stopwatch();
            sw.Start();

            bu.RunTest();

            actualDuration = sw.ElapsedMilliseconds;
            Console.WriteLine("Observed delay: {0}", actualDuration);
            Assert.AreEqual(actualDuration, stepDelayDuration, 20);
        }

        [TestMethod]
        public void DelaySampleTest()
        {
            // Create the test case
            var testCase = new TestCase();

            // Create test steps...
            var delayStep = new DelayStep {DelayMilliSeconds = 500};

            // Add test steps to the required test stage
            testCase.ExecutionSteps.Add(delayStep);

            // Create a new instance of BizUnit and run the test
            var bizUnit = new BizUnit(testCase);
            bizUnit.RunTest();

            // Save Test Case
            TestCase.SaveToFile(testCase, "DelaySampleTest.xml");
        }

        [TestMethod]
        public void LoadSampleTest()
        {
            DelaySampleTest();

            // Load Test Case
            var testCase = TestCase.LoadFromFile("DelaySampleTest.xml");

            // Create test steps...
            var dataLoader = new FileDataLoader {FilePath = @"TestData\InputPO.xml"};

            var fileCreate = new CreateStep {CreationPath = @"C\InputFile", DataSource = dataLoader};

            testCase.ExecutionSteps.Add(fileCreate);

            // Save Test Case
            TestCase.SaveToFile(testCase, "DelaySampleTest.xml");
        }
    }
}
