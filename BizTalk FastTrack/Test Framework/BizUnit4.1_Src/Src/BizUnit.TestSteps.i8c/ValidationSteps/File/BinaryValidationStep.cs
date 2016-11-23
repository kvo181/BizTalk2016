//---------------------------------------------------------------------
// File: BinaryValidationStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.IO;
using BizUnit.Xaml;
using StreamHelper = BizUnit.TestSteps.Common.StreamHelper;

namespace BizUnit.TestSteps.i8c.ValidationSteps.File
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
        private string _comparisonDataPath;
        private bool _compareAsUtf8;

        ///<summary>
        /// The path of the data to compare against.
        ///</summary>
        public string ComparisonDataPath
        {
            set
            {
                _comparisonDataPath = value;
            }
        }

        ///<summary>
        /// true if both ComparisonDataPath and the data are to be compared to UTF8 before comparing (optional)
        /// (default=false)
        ///</summary>
        public bool CompareAsUtf8
        {
            set
            {
                _compareAsUtf8 = value;
            }
        }

        /// <summary>
        /// true if ComparisonDataPath has to be converted into unicode
        /// </summary>
        public bool ReadAsUnicode { get; set; }

        /// <summary>
        /// true if ComparisonDataPath has to be converted into string before compare
        /// </summary>
        public bool ReadAsString { get; set; }

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
                    if (ReadAsString)
                    {
                        dataToValidateAgainst = 
                            StreamHelper.LoadMemoryStream(Common.StreamHelper.LoadFileToString(_comparisonDataPath));
                    }
                    else
                        dataToValidateAgainst = !ReadAsUnicode ? StreamHelper.LoadFileToStream(_comparisonDataPath) :
                            Common.StreamHelper.LoadFileToStream(_comparisonDataPath);
                }
                catch (Exception e)
                {
                    context.LogError("BinaryValidationStep failed, exception caugh trying to open comparison file: {0}", _comparisonDataPath);
                    context.LogException(e);
                    throw;
                }

                try
                {
                    data.Seek(0, SeekOrigin.Begin);
                    dataToValidateAgainst.Seek(0, SeekOrigin.Begin);

                    if (_compareAsUtf8)
                    {
                        // Compare the streams, make sure we are comparing like for like
                        StreamHelper.CompareStreams(StreamHelper.EncodeStream(data, System.Text.Encoding.UTF8), StreamHelper.EncodeStream(dataToValidateAgainst, System.Text.Encoding.UTF8));
                    }
                    else
                    {
                        StreamHelper.CompareStreams(data, dataToValidateAgainst);
                    }
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
            // compareAsUTF8 - optional
            if (string.IsNullOrEmpty(_comparisonDataPath))
            {
                throw new ArgumentNullException("ComparisonDataPath is either null or of zero length");
            }
            _comparisonDataPath = context.SubstituteWildCards(_comparisonDataPath);
        }
    }
}
