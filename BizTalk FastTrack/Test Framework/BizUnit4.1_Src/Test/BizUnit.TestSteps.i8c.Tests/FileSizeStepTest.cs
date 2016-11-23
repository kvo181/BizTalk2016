using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BizUnit;
using BizUnit.Xaml;
using System.IO;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.i8c.ValidationSteps.File.PropertyValidation;
using BizUnit.TestSteps.i8c.File;

namespace BizUnit.TestSteps.i8c.Tests
{   
    /// <summary>
    ///This is a test class for FileSizeValidationTestStep and is intended
    ///to contain all FileSizeValidationTestStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FileSizeStepTest
    {
        private const bool LARGE_FILES = true;

        private static Random _r = new Random();
        private static string TESTDIRECTORY;
        private static string FILENAME;
        private static int FILESIZE;
               
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

        public FileSizeStepTest()
        {
            TESTDIRECTORY = Path.GetTempPath();
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            TESTDIRECTORY = TESTDIRECTORY + _r.Next();
            ensureDirectoryExists(TESTDIRECTORY);
        }
        //
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            removeCreatedFiles();
        }
        //
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            FILENAME = DateTime.Now.GetHashCode().ToString();
            FILESIZE = _r.Next() / (LARGE_FILES ? 1000 : 100000);
            using (FileStream stream = new FileStream(TESTDIRECTORY + @"\" + FILENAME, FileMode.Create, FileAccess.Write, FileShare.Read))
            {
                stream.Write(new Byte[FILESIZE], 0, FILESIZE);
            }
        }
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        [TestMethod()]
        public void TestFileEquals()
        {
            TestCase btc = new TestCase();
            var read = new FileFormattedReadMultipleStep()
            {
                DirectoryPath = TESTDIRECTORY,
                FormattedSearchPattern = FILENAME,
                ExpectedNumberOfFiles = 1,
                NumberOfCharsToLog = -1,
                Timeout = 100
            };

            var validate = new FileSizeValidationStep()
            {
                Value = FILESIZE,
                SizeMode = FileSizeValidationStep.SizeModes.B,
                CompareMode = NumericPropertyValidationStep.CompareModes.Equals
            };

            read.SubSteps.Add(validate);
            btc.ExecutionSteps.Add(read);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod()]
        public void TestFileEqualsDefaults()
        {
            TestCase btc = new TestCase();
            var read = new FileFormattedReadMultipleStep()
            {
                DirectoryPath = TESTDIRECTORY,
                FormattedSearchPattern = FILENAME,
                ExpectedNumberOfFiles = 1,
                NumberOfCharsToLog = -1,
                Timeout = 100
            };

            var validate = new FileSizeValidationStep()
            {
                Value = FILESIZE
            };

            read.SubSteps.Add(validate);
            btc.ExecutionSteps.Add(read);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod()]
        public void TestFileGreaterThan()
        {
            TestCase btc = new TestCase();
            var read = new FileFormattedReadMultipleStep()
            {
                DirectoryPath = TESTDIRECTORY,
                FormattedSearchPattern = FILENAME,
                ExpectedNumberOfFiles = 1,
                NumberOfCharsToLog = -1,
                Timeout = 100
            };

            var validate = new FileSizeValidationStep()
            {
                Value = 0,
                CompareMode = NumericPropertyValidationStep.CompareModes.GreaterThan
            };

            read.SubSteps.Add(validate);
            btc.ExecutionSteps.Add(read);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod()]
        public void TestFileLessThan()
        {
            TestCase btc = new TestCase();
            var read = new FileFormattedReadMultipleStep()
            {
                DirectoryPath = TESTDIRECTORY,
                FormattedSearchPattern = FILENAME,
                ExpectedNumberOfFiles = 1,
                NumberOfCharsToLog = -1,
                Timeout = 100
            };

            var validate = new FileSizeValidationStep()
            {
                Value = int.MaxValue,
                CompareMode = NumericPropertyValidationStep.CompareModes.LessThan
            };

            read.SubSteps.Add(validate);
            btc.ExecutionSteps.Add(read);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        #region Directory Helpers
        private static int lvl = 0;
        private static string createdPath = string.Empty;
        protected static void ensureDirectoryExists(string path)
        {
            ensureDirectoryExists(new DirectoryInfo(path));
        }

        protected static void ensureDirectoryExists(DirectoryInfo info)
        {
            if (createdPath == string.Empty)
                createdPath = info.FullName;

            Console.WriteLine("Creating: " + info.FullName);

            while (info.Parent != null && !info.Parent.Exists)
            {
                lvl++;
                ensureDirectoryExists(info.Parent);
            }

            if (!info.Exists)
            {
                info.Create();
            }
        }

        protected static void removeCreatedFiles()
        {
            if (createdPath != string.Empty)
            {
                Console.WriteLine(createdPath);
                Console.WriteLine("Going up " + lvl + " levels...");
                DirectoryInfo info = new DirectoryInfo(createdPath);
                while (info.Parent != null && lvl > 0)
                {
                    lvl--;
                    info = info.Parent;
                }
                Console.WriteLine("Delete: " + info.FullName);
                info.Delete(true);
            }
            else
            {
                Console.WriteLine("Empty created path!");
            }
        }
        #endregion
    }
}
