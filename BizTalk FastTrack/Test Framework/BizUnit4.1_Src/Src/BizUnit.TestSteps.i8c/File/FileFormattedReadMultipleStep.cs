//---------------------------------------------------------------------
// File: FileFormattedReadMultipleStep.cs
// 
// Summary: 
//
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Collections.ObjectModel;
using BizUnit.BizUnitOM;
using BizUnit.Common;
using BizUnit.TestSteps.File;
using BizUnit.TestSteps.i8c.Common;
using BizUnit.Xaml;
using StreamHelper = BizUnit.TestSteps.Common.StreamHelper;

namespace BizUnit.TestSteps.i8c.File
{
    /// <summary>
    /// The FileMultiValidateStep step checks a given directory for files matching the file masks and iterates around all of the specified validate steps
    /// to validate the file.
    /// </summary>
    public class FileFormattedReadMultipleStep : FileReadMultipleStep
    {
        ///<summary>
        /// Filter to apply to directory path, e.g. "*.xml" or "MyFile*.txt"
        ///</summary>
        public string FormattedSearchPattern { get; set; }

        private DateTime? _createdAfter;
        ///<summary>
        /// (optional)
        /// The files must have a creation date recenter than this one.
        ///</summary>
        public DateTime? CreatedAfter
        {
            get
            {
                if (!_createdAfter.HasValue)
                    _createdAfter = DateTime.Today;
                return _createdAfter;
            }
            set
            {
                _createdAfter = value;
                _createdAfterSet = true;
            }
        }

        private bool _createdAfterSet = false;

        /// <summary>
        /// The parameters to substitute into the the cref="FormattedSearchPattern", 
        /// come from the context
        /// </summary>
        public Collection<ParameterFromContext> SearchContextParameters { get; set; }

        ///<summary>
        /// Default constructor
        ///</summary>
        public FileFormattedReadMultipleStep()
        {
            SearchContextParameters = new Collection<ParameterFromContext>();
        }

        /// <summary>
        /// TestStepBase.Execute() implementation
        /// </summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        public override void Execute(Context context)
        {
            var endTime = DateTime.Now.AddMilliseconds(Timeout);
            string[] filelist;

            SearchPattern = GetFormattedSearchQuery(context);

            context.LogInfo("Searching directory: {0}, search pattern: {1}", DirectoryPath, SearchPattern);

            do
            {
                Thread.Sleep(100);

                if (CreatedAfter.HasValue)
                {
                    var di = new DirectoryInfo(DirectoryPath);
                    var fis = di.GetFiles(SearchPattern);
                    var fileList = (from fi in fis
                                    where _createdAfter != null
                                    where fi.LastAccessTimeUtc > CreatedAfter.Value
                                    select fi.FullName).ToList();
                    filelist = fileList.ToArray();
                }
                else
                {
                    // Get the list of files in the directory
                    filelist = Directory.GetFiles(DirectoryPath, SearchPattern);
                }

                if (filelist.Length == ExpectedNumberOfFiles)
                    break;

            } while (endTime > DateTime.Now);

            context.LogInfo("Number of files found: {0}", filelist.Length);

            if (filelist.Length == 0)
            {
                // Expecting more than one file 
                throw new ApplicationException(String.Format("Directory contains no files matching the pattern!"));
            }

            if (0 < ExpectedNumberOfFiles && filelist.Length != ExpectedNumberOfFiles)
            {
                // Expecting a specified number of files
                throw new ApplicationException(String.Format("Directory contained: {0} files, but the step expected: {1} files", filelist.Length, ExpectedNumberOfFiles));
            }

            // For each file in the file list
            foreach (var filePath in filelist)
            {
                context.LogInfo("FileReadMultipleStep validating file: {0}", filePath);

                // add the current filepath to the context for validation based on file properties
                context.Add("validatingFilePath", filePath);

                Stream fileData = StreamHelper.LoadFileToStream(filePath, Timeout);
                context.LogData("File: " + filePath, fileData, NumberOfCharsToLog);
                fileData.Seek(0, SeekOrigin.Begin);

                // Check it against the validate steps to see if it matches one of them
                foreach (var subStep in SubSteps)
                {
                    try
                    {
                        // Try the validation and catch the exception
                        fileData = subStep.Execute(fileData, context);
                    }
                    catch (Exception ex)
                    {
                        context.LogException(ex);
                        throw;
                    }
                }

                if (DeleteFiles)
                {
                    System.IO.File.Delete(filePath);
                }

                // remove the current filepath from the context
                context.Remove("validatingFilePath");
            }
        }

        public override void Validate(Context context)
        {
            ArgumentValidation.CheckForEmptyString(DirectoryPath, "DirectoryPath");
            ArgumentValidation.CheckForEmptyString(FormattedSearchPattern, "FormattedSearchPattern");
            if (ExpectedNumberOfFiles < 1)
                throw new ArgumentException(string.Format("ExpectedNumberOfFiles should be greater than zero, but was set to: {0}", ExpectedNumberOfFiles));

        }

        ///<summary>
        /// Formats the query string, replacing the formatting instructions in cref="RawSqlQuery" with the parameters in cref="QueryParameters"
        ///</summary>
        /// <param name='context'>The context for the test, this holds state that is passed beteen tests</param>
        ///<returns></returns>
        private string GetFormattedSearchQuery(Context context)
        {
            if (SearchContextParameters.Count > 0)
            {
                var bAllFound = true;
                var c = 0;
                var objParams = new object[SearchContextParameters.Count];
                foreach (var obj in SearchContextParameters)
                {
                    if (!context.ContainsKey(obj.Key))
                    {
                        bAllFound = false;
                        continue;
                    }
                    var objValue = context.GetValue(obj.Key);
                    if (obj.DataType == "System.Guid")
                        objValue = Guid.Parse(objValue).ToString();
                    objParams[c++] = objValue;
                }
                return bAllFound ? string.Format(FormattedSearchPattern, objParams) : FormattedSearchPattern;
            }

            return FormattedSearchPattern;
        }
    }
}
