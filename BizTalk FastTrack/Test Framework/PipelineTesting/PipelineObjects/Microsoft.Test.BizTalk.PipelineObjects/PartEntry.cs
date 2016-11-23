namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Message.Interop;
    using System;

    internal class PartEntry
    {
        private IBaseMessagePart messagePart;
        private string partName;

        public PartEntry(string partName, IBaseMessagePart messagePart)
        {
            this.partName = partName;
            this.messagePart = messagePart;
        }

        public string Name
        {
            get
            {
                return this.partName;
            }
        }

        public IBaseMessagePart Part
        {
            get
            {
                return this.messagePart;
            }
        }
    }
}

