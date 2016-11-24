using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace bizilante.Tools.CommandLine
{
    [Serializable]
    public class CommandLineArgumentException : ArgumentException
    {
        private TraceLevel severity;
        private const string severityFieldName = "severity";

        public CommandLineArgumentException()
        {
        }

        public CommandLineArgumentException(string message) : base(message)
        {
        }

        protected CommandLineArgumentException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.severity = (TraceLevel) info.GetInt32("severity");
        }

        public CommandLineArgumentException(string message, Exception exception) : base(message, exception)
        {
        }

        public CommandLineArgumentException(string message, string parameterName) : base(message, parameterName)
        {
        }

        public CommandLineArgumentException(string message, string parameterName, TraceLevel severity) : base(message, parameterName)
        {
            this.severity = severity;
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("severity", (int) this.severity, typeof(int));
            base.GetObjectData(info, context);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public TraceLevel Severity
        {
            get
            {
                return this.severity;
            }
            set
            {
                this.severity = value;
            }
        }
    }
}

