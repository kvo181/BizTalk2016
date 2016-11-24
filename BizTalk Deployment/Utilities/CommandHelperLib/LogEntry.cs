using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace bizilante.Tools.CommandLine
{
    [Serializable]
    public class LogEntry
    {
        // Fields
        private string message;
        private LogEntryType type;

        // Methods
        public LogEntry()
        {
        }

        public LogEntry(string message)
        {
            this.type = LogEntryType.Information;
            this.message = message;
        }

        public LogEntry(string message, LogEntryType type)
        {
            this.type = type;
            this.message = message;
        }

        public override string ToString()
        {
            switch (this.type)
            {
                case LogEntryType.Error:
                    return string.Format("Error: {0}", this.message);

                case LogEntryType.Warning:
                    return string.Format("Warning : {0}", this.message);

                case LogEntryType.Information:
                    return string.Format("Information : {0}", this.message);
            }
            return this.message;
        }

        // Properties
        [XmlElement("Message")]
        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                this.message = value;
            }
        }

        [XmlAttribute("Type")]
        public LogEntryType Type
        {
            get
            {
                return this.type;
            }
            set
            {
                this.type = value;
            }
        }
    }
}
