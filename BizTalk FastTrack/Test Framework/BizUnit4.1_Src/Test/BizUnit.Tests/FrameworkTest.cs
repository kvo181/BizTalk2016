using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.TestSteps.Time;
using BizUnit.TestScenario;
using BizUnit.Xaml;
using BizUnit.TestSteps.File;

namespace BizUnit.Tests
{
    [TestClass]
    public class FrameworkTest : BaseTestScenario
    {
        #region Variable Data
        private const int DELAY = 1;
        #endregion

        #region Test Invoker
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            string testDir = testContext.TestDir;
            string saveDir;
            if (testDir.Contains("TestResults"))
            {
                saveDir = testDir.Substring(0, testDir.IndexOf("TestResults") - 1);
            }
            else
            {
                saveDir = Path.Combine(testDir, "xml");
            }
            init(saveDir);
        }

        [TestMethod]
        public void TestFramework()
        {
            test();
        }
        #endregion

        #region Overrides
        protected override string getName()
        {
            return "Framework Test";
        }

        protected override string getDescription()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Small delays each stage (testing the calling order inside the framework)");
            sb.AppendLine();
            sb.AppendLine("Steps:");
            sb.AppendLine("[SETUP]");
            sb.AppendLine("-+-- Delay " + DELAY + "ms");
            sb.AppendLine("[EXECUTE]");
            sb.AppendLine("-+-- Delay " + DELAY + "ms");
            sb.AppendLine("[CLEANUP]");
            sb.AppendLine("-+-- Delay " + DELAY + "ms");
            return sb.ToString();
        }

        protected override void constructSetup(ref TestCase testCase)
        {
            delay();
        }

        protected override void constructExecution(ref TestCase testCase)
        {
            delay();
            var fileRead = new FileReadMultipleStep()
                               {
                                   DirectoryPath = "c:\\hsdlfhsekrfzesfhfsdjfhdjkfshdkjfghsdkjf",
                                   Timeout = 1000,
                                   NumberOfCharsToLog = 0,
                                   SearchPattern = "Test",
                                   ExpectedNumberOfFiles = 1
                               };
            add(fileRead);
        }

        protected override void constructCleanup(ref TestCase testCase)
        {
            delay();
        }
        #endregion

        #region Helper
        private void delay()
        {
            add(new DelayStep { DelayMilliSeconds = DELAY });
        }
        #endregion
    }
}
