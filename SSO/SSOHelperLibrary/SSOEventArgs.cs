using System;

namespace bizilante.SSO.Helper
{
    public class SSOEventArgs : EventArgs
    {
        public SSOEventArgs(string message, bool error)
        {
            Message = message;
            IsError = error;
        }

        public SSOEventArgs(string source, string message, bool isError)
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