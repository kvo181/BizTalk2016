namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using System;

    public class CallEventArgs : EventArgs
    {
        private string message;

        public CallEventArgs(string message)
        {
            this.message = message;
        }

        public string Message
        {
            get
            {
                return this.message;
            }
        }
    }
}

