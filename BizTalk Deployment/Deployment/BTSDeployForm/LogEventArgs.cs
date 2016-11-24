using System;
using System.Diagnostics;

namespace bizilante.Deployment.BTSDeployForm
{
    public class LogEventArgs : EventArgs
    {
        public string Message { get; set; }
        public EventLogEntryType Type { get; set; }
    }
}
