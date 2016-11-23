namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Message.Interop;
    using System;

    public class MessageFactory : IBaseMessageFactory
    {
        public IBaseMessage CreateMessage()
        {
            return new Message();
        }

        public IBaseMessageContext CreateMessageContext()
        {
            return new MessageContext();
        }

        public IBaseMessagePart CreateMessagePart()
        {
            return new MessagePart();
        }

        public IBasePropertyBag CreatePropertyBag()
        {
            return new PropertyBag();
        }
    }
}

