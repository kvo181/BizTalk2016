using System;
using BizUnit.Xaml;
using System.Reflection;
using System.IO;

namespace BizUnit.TestScenario
{
    /// <summary>
    /// Abstract class that enforces I8C standards when creating test scenario's using the BizUnit framework.
    /// This implementation autmatically saves the scenario to an xml persistence file, runs the testcase and provides easy to use methods to construct the BizUnit testcase.
    /// </summary>
    public abstract class BaseTestScenario
    {
        #region Properties
        /// <summary>
        /// The actual BizUnit TestCase.
        /// </summary>
        protected TestCase Case { get; private set; }
        #endregion

        #region Initialize
        /// <summary>
        /// The current base directory used to save the xml persistence file.
        /// </summary>
        private static string _baseDir = string.Empty;

        private static bool _overwrite = false;
        /// <summary>
        /// Initializes the directory in which the xml persistence files are saved.
        /// </summary>
        /// <param name="basedir">The base directory</param>
        /// <param name="overwriteExistingFiles">Whether or not to replace existing files with each testrun</param>
        protected static void init(string basedir, bool overwriteExistingFiles = false)
        {
            _overwrite = overwriteExistingFiles;
            _baseDir = string.Format(@"{0}\Tests\{1}\TestCase\", basedir, System.Reflection.Assembly.GetCallingAssembly().GetName().Name);
        }
        #endregion

        #region Test Controller
        private Phases currentPhase { get; set; }
        private enum Phases { describeTestCase, addSetupSteps, addExecutionSteps, addCleanupSteps, persistTestCase, runTestCase }; 
        private const Phases DEFAULT_ADD_PHASE = Phases.addExecutionSteps; // Note: the given case should be handled in the helper 'add' method! -> else: endless loops!

        /// <summary>
        /// Performs all steps necessairy to construct, save and run the tests.
        /// </summary>
        protected void test()
        {
            // construct an empty testcase
            TestCase testCase = new TestCase();
            Case = testCase;

            // loop each phase and perform the appropriately actions
            foreach (Phases phase in Enum.GetValues(typeof(Phases)))
            {
                try
                {
                    currentPhase = phase;
                    MethodInfo phaseMethod = this.GetType().BaseType.GetMethod(currentPhase.ToString(), BindingFlags.NonPublic | BindingFlags.Instance);
                    phaseMethod.Invoke(this, new object[] { testCase });
                }
                catch (TargetInvocationException e)
                {
                    // filter out the invocation exception and rethrow the inner exception that occured
                    throw e.InnerException;
                }
                catch (Exception ex)
                {
                    // wrap the exceptions that can happen when using reflection in a TestingFrameworkException
                    throw new TestScenarioInvocationException("Could not invoke '" + phase.ToString() + "': " + ex.Message, ex);
                }
            }
        }
        #endregion

        #region Phases

        #region Describe TestCase
        /// <summary>
        /// Implementation of the describe phase of the testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        private void describeTestCase(ref TestCase testCase)
        {
            // required descriptions
            testCase.Name = getName();
            testCase.Description = getDescription();
        }
        /// <summary>
        /// Give a name to the testcase.
        /// </summary>
        /// <returns>Name of the testcase</returns>
        protected abstract string getName();
        /// <summary>
        /// Give a description to the testcase.
        /// </summary>
        /// <returns>Description of the testcase</returns>
        protected abstract string getDescription();
        #endregion

        #region Add Setup Steps
        /// <summary>
        /// Implementation of the construct-setup phase of the testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        private void addSetupSteps(ref TestCase testCase)
        {
            // global setup stuff
            // ... nothing yet...

            // custom setup stuff
            constructSetup(ref testCase);
        }
        /// <summary>
        /// Contructs the setup phase of the BizUnit testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        protected abstract void constructSetup(ref TestCase testCase);
        #endregion

        #region Add Execution Steps
        /// <summary>
        /// Implementation of the construct-execution phase of the testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        private void addExecutionSteps(ref TestCase testCase)
        {
            // global execute stuff
            // ... nothing yet...

            // custom execute stuff
            constructExecution(ref testCase);
        }
        /// <summary>
        /// Contructs the execution phase of the BizUnit testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        protected abstract void constructExecution(ref TestCase testCase);
        #endregion

        #region Add Cleanup Steps
        /// <summary>
        /// Implementation of the construct-cleanup phase of the testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        private void addCleanupSteps(ref TestCase testCase)
        {
            // global cleanup stuff
            // ... nothing yet...

            // custom execute stuff
            constructCleanup(ref testCase);
        }
        /// <summary>
        /// Constructs the cleanup phase of the BizUnit testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        protected abstract void constructCleanup(ref TestCase testCase);
        #endregion

        #region Persist TestCase
        /// <summary>
        /// Implementation of the persist phase of the testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        private void persistTestCase(ref TestCase testCase)
        {
            // call the global persist method with the desired name for the file
            persist(testCase, getPersistTestFileName());
        }
        #endregion

        #region Run TestCase
        /// <summary>
        /// Implementation of the run phase of the testcase.
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        private void runTestCase(ref TestCase testCase)
        {
            // print some info to the console / test results
            Console.WriteLine("Test:");
            Console.WriteLine(testCase.Name);
            Console.WriteLine();
            Console.WriteLine("Description:");
            Console.WriteLine(testCase.Description);
            Console.WriteLine();
            Console.WriteLine();

            // run the testcase using BizUnit
            new BizUnit(testCase).RunTest();
        }
        #endregion

        #endregion

        #region Persistence
        /// <summary>
        /// Returns the filename of the persistence file. Default takes the name of the class, can be overridden to provide a custom name.
        /// </summary>
        /// <returns>The name of the persistence file</returns>
        protected virtual string getPersistTestFileName()
        {
            return GetType().Name + ".xml";
        }

        /// <summary>
        /// Persists the test to disk (xml).
        /// </summary>
        /// <param name="testCase">The active testcase</param>
        /// <param name="filename">The filename</param>
        private static void persist(TestCase testCase, string filename)
        {
            ensureDirectoryExists(_baseDir);
            string location = _baseDir + filename;
            try
            {
                Console.WriteLine("Persist TestCase to XML:");
                Console.WriteLine(location);
                bool exists = File.Exists(location);
                // if the testcase isn't empty and we don't have the file yet, save it
                if ((_overwrite || !exists) && (testCase.SetupSteps.Count > 0 || testCase.ExecutionSteps.Count > 0 || testCase.CleanupSteps.Count > 0))
                {
                    TestCase.SaveToFile(testCase, location);
                    Console.WriteLine("Test saved!");
                }
                else
                {
                    Console.WriteLine(string.Format("{0}! The test will not be saved...", exists ? "XML already exists" : "TestCase is empty"));
                }
                Console.WriteLine();
            }
            catch (Exception e) { Console.WriteLine(string.Format("{0} while saving XML: {1}", e.GetType().Name, e.Message)); }
        }

        /// <summary>
        /// Ensures that the given path actually exists on the machine.
        /// </summary>
        /// <param name="path">Desired path as string</param>
        private static void ensureDirectoryExists(string path)
        {
            ensureDirectoryExists(new DirectoryInfo(path));
        }

        /// <summary>
        /// Ensures that the given path actually exists on the machine.
        /// </summary>
        /// <param name="info">Desired path as DirectoryInfo</param>
        private static void ensureDirectoryExists(DirectoryInfo info)
        {
            while (info.Parent != null && !info.Parent.Exists)
            {
                ensureDirectoryExists(info.Parent);
            }

            if (!info.Exists)
            {
                info.Create();
            }
        }
        #endregion

        #region Helper
        /// <summary>
        /// Helper method to add a TestStep to the case without specifying the stage (stage will be determined automatically).
        /// </summary>
        /// <param name="step">TestStep to add</param>
        protected void add(TestStepBase step)
        {
            add(step, currentPhase);
        }

        private void add(TestStepBase step, Phases phase)
        {
            switch (phase)
            {
                case Phases.addSetupSteps:
                    Case.SetupSteps.Add(step);
                    break;
                case Phases.addExecutionSteps:
                    Case.ExecutionSteps.Add(step);
                    break;
                case Phases.addCleanupSteps:
                    Case.CleanupSteps.Add(step);
                    break;
                default:
                    add(step, DEFAULT_ADD_PHASE);
                    break;
            }
        }
        #endregion
    }
}
