using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using BizUnit.TestSteps.DataLoaders.File;
using BizUnit.TestSteps.i8c.DataLoaders.Sql;
using BizUnit.TestSteps.i8c.Msmq;
using BizUnit.TestSteps.i8c.ValidationSteps.File;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Tests
{
    /// <summary>
    /// Summary description for Msmq test Steps
    /// </summary>
    [TestClass]
    public class MsmqStepTests
    {
        public MsmqStepTests()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
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
        public void MsmqInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Create, Send, Peek, Read and Delete operations on a MSMQ queue";
            btc.Description = "Check/Validate the MSMQ related steps";
            btc.BizUnitVersion = "4.0.0.1";

            var queuePaths = new Collection<QueuePathDefinition>();
            queuePaths.Add(new QueuePathDefinition
                               {
                                   QueuePath = ".\\Private$\\MyTestQueue"
                                   , Transactional = true
                                   , ShouldExist = false
                               });

            // The queue should not exist yet
            var wsExists = new MsmqQueueExistsStep();
            wsExists.QueuePaths = queuePaths;
            wsExists.ThrowError = false;
            wsExists.DeleteWhenExists = true;
            // Add step
            btc.ExecutionSteps.Add(wsExists);

            // Create a queue
            var ws = new MsmqCreateQueueStep();
            ws.QueuePaths = queuePaths;
            // Add step
            btc.ExecutionSteps.Add(ws);

            // Put a message onto the queue
            var dataLoader = new FileDataLoader();
            dataLoader.FilePath = @"..\..\..\BizUnit.TestSteps.i8c.Tests\TestData\BizTalk2006.ToBeProcessed.A820.Request.xml";
            var wsPut = new MsmqWriteStep();
            wsPut.QueuePath = queuePaths[0].QueuePath;
            wsPut.MessageBody = dataLoader;
            wsPut.MessageLabel = "Some Test";
            wsPut.UseTransactions = true;
            // Add step
            btc.ExecutionSteps.Add(wsPut);

            // Peek into the queue and count the number of messages
            var wsPeek = new MsmqPeekStep();
            wsPeek.QueuePath = queuePaths[0].QueuePath;
            wsPeek.ExpectedNumberOfMessages = 1;
            wsPeek.TimeOut = 100;
            // Add step
            btc.ExecutionSteps.Add(wsPeek);

            // Read and validate the message
            var wsRead = new MsmqReadStep();
            wsRead.QueuePath = queuePaths[0].QueuePath;
            wsRead.TimeOut = 100;
            wsRead.SubSteps.Add(new BinaryValidationStep
                                    {
                                        ComparisonDataPath = @"..\..\..\BizUnit.TestSteps.i8c.Tests\TestData\BizTalk2006.ToBeProcessed.A820.Request.xml"
                                    });
            // Add step
            btc.ExecutionSteps.Add(wsRead);

            // Delete the queue
            var wsDelete = new MsmqDeleteQueueStep();
            wsDelete.QueuePaths = queuePaths;
            // Add step
            btc.ExecutionSteps.Add(wsDelete);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "MsmqInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void MsmqDmfaInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Create, Send, Peek, Read and Delete operations on a MSMQ queue";
            btc.Description = "Check/Validate the MSMQ related steps";
            btc.BizUnitVersion = "4.0.0.1";

            var queuePaths = new Collection<QueuePathDefinition>();
            queuePaths.Add(new QueuePathDefinition
            {
                QueuePath = ".\\Private$\\MyTestQueue"
                ,
                Transactional = true
                ,
                ShouldExist = false
            });

            // The queue should not exist yet
            var wsExists = new MsmqQueueExistsStep();
            wsExists.QueuePaths = queuePaths;
            wsExists.ThrowError = false;
            wsExists.DeleteWhenExists = true;
            // Add step
            btc.ExecutionSteps.Add(wsExists);

            // Create a queue
            var ws = new MsmqCreateQueueStep();
            ws.QueuePaths = queuePaths;
            // Add step
            btc.ExecutionSteps.Add(ws);

            // Put a message onto the queue
            var dataLoader = new FileDataLoader();
            dataLoader.FilePath = @"..\..\..\BizUnit.TestSteps.i8c.Tests\TestData\BizTalk2006.ToBeProcessed.A820.Request.xml";
            var wsPut = new MsmqWriteStep();
            wsPut.QueuePath = queuePaths[0].QueuePath;
            wsPut.MessageBody = dataLoader;
            wsPut.MessageLabel = "Some DMFA Test";
            wsPut.UseTransactions = true;
            wsPut.BodyType = VarEnum.VT_LPWSTR;
            // Add step
            btc.ExecutionSteps.Add(wsPut);

            // Peek into the queue and count the number of messages
            var wsPeek = new MsmqPeekStep();
            wsPeek.QueuePath = queuePaths[0].QueuePath;
            wsPeek.ExpectedNumberOfMessages = 1;
            wsPeek.TimeOut = 100;
            // Add step
            btc.ExecutionSteps.Add(wsPeek);

            // Read and validate the message
            var wsRead = new MsmqReadStep();
            wsRead.QueuePath = queuePaths[0].QueuePath;
            wsRead.TimeOut = 100;
            wsRead.BodyType = VarEnum.VT_LPWSTR;
            wsRead.SubSteps.Add(new BinaryValidationStep
            {
                ComparisonDataPath = @"..\..\..\BizUnit.TestSteps.i8c.Tests\TestData\BizTalk2006.ToBeProcessed.A820.Request.xml"
                , ReadAsUnicode = true
            });
            // Add step
            btc.ExecutionSteps.Add(wsRead);

            // Delete the queue
            var wsDelete = new MsmqDeleteQueueStep();
            wsDelete.QueuePaths = queuePaths;
            // Add step
            btc.ExecutionSteps.Add(wsDelete);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "MsmqDmfaInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void MsmqDmfaFromSQLInvoke()
        {
            TestCase btc = new TestCase();
            btc.Name = "Create, Send, Peek, Read and Delete operations on a MSMQ queue";
            btc.Description = "Check/Validate the MSMQ related steps (test data is retrieved out of a SQL database)";
            btc.BizUnitVersion = "4.0.0.1";

            var queuePaths = new Collection<QueuePathDefinition>();
            queuePaths.Add(new QueuePathDefinition
            {
                QueuePath = ".\\Private$\\MyTestQueue",
                Transactional = true,
                ShouldExist = false
            });

            // The queue should not exist yet
            var wsExists = new MsmqQueueExistsStep();
            wsExists.QueuePaths = queuePaths;
            wsExists.ThrowError = false;
            wsExists.DeleteWhenExists = true;
            // Add step
            btc.ExecutionSteps.Add(wsExists);

            // Create a queue
            var ws = new MsmqCreateQueueStep();
            ws.QueuePaths = queuePaths;
            // Add step
            btc.ExecutionSteps.Add(ws);

            // Put a message onto the queue
            var dataLoader = new SqlDataLoader();
            dataLoader.ConnectionString = "Data Source=.\\btsloc;Initial Catalog=BizTalkFluxDb_ForTesting;Integrated Security=True";
            dataLoader.Command = "SELECT [FluxData] FROM [BizTalkFluxDb_ForTesting].[dbo].[FluxData] WHERE [ID] = '970458E6-7F20-4F46-B69D-AA3D952ACA13'";
            var wsPut = new MsmqWriteStep();
            wsPut.QueuePath = queuePaths[0].QueuePath;
            wsPut.MessageBody = dataLoader;
            wsPut.MessageLabel = "Some DMFA Test";
            wsPut.UseTransactions = true;
            wsPut.BodyType = VarEnum.VT_LPWSTR;
            // Add step
            btc.ExecutionSteps.Add(wsPut);

            // Peek into the queue and count the number of messages
            var wsPeek = new MsmqPeekStep();
            wsPeek.QueuePath = queuePaths[0].QueuePath;
            wsPeek.ExpectedNumberOfMessages = 1;
            wsPeek.TimeOut = 100;
            // Add step
            btc.ExecutionSteps.Add(wsPeek);

            // Read and validate the message
            var wsRead = new MsmqReadStep();
            wsRead.QueuePath = queuePaths[0].QueuePath;
            wsRead.TimeOut = 100;
            wsRead.BodyType = VarEnum.VT_LPWSTR;
            wsRead.SubSteps.Add(new ValidationSteps.Sql.BinaryValidationStep
            {
                ConnectionString = "Data Source=.\\btsloc;Initial Catalog=BizTalkFluxDb_ForTesting;Integrated Security=True",
                Command = "SELECT [FluxData] FROM [BizTalkFluxDb_ForTesting].[dbo].[FluxData] WHERE [ID] = '970458E6-7F20-4F46-B69D-AA3D952ACA13'",
                ReadAsUnicode = true
            });
            // Add step
            btc.ExecutionSteps.Add(wsRead);

            // Delete the queue
            var wsDelete = new MsmqDeleteQueueStep();
            wsDelete.QueuePaths = queuePaths;
            // Add step
            btc.ExecutionSteps.Add(wsDelete);

            // Save and Execute test
            BizUnit bu = new BizUnit(btc);
            TestCase.SaveToFile(btc, "MsmqDmfaSqlInvoke.xaml");
            bu.RunTest();
        }

        [TestMethod]
        public void MsmqInvoke_LoadFromXaml()
        {
            var tc = TestCase.LoadFromFile(@"..\..\..\BizUnit.TestSteps.i8c.Tests\TestCases\MsmqInvoke.xml");
            BizUnit bu = new BizUnit(tc);
            bu.RunTest();
        }
    }
}
