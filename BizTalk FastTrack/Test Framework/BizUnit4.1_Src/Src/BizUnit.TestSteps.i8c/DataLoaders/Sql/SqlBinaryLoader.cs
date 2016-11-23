//---------------------------------------------------------------------
// File: FileDataLoader.cs
// 
// Summary: 
//
//---------------------------------------------------------------------
// Copyright (c) 2016, bizilante. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System.IO;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;
using System.Data.SqlClient;

namespace BizUnit.TestSteps.i8c.DataLoaders.Sql
{
    /// <summary>
    /// The SqlDataLoader maybe used to load a file from Sql and passed to a test 
    /// step or sub-step which accepts a dataloader. Test steps which use data loaders 
    /// benefit from increased flexibility around how they load data by de-coupling 
    /// the test step from how it loads its data.
    /// </summary>
    /// 
    /// <remarks>
    /// The following example demonstrates how to create and use a dataloader:
    /// 
    /// <code escaped="true">
    /// // The WebServiceStep allows a DataLoader to be used to set the RequestBody,
    /// // this allows greater flexibility around how data is loaded by a test step.
    /// 
    /// var ws = new WebServiceStep();
    ///	ws.Action = "http://schemas.affinus.com/finservices/tradeflow";
    /// 
    /// // Create the dataloader and configure...
    /// FileDataLoader dl = new FileDataLoader();
    /// dl.FilePath = @"..\..\..\Tests\Affinus.TradeServices.BVTs\TradeFlow\BookTrade_RQ.xml";
    /// 
    /// // Assign the dataloader to the RequestBody
    /// ws.RequestBody = dl;
    /// ws.ServiceUrl = "http://localhost/TradeServices/TradeFlow.svc";
    /// ws.Username = @"domain\user";
    ///	</code>
    /// </remarks>
    public class SqlBinaryLoader : DataLoaderBase
    {
        ///<summary>
        /// The connection string for the database from where the data will be loaded by the SqlDataLoader
        ///</summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// The SQL command to execute
        /// <remarks>Only the first column specified in the query string is used (it should be the varbinary column)</remarks>
        /// </summary>
        public string Command { get; set; }

        public override Stream Load(Context context)
        {
            using (var sqlConnection = new SqlConnection(ConnectionString))
            {
                sqlConnection.Open();
                var sqlCommand = new SqlCommand(Command, sqlConnection);
                var objData = (byte[])sqlCommand.ExecuteScalar();
                return StreamHelper.LoadMemoryStream(objData);
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(ConnectionString, "ConnectionString");
            ArgumentValidation.CheckForEmptyString(Command, "Command");
        }
    }
}
