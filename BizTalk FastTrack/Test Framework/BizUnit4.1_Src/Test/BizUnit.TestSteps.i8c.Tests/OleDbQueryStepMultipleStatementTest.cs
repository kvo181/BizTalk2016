using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BizUnit.TestSteps.i8c.OleDb;
using BizUnit.Xaml;

namespace BizUnit.TestSteps.i8c.Tests
{
    [TestClass]
    public class OleDbQueryStepMultipleStatementTest
    {
        [TestMethod]
        public void TestOld()
        {
            OleDbQueryStep step = new OleDbQueryStep
            {
                ConnectionString = "Provider=OraOLEDB.Oracle;Data Source=10.0.0.233;User Id=vanhos2;Password=vanhos2;Initial Catalog=db11g",
                SQLQuery = new OleDbQuery
                {
                    RawSqlQuery = "SELECT * FROM TESTING",
                },
                NumberOfRowsExpected = 1
            };

            TestCase btc = new TestCase();
            btc.ExecutionSteps.Add(step);
            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod]
        public void TestNew()
        {
            OleDbQueryStep step = new OleDbQueryStep
            {
                ConnectionString = "Provider=OraOLEDB.Oracle;Data Source=10.0.0.233;User Id=vanhos2;Password=vanhos2;Initial Catalog=db11g",
                SQLQuery = new OleDbQuery
                {
                    RawSqlQuery = "SELECT * FROM TESTING",
                    NumberOfRowsExpected = 1
                }
            };

            TestCase btc = new TestCase();
            btc.ExecutionSteps.Add(step);
            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod]
        public void TestMultiple()
        {
            OleDbQuerySequenceStep step = new OleDbQuerySequenceStep
            {
                ConnectionString = "Provider=OraOLEDB.Oracle;Data Source=10.0.0.233;User Id=vanhos2;Password=vanhos2;Initial Catalog=db11g",
                SQLQuerySequence = new OleDbQuery[]
                {
                    new OleDbQuery
                    {
                        RawSqlQuery = "SELECT * FROM TESTING",
                        NumberOfRowsExpected = 1
                    },
                    new OleDbQuery
                    {
                        RawSqlQuery = "SELECT COUNT(*) AS cnt FROM TESTING",
                        NumberOfRowsExpected = 1
                    }
                }
            };

            TestCase btc = new TestCase();
            btc.ExecutionSteps.Add(step);
            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod]
        public void TestMultipleBackwardsCompatible()
        {
            OleDbQuerySequenceStep step = new OleDbQuerySequenceStep
            {
                ConnectionString = "Provider=OraOLEDB.Oracle;Data Source=10.0.0.233;User Id=vanhos2;Password=vanhos2;Initial Catalog=db11g",
                SQLQuerySequence = new OleDbQuery[]
                {
                    new OleDbQuery
                    {
                        RawSqlQuery = "SELECT * FROM TESTING"
                    },
                    new OleDbQuery
                    {
                        RawSqlQuery = "SELECT COUNT(*) AS cnt FROM TESTING"
                    }
                },
                NumberOfRowsExpected = 1
            };

            TestCase btc = new TestCase();
            btc.ExecutionSteps.Add(step);
            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }

        [TestMethod]
        public void TestGenerator()
        {
            string connectionstring = "Provider=OraOLEDB.Oracle;Data Source=10.0.0.233;User Id=vanhos2;Password=vanhos2;Initial Catalog=db11g";
            string statements = "SELECT * FROM TESTING; SELECT COUNT(*) AS cnt FROM TESTING";
            string delimiter = ";";
            int[] recordcounts = new int[]{ 1, 1};

            OleDbQuerySequenceStep step = OleDbQuerySequenceStepGenerator.getQuerySequenceStep(connectionstring, statements, delimiter, recordcounts);

            TestCase btc = new TestCase();
            btc.ExecutionSteps.Add(step);
            BizUnit bu = new BizUnit(btc);
            bu.RunTest();
        }
    }
}
