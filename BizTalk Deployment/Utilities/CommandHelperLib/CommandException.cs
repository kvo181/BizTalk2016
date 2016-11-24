using System;
using System.Diagnostics;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace bizilante.Tools.CommandLine
{
    [Serializable]
    public class CommandException : Exception
    {
        private TraceLevel severity;
        private const string severityFieldName = "severityFieldName";

        public CommandException()
        {
        }

        public CommandException(string message) : base(message)
        {
        }

        protected CommandException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            this.severity = (TraceLevel) info.GetInt32("severityFieldName");
        }

        public CommandException(string message, TraceLevel severity) : base(message)
        {
            this.severity = severity;
        }

        public CommandException(string message, Exception exception) : base(message, exception)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter=true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            info.AddValue("severityFieldName", (int) this.severity, typeof(int));
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

