
namespace BizUnit
{
    using System;

    public class TestStepEventArgs : EventArgs
    {
        private readonly TestStage _stage;
        private readonly string _testCaseName;
        private readonly string _testStepTypeName;

        internal TestStepEventArgs(TestStage stage, string testCaseName, string testStepTypeName)
        {
            _stage = stage;
            _testCaseName = testCaseName;
            _testStepTypeName = testStepTypeName;
        }

        public TestStage Stage
        {
            get
            {
                return _stage;
            }
        }

        public string TestCaseName
        {
            get
            {
                return _testCaseName;
            }
        }

        public string TestStepTypeName
        {
            get
            {
                return _testStepTypeName;
            }
        }
    }
}
