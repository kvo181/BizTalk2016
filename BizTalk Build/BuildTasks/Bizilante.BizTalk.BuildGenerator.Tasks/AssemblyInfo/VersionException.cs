using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bizilante.BuildGenerator.Tasks
{
    public class VersionException : Exception
    {
        public VersionException(string message) : base(message)
        {
        }
    }
}
