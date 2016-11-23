//---------------------------------------------------------------------
// File: DatabaseHelperEx.cs
// 
// Summary: 
//
// Copyright (c) http://bizunitextensions.codeplex.com. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Data;
using System.Data.SqlClient;

namespace BizUnit.Extensions.Utilities
{
	/// <summary>
	/// Static Helper for executing SQL statements
	/// </summary>
	public class DatabaseHelperEx
	{
        #region constructor(s)
        /// <summary>
        /// Constructor for class, default constructor is private to prevent instances being
        /// created as the class only has static methods
        /// </summary>
        public DatabaseHelperEx()
		{
		}
        #endregion

        #region Static Methods
        /// <summary>
        /// Excecutes the SQL statement against the database and returns a DataSet with the results
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        /// <returns>DataSet with the results of the executed command</returns>
        public DataSet ExecuteSqlCommand( string connectionString, string sqlCommand )
        {
            DataSet ds = new DataSet() ;

            using ( SqlConnection connection = new SqlConnection( connectionString ) )
            {
                SqlDataAdapter adapter = new SqlDataAdapter( sqlCommand, connection ) ;
                adapter.Fill( ds ) ;
            }   // connection

            return ds ;
        }

        /// <summary>
        /// Executes the SQL statement and returns the first column of the first row in the resultset returned by the query.
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        /// <returns>The contents of the first column of the first row in the resultset</returns>
        public int ExecuteScalar( string connectionString, string sqlCommand )
        {
            SqlConnection connection = null ;
            object col = 0 ;

            try 
            {
                connection = new SqlConnection( connectionString ) ;
                SqlCommand command = new SqlCommand( sqlCommand, connection ) ;
                command.Connection.Open() ;
                col = command.ExecuteScalar() ;
            }
            catch ( Exception )
            {
            	//TODO: Evaluate suppression of exception here !!!
            }
            finally 
            {
                connection.Close() ;
            }

            return Convert.ToInt32( col ) ;
        }

        /// <summary>
        /// Executes the SQL statement
        /// </summary>
        /// <param name="connectionString">Database connection string</param>
        /// <param name="sqlCommand">SQL statement to execute</param>
        public void ExecuteNonQuery( string connectionString, string sqlCommand )
        {
            SqlConnection connection = null ;

            try 
            {
                connection = new SqlConnection( connectionString ) ;
                SqlCommand command = new SqlCommand( sqlCommand, connection ) ;
                command.Connection.Open() ;
                command.ExecuteNonQuery() ;
            }
            catch ( Exception )
            {
            	//TODO: Evaluate suppression of exception here !!!
            }
            finally 
            {
                connection.Close() ;
            }
        }
        #endregion
	}
}
