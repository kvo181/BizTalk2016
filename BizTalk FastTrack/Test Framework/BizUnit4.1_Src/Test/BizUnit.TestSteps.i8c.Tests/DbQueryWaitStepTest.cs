using System.Collections.ObjectModel;
using System.Threading;
using BizUnit.TestSteps.i8c.Sql;
using BizUnit.TestSteps.Sql;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using BizUnit;

namespace BizUnit.TestSteps.i8c.Tests
{
    
    
    /// <summary>
    ///This is a test class for DbQueryWaitStepTest and is intended
    ///to contain all DbQueryWaitStepTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DbQueryWaitStepTest
    {


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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void DbQueryWaitStepOk()
        {
            var step = new DbQueryStep();
            step.ConnectionString = "Data Source=.\\btsloc;Initial Catalog=BizUnitDb;Integrated Security=True";
            step.SQLQuery = new SqlQueryExtended();
            step.SQLQuery.RawSqlQuery = "CREATE TABLE dbo.Test ( Value int NOT NULL ); INSERT INTO dbo.Test Values (1);";
            step.NumberOfRowsExpected = 0;
            step.Execute(new Context());

            var target = new DbQueryWaitStep();
            target.ConnectionString = "Data Source=.\\btsloc;Initial Catalog=BizUnitDb;Integrated Security=True";
            target.DelayBeforeCheck = 1;
            target.Timeout = 50000;
            target.SQLQuery = new SqlQueryExtended();
            target.SQLQuery.RawSqlQuery = "SELECT * FROM dbo.Test WHERE Value = {0}";
            target.SQLQuery.QueryParameters = new Collection<object>();
            target.SQLQuery.QueryParameters.Add(1);
            target.NumberOfRowsExpected = 1;

            try
            {
                target.Execute(new Context());
            }
            finally
            {
                step.SQLQuery.RawSqlQuery = "DROP TABLE dbo.Test;";
                step.NumberOfRowsExpected = 0;
                step.Execute(new Context());
            }


        }

        /// <summary>
        ///A test for Execute
        ///</summary>
        [TestMethod()]
        public void DbQueryWaitStepNok()
        {
            var step = new DbQueryStep();
            step.ConnectionString = "Data Source=.\\btsloc;Initial Catalog=BizUnitDb;Integrated Security=True";
            step.SQLQuery = new SqlQueryExtended();
            step.SQLQuery.RawSqlQuery = "CREATE TABLE dbo.Test ( Value int NOT NULL ); INSERT INTO dbo.Test Values (1);";
            step.NumberOfRowsExpected = 0;
            step.Execute(new Context());

            var target = new DbQueryWaitStep();
            target.ConnectionString = "Data Source=.\\btsloc;Initial Catalog=BizUnitDb;Integrated Security=True";
            target.DelayBeforeCheck = 1;
            target.Timeout = 5000;
            target.SQLQuery = new SqlQueryExtended();
            target.SQLQuery.RawSqlQuery = "SELECT * FROM dbo.Test WHERE Value = {0}";
            target.SQLQuery.QueryParameters = new Collection<object>();
            target.SQLQuery.QueryParameters.Add(1);
            target.NumberOfRowsExpected = 2;

            try
            {
                target.Execute(new Context());
                throw new AssertFailedException("Wait succeeded");
            }
            catch (Exception ex)
            {
                testContextInstance.WriteLine("Found exception: {0}", ex.GetType());
            }
            finally
            {
                step.SQLQuery.RawSqlQuery = "DROP TABLE dbo.Test;";
                step.NumberOfRowsExpected = 0;
                step.Execute(new Context());
            }
        }
    }
}
