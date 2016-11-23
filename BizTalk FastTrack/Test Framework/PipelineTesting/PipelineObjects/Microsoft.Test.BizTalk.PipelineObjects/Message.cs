namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class Message : IBaseMessage, System.ICloneable
    {
        private PartEntry bodyPartEntry = null;
        private Exception exception;
        private IBaseMessageContext messageContext = null;
        private Guid messageId = Guid.NewGuid();
        private ArrayList partsList = new ArrayList();

        public void AddPart(string partName, IBaseMessagePart part, bool isBody)
        {
            PartEntry entry = new PartEntry(partName, part);
            this.partsList.Add(entry);
            if (isBody)
            {
                this.bodyPartEntry = entry;
            }
        }

        public Exception GetErrorInfo()
        {
            return this.exception;
        }

        public IBaseMessagePart GetPart(string partName)
        {
            for (int i = 0; i < this.partsList.Count; i++)
            {
                PartEntry entry = (PartEntry) this.partsList[i];
                if (entry.Name == partName)
                {
                    return entry.Part;
                }
            }
            return null;
        }

        public IBaseMessagePart GetPartByIndex(int index, out string partName)
        {
            partName = ((PartEntry) this.partsList[index]).Name;
            return ((PartEntry) this.partsList[index]).Part;
        }

        public void GetSize(out ulong size, out bool implemented)
        {
            size = 0L;
            implemented = false;
        }

        public void RemovePart(string partName)
        {
            for (int i = 0; i < this.partsList.Count; i++)
            {
                PartEntry entry = (PartEntry) this.partsList[i];
                if (entry.Name == partName)
                {
                    this.partsList.RemoveAt(i);
                    return;
                }
            }
        }

        public void SetErrorInfo(Exception exception)
        {
            this.exception = exception;
        }

        public IBaseMessagePart BodyPart
        {
            get
            {
                if (this.bodyPartEntry != null)
                {
                    return this.bodyPartEntry.Part;
                }
                return null;
            }
        }

        public string BodyPartName
        {
            get
            {
                if (this.bodyPartEntry != null)
                {
                    return this.bodyPartEntry.Name;
                }
                return null;
            }
        }

        public IBaseMessageContext Context
        {
            get
            {
                if (this.messageContext == null)
                {
                    this.messageContext = new MessageContext();
                }
                return this.messageContext;
            }
            set
            {
                this.messageContext = value;
            }
        }

        public bool IsMutable
        {
            get
            {
                return true;
            }
        }

        public Guid MessageID
        {
            get
            {
                return this.messageId;
            }
        }

        public int PartCount
        {
            get
            {
                return this.partsList.Count;
            }
        }


        public object Clone()
        {
            int partCount = this.PartCount;
            IBaseMessageFactory factory = new MessageFactory();
            IBaseMessage message = factory.CreateMessage();
            Message message2 = (Message)message;
            message2.messageId = this.MessageID;
            IBaseMessageContext oldCtx = this.Context;
            if (oldCtx != null)
            {
                IBaseMessageContext context2 = PipelineUtil.CloneMessageContext(oldCtx);
                message.Context = context2;
            }
            Exception errorInfo = this.GetErrorInfo();
            if (errorInfo != null)
            {
                message.SetErrorInfo(errorInfo);
            }
            string bodyPartName = this.BodyPartName;
            for (int i = 0; i < partCount; i++)
            {
                string str2;
                IBaseMessagePart partByIndex = this.GetPartByIndex(i, out str2);
                IBaseMessagePart part = ((MessagePart)partByIndex).CopyMessagePart(factory);
                message.AddPart(str2, part, bodyPartName == str2);
            }
            return message;
        }
    }
}

