using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BizUnit.TestScenario
{
    class TestScenarioInvocationException : Exception
    {
        public TestScenarioInvocationException(String message, Exception innerException) : base(message, innerException)
        { }
    }
}
