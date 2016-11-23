//---------------------------------------------------------------------
// File: BinaryValidationStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Data.SqlClient;
using System.IO;
using BizUnit.Common;
using BizUnit.TestSteps.Common;
using BizUnit.Xaml;
using System.Text;

namespace BizUnit.TestSteps.i8c.ValidationSteps.Sql
{
    /// <summary>
    /// The BinaryValidationStep performs a binary validation of the data supplied.
    /// </summary>
    /// 
    /// <remarks>
    /// The following shows an example of the Xml representation of this test step.
    /// 
    /// <code escaped="true">
    ///	<ValidationStep assemblyPath="" typeName="BizUnit.BinaryValidationStep">
    ///		<ComparisonDataPath>.\TestData\ResultDoc1.xml</ComparisonDataPath>
    ///		<CompareAsUTF8>true</CompareAsUTF8>
    ///	</ValidationStep>
    ///	</code>
    ///	
    ///	<list type="table">
    ///		<listheader>
    ///			<term>Tag</term>
    ///			<description>Description</description>
    ///		</listheader>
    ///		<item>
    ///			<term>ComparisonDataPath</term>
    ///			<description>The path of the data to compare against.</description>
    ///		</item>
    ///		<item>
    ///			<term>CompareAsUTF8</term>
    ///			<description>true if both ComparisonDataPath and the data are to be compared to UTF8 before comparing (optional)(default=false)</description>
    ///		</item>
    ///	</list>
    ///	</remarks>	
    public class BinaryValidationStep : SubStepBase
    {
        ///<summary>
        /// The connection string for the database from where the data will be loaded
        ///</summary>
        public string ConnectionString { get; set; }
        /// <summary>
        /// The SQL command to execute
        /// <remarks>Only the first column specified in the query string is used (it should be the data column)</remarks>
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// true if ComparisonDataPath has to be converted into unicode (optional)
        /// </summary>
        public bool ReadAsUnicode { get; set; }

        /// <summary>
        /// IValidationStep.ExecuteValidation() implementation
        /// </summary>
        /// <param name='data'>The stream cintaining the data to be validated.</param>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override Stream Execute(Stream data, Context context)
        {
            MemoryStream dataToValidateAgainst = null;

            try
            {
                try
                {
                    var s = string.Empty;
                    using (var sqlConnection = new SqlConnection(ConnectionString))
                    {
                        sqlConnection.Open();
                        var sqlCommand = new SqlCommand(Command, sqlConnection);
                        var objData = sqlCommand.ExecuteScalar();
                        s = objData.ToString();
                    }
                    var strm = StreamHelper.LoadMemoryStream(s);
                    dataToValidateAgainst = !ReadAsUnicode ? strm :
                        StreamHelper.LoadMemoryStream(StreamHelper.EncodeStream(strm, Encoding.Unicode));
                }
                catch (Exception e)
                {
                    context.LogError("BinaryValidationStep failed, exception caugh trying to load data: {0}-{1}", ConnectionString, Command);
                    context.LogException(e);
                    throw;
                }

                try
                {
                    data.Seek(0, SeekOrigin.Begin);
                    dataToValidateAgainst.Seek(0, SeekOrigin.Begin);
                    StreamHelper.CompareStreams(data, dataToValidateAgainst);
                }
                catch (Exception e)
                {
                    context.LogError("Binary validation failed while comparing the two data streams with the following exception: {0}", e.ToString());

                    // Dump out streams for validation...
                    data.Seek(0, SeekOrigin.Begin);
                    dataToValidateAgainst.Seek(0, SeekOrigin.Begin);
                    context.LogData("Stream 1:", data);
                    context.LogData("Stream 2:", dataToValidateAgainst);

                    throw;
                }
            }
            finally
            {
                if (null != dataToValidateAgainst)
                {
                    dataToValidateAgainst.Close();
                }
            }
            return data;
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(ConnectionString, "ConnectionString");
            ArgumentValidation.CheckForEmptyString(Command, "Command");
        }
    }
}
