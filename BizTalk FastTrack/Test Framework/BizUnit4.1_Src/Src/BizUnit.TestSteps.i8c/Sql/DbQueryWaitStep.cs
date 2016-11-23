//---------------------------------------------------------------------
// File: DbQueryWaitStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) i8c 2016, Koen Van Oost. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using BizUnit.TestSteps.Sql;

namespace BizUnit.TestSteps.i8c.Sql
{
    ///<summary>
    ///</summary>
    public class DbQueryWaitStep : DbQueryStep
    {
        ///<summary>
        /// The time to wait in milisecs for the result, after which the step will fail if the result is not found.
        /// Starts after the delay.
        ///</summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Identifies those columns from which the data should be stored in the context
        /// </summary>
        public Collection<DbCellToContext> DbCellsToContext { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public DbQueryWaitStep()
        {
            DbCellsToContext = new Collection<DbCellToContext>();
        }

        /// <summary>
        /// Execute()
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            context.LogInfo("Using database connection string: {0}", ConnectionString);

            string sqlQueryToExecute ;
            if (SQLQuery is SqlQueryExtended)
                sqlQueryToExecute = ((SqlQueryExtended)SQLQuery).GetFormattedSqlQuery(context);
            else
                sqlQueryToExecute = SQLQuery.GetFormattedSqlQuery(context);

            var cont = true;
            string errorMessage;
            var now = DateTime.Now;
            do
            {
                // Sleep for delay seconds...
                if (0 < DelayBeforeCheck)
                {
                    context.LogInfo("Sleeping for: {0} seconds", DelayBeforeCheck);
                    System.Threading.Thread.Sleep(DelayBeforeCheck * 1000);
                }

                context.LogInfo("Executing database query: {0}", sqlQueryToExecute);
                var ds = FillDataSet(ConnectionString, sqlQueryToExecute);

                if (CheckData(context, ds, out errorMessage)) cont = false;

            } while (cont && (now.AddMilliseconds(Timeout).CompareTo(DateTime.Now) > 0));

            if (!string.IsNullOrEmpty(errorMessage))
                throw new Exception(errorMessage);

        }

        private bool CheckData(Context context, DataSet ds, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (NumberOfRowsExpected != ds.Tables[0].Rows.Count)
            {
                errorMessage = string.Format("Number of rows expected to be returned by the query does not match the value specified in the teststep. Number of rows the NnumberOfRowsExpected were: {0}, actual: {1}", NumberOfRowsExpected, ds.Tables[0].Rows.Count);
                return false;
            }

            context.LogInfo("NumberOfRowsExpected: {0}, actual number returned: {1}", NumberOfRowsExpected, ds.Tables[0].Rows.Count);

            if (0 == NumberOfRowsExpected)
                return true;

            if (0 < DbRowsToValidate.Count)
            {
                int rowCount = 0;

                foreach (var dbRowToValidate in DbRowsToValidate)
                {
                    context.LogInfo("Validating row number: {0}", rowCount);

                    var resultRow = ds.Tables[0].Rows[rowCount];
                    ;
                    var cells = dbRowToValidate.Cells;

                    foreach (DbCellToValidate cell in cells)
                    {
                        object dbData = resultRow[cell.ColumnName];
                        var dbDataStringValue = string.Empty;

                        if (0 == ValidateData(dbData, cell.ExpectedValue, ref dbDataStringValue))
                        {
                            context.LogInfo("Validation succeeded for field: {0}. Expected value: {1}",
                                            cell.ColumnName, dbDataStringValue);
                        }
                        else
                        {
                            errorMessage =
                                String.Format(
                                    "Validation failed for field: {0}. Expected value: {1}, actual value: {2}",
                                    cell.ColumnName, cell.ExpectedValue, dbDataStringValue);
                            return false;
                        }
                    }

                    rowCount++;
                }
            }

            if (0 < DbCellsToContext.Count)
            {
                foreach (DataRow resultRow in ds.Tables[0].Rows)
                {
                    foreach (var cell in DbCellsToContext)
                    {
                        var dbData = resultRow[cell.ColumnName];
                        context.Add(cell.Key, dbData, true);
                        context.LogInfo("Column value ({0}) added to context: {1}-{2}", cell.ColumnName, cell.Key, dbData);
                    }
                }
            }

            return true;
        }

        private static int ValidateData(object dbData, string targetValue, ref string dbDataStringValue)
        {
            dbDataStringValue = Convert.ToString(dbData);

            switch (dbData.GetType().ToString())
            {
                case ("System.DateTime"):
                    var dbDt = (DateTime)dbData;
                    DateTime targetDt = Convert.ToDateTime(targetValue);
                    return targetDt.CompareTo(dbDt);

                case ("System.DBNull"):
                    dbDataStringValue = "null";
                    return targetValue.CompareTo("null");

                case ("System.String"):
                    dbDataStringValue = (string)dbData;
                    return targetValue.CompareTo((string)dbData);

                case ("System.Int16"):
                    var dbInt16 = (System.Int16)dbData;
                    var targetInt16 = Convert.ToInt16(targetValue);
                    return targetInt16.CompareTo(dbInt16);

                case ("System.Int32"):
                    var dbInt32 = (System.Int32)dbData;
                    var targetInt32 = Convert.ToInt32(targetValue);
                    return targetInt32.CompareTo(dbInt32);

                case ("System.Int64"):
                    var dbInt64 = (System.Int64)dbData;
                    var targetInt64 = Convert.ToInt64(targetValue);
                    return targetInt64.CompareTo(dbInt64);

                case ("System.Double"):
                    var dbDouble = (System.Double)dbData;
                    var targetDouble = Convert.ToDouble(targetValue);
                    return targetDouble.CompareTo(dbDouble);

                case ("System.Decimal"):
                    var dbDecimal = (System.Decimal)dbData;
                    var targetDecimal = Convert.ToDecimal(targetValue);
                    return targetDecimal.CompareTo(dbDecimal);

                case ("System.Boolean"):
                    var dbBoolean = (System.Boolean)dbData;
                    var targetBoolean = Convert.ToBoolean(targetValue);
                    return targetBoolean.CompareTo(dbBoolean);

                case ("System.Byte"):
                    var dbByte = (System.Byte)dbData;
                    var targetByte = Convert.ToByte(targetValue);
                    return targetByte.CompareTo(dbByte);

                case ("System.Char"):
                    var dbChar = (System.Char)dbData;
                    var targetChar = Convert.ToChar(targetValue);
                    return targetChar.CompareTo(dbChar);

                case ("System.SByte"):
                    var dbSByte = (System.SByte)dbData;
                    var targetSByte = Convert.ToSByte(targetValue);
                    return targetSByte.CompareTo(dbSByte);

                default:
                    throw new ApplicationException(string.Format("Unsupported type: {0}", dbData.GetType()));
            }
        }

        private static DataSet FillDataSet(string connectionString, string sqlQuery)
        {
            var connection = new SqlConnection(connectionString);
            var ds = new DataSet();

            using (connection)
            {
                var adapter = new SqlDataAdapter
                {
                    SelectCommand = new SqlCommand(sqlQuery, connection)
                };
                adapter.Fill(ds);
                return ds;
            }
        }

        public override void Validate(Context context)
        {
            // _delayBeforeCheck - optional

            if (string.IsNullOrEmpty(ConnectionString))
            {
                throw new ArgumentNullException("ConnectionString");
            }
            ConnectionString = context.SubstituteWildCards(ConnectionString);

            if (null == SQLQuery)
            {
                throw new ArgumentNullException("SQLQuery");
            }

            SQLQuery.Validate(context);
        }
    }
}
