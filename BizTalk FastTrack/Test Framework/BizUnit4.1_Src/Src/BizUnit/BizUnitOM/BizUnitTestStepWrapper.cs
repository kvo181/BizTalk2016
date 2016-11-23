//---------------------------------------------------------------------
// File: BizUnitTestStepWrapper.cs
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

using BizUnit.Common;

namespace BizUnit.BizUnitOM
{
    using System;
    using System.Xml;

    /// <summary>
    /// BizUnitTestStepWrapper wraps BizUnit test steps and provides access to any exceptions raised at runtime.
    /// </summary>
    [Obsolete("BizUnitTestStepWrapper has been deprecated. Please investigate the use of BizUnit.Xaml.TestCase.")]
    public class BizUnitTestStepWrapper
    {
        private readonly ITestStep _testStep;
        private readonly TestStepBuilder _testStepBuilder;
        private XmlNode _stepConfig;
        private bool _runConcurrently;
        private bool _failOnError = true;
        private string _typeName;
        private string _assemblyPath;
        private Exception _executeException;

        internal BizUnitTestStepWrapper(XmlNode stepConfig)
        {
            ArgumentValidation.CheckForNullReference(stepConfig, "stepConfig");

            LoadStepConfig(stepConfig);
            object obj = ObjectCreator.CreateStep(_typeName, _assemblyPath);
            _testStep = obj as ITestStep;

            if (null == _testStep)
            {
                throw new ArgumentException(string.Format("The test step could not be created, check the test step type and assembly path are correct, type: {0}, assembly path: {1}", _typeName, _assemblyPath));
            }
        }

        internal BizUnitTestStepWrapper(ITestStep testStep, XmlNode stepConfig)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(stepConfig, "stepConfig");

            LoadStepConfig(stepConfig);
            _testStep = testStep;
        }

        internal BizUnitTestStepWrapper(ITestStep testStep, XmlNode stepConfig, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            ArgumentValidation.CheckForNullReference(stepConfig, "stepConfig");

            LoadStepConfig(stepConfig);
            _testStep = testStep;
            _runConcurrently = runConcurrently;
            _failOnError = failOnError;
        }

        internal BizUnitTestStepWrapper(ITestStepOM testStep, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStep, "testStep");
            
            _testStepBuilder = new TestStepBuilder(testStep);
            _runConcurrently = runConcurrently;
            _failOnError = failOnError;
            _typeName = testStep.GetType().ToString();
        }

        internal BizUnitTestStepWrapper(TestStepBuilder testStepBuilder, bool runConcurrently, bool failOnError)
        {
            ArgumentValidation.CheckForNullReference(testStepBuilder, "testStepBuilder");

            _testStepBuilder = testStepBuilder;
            _runConcurrently = runConcurrently;
            _failOnError = failOnError;
            _typeName = testStepBuilder.TestStepOM.GetType().ToString();
        }

        internal bool RunConcurrently
        {
            get
            {
                return _runConcurrently;
            }
        }

        internal bool FailOnError
        {
            get
            {
                return _failOnError;
            }
        }

        internal string TypeName
        {
            get
            {
                return _typeName;
            }
        }

        internal void Execute(Context ctx)
        {
            try
            {
                var tsea = new TestStepEventArgs(ctx.CurrentTestStage, ctx.TestName, TypeName);
                ctx.BizUnitObject.OnTestStepStart(tsea);

                if (null != _stepConfig)
                {
                    _testStep.Execute(_stepConfig, ctx);
                }
                else
                {
                    _testStepBuilder.PrepareStepForExecution(ctx);

                    _testStepBuilder.PrepareSubStepsForExecution(ctx);

                    ctx.BizUnitObject.OnTestStepStart(tsea);

                    _testStepBuilder.TestStepOM.Validate(ctx);
                    _testStepBuilder.TestStepOM.Execute(ctx);
                }

                ctx.BizUnitObject.OnTestStepStop(tsea);
            }
            catch (Exception executionException)
            {
                _executeException = executionException;
                throw;
            }
        }

        private void LoadStepConfig(XmlNode config)
        {
            _stepConfig = config;
            XmlNode assemblyPathNode = _stepConfig.SelectSingleNode("@assemblyPath");
            XmlNode typeNameNode = _stepConfig.SelectSingleNode("@typeName");
            XmlNode runConcurrentlyNode = _stepConfig.SelectSingleNode("@runConcurrently");
            XmlNode failOnErrorNode = _stepConfig.SelectSingleNode("@failOnError");

            if (null != runConcurrentlyNode)
            {
                _runConcurrently = Convert.ToBoolean(runConcurrentlyNode.Value);
            }

            if (null != failOnErrorNode)
            {
                _failOnError = Convert.ToBoolean(failOnErrorNode.Value);
            }

            _typeName = typeNameNode.Value;
            if (null != assemblyPathNode)
            {
                _assemblyPath = assemblyPathNode.Value;
            }
            else
            {
                _assemblyPath = string.Empty;
            }
        }

        /// <summary>
        /// Returns the exception generated during execution, otherwise null.
        /// </summary>
        /// <value>The exception which occured during execution.</value>
        public Exception ExecuteException
        {
            get { return _executeException; }
        }
    }
}
