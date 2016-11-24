using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace bizilante.Tools.CommandLine
{
    [Serializable]
    public enum LogEntryType
    {
        [XmlEnum("Error")]
        Error = 1,
        [XmlEnum("Information")]
        Information = 3,
        [XmlEnum("None")]
        None = 0,
        [XmlEnum("Verbose")]
        Verbose = 4,
        [XmlEnum("Warning")]
        Warning = 2
    }
}
