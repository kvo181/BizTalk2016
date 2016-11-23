namespace Microsoft.Test.BizTalk.PipelineObjects
{
    using Microsoft.BizTalk.Message.Interop;
    using System;
    using System.Collections;
    using System.Runtime.InteropServices;

    public class MessageContext : PropertyBag, IBaseMessageContext, IBasePropertyBag, System.ICloneable, Microsoft.Test.BizTalk.PipelineObjects.ICloneable
    {
        private Hashtable promotedProperties = new Hashtable();

        public void AddPredicate(string propertyName, string propertyNamespace, object obj)
        {
        }

        private MessageContext CloneSelf()
        {
            MessageContext context = new MessageContext();
            context.CloneProperties(base.Properties);
            context.promotedProperties = (Hashtable) this.promotedProperties.Clone();
            return context;
        }

        public ContextPropertyType GetPropertyType(string propertyName, string propertyNamespace)
        {
            object obj2 = this.promotedProperties[base.GetKey(propertyName, propertyNamespace)];
            if ((obj2 != null) && (1 == ((int) obj2)))
            {
                return ContextPropertyType.PropPromoted;
            }
            return ContextPropertyType.PropWritten;
        }

        public bool IsPromoted(string propertyName, string propertyNamespace)
        {
            string key = base.GetKey(propertyName, propertyNamespace);
            object obj2 = this.promotedProperties[key];
            return ((obj2 != null) && (1 == ((int) obj2)));
        }

        IntPtr Microsoft.Test.BizTalk.PipelineObjects.ICloneable.Clone(ref Guid riid)
        {
            return Marshal.GetComInterfaceForObject(this.CloneSelf(), typeof(Microsoft.Test.BizTalk.PipelineObjects.ICloneable));
        }

        public void Promote(string propertyName, string propertyNamespace, object value)
        {
            base.Write(propertyName, propertyNamespace, value);
            this.promotedProperties[base.GetKey(propertyName, propertyNamespace)] = 1;
        }

        object System.ICloneable.Clone()
        {
            return this.CloneSelf();
        }
    }
}

