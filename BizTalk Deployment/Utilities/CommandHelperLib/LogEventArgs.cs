using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bizilante.Tools.CommandLine
{
[Serializable]
public class LogEventArgs : EventArgs
{
    // Fields
    private readonly LogEntry logEntry;

    // Methods
    public LogEventArgs()
    {
    }

    public LogEventArgs(LogEntry logEntry)
    {
        this.logEntry = logEntry;
    }

    public override string ToString()
    {
        if (this.logEntry == null)
        {
            return string.Empty;
        }
        return this.logEntry.ToString();
    }

    // Properties
    public LogEntry LogEntry
    {
        get
        {
            return this.logEntry;
        }
    }
}
 
}
