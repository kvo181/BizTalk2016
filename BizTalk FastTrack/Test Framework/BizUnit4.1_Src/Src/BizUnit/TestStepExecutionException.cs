//---------------------------------------------------------------------
// File: TestStepExecutionException.cs
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

namespace BizUnit
{
    using System;

    /// <summary>
    /// TestStepExecutionException is thrown by BizUnit to indicate a validation step failed.
    /// </summary>
    /// <remarks>The ValidationStepExecutionException is thrown by BizUnit when a validation step fails, the 
    /// framework automatically wraps the exception thrown by the validaiton step with an 
    /// TestStepExecutionException</remarks>
    public class TestStepExecutionException : Exception
    {
        private readonly string _testCaseName;
        private readonly string _testStepName;
        private readonly TestStage _stage;

        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="testCaseName">The name of the BizUnit test case executing whilst the validation step failed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        ///	</remarks>
        public TestStepExecutionException(string message, TestStage stage, string testCaseName, string testStepName)
            : base(message) 
        {
            _stage = stage;
            _testCaseName = testCaseName;
            _testStepName = testStepName;
        }

        /// <summary>
        /// TestStepExecutionException constructor.
        /// </summary>
        /// <param name="message">The message associated with this exception.</param>
        /// <param name="testCaseName">The name of the BizUnit test case executing whilst the validation step failed.</param>
        /// 
        /// <remarks>
        /// The following example demonstrates how to create and call BizUnit using 
        /// the BizUnit Test Case Object Model:
        ///	</remarks>
        public TestStepExecutionException(string message, Exception innerException, TestStage stage, string testCaseName, string testStepName)
            : base(message, innerException)
        {
            _stage = stage;
            _testCaseName = testCaseName;
            _testStepName = testStepName;
        }

        /// <summary>
        /// The name of the test case
        /// </summary>
        public string TestCaseName
        {
            get { return _testCaseName; }
        }

        /// <summary>
        /// The name of the test step
        /// </summary>
        public string TestStepName
        {
            get { return _testStepName; }
        }

        /// <summary>
        /// The test stage being executed
        /// </summary>
        public TestStage Stage
        {
            get { return _stage; }
        }
    }
}
