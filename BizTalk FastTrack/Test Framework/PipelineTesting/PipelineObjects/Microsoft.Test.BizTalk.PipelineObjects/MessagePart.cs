namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Component.Interop;
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class MessagePart : IBaseMessagePart
    {
        private Stream partData = null;
        private Guid partId = Guid.NewGuid();
        private IBasePropertyBag partProperties = null;

        public Stream GetOriginalDataStream()
        {
            return this.partData;
        }

        public void GetSize(out ulong size, out bool implemented)
        {
            size = 0L;
            implemented = false;
        }

        public string Charset
        {
            get
            {
                return (string)this.PartProperties.Read("Charset", "");
            }
            set
            {
                this.PartProperties.Write("Charset", "", value);
            }
        }

        public string ContentType
        {
            get
            {
                return (string)this.PartProperties.Read("ContentType", "");
            }
            set
            {
                this.PartProperties.Write("ContentType", "", value);
            }
        }

        public Stream Data
        {
            get
            {
                return this.partData;
            }
            set
            {
                this.partData = value;
            }
        }

        public bool IsMutable
        {
            get
            {
                return true;
            }
        }

        public Guid PartID
        {
            get
            {
                return this.partId;
            }
        }

        public IBasePropertyBag PartProperties
        {
            get
            {
                if (this.partProperties == null)
                {
                    this.partProperties = new PropertyBag();
                }
                return this.partProperties;
            }
            set
            {
                this.partProperties = value;
            }
        }

        internal IBaseMessagePart CopyMessagePart(IBaseMessageFactory factory)
        {
            IBaseMessagePart part = factory.CreateMessagePart();
            part.Data = this.GetOriginalDataStream();
            IBasePropertyBag partProperties = this.PartProperties;
            if (partProperties != null)
            {
                IBasePropertyBag bag2 = PipelineUtil.CopyPropertyBag(partProperties, factory);
                part.PartProperties = bag2;
            }
            return part;
        }
    }
}

