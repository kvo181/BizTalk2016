//---------------------------------------------------------------------
// File: ConcurrentTestStepWrapper.cs
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

using BizUnit.BizUnitOM;
using BizUnit.Xaml;

namespace BizUnit
{
	using System;

	/// <summary>
	/// Summary description for ConcurrentTestStepWrapper.
	/// </summary>
	internal class ConcurrentTestStepWrapper
	{
	    private BizUnitTestStepWrapper _stepWrapper;
		private Context _context;
		private ILogger _logger;
		private Exception _ex;
	    private TestStepBase _testStep;

        public ConcurrentTestStepWrapper(TestStepBase testStep, Context ctx )
        {
            _testStep = testStep;
            _logger = new Logger();
		    _logger.ConcurrentExecutionMode = true;
            _context = ctx.CloneForConcurrentUse(_logger);
        }

		public ConcurrentTestStepWrapper(BizUnitTestStepWrapper stepWrapper, Context ctx )
		{
            _stepWrapper = stepWrapper;
            // TODO:.... ILogger
            _logger = new Logger();
		    _logger.ConcurrentExecutionMode = true;
            _context = ctx.CloneForConcurrentUse(_logger);
        }

	    
		public string Name
		{
			get 
			{
				return (null != _testStep) ? _testStep.GetType().ToString() : _stepWrapper.TypeName;
			}
		}

        public bool FailOnError
        {
            get
            {
                return (null != _testStep) ? _testStep.FailOnError : _stepWrapper.FailOnError;
            }
        }

	    public string StepName
	    {
	        get
	        {
                if (null != _testStep)
                    return _testStep.GetType().ToString();
                
                if(null != _stepWrapper)
	                return _stepWrapper.TypeName;

	            return null;
	        }
	    }

	    public BizUnitTestStepWrapper StepWrapper
	    {
	        get
	        {
	            return _stepWrapper;
	        }
	    }

	    public TestStepBase TestStep
	    {
	        get
	        {
	            return _testStep;
	        }
	    }

		public Exception FailureException
		{
			get
			{
				return _ex;
			}
		}

		public void Execute()
		{
			try
			{
                if(null != _testStep)
                {
                    _logger.TestStepStart(_testStep.GetType().ToString(), DateTime.Now, true, _testStep.FailOnError);
                    if (_testStep is ImportTestCaseStep)
                    {
                        ExecuteImportedTestCase(_testStep as ImportTestCaseStep, _context);
                    }
                    else
                    {
                        _testStep.Execute(_context);
                    }
                }
                else
                {
                    _logger.TestStepStart(_stepWrapper.TypeName, DateTime.Now, true, _stepWrapper.FailOnError);
                    _stepWrapper.Execute(_context);
                }
			}
			catch(Exception e)
			{
				_logger.LogException( e );
				_ex = e;
			}
		}

		public ILogger Logger
		{
            get
            {
                return _logger;
            }
		}

        private static void ExecuteImportedTestCase(ImportTestCaseStep testStep, Context context)
        {
            var testCase = testStep.GetTestCase();
            var bu = new BizUnit(testCase, context);
            bu.RunTest();
        } 
	}
}
