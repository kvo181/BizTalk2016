using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BizUnit;
using BizUnit.Xaml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.i8c.ValidationSteps.File.PropertyValidation;
using BizUnit.TestSteps.i8c.File;

namespace BizUnit.TestSteps.i8c.Tests
{
    [TestClass]
    public class FileFormattedReadMultipleStepTest
    {
        private static string TESTDIRECTORY = @"C:\Users\stvaho\Desktop";
        private static string FILENAME = "test.txt";

        /*
         * Temporary test for optimizing logging when I have time
         * 
         */
        [TestMethod()]
        public void Test()
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
            btc.ExecutionSteps.Add(read);

            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }
    }
}
