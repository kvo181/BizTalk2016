using System;

namespace bizilante.Rules.Helper
{
    public class RuleEventArgs : EventArgs
    {
        public RuleEventArgs(string message, bool error)
        {
            Message = message;
            IsError = error;
        }

        public RuleEventArgs(string source, string message, bool isError)
        {
            Source = source;
            Message = message;
            IsError = isError;
        }

        public bool IsError { get; set; }
        public string Message { get; set; }
        public string Source { get; set; }
    }
}